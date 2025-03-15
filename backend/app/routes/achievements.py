from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import Achievement
from app.firebase_auth import verify_firebase_token

achievements_bp = Blueprint("achievements", __name__)

@achievements_bp.route("/unlock", methods=["POST"])
@verify_firebase_token
def unlock_achievement(user):
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
@verify_firebase_token
def get_achievements(user, campaignID):
    achievements = Achievement.query.filter_by(campaignID=campaignID).all()
    return jsonify([
        {
            "achievementID": a.achievementID,
            "title": a.title,
            "description": a.description
        } for a in achievements
    ])

@achievements_bp.route("/delete/<int:achievementID>", methods=["DELETE"])
@verify_firebase_token
def delete_achievement(user, achievementID):
    achievement = Achievement.query.get(achievementID)
    if not achievement:
        return jsonify({"error": "Achievement not found"}), 404

    db.session.delete(achievement)
    db.session.commit()
    return jsonify({"message": "Achievement deleted successfully"})
