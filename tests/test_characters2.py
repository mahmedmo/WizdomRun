# import sys
# import os
# from unittest.mock import Mock, patch
# from flask import Flask, json
# import pytest
# import firebase_admin

# # Get absolute path of the project's backend directory
# BASE_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), '..', 'backend'))
# sys.path.insert(0, BASE_DIR)  # Add backend directory to sys.path

# # Debugging: Print the modified sys.path
# print("Updated sys.path:", sys.path)

# from app.models import PlayerCharacter  # Now Python should find the 'app' module

# @patch("firebase_admin.initialize_app")  # Mock Firebase initialization
# @patch("firebase_admin.auth")  # Mock Firebase Auth
# def test_something(mock_auth, mock_firebase):
#     assert firebase_admin is not None  # Ensures Firebase is mocked
#     mock_auth.create_user.assert_not_called()  # Firebase Auth isn't used

# # Mock os.getenv globally
# def mock_getenv(key, default=None):
#     if "FIREBASE" in key:
#         return '{"type": "service_account", "project_id": "test", "client_email": "test@example.com", "token_uri": "https://test.com"}'
#     return default

# # Debugging: Print the value to confirm it's mocked
# print(f"os.getenv('FIREBASE_CREDENTIALS') before patching:", os.getenv("FIREBASE_CREDENTIALS"))

# @pytest.fixture(autouse=True)
# @patch('os.getenv', side_effect=mock_getenv)
# def app(mock_getenv):
#     """Create a test Flask app with the characters blueprint, bypassing auth."""
#     print(f"os.getenv('FIREBASE_CREDENTIALS') inside fixture:", os.getenv("FIREBASE_CREDENTIALS"))  # Debugging print

#     # Apply necessary patches before importing
#     with patch('firebase_admin.initialize_app', Mock()), \
#          patch('firebase_admin.credentials.Certificate', Mock(return_value=Mock())), \
#          patch('flask_sqlalchemy.SQLAlchemy.init_app', Mock()), \
#          patch('flask_migrate.Migrate.init_app', Mock()):

#         # Import after patching to avoid premature calls
#         from app.routes.characters import characters_bp
#         from app.models import PlayerCharacter

#     app = Flask(__name__)

#     # Configure the app
#     app.config.update({
#         'TESTING': True,
#         'SQLALCHEMY_DATABASE_URI': 'sqlite:///:memory:',
#         'SQLALCHEMY_TRACK_MODIFICATIONS': False,
#         'SECRET_KEY': 'test-secret-key'
#     })

#     # Mock database
#     mock_db = Mock()
#     mock_db.session = Mock()
#     mock_db.session.add = Mock()
#     mock_db.session.commit = Mock()
#     mock_db.session.query = Mock(return_value=Mock(filter_by=Mock(return_value=Mock(first=Mock(return_value=None)))))
#     app.extensions['sqlalchemy'] = mock_db

#     # Mock user for routes
#     mock_user = Mock()
#     mock_user.uid = "test_user_id"

#     # Wrap blueprint routes to bypass verify_firebase_token
#     def bypass_auth(route_func):
#         def wrapper(*args, **kwargs):
#             return route_func(mock_user, *args, **kwargs)
#         wrapper.__name__ = route_func.__name__  # Preserve function name
#         return wrapper

#     # Apply bypass to all blueprint routes
#     for rule in characters_bp.url_map.iter_rules():
#         endpoint = characters_bp.view_functions[rule.endpoint]
#         characters_bp.view_functions[rule.endpoint] = bypass_auth(endpoint)

#     # Register the modified blueprint
#     app.register_blueprint(characters_bp, url_prefix="/characters")

#     with app.app_context():
#         mock_db.create_all = Mock()
#         mock_db.drop_all = Mock()
#         yield app

# @pytest.fixture
# def client(app):
#     """Create a test client."""
#     return app.test_client()

# def test_create_character_success(client):
#     """Test successful character creation."""
#     character_data = {
#         "userID": "test_user_id",
#         "modelID": 1,
#         "hairID": 2,
#         "robeID": 3,
#         "bootID": 4
#     }

#     response = client.post(
#         '/characters/create',
#         data=json.dumps(character_data),
#         content_type='application/json'
#     )

#     assert response.status_code == 201
#     data = json.loads(response.data)
#     assert data["userID"] == "test_user_id"
#     assert data["modelID"] == 1
#     assert data["hairID"] == 2
#     assert data["robeID"] == 3
#     assert data["bootID"] == 4
#     assert "characterID" in data

# def test_create_character_missing_fields(client):
#     """Test character creation with missing required fields."""
#     character_data = {
#         "userID": "test_user_id"
#     }

#     response = client.post(
#         '/characters/create',
#         data=json.dumps(character_data),
#         content_type='application/json'
#     )

#     assert response.status_code == 400
#     data = json.loads(response.data)
#     assert "error" in data
#     assert data["error"] == "Missing required fields"

