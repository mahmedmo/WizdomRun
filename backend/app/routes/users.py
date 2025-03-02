from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import User
from ..firebase_auth import verify_firebase_token

users_bp = Blueprint('users', __name__)

@users_bp.route('/<string:userID>', methods=['GET'])
@verify_firebase_token
def get_user(user, userID):
    user = User.query.get(userID)
    if not user:
        return jsonify({"error": "User not found"}), 404

    return jsonify({"userID": user.userID, "screenName": user.screenName, "createdAt": user.createdAt})

@users_bp.route('/<string:userID>', methods=['DELETE'])
@verify_firebase_token
def delete_user(user, userID):
    user = User.query.get(userID)
    if not user:
        return jsonify({"error": "User not found"}), 404

    db.session.delete(user)
    db.session.commit()
    return jsonify({"message": "User deleted successfully"}), 200

@users_bp.route("/update/<string:userID>", methods=["PUT"])
@verify_firebase_token
def update_user(user, userID):
    data = request.get_json()
    user = User.query.get(userID)
    if not user:
        return jsonify({"error": "User not found"}), 404

    if "screenName" in data:
        user.screenName = data["screenName"]

    db.session.commit()
    return jsonify({"message": "User updated successfully"})

@users_bp.route("/", methods=["GET"])
@verify_firebase_token
def get_all_users(user):
    users = User.query.all()
    return jsonify([{"userID": u.userID, "screenName": u.screenName} for u in users])