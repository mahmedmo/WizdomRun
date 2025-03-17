from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import Campaign
from app.firebase_auth import verify_firebase_token

campaigns_bp = Blueprint("campaigns", __name__)

@campaigns_bp.route("/create", methods=["POST"])
@verify_firebase_token
def create_campaign(user):
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

@campaigns_bp.route("/<string:userID>", methods=["GET"])
@verify_firebase_token
def get_campaigns(user, userID):
    campaigns = Campaign.query.filter_by(userID=userID).all()
    return jsonify([
        {
            "campaignID": c.campaignID,
            "title": c.title,
            "campaignLength": c.campaignLength,
            "currLevel": c.currLevel,
            "remainingTries": c.remainingTries,
            "lastUpdated": c.lastUpdated
        } for c in campaigns
    ])

@campaigns_bp.route("/update/<int:campaignID>", methods=["PUT"])
@verify_firebase_token
def update_campaign(user, campaignID):
    data = request.get_json()
    campaign = Campaign.query.get(campaignID)

    if not campaign:
        return jsonify({"error": "Campaign not found"}), 404

    if "currLevel" in data:
        campaign.currLevel = data["currLevel"]
    if "remainingTries" in data:
        campaign.remainingTries = data["remainingTries"]
    campaign.lastUpdated = db.func.current_timestamp()

    db.session.commit()
    return jsonify({"message": "Campaign updated successfully"})

@campaigns_bp.route("/delete/<int:campaignID>", methods=["DELETE"])
@verify_firebase_token
def delete_campaign(user, campaignID):
    campaign = Campaign.query.get(campaignID)
    if not campaign:
        return jsonify({"error": "Campaign not found"}), 404

    db.session.delete(campaign)
    db.session.commit()
    return jsonify({"message": "Campaign deleted successfully"})

@campaigns_bp.route("/single/<int:campaignID>", methods=["GET"])
@verify_firebase_token
def get_campaign(user, campaignID):
    campaign = Campaign.query.get(campaignID)
    if not campaign:
        return jsonify({"error": "Campaign not found"}), 404

    return jsonify({
        "campaignID": campaign.campaignID,
        "title": campaign.title,
        "campaignLength": campaign.campaignLength,
        "currLevel": campaign.currLevel,
        "remainingTries": campaign.remainingTries
    })

@campaigns_bp.route("/<int:campaignID>/restart", methods=["PATCH"])
@verify_firebase_token
def restart_campaign(user, campaignID):
    campaign = Campaign.query.get(campaignID)
    if not campaign:
        return jsonify({"error": "Campaign not found"}), 404

    campaign.currLevel = 1
    campaign.remainingTries = 2
    campaign.lastUpdated = db.func.current_timestamp()

    db.session.commit()
    return jsonify({"message": "Campaign restarted successfully"})
