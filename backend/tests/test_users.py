import sys
import os
from unittest.mock import MagicMock, patch
import pytest
from flask import json

# Mock Firebase modules to prevent real initialization
sys.modules['firebase_admin'] = MagicMock()
sys.modules['firebase_admin.credentials'] = MagicMock()
sys.modules['firebase_admin.auth'] = MagicMock()

# Set up sys.path for backend imports from WizdomRun\backend\tests\
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

# Define mock_getenv for Firebase, OpenAI, and SQLAlchemy credentials
def mock_getenv(key, default=None):
    if key in ["FIREBASE_CREDENTIALS", "FIREBASE_ADMIN_CREDENTIALS"]:
        return '{"type": "service_account", "project_id": "test", "client_email": "test@example.com", "token_uri": "https://test.com"}'
    if key == "OPEN_AI_KEY":
        return "mock-openai-key"
    if key == "DATABASE_URL":
        return "sqlite:///test.db"  # Dummy URI for testing
    if key == "TEMP":
        return os.path.join(os.path.abspath(os.path.dirname(__file__)), "temp")
    return default

# Apply os.getenv patch at module level
os_getenv_patcher = patch('os.getenv', side_effect=mock_getenv)
os_getenv_patcher.start()

@pytest.fixture
def app():
    with patch('app.routes.users.verify_firebase_token', return_value={"uid": "test_user_id"}), \
         patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test_user_id"}):

        from app import create_app
        app = create_app()
        app.config["TESTING"] = True

        with app.app_context(), \
             patch('app.routes.users.db') as mock_db, \
             patch('app.models.User.query') as mock_user_query:
            
            mock_user = MagicMock()
            mock_user.userID = "test_user_id"
            mock_filter = mock_user_query.filter_by.return_value
            mock_filter.first.return_value = mock_user

            yield app

@pytest.fixture
def client(app):
    return app.test_client()

def test_get_user_success(client):
    with patch('app.routes.users.User') as MockUser:
        mock_user = MagicMock()
        mock_user.userID = "user1"
        mock_user.screenName = "TestUser"
        mock_user.createdAt = "2023-01-01"
        MockUser.query.get.return_value = mock_user

        response = client.get("/users/user1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == {
            "userID": "user1",
            "screenName": "TestUser",
            "createdAt": "2023-01-01"
        }

def test_get_user_not_found(client):
    with patch('app.routes.users.User') as MockUser:
        MockUser.query.get.return_value = None

        response = client.get("/users/user999", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 404
        assert response.get_json() == {"error": "User not found"}

def test_delete_user_success(client):
    with patch('app.routes.users.User') as MockUser, \
         patch('app.routes.users.db.session') as mock_session:
        mock_user = MagicMock()
        mock_user.userID = "user1"
        MockUser.query.get.return_value = mock_user

        response = client.delete("/users/user1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == {"message": "User deleted successfully"}
        mock_session.delete.assert_called_once_with(mock_user)
        mock_session.commit.assert_called_once()

def test_delete_user_not_found(client):
    with patch('app.routes.users.User') as MockUser:
        MockUser.query.get.return_value = None

        response = client.delete("/users/user999", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 404
        assert response.get_json() == {"error": "User not found"}

def test_update_user_success(client):
    with patch('app.routes.users.User') as MockUser, \
         patch('app.routes.users.db.session') as mock_session:
        mock_user = MagicMock()
        mock_user.userID = "user1"
        mock_user.screenName = "OldName"
        MockUser.query.get.return_value = mock_user

        payload = {"screenName": "NewName"}
        response = client.put(
            "/users/update/user1",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 200
        assert response.get_json() == {"message": "User updated successfully"}
        assert mock_user.screenName == "NewName"
        mock_session.commit.assert_called_once()

def test_update_user_not_found(client):
    with patch('app.routes.users.User') as MockUser:
        MockUser.query.get.return_value = None

        payload = {"screenName": "NewName"}
        response = client.put(
            "/users/update/user999",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 404
        assert response.get_json() == {"error": "User not found"}

def test_get_all_users_success(client):
    with patch('app.routes.users.User') as MockUser:
        mock_user1 = MagicMock()
        mock_user1.userID = "user1"
        mock_user1.screenName = "TestUser1"
        mock_user2 = MagicMock()
        mock_user2.userID = "user2"
        mock_user2.screenName = "TestUser2"
        MockUser.query.all.return_value = [mock_user1, mock_user2]

        response = client.get("/users/", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == [
            {"userID": "user1", "screenName": "TestUser1"},
            {"userID": "user2", "screenName": "TestUser2"}
        ]

def test_get_all_users_empty(client):
    with patch('app.routes.users.User') as MockUser:
        MockUser.query.all.return_value = []

        response = client.get("/users/", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == []

def pytest_sessionfinish():
    os_getenv_patcher.stop()

if __name__ == '__main__':
    pytest.main()