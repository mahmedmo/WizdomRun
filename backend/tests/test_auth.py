import sys
import os
import pytest
from flask import Flask
from unittest.mock import patch, MagicMock

sys.modules['firebase_admin'] = MagicMock()
sys.modules['firebase_admin.auth'] = MagicMock()

# Ensure app directory is in path
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture
def app():
    with patch('app.routes.auth.db') as mock_db:
        app = Flask(__name__)
        app.config['TESTING'] = True

        from app.routes.auth import auth_bp
        app.register_blueprint(auth_bp, url_prefix='/auth')

        return app

@pytest.fixture
def client(app):
    return app.test_client()

def test_signup_success(client):
    payload = {
        "email": "test@example.com",
        "password": "testpassword",
        "screenName": "TestUser"
    }

    with patch('app.routes.auth.auth.create_user') as mock_create_user, \
         patch('app.routes.auth.db.session') as mock_session:

        mock_create_user.return_value.uid = "mock-uid"

        response = client.post("/auth/signup", json=payload)

        assert response.status_code == 201
        assert response.get_json() == {
            "message": "User created successfully",
            "userID": "mock-uid"
        }

        mock_create_user.assert_called_once_with(email="test@example.com", password="testpassword")
        mock_session.add.assert_called()
        mock_session.commit.assert_called()

def test_signup_missing_fields(client):
    payload = {
        "email": "test@example.com",
        # Missing password and screenName
    }

    response = client.post("/auth/signup", json=payload)

    assert response.status_code == 400
    assert response.get_json() == {"error": "Missing fields"}

def test_signup_firebase_error(client):
    payload = {
        "email": "test@example.com",
        "password": "testpassword",
        "screenName": "TestUser"
    }

    with patch('app.routes.auth.auth.create_user', side_effect=Exception("Firebase error")), \
         patch('app.routes.auth.db.session') as mock_session:
        
        response = client.post("/auth/signup", json=payload)

        assert response.status_code == 400
        assert response.get_json() == {"error": "Firebase error"}
