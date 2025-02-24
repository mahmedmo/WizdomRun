from flask import Blueprint, request, jsonify
from app.extensions import db
from app.models import User

users_bp = Blueprint('users', __name__)

@users_bp.route('/create', methods=['POST'])
def create_user():
    data = request.get_json()
    if not data or 'screenName' not in data:
        return jsonify({"error": "Missing screenName"}), 400

    new_user = User(screenName=data['screenName'])
    db.session.add(new_user)
    db.session.commit()

    return jsonify({"userID": new_user.userID, "screenName": new_user.screenName, "createdAt": new_user.createdAt}), 201

@users_bp.route('/<int:userID>', methods=['GET'])
def get_user(userID):
    user = User.query.get(userID)
    if not user:
        return jsonify({"error": "User not found"}), 404

    return jsonify({"userID": user.userID, "screenName": user.screenName, "createdAt": user.createdAt})

@users_bp.route('/<int:userID>', methods=['DELETE'])
def delete_user(userID):
    user = User.query.get(userID)
    if not user:
        return jsonify({"error": "User not found"}), 404

    db.session.delete(user)
    db.session.commit()
    return jsonify({"message": "User deleted successfully"}), 200
