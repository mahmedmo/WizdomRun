import sys
import os
from unittest.mock import patch, MagicMock
import pytest
from flask import json

# Mock Firebase to avoid real credential loading
sys.modules["firebase_admin"] = MagicMock()
sys.modules["firebase_admin.auth"] = MagicMock()

# Set up sys.path for backend imports from WizdomRun\backend\tests\
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture
def app():
    os.environ["DATABASE_URL"] = "sqlite:///:memory:"
    from app import create_app
    app = create_app()
    app.config["TESTING"] = True
    with app.app_context():
        from app.extensions import db
        db.create_all()
        yield app
        db.session.remove()
        db.drop_all()
    os.environ.pop("DATABASE_URL", None)

def test_verify_firebase_token_valid(app):
    from app.firebase_auth import verify_firebase_token
    def dummy_func(user, *args, **kwargs):
        return {"uid": user.userID}  # Access userID attribute from User object

    with app.test_request_context(headers={"Authorization": "Bearer valid-token"}), \
         patch('firebase_admin.auth.verify_id_token', return_value={"uid": "test_user"}), \
         patch('app.models.User.query') as mock_query:
        mock_user = MagicMock()
        mock_user.userID = "test_user"  # Set attribute directly
        mock_query.filter_by.return_value.first.return_value = mock_user
        decorated = verify_firebase_token(dummy_func)
        result = decorated()
        assert result == {"uid": "test_user"}

def test_verify_firebase_token_missing_header(app):
    from app.firebase_auth import verify_firebase_token
    def dummy_func(user, *args, **kwargs):
        return {"uid": "uid"}

    with app.test_request_context():
        decorated = verify_firebase_token(dummy_func)
        response = decorated()
        assert response[1] == 401
        assert response[0].get_json() == {"error": "Missing token"}

def test_verify_firebase_token_invalid_token(app):
    from app.firebase_auth import verify_firebase_token
    def dummy_func(user, *args, **kwargs):
        return {"uid": "uid"}

    with app.test_request_context(headers={"Authorization": "Bearer invalid-token"}), \
         patch('firebase_admin.auth.verify_id_token', side_effect=Exception("Invalid token")):
        decorated = verify_firebase_token(dummy_func)
        response = decorated()
        assert response[1] == 403
        assert response[0].get_json()["error"] == "Invalid token"

def test_verify_firebase_token_user_not_found(app):
    from app.firebase_auth import verify_firebase_token
    def dummy_func(user, *args, **kwargs):
        return {"uid": "uid"}

    with app.test_request_context(headers={"Authorization": "Bearer valid-token"}), \
         patch('firebase_admin.auth.verify_id_token', return_value={"uid": "test_user"}), \
         patch('app.models.User.query') as mock_query:
        mock_query.filter_by.return_value.first.return_value = None
        decorated = verify_firebase_token(dummy_func)
        response = decorated()
        assert response[1] == 403
        assert response[0].get_json() == {"error": "User not registered"}

if __name__ == '__main__':
    pytest.main()