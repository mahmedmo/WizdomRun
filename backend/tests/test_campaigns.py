import sys
import os
import pytest
from flask import Flask
from unittest.mock import patch, MagicMock

sys.modules['firebase_admin'] = MagicMock()
sys.modules['firebase_admin.credentials'] = MagicMock()
sys.modules['firebase_admin.auth'] = MagicMock()

sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture
def app():
    with patch('app.routes.campaigns.db') as mock_db:
        app = Flask(__name__)
        app.config['TESTING'] = True

        from app.routes.campaigns import campaigns_bp
        app.register_blueprint(campaigns_bp, url_prefix="/campaigns")

        return app

@pytest.fixture
def client(app):
    return app.test_client()

@patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test-user"})
def test_create_campaign(_, client):
    with patch("app.routes.campaigns.Campaign") as MockCampaign, \
         patch("app.routes.campaigns.db.session") as mock_session:
         
        mock_instance = MagicMock()
        MockCampaign.return_value = mock_instance
        mock_instance.campaignID = 1
        mock_instance.userID = "test-user"
        mock_instance.title = "Test Campaign"
        mock_instance.campaignLength = 10
        mock_instance.currLevel = 1
        mock_instance.remainingTries = 2

        payload = {
            "userID": "test-user",
            "title": "Test Campaign",
            "campaignLength": 10,
            "currLevel": 1
        }

        response = client.post("/campaigns/create", json=payload, 
                               headers={"Authorization": "Bearer test-token"})

        assert response.status_code == 201
        data = response.get_json()
        assert data["campaignID"] == 1
        assert data["title"] == "Test Campaign"
        assert data["userID"] == "test-user"
        assert data["campaignLength"] == 10
        assert data["currLevel"] == 1


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_create_campaign_missing_fields(mock_verify_token, client):
    payload = {
        "userID": "test-user",
        "title": "Test Campaign"
        # Missing campaignLength, currLevel
    }

    with patch("app.routes.campaigns.db.session") as mock_session:
        response = client.post("/campaigns/create", json=payload, headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 400
        assert response.get_json()["error"] == "Missing required fields"


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_get_campaigns(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.routes.campaigns.db") as mock_db, \
         patch('app.firebase_auth.User') as MockUser:

        # Create a mock campaign instance
        mock_campaign = MagicMock()
        mock_campaign.campaignID = 1
        mock_campaign.title = "Test Campaign"
        mock_campaign.campaignLength = 10
        mock_campaign.currLevel = 1
        mock_campaign.remainingTries = 2
        mock_campaign.lastUpdated = "2024-01-01"

        # Mock the query behavior
        mock_query = MagicMock()
        mock_query.filter_by.return_value.all.return_value = [mock_campaign]
        mock_campaign_class.query = mock_query

        # Make request
        response = client.get("/campaigns/test-user", headers={"Authorization": "Bearer test-token"})

        # Assertions
        assert response.status_code == 200
        data = response.get_json()
        assert isinstance(data, list)
        assert data[0]["campaignID"] == 1
        assert data[0]["title"] == "Test Campaign"
        assert data[0]["campaignLength"] == 10
        assert data[0]["currLevel"] == 1
        assert data[0]["remainingTries"] == 2
        assert data[0]["lastUpdated"] == "2024-01-01"


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_update_campaign(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.routes.campaigns.db") as mock_db, \
         patch("app.firebase_auth.User") as MockUser:

        mock_campaign = MagicMock()
        mock_campaign.campaignID = 1
        mock_campaign.title = "Test Campaign"
        mock_campaign.currLevel = 1
        mock_campaign.remainingTries = 2
        mock_campaign.lastUpdated = "2024-01-01"

        mock_campaign_class.query.get.return_value = mock_campaign
        mock_db.func.current_timestamp.return_value = "2025-03-20"

        response = client.put(
            "/campaigns/update/1",
            json={"currLevel": 5, "remainingTries": 3},
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 200
        assert response.get_json()["message"] == "Campaign updated successfully"


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_update_campaign_not_found(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.routes.campaigns.db") as mock_db, \
         patch("app.firebase_auth.User") as MockUser:

        mock_campaign_class.query.get.return_value = None

        response = client.put(
            "/campaigns/update/999",  # Non-existent ID
            json={"currLevel": 5, "remainingTries": 3},
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 404
        assert response.get_json() == {"error": "Campaign not found"}



@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_delete_campaign(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.routes.campaigns.db") as mock_db, \
         patch("app.firebase_auth.User") as MockUser:

        mock_campaign = MagicMock()
        mock_campaign_class.query.get.return_value = mock_campaign

        response = client.delete(
            "/campaigns/delete/1",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 200
        assert response.get_json()["message"] == "Campaign deleted successfully"


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_delete_campaign_not_found(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.routes.campaigns.db") as mock_db, \
         patch("app.firebase_auth.User") as MockUser:

        mock_campaign_class.query.get.return_value = None

        response = client.delete(
            "/campaigns/delete/999",  # Non-existent ID
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 404
        assert response.get_json() == {"error": "Campaign not found"}


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_get_single_campaign(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.firebase_auth.User") as MockUser:

        mock_campaign = MagicMock()
        mock_campaign.campaignID = 1
        mock_campaign.title = "Test Campaign"
        mock_campaign.campaignLength = 10
        mock_campaign.currLevel = 3
        mock_campaign.remainingTries = 1

        mock_campaign_class.query.get.return_value = mock_campaign

        response = client.get("/campaigns/single/1", headers={"Authorization": "Bearer test-token"})

        assert response.status_code == 200
        data = response.get_json()
        assert data["campaignID"] == 1
        assert data["title"] == "Test Campaign"
        assert data["campaignLength"] == 10
        assert data["currLevel"] == 3
        assert data["remainingTries"] == 1


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_restart_campaign(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.routes.campaigns.db") as mock_db, \
         patch("app.firebase_auth.User") as MockUser:

        mock_campaign = MagicMock()
        mock_campaign_class.query.get.return_value = mock_campaign
        mock_db.func.current_timestamp.return_value = "2025-03-20"

        response = client.patch(
            "/campaigns/1/restart",
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 200
        assert response.get_json()["message"] == "Campaign restarted successfully"


@patch("app.firebase_auth.verify_firebase_token", return_value={"uid": "test-user"})
def test_restart_campaign_not_found(mock_verify_token, client):
    with patch("app.routes.campaigns.Campaign") as mock_campaign_class, \
         patch("app.routes.campaigns.db") as mock_db, \
         patch("app.firebase_auth.User") as MockUser:

        mock_campaign_class.query.get.return_value = None

        response = client.patch(
            "/campaigns/999/restart",  # Non-existent ID
            headers={"Authorization": "Bearer test-token"}
        )

        assert response.status_code == 404
        assert response.get_json() == {"error": "Campaign not found"}

