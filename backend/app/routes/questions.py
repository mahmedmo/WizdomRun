from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import Question

questions_bp = Blueprint("questions", __name__)

@questions_bp.route("/<int:campaignID>", methods=["GET"])
def get_questions(campaignID):
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
def answer_question(questionID):
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
def batch_create_questions():
    data = request.get_json()
    if not isinstance(data, list):
        return jsonify({"error": "Expected a list of questions"}), 400

    new_questions = []
    for item in data:
        new_questions.append(Question(
            campaignID=item["campaignID"],
            difficulty=item["difficulty"],
            questionStr=item["questionStr"],
            gotCorrect=False,
            wrongAttempts=0
        ))

    db.session.bulk_save_objects(new_questions)
    db.session.commit()
    return jsonify({"message": "Questions created successfully"})

@questions_bp.route("/question/<int:questionID>", methods=["GET"])
def get_question(questionID):
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
def delete_question(questionID):
    question = Question.query.get(questionID)
    if not question:
        return jsonify({"error": "Question not found"}), 404

    db.session.delete(question)
    db.session.commit()
    return jsonify({"message": "Question deleted successfully"})