# def test_get_character_success(client, app):
#     """Test getting an existing character."""
#     with app.app_context():
#         character = PlayerCharacter(
#             userID="test_user_id",
#             modelID=1,
#             hairID=2,
#             robeID=3,
#             bootID=4
#         )
#         app.extensions['sqlalchemy'].session.add(character)
#         app.extensions['sqlalchemy'].session.commit()
        
#         response = client.get('/characters/test_user_id')
        
#         assert response.status_code == 200
#         data = json.loads(response.data)
#         assert data["userID"] == "test_user_id"
#         assert data["modelID"] == 1
#         assert data["hairID"] == 2
#         assert data["robeID"] == 3
#         assert data["bootID"] == 4

# def test_get_character_not_found(client):
#     """Test getting a non-existent character."""
#     response = client.get('/characters/non_existent_user')
    
#     assert response.status_code == 404
#     data = json.loads(response.data)
#     assert "error" in data
#     assert data["error"] == "Character not found"

# def test_update_character_success(client, app):
#     """Test updating an existing character."""
#     with app.app_context():
#         character = PlayerCharacter(
#             userID="test_user_id",
#             modelID=1,
#             hairID=2,
#             robeID=3,
#             bootID=4
#         )
#         app.extensions['sqlalchemy'].session.add(character)
#         app.extensions['sqlalchemy'].session.commit()
#         character_id = character.characterID

#     update_data = {
#         "modelID": 5,
#         "hairID": 6
#     }

#     response = client.put(
#         f'/characters/update/{character_id}',
#         data=json.dumps(update_data),
#         content_type='application/json'
#     )

#     assert response.status_code == 200
#     data = json.loads(response.data)
#     assert data["message"] == "Character updated successfully"

# def test_update_character_not_found(client):
#     """Test updating a non-existent character."""
#     update_data = {"modelID": 5}
    
#     response = client.put(
#         '/characters/update/999',
#         data=json.dumps(update_data),
#         content_type='application/json'
#     )

#     assert response.status_code == 404
#     data = json.loads(response.data)
#     assert "error" in data
#     assert data["error"] == "Character not found"

# def test_delete_character_success(client, app):
#     """Test deleting an existing character."""
#     with app.app_context():
#         character = PlayerCharacter(
#             userID="test_user_id",
#             modelID=1
#         )
#         app.extensions['sqlalchemy'].session.add(character)
#         app.extensions['sqlalchemy'].session.commit()
#         character_id = character.characterID

#     response = client.delete(f'/characters/delete/{character_id}')
    
#     assert response.status_code == 200
#     data = json.loads(response.data)
#     assert data["message"] == "Character deleted successfully"

# def test_delete_character_not_found(client):
#     """Test deleting a non-existent character."""
#     response = client.delete('/characters/delete/999')
    
#     assert response.status_code == 404
#     data = json.loads(response.data)
#     assert "error" in data
#     assert data["error"] == "Character not found"

# if __name__ == '__main__':
#     pytest.main()
# test_characters.py
import sys
import os
from unittest.mock import MagicMock, patch
import pytest
from flask import Flask, json

# Mock Firebase modules to prevent real initialization
sys.modules['firebase_admin'] = MagicMock()
sys.modules['firebase_admin.credentials'] = MagicMock()
sys.modules['firebase_admin.auth'] = MagicMock()

# Set up sys.path for backend imports
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture
def app():
    # Patch SQLAlchemy instance BEFORE blueprint import
    with patch('app.routes.characters.db') as mock_db:
        app = Flask(__name__)
        app.config["TESTING"] = True

        from app.routes.characters import characters_bp
        app.register_blueprint(characters_bp, url_prefix="/characters")

        return app

@pytest.fixture
def client(app):
    return app.test_client()

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_create_character_success(_, client):
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
        mock_character_instance.characterID = 1  # Simulate DB-assigned ID
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

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_create_character_missing_fields(_, client):
    payload = {"userID": "test_user_id"}

    response = client.post(
        "/characters/create",
        data=json.dumps(payload),
        content_type="application/json",
        headers={"Authorization": "Bearer test-token"}
    )

    assert response.status_code == 400
    assert response.get_json() == {"error": "Missing required fields"}

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_get_character_success(_, client):
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

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_get_character_not_found(_, client):
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

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_update_character_success(_, client):
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

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_update_character_not_found(_, client):
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

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_delete_character_success(_, client):
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

@patch('app.firebase_auth.verify_firebase_token', return_value=lambda x: x)
def test_delete_character_not_found(_, client):
    with patch('app.routes.characters.PlayerCharacter') as MockCharacter:
        MockCharacter.query.get.return_value = None

        response = client.delete(
            "/characters/delete/999",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 404
        assert response.get_json() == {"error": "Character not found"}

if __name__ == '__main__':
    pytest.main()

