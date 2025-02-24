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
            "question": q.question,
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

    if "isCorrect" not in data:
        return jsonify({"error": "Missing isCorrect field"}), 400

    if data["isCorrect"]:
        question.gotCorrect = True
    else:
        question.wrongAttempts += 1

    db.session.commit()
    return jsonify({"message": "Answer recorded successfully"})
