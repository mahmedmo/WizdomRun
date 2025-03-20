import unittest
from unittest.mock import Mock, patch
from flask import Flask, json
import os
import sys

# Add project root to sys.path to ensure 'backend' is importable
project_root = os.path.abspath(os.path.join(os.path.dirname(__file__), '..'))
sys.path.insert(0, project_root)

patch('firebase_admin.credentials.Certificate', return_value=Mock()).start()
patch('firebase_admin.initialize_app').start()

# Correctly formatted mock Firebase credentials
mock_firebase_creds = {
    "type": "service_account",
    "project_id": "your_project_id",
    "private_key_id": "your_private_key_id",
    "private_key": "-----BEGIN PRIVATE KEY-----\n...\n-----END PRIVATE KEY-----\n",
    "client_email": "your_client_email@your_project_id.iam.gserviceaccount.com",
    "client_id": "your_client_id",
    "auth_uri": "https://accounts.google.com/o/oauth2/auth",
    "token_uri": "https://oauth2.googleapis.com/token",
    "auth_provider_x509_cert_url": "https://www.googleapis.com/oauth2/v1/certs",
    "client_x509_cert_url": "https://www.googleapis.com/robot/v1/metadata/x509/your_client_email%40your_project_id.iam.gserviceaccount.com",
    "universe_domain": "googleapis.com"
}

os.environ['FIREBASE_ADMIN_CREDENTIALS'] = json.dumps(mock_firebase_creds)

print("FIREBASE_ADMIN_CREDENTIALS:", os.environ['FIREBASE_ADMIN_CREDENTIALS'][:50])

# Alias 'app' to 'backend.app' to match users.py's import without changing it
import backend.app
sys.modules['app'] = backend.app


# Mock PlayerCharacter model for testing
class MockPlayerCharacter:
    def __init__(self, userID, modelID, hairID=None, robeID=None, bootID=None):
        self.characterID = 1
        self.userID = userID
        self.modelID = modelID
        self.hairID = hairID
        self.robeID = robeID
        self.bootID = bootID

    def save(self):
        pass

#Set up module-level patching to ensure it happens before any imports
def setUpModule():
    #Patch environment variable and Firebase before any imports
    patch.dict(os.environ, {'FIREBASE_ADMIN_CREDENTIALS': json.dumps(mock_firebase_creds)}).__enter__()
    patch('firebase_admin.credentials.Certificate', return_value=Mock()).start()
    patch('firebase_admin.initialize_app').start()

def tearDownModule():
    patch.stopall()
    # Clean up sys.modules to avoid affecting other tests
    if 'app' in sys.modules:
        del sys.modules['app']

