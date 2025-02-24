from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import Achievement

achievements_bp = Blueprint("achievements", __name__)

@achievements_bp.route("/unlock", methods=["POST"])
def unlock_achievement():
    data = request.get_json()
    if not all(key in data for key in ["campaignID", "title", "description"]):
        return jsonify({"error": "Missing required fields"}), 400

    new_achievement = Achievement(
        campaignID=data["campaignID"],
        title=data["title"],
        description=data["description"]
    )
    db.session.add(new_achievement)
    db.session.commit()

    return jsonify({"message": "Achievement unlocked"})

@achievements_bp.route("/<int:campaignID>", methods=["GET"])
def get_achievements(campaignID):
    achievements = Achievement.query.filter_by(campaignID=campaignID).all()
    return jsonify([
        {
            "achievementID": a.achievementID,
            "title": a.title,
            "description": a.description
        } for a in achievements
    ])
