from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import Question
from app.models import Answer
from app.models import Campaign
from ..firebase_auth import verify_firebase_token
import os
import sys
from werkzeug.utils import secure_filename
sys.path.append(os.path.abspath(os.path.join(os.path.dirname(__file__), "../../../llm")))
from qa_app import run_qa_session

questions_bp = Blueprint("questions", __name__)

@questions_bp.route("/<int:campaignID>", methods=["GET"])
@verify_firebase_token
def get_questions(user, campaignID):
    questions = Question.query.filter_by(campaignID=campaignID).all()
    return jsonify([
        {
            "questionID": q.questionID,
            "difficulty": q.difficulty,
            "questionStr": q.questionStr,
            "gotCorrect": q.gotCorrect,
            "wrongAttempts": q.wrongAttempts
        } for q in questions
    ])

@questions_bp.route("/answer/<int:questionID>", methods=["PUT"])
@verify_firebase_token
def answer_question(user, questionID):
    data = request.get_json()
    question = Question.query.get(questionID)

    if not question:
        return jsonify({"error": "Question not found"}), 404

    if "gotCorrect" not in data:
        return jsonify({"error": "Missing gotCorrect field"}), 400

    if data["gotCorrect"]:
        question.gotCorrect = True
    else:
        question.wrongAttempts += 1

    db.session.commit()
    return jsonify({"message": "Answer recorded successfully"})

@questions_bp.route("/batch_create", methods=["POST"])
@verify_firebase_token
def batch_create_questions(user, data):
    if not isinstance(data, list):
        return jsonify({"error": "Expected a list of questions"}), 400

    new_answers = []

    try:
        with db.session.begin_nested():
            for item in data:
                if not all(key in item for key in ["campaignID", "difficulty", "questionStr", "answers"]):
                    return jsonify({"error": "Missing required fields in one or more questions"}), 400
                
                if len(item["answers"]) not in [2, 4]:
                    return jsonify({"error": f"Question '{item['questionStr']}' must have exactly 2 or 4 answers."}), 400

                new_question = Question(
                    campaignID=item["campaignID"],
                    difficulty=item["difficulty"],
                    questionStr=item["questionStr"],
                    gotCorrect=False,
                    wrongAttempts=0
                )
                db.session.add(new_question)
                db.session.flush()

                for ans in item["answers"]:
                    if "answerStr" not in ans or "isCorrect" not in ans:
                        return jsonify({"error": "Each answer must include 'answerStr' and 'isCorrect'"}), 400

                    new_answers.append(Answer(
                        questionID=new_question.questionID,
                        answerStr=ans["answerStr"],
                        isCorrect=ans["isCorrect"]
                    ))

            db.session.bulk_save_objects(new_answers)
        db.session.commit()

    except Exception as e:
        db.session.rollback() 
        return jsonify({"error": str(e)}), 500

    return jsonify({"message": "Questions and answers created successfully"}), 201


@questions_bp.route("/question/<int:questionID>", methods=["GET"])
@verify_firebase_token
def get_question(user, questionID):
    question = Question.query.get(questionID)
    if not question:
        return jsonify({"error": "Question not found"}), 404

    return jsonify({
        "questionID": question.questionID,
        "campaignID": question.campaignID,
        "difficulty": question.difficulty,
        "questionStr": question.questionStr,
        "gotCorrect": question.gotCorrect,
        "wrongAttempts": question.wrongAttempts
    })

@questions_bp.route("/delete/<int:questionID>", methods=["DELETE"])
@verify_firebase_token
def delete_question(user, questionID):
    question = Question.query.get(questionID)
    if not question:
        return jsonify({"error": "Question not found"}), 404

    db.session.delete(question)
    db.session.commit()
    return jsonify({"message": "Question deleted successfully"})

@questions_bp.route("/answers/<int:questionID>", methods=["GET"])
@verify_firebase_token
def get_answers(user, questionID):
    answers = Answer.query.filter_by(questionID=questionID).all()
    
    if not answers:
        return jsonify({"error": "No answers found for this question"}), 404

    return jsonify([
        {
            "answerID": ans.answerID,
            "questionID": ans.questionID,
            "answerStr": ans.answerStr,
            "isCorrect": ans.isCorrect
        }
        for ans in answers
    ])

@questions_bp.route("/wrong_attempt/<int:questionID>", methods=["PUT"])
@verify_firebase_token
def increment_wrong_attempt(user, questionID):
    question = Question.query.get(questionID)
    if not question:
        return jsonify({"error": "Question not found"}), 404

    question.wrongAttempts += 1
    db.session.commit()
    
    return jsonify({
        "message": "Wrong attempt recorded",
        "questionID": questionID,
        "wrongAttempts": question.wrongAttempts
    })

@questions_bp.route("/unanswered", methods=["GET"])
@verify_firebase_token
def get_unanswered_questions(user):
    unanswered_questions = Question.query.filter_by(gotCorrect=False).all()
    return jsonify([
        {
            "questionID": q.questionID,
            "campaignID": q.campaignID,
            "difficulty": q.difficulty,
            "questionStr": q.questionStr,
            "wrongAttempts": q.wrongAttempts
        }
        for q in unanswered_questions
    ])

@questions_bp.route("/difficulty/<string:difficulty>", methods=["GET"])
@verify_firebase_token
def get_questions_by_difficulty(user, difficulty):
    valid_difficulties = {"easy", "medium", "hard"}
    
    if difficulty.lower() not in valid_difficulties:
        return jsonify({"error": "Invalid difficulty level"}), 400

    questions = Question.query.filter_by(difficulty=difficulty.lower()).all()
    
    return jsonify([
        {
            "questionID": q.questionID,
            "campaignID": q.campaignID,
            "questionStr": q.questionStr,
            "gotCorrect": q.gotCorrect,
            "wrongAttempts": q.wrongAttempts
        }
        for q in questions
    ])

@questions_bp.route("/create", methods=["POST"])
@verify_firebase_token
def create_questions(user):
    if "file" not in request.files:
        return jsonify({"error": "No file provided"}), 400

    pdf_file = request.files["file"]
    if pdf_file.filename == "":
        return jsonify({"error": "Empty file uploaded"}), 400

    campaign_id = request.form.get("campaignID")
    
    if not campaign_id:
        return jsonify({"error": "Missing campaignID"}), 400

    current_campaign = Campaign.query.filter_by(userID=user.userID, campaignID=campaign_id).first()

    if not current_campaign:
        return jsonify({"error": "Invalid campaign or unauthorized access"}), 403

    campaign_length_map = {"quest": 5, "odyssey": 10, "saga": 15}
    num_rounds = campaign_length_map.get(current_campaign.campaignLength, 5)

    filename = secure_filename(pdf_file.filename)

    temp_dir = os.getenv("TEMP") if os.name == "nt" else "/tmp"
    os.makedirs(temp_dir, exist_ok=True)

    temp_pdf_path = os.path.join(temp_dir, filename)

    print(f"Saving uploaded PDF to: {temp_pdf_path}")

    pdf_file.save(temp_pdf_path)

    try:
        questions_data = run_qa_session(temp_pdf_path, num_rounds, campaign_id)

        if not questions_data:
            return jsonify({"error": "No questions generated"}), 500

        return batch_create_questions(questions_data)

    except Exception as e:
        return jsonify({"error": str(e)}), 500

    finally:
        if os.path.exists(temp_pdf_path):
            os.remove(temp_pdf_path)