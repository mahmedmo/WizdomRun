import os
import json
import openai
import random
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

def create_qa(context, num_q, difficulty, previous_questions):
    previous_questions_str = "\n".join(previous_questions) if previous_questions else "None"
    q_a_prompt = f"""
    Create exactly {num_q} multiple-choice questions (MCQs) based solely on the following notes:\n\n{context}\n\n
    Each question must have exactly 4 answer choices labeled A, B, C, and D.
    The difficulty level for all questions is: {difficulty}.
    Do not generate additional questions beyond the specified number ({num_q}).
    Ensure the questions:
    - Cover different topics and sections from the entire document, not just a small portion.
    - Are completely distinct and not reworded versions of the following previous questions:\n{previous_questions_str}\n
    - Explore unique aspects of the content that haven’t been addressed yet.
    - Randomize the position of the correct answer between A, B, C, and D.
    Format the output strictly as follows, with precisely {num_q} questions:

    Q1: <question text> ({difficulty})
    A) <option 1>
    B) <option 2>
    C) <option 3>
    D) <option 4>
    Answer: <correct option>

    Ensure that the correct answer appears randomly in different options A, B, C, or D rather than just at position B. Let's begin:
    """
    response = client.chat.completions.create(
        model="gpt-4o-mini",
        messages=[{"role": "system", "content": "You are a helpful research and analysis assistant"},
                  {"role": "user", "content": q_a_prompt}]
    )
    return response.choices[0].message.content

def run_qa_session(docs, num_rounds, campaign_id):
    difficulties = ["Easy", "Medium", "Hard"]
    qa_list = []
    context = "".join([page.page_content for page in docs])
    save_path = os.path.dirname(os.path.abspath(__file__))
    previous_questions = []

    for round_num in range(num_rounds):
        difficulty = difficulties[min(round_num, len(difficulties)-1)]
        q_a = create_qa(context, num_rounds, difficulty, previous_questions)
        q_a_list = q_a.strip().split("\n\n")[:num_rounds]

        for qa in q_a_list:
            lines = qa.strip().split("\n")
            if len(lines) < 6:
                continue

            question = lines[0].split(f" ({difficulty})")[0].replace("Q", "").replace(":", "").strip()
            question = ' '.join(question.split()[1:])
            options = lines[1:5]
            correct_answer = lines[5].split("Answer:")[-1].strip().upper()

            if question in previous_questions:
                continue

            random.shuffle(options)

            answers = []
            for option in options:
                option_letter = option[0]
                option_text = option[2:].strip()
                answers.append({
                    "answerStr": option_text,
                    "isCorrect": option_letter == correct_answer
                })

            qa_list.append({
                "campaignID": campaign_id,
                "difficulty": difficulty.lower(),
                "questionStr": question,
                "answers": answers
            })
            previous_questions.append(question)

    json_file_path = os.path.join(save_path, 'qa_data.json')
    with open(json_file_path, 'w') as f:
        json.dump(qa_list, f, indent=4)

def main():
    file_path = "C:/Users/sahib/Desktop/Sahib-Thethi_Third-Year(Winter)/SENG401/Project/WizdomRun/llm/Linear_Models.pdf"
    docs = load_paper(file_path)
    num_rounds = 4
    campaign_id = 1
    run_qa_session(docs, num_rounds, campaign_id)

if __name__ == "__main__":
    main()
