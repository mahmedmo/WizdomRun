from flask import request, jsonify, Blueprint
from app.extensions import db
from app.models import User
import firebase_admin
from firebase_admin import auth


auth_bp = Blueprint("auth", __name__)

@auth_bp.route("/login", methods=["POST"])
def login():
    data = request.json
    id_token = data.get("idToken")
    if not id_token:
        return jsonify({"error": "Missing idToken"}), 400

    try:
        # Verify the ID token from Unity
        decoded_token = auth.verify_id_token(id_token)
        user_id = decoded_token.get("uid")
        return jsonify({"userID": user_id}), 200
    except Exception as e:
        return jsonify({"error": str(e)}), 400
        
@auth_bp.route("/signup", methods=["POST"])
def signup():
    data = request.json
    email = data.get("email")
    password = data.get("password")
    screen_name = data.get("screenName")

    if not email or not password or not screen_name:
        return jsonify({"error": "Missing fields"}), 400

    try:
        firebase_user = auth.create_user(email=email, password=password)

        new_user = User(userID=firebase_user.uid, screenName=screen_name)
        db.session.add(new_user)
        db.session.commit()

        return jsonify({"message": "User created successfully", "userID": firebase_user.uid}), 201
    except Exception as e:
        return jsonify({"error": str(e)}), 400
