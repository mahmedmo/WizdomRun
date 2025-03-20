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
    return default

# Apply os.getenv patch at module level
os_getenv_patcher = patch('os.getenv', side_effect=mock_getenv)
os_getenv_patcher.start()

@pytest.fixture
def app():
    # Patch verify_firebase_token before importing create_app
    with patch('app.routes.characters.verify_firebase_token', return_value={"uid": "test_user_id"}), \
         patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test_user_id"}):

        from app import create_app
        app = create_app()
        app.config["TESTING"] = True

        # Enter app context before patching User.query
        with app.app_context(), \
             patch('app.routes.characters.db') as mock_db, \
             patch('app.models.User.query') as mock_user_query:
            
            # Mock User.query.filter_by().first() to return a dummy user
            mock_user = MagicMock()
            mock_user.userID = "test_user_id"
            mock_filter = mock_user_query.filter_by.return_value
            mock_filter.first.return_value = mock_user

            yield app

@pytest.fixture
def client(app):
    return app.test_client()

def test_create_character_success(client):
    payload = {
        "userID": "test_user_id",
        "modelID": 1,
        "hairID": 2,
        "robeID": 3,
        "bootID": 4
    }

    with patch('app.routes.characters.PlayerCharacter') as MockCharacter, \
         patch('app.routes.characters.db.session') as mock_session:

        mock_character_instance = MockCharacter.return_value
        mock_character_instance.characterID = 1
        mock_character_instance.userID = payload["userID"]
        mock_character_instance.modelID = payload["modelID"]
        mock_character_instance.hairID = payload["hairID"]
        mock_character_instance.robeID = payload["robeID"]
        mock_character_instance.bootID = payload["bootID"]

        response = client.post(
            "/characters/create",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 201
        data = response.get_json()
        assert data == {
            "characterID": 1,
            "userID": "test_user_id",
            "modelID": 1,
            "hairID": 2,
            "robeID": 3,
            "bootID": 4
        }
        mock_session.add.assert_called_once_with(mock_character_instance)
        mock_session.commit.assert_called_once()

def test_create_character_missing_fields(client):
    payload = {"userID": "test_user_id"}

    response = client.post(
        "/characters/create",
        data=json.dumps(payload),
        content_type="application/json",
        headers={"Authorization": "Bearer test-token"}
    )

    assert response.status_code == 400
    assert response.get_json() == {"error": "Missing required fields"}

def test_get_character_success(client):
    with patch('app.routes.characters.PlayerCharacter') as MockCharacter:
        mock_query = MockCharacter.query
        mock_filter = mock_query.filter_by.return_value
        mock_character = MagicMock()
        mock_character.characterID = 1
        mock_character.userID = "test_user_id"
        mock_character.modelID = 1
        mock_character.hairID = 2
        mock_character.robeID = 3
        mock_character.bootID = 4
        mock_filter.first.return_value = mock_character

        response = client.get(
            "/characters/test_user_id",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 200
        assert response.get_json() == {
            "characterID": 1,
            "userID": "test_user_id",
            "modelID": 1,
            "hairID": 2,
            "robeID": 3,
            "bootID": 4
        }

def test_get_character_not_found(client):
    with patch('app.routes.characters.PlayerCharacter') as MockCharacter:
        mock_query = MockCharacter.query
        mock_filter = mock_query.filter_by.return_value
        mock_filter.first.return_value = None

        response = client.get(
            "/characters/non_existent_user",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 404
        assert response.get_json() == {"error": "Character not found"}

def test_update_character_success(client):
    update_data = {"modelID": 5, "hairID": 6}

    with patch('app.routes.characters.PlayerCharacter') as MockCharacter, \
         patch('app.routes.characters.db.session') as mock_session:
        mock_character = MagicMock()
        mock_character.characterID = 1
        MockCharacter.query.get.return_value = mock_character

        response = client.put(
            "/characters/update/1",
            data=json.dumps(update_data),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 200
        assert response.get_json() == {"message": "Character updated successfully"}
        assert mock_character.modelID == 5
        assert mock_character.hairID == 6
        mock_session.commit.assert_called_once()

def test_update_character_not_found(client):
    update_data = {"modelID": 5}

    with patch('app.routes.characters.PlayerCharacter') as MockCharacter:
        MockCharacter.query.get.return_value = None

        response = client.put(
            "/characters/update/999",
            data=json.dumps(update_data),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 404
        assert response.get_json() == {"error": "Character not found"}

def test_delete_character_success(client):
    with patch('app.routes.characters.PlayerCharacter') as MockCharacter, \
         patch('app.routes.characters.db.session') as mock_session:
        mock_character = MagicMock()
        MockCharacter.query.get.return_value = mock_character

        response = client.delete(
            "/characters/delete/1",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 200
        assert response.get_json() == {"message": "Character deleted successfully"}
        mock_session.delete.assert_called_once_with(mock_character)
        mock_session.commit.assert_called_once()

def test_delete_character_not_found(client):
    with patch('app.routes.characters.PlayerCharacter') as MockCharacter:
        MockCharacter.query.get.return_value = None

        response = client.delete(
            "/characters/delete/999",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 404
        assert response.get_json() == {"error": "Character not found"}

def pytest_sessionfinish():
    os_getenv_patcher.stop()

if __name__ == '__main__':
    pytest.main()