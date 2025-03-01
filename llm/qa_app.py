import os
import openai
from dotenv import load_dotenv
from langchain.document_loaders import PyPDFLoader

load_dotenv()
api_key = os.getenv("OPEN_AI_KEY", None)

if api_key is None:
    raise ValueError("OPEN_AI_KEY environment variable not set.")

client = openai.OpenAI(api_key=api_key)

def load_paper(file_path):
    loader = PyPDFLoader(file_path)
    docs = loader.load()
    return docs

def create_qa(context, num_q=5): #TODO: Add difficulty part
    # Prompt to create the questions
    q_a_prompt = f"""
    Create {num_q} multiple-choice questions (MCQs) based solely on the following notes:\n\n{context}\n\n 
    Each question should have 4 answer choices labeled A, B, C, and D.  
    Format the output as follows:

    Q1: <question text>
    A) <option 1>
    B) <option 2>
    C) <option 3>
    D) <option 4>
    Answer: <correct option>

    Q2: <question text>
    A) <option 1>
    B) <option 2>
    C) <option 3>
    D) <option 4>
    Answer: <correct option>

    Ensure that the correct answer appears randomly in different options 1, 2, 3, or 4 rather than just at position B. Let's begin:
    """
    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[{"role": "system", "content": "You are a helpful research and analysis assistant"},
                  {"role": "user", "content": q_a_prompt}]
    )
    return response.choices[0].message.content

def run_qa_session(docs, num, context_window):
    total_score = 0
    qa_dict = {} #TODO: Convert to JSON
    for i, page_num in enumerate(range(0, len(docs), context_window)):
        # Using context window to give specific context for the Q&A
        context = "".join([page.page_content for page in docs[page_num:page_num + context_window]])
        q_a = create_qa(context, num)
        
        q_a_list = q_a.strip().split("\n\n")  # Each MCQ is separated by a double newline
        
        scores_list = []
        for qa in q_a_list:
            lines = qa.strip().split("\n")
            if len(lines) < 6: #Question + 4 options + answer 
                continue
            
            question = lines[0]  # Extract question
            options = lines[1:5]  # Extract the 4 answer choices
            correct_answer = lines[5].split("Answer:")[-1].strip().upper()  # Extract correct answer letter

            # Display question and choices
            print(question)
            for option in options:
                print(option)

            # Get user input (ensuring they enter A, B, C, or D)
            while True:
                user_answer = input("Your answer (A, B, C, or D): ").strip().upper()
                if user_answer in ["A", "B", "C", "D"]:
                    break
                print("Invalid input. Please enter A, B, C, or D.")

            # Store user response
            qa_dict[f"Round {i}"] = {
                "question": question,
                "options": options,
                "correct_answer": correct_answer,
                "user_answer": user_answer #TODO: Do we need to store user answer? - Not yet
            }

            # Evaluate answer
            is_correct = (user_answer == correct_answer)
            score = 1 if is_correct else 0
            scores_list.append(score)

            #evaluate user input
            if is_correct:
                total_score += 1
                print("✅ Correct!")
            else:
                total_score -= 1
                print(f"❌ Incorrect! The correct answer was: {correct_answer}")

        # Calculate round score
        print(f"CURRENT SCORE: {total_score}")
        print("*********")

        # Continue or quit
        continue_input = input("Press Enter to continue to the next round or 'q' to quit: ")
        if continue_input.lower() == "q":
            break

    return total_score, qa_dict



def main():
    file_path = "./llm/refactoring.pdf"
    docs = load_paper(file_path)
    num = 1
    context_window = 4
    qa_dict = run_qa_session(docs, num, context_window)
    print("The Q&A data: ", qa_dict)

if __name__ == "__main__":
    main()


