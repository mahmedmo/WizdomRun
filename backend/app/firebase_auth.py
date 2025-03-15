import functools
from flask import request, jsonify
from firebase_admin import auth
from app.models import User
import logging

logger = logging.getLogger(__name__)

def verify_firebase_token(f):
    @functools.wraps(f)
    def decorated_function(*args, **kwargs):
        auth_header = request.headers.get("Authorization")

        if not auth_header:
            logger.warning("Missing Authorization Header")
            return jsonify({"error": "Missing token"}), 401

        token = auth_header.split("Bearer ")[-1]  # Extract token

        try:
            # Verify the token with Firebase
            decoded_token = auth.verify_id_token(token)
            user_id = decoded_token["uid"]
            logger.info(f"Token verified for user: {user_id}")

            # Check if the user exists in PostgreSQL
            user = User.query.filter_by(userID=user_id).first()
            if not user:
                logger.warning(f"User {user_id} not found in database")
                return jsonify({"error": "User not registered"}), 403

            return f(user, *args, **kwargs)
        except Exception as e:
            logger.error(f"Token verification failed: {str(e)}")
            return jsonify({"error": "Invalid token", "message": str(e)}), 403

    return decorated_function