class TestCharactersBlueprint(unittest.TestCase):
    def setUp(self):
        self.app = Flask(__name__)
        from backend.app.routes.characters import characters_bp
        self.app.register_blueprint(characters_bp)
        self.client = self.app.test_client()

        with patch('backend.app.extensions.db', autospec=True) as mock_db:
            mock_db.session = Mock()
            self.mock_db = mock_db

        self.mock_user = {"uid": "test_user"}

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter.query')
    def test_get_character_success(self, mock_query, mock_verify):
        mock_verify.return_value = self.mock_user
        mock_character = MockPlayerCharacter(userID="test_user", modelID=1)
        mock_query.filter_by().first.return_value = mock_character

        response = self.client.get('/test_user', headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 200)
        self.assertEqual(data["userID"], "test_user")
        self.assertEqual(data["modelID"], 1)

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter.query')
    def test_get_character_not_found(self, mock_query, mock_verify):
        mock_verify.return_value = self.mock_user
        mock_query.filter_by().first.return_value = None

        response = self.client.get('/test_user', headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 404)
        self.assertEqual(data["error"], "Character not found")

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter')
    def test_create_character_success(self, mock_player, mock_verify):
        mock_verify.return_value = self.mock_user
        mock_instance = MockPlayerCharacter(userID="test_user", modelID=1, hairID=2, robeID=3, bootID=4)
        mock_player.return_value = mock_instance
        mock_player.query = Mock()

        response = self.client.post('/create', 
                                  data=json.dumps({"userID": "test_user", "modelID": 1, "hairID": 2, "robeID": 3, "bootID": 4}),
                                  content_type='application/json',
                                  headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 201)
        self.assertEqual(data["userID"], "test_user")
        self.assertEqual(data["modelID"], 1)
        self.assertEqual(data["hairID"], 2)
        self.assertEqual(data["robeID"], 3)
        self.assertEqual(data["bootID"], 4)

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter.query')
    def test_create_character_missing_fields(self, mock_query, mock_verify):
        mock_verify.return_value = self.mock_user

        response = self.client.post('/create', 
                                  data=json.dumps({"userID": "test_user"}),  # Missing modelID
                                  content_type='application/json',
                                  headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 400)
        self.assertEqual(data["error"], "Missing required fields")

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter.query')
    def test_update_character_success(self, mock_query, mock_verify):
        mock_verify.return_value = self.mock_user
        mock_character = MockPlayerCharacter(userID="test_user", modelID=1)
        mock_query.get.return_value = mock_character

        response = self.client.put('/update/1', 
                                 data=json.dumps({"modelID": 2, "hairID": 5}),
                                 content_type='application/json',
                                 headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 200)
        self.assertEqual(data["message"], "Character updated successfully")
        self.assertEqual(mock_character.modelID, 2)
        self.assertEqual(mock_character.hairID, 5)

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter.query')
    def test_update_character_not_found(self, mock_query, mock_verify):
        mock_verify.return_value = self.mock_user
        mock_query.get.return_value = None

        response = self.client.put('/update/1', 
                                 data=json.dumps({"modelID": 2}),
                                 content_type='application/json',
                                 headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 404)
        self.assertEqual(data["error"], "Character not found")

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter.query')
    def test_delete_character_success(self, mock_query, mock_verify):
        mock_verify.return_value = self.mock_user
        mock_character = MockPlayerCharacter(userID="test_user", modelID=1)
        mock_query.get.return_value = mock_character

        response = self.client.delete('/delete/1', headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 200)
        self.assertEqual(data["message"], "Character deleted successfully")

    @patch('backend.app.routes.characters.verify_firebase_token')
    @patch('backend.app.routes.characters.PlayerCharacter.query')
    def test_delete_character_not_found(self, mock_query, mock_verify):
        mock_verify.return_value = self.mock_user
        mock_query.get.return_value = None

        response = self.client.delete('/delete/1', headers={'Authorization': 'Bearer token'})
        data = json.loads(response.data)

        self.assertEqual(response.status_code, 404)
        self.assertEqual(data["error"], "Character not found")

if __name__ == '__main__':
    unittest.main()

# # Patch Firebase credentials in environment variables and check if they are set correctly
# with patch.dict(os.environ, {'FIREBASE_CREDENTIALS': mock_firebase_creds}):
#     # Ensure the environment variable is correctly patched
#     print("Firebase Credentials Set:", os.environ.get('FIREBASE_CREDENTIALS'))  # Debugging line

#     with patch('backend.app.extensions.firebase_admin', autospec=True) as mock_firebase:
#         mock_firebase.initialize_app.return_value = Mock()  # Mock the Firebase app initialization
        
#         # Import the required modules after patching
#         from backend.app.routes.characters import characters_bp
#         from backend.app.extensions import db

#     # Set up test app
#     app = None
#     with patch('backend.app.routes.characters.db', autospec=True) as mock_db:
#         app = Mock()
#         app.config = {}
#         characters_bp.app = app
#         mock_db.session = Mock()

#         class TestCharactersBlueprint(unittest.TestCase):
#             def setUp(self):
#                 self.client = app.test_client()
#                 self.mock_user = {"uid": "test_user"}

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter.query')
#             def test_get_character_success(self, mock_query, mock_verify):
#                 mock_verify.return_value = self.mock_user
#                 mock_character = MockPlayerCharacter(userID="test_user", modelID=1)
#                 mock_query.filter_by().first.return_value = mock_character

