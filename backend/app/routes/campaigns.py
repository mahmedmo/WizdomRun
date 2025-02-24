from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import Campaign

campaigns_bp = Blueprint("campaigns", __name__)

@campaigns_bp.route("/create", methods=["POST"])
def create_campaign():
    data = request.get_json()
    if not all(key in data for key in ["userID", "title", "campaignLength", "currLevel"]):
        return jsonify({"error": "Missing required fields"}), 400

    new_campaign = Campaign(
        userID=data["userID"],
        title=data["title"],
        campaignLength=data["campaignLength"],
        currLevel=data["currLevel"],
        remainingTries=2  # Default value
    )
    db.session.add(new_campaign)
    db.session.commit()

    return jsonify({
        "campaignID": new_campaign.campaignID,
        "userID": new_campaign.userID,
        "title": new_campaign.title,
        "campaignLength": new_campaign.campaignLength,
        "currLevel": new_campaign.currLevel,
        "remainingTries": new_campaign.remainingTries
    }), 201

@campaigns_bp.route("/<int:userID>", methods=["GET"])
def get_campaigns(userID):
    campaigns = Campaign.query.filter_by(userID=userID).all()
    return jsonify([
        {
            "campaignID": c.campaignID,
            "title": c.title,
            "campaignLength": c.campaignLength,
            "currLevel": c.currLevel,
            "remainingTries": c.remainingTries
        } for c in campaigns
    ])

@campaigns_bp.route("/update/<int:campaignID>", methods=["PUT"])
def update_campaign(campaignID):
    data = request.get_json()
    campaign = Campaign.query.get(campaignID)

    if not campaign:
        return jsonify({"error": "Campaign not found"}), 404

    if "currLevel" in data:
        campaign.currLevel = data["currLevel"]
    if "remainingTries" in data:
        campaign.remainingTries = data["remainingTries"]

    db.session.commit()
    return jsonify({"message": "Campaign updated successfully"})

@campaigns_bp.route("/delete/<int:campaignID>", methods=["DELETE"])
def delete_campaign(campaignID):
    campaign = Campaign.query.get(campaignID)
    if not campaign:
        return jsonify({"error": "Campaign not found"}), 404

    db.session.delete(campaign)
    db.session.commit()
    return jsonify({"message": "Campaign deleted successfully"})
