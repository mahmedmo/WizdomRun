import sys
import os
from unittest.mock import MagicMock, patch
import pytest
from flask import Flask

sys.modules['firebase_admin'] = MagicMock()
sys.modules['firebase_admin.credentials'] = MagicMock()
sys.modules['firebase_admin.auth'] = MagicMock()

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture
def app():
    # Patch SQLAlchemy instance BEFORE blueprint import
    with patch('app.routes.achievements.db') as mock_db:
        app = Flask(__name__)
        app.config["TESTING"] = True

        from app.routes.achievements import achievements_bp
        app.register_blueprint(achievements_bp, url_prefix="/achievements")

        return app

@pytest.fixture
def client(app):
    return app.test_client()


@patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test-user"})
def test_unlock_achievement(_, client):
    payload = {
        "campaignID": 1,
        "title": "Test Achievement",
        "description": "This is a test achievement"
    }

    with patch('app.routes.achievements.db.session') as mock_session, \
         patch('app.routes.achievements.Achievement') as MockAchievement:

        mock_achievement_instance = MockAchievement.return_value

        response = client.post("/achievements/unlock", json=payload,
                               headers={"Authorization": "Bearer test-token"})

        assert response.status_code == 200
        assert response.get_json() == {"message": "Achievement unlocked"}
        mock_session.add.assert_called_once_with(mock_achievement_instance)
        mock_session.commit.assert_called_once()


@patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test-user"})
def test_get_achievements(_, client):
    with patch('app.routes.achievements.Achievement') as MockAchievement, \
         patch('app.firebase_auth.User') as MockUser:
        
        # Mock the user query inside the decorator
        mock_user_query = MockUser.query
        mock_user_query.filter_by().first() == MagicMock()

       
        mock_query = MockAchievement.query
        mock_filter = mock_query.filter_by.return_value
        mock_filter.all.return_value = [
            MagicMock(achievementID=1, title="Title 1", description="Desc 1"),
            MagicMock(achievementID=2, title="Title 2", description="Desc 2"),
        ]

        response = client.get("/achievements/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200


@patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test-user"})
def test_delete_achievement(_, client):
    mock_achievement = MagicMock()

    with patch('app.routes.achievements.Achievement') as MockAchievement, \
         patch('app.routes.achievements.db.session') as mock_session:

        MockAchievement.query.get.return_value = mock_achievement

        response = client.delete("/achievements/delete/1", headers={"Authorization": "Bearer test-token"})

        assert response.status_code == 200
        assert response.get_json() == {"message": "Achievement deleted successfully"}
        MockAchievement.query.get.assert_called_once_with(1)
        mock_session.delete.assert_called_once_with(mock_achievement)
        mock_session.commit.assert_called_once()


@patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test-user"})
def test_delete_achievement_not_found(_, client):
    with patch('app.routes.achievements.Achievement') as MockAchievement, \
         patch('app.firebase_auth.User') as MockUser:
        
        MockUser.query.filter_by.return_value.first.return_value = MagicMock()
        MockAchievement.query.get.return_value = None

        response = client.delete("/achievements/delete/999", headers={"Authorization": "Bearer test-token"})

        assert response.status_code == 404
        assert response.get_json() == {"error": "Achievement not found"}