#                 response = self.client.get('/test_user', headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 200)
#                 self.assertEqual(data["userID"], "test_user")
#                 self.assertEqual(data["modelID"], 1)

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter.query')
#             def test_get_character_not_found(self, mock_query, mock_verify):
#                 mock_verify.return_value = self.mock_user
#                 mock_query.filter_by().first.return_value = None

#                 response = self.client.get('/test_user', headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 404)
#                 self.assertEqual(data["error"], "Character not found")

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter')
#             def test_create_character_success(self, mock_player, mock_verify):
#                 mock_verify.return_value = self.mock_user
#                 mock_instance = MockPlayerCharacter(userID="test_user", modelID=1, hairID=2, robeID=3, bootID=4)
#                 mock_player.return_value = mock_instance
#                 mock_player.query = Mock()

#                 response = self.client.post('/create', 
#                                           data=json.dumps({"userID": "test_user", "modelID": 1, "hairID": 2, "robeID": 3, "bootID": 4}),
#                                           content_type='application/json',
#                                           headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 201)
#                 self.assertEqual(data["userID"], "test_user")
#                 self.assertEqual(data["modelID"], 1)
#                 self.assertEqual(data["hairID"], 2)
#                 self.assertEqual(data["robeID"], 3)
#                 self.assertEqual(data["bootID"], 4)

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter.query')
#             def test_create_character_missing_fields(self, mock_query, mock_verify):
#                 mock_verify.return_value = self.mock_user

#                 response = self.client.post('/create', 
#                                           data=json.dumps({"userID": "test_user"}),  # Missing modelID
#                                           content_type='application/json',
#                                           headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 400)
#                 self.assertEqual(data["error"], "Missing required fields")

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter.query')
#             def test_update_character_success(self, mock_query, mock_verify):
#                 mock_verify.return_value = self.mock_user
#                 mock_character = MockPlayerCharacter(userID="test_user", modelID=1)
#                 mock_query.get.return_value = mock_character

#                 response = self.client.put('/update/1', 
#                                          data=json.dumps({"modelID": 2, "hairID": 5}),
#                                          content_type='application/json',
#                                          headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 200)
#                 self.assertEqual(data["message"], "Character updated successfully")
#                 self.assertEqual(mock_character.modelID, 2)
#                 self.assertEqual(mock_character.hairID, 5)

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter.query')
#             def test_update_character_not_found(self, mock_query, mock_verify):
#                 mock_verify.return_value = self.mock_user
#                 mock_query.get.return_value = None

#                 response = self.client.put('/update/1', 
#                                          data=json.dumps({"modelID": 2}),
#                                          content_type='application/json',
#                                          headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 404)
#                 self.assertEqual(data["error"], "Character not found")

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter.query')
#             def test_delete_character_success(self, mock_query, mock_verify):
#                 mock_verify.return_value = self.mock_user
#                 mock_character = MockPlayerCharacter(userID="test_user", modelID=1)
#                 mock_query.get.return_value = mock_character

#                 response = self.client.delete('/delete/1', headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 200)
#                 self.assertEqual(data["message"], "Character deleted successfully")

#             @patch('backend.app.routes.characters.verify_firebase_token')
#             @patch('backend.app.routes.characters.PlayerCharacter.query')
#             def test_delete_character_not_found(self, mock_query, mock_verify):
#                 mock_verify.return_value = self.mock_user
#                 mock_query.get.return_value = None

#                 response = self.client.delete('/delete/1', headers={'Authorization': 'Bearer token'})
#                 data = json.loads(response.data)

#                 self.assertEqual(response.status_code, 404)
#                 self.assertEqual(data["error"], "Character not found")

# if __name__ == '__main__':
#     unittest.main()
