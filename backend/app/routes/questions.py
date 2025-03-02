from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import Question
from app.models import Answer
from ..firebase_auth import verify_firebase_token

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
def batch_create_questions(user):
    data = request.get_json()
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
