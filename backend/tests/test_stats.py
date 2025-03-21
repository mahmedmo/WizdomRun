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
    with patch('app.routes.stats.verify_firebase_token', return_value={"uid": "test_user_id"}), \
         patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test_user_id"}):

        from app import create_app
        app = create_app()
        app.config["TESTING"] = True

        with app.app_context(), \
             patch('app.routes.stats.db') as mock_db, \
             patch('app.models.User.query') as mock_user_query:
            
            mock_user = MagicMock()
            mock_user.userID = "test_user_id"
            mock_filter = mock_user_query.filter_by.return_value
            mock_filter.first.return_value = mock_user

            yield app

@pytest.fixture
def client(app):
    return app.test_client()

def test_get_player_stats_success(client):
    with patch('app.routes.stats.PlayerStats') as MockPlayerStats:
        mock_stats = MagicMock()
        mock_stats.campaignID = 1
        mock_stats.attack = 10
        mock_stats.hp = 50
        mock_stats.mana = 30
        mock_stats.affinity = "fire"
        MockPlayerStats.query.get.return_value = mock_stats

        response = client.get("/stats/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == {
            "campaignID": 1,
            "attack": 10,
            "hp": 50,
            "mana": 30,
            "affinity": "fire"
        }

def test_get_player_stats_not_found(client):
    with patch('app.routes.stats.PlayerStats') as MockPlayerStats:
        MockPlayerStats.query.get.return_value = None

        response = client.get("/stats/999", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 404
        assert response.get_json() == {"error": "Player stats not found"}

def test_update_player_stats_success(client):
    with patch('app.routes.stats.PlayerStats') as MockPlayerStats, \
         patch('app.routes.stats.db.session') as mock_session:
        mock_stats = MagicMock()
        mock_stats.campaignID = 1
        mock_stats.attack = 10
        mock_stats.hp = 50
        mock_stats.mana = 30
        mock_stats.affinity = "fire"
        MockPlayerStats.query.get.return_value = mock_stats

        payload = {"attack": 15, "hp": 60}
        response = client.put(
            "/stats/update/1",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 200
        assert response.get_json() == {"message": "Player stats updated successfully"}
        assert mock_stats.attack == 15
        assert mock_stats.hp == 60
        mock_session.commit.assert_called_once()

def test_update_player_stats_not_found(client):
    with patch('app.routes.stats.PlayerStats') as MockPlayerStats:
        MockPlayerStats.query.get.return_value = None

        payload = {"attack": 15}
        response = client.put(
            "/stats/update/999",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 404
        assert response.get_json() == {"error": "Player stats not found"}

def test_get_spells_success(client):
    with patch('app.routes.stats.Spell') as MockSpell:
        mock_spell = MagicMock()
        mock_spell.spellID = 1
        mock_spell.spellName = "Fireball"
        mock_spell.spellElement = "fire"
        MockSpell.query.all.return_value = [mock_spell]

        response = client.get("/stats/spells", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == [{"spellID": 1, "name": "Fireball", "element": "fire"}]

def test_assign_spells_success(client):
    payload = {"campaignID": 1, "spellIDs": [1, 2, 3, 4]}

    with patch('app.routes.stats.PlayerSpells') as MockPlayerSpells, \
         patch('app.routes.stats.db.session') as mock_session:
        MockPlayerSpells.query.filter_by.return_value.delete.return_value = None

        response = client.post(
            "/stats/assign_spells",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 200
        assert response.get_json() == {"message": "Spells assigned successfully"}
        mock_session.bulk_save_objects.assert_called_once()
        mock_session.commit.assert_called_once()

def test_assign_spells_invalid_payload(client):
    payload = {"campaignID": 1, "spellIDs": [1, 2]}  # Less than 4 spellIDs

    with patch('app.routes.stats.PlayerSpells'), \
         patch('app.routes.stats.db.session'):
        response = client.post(
            "/stats/assign_spells",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 400
        assert response.get_json() == {"error": "Request must contain 'campaignID' and exactly 4 'spellIDs'"}

def test_get_player_spells_success(client):
    with patch('app.routes.stats.db.session') as mock_session:
        mock_ps = MagicMock()
        mock_ps.playerspellID = 1
        mock_ps.spellID = 1
        mock_ps.playerID = 1
        mock_spell = MagicMock()
        mock_spell.spellID = 1
        mock_spell.spellName = "Fireball"
        mock_spell.spellElement = "fire"
        mock_query = mock_session.query.return_value
        mock_query.join.return_value.filter.return_value.all.return_value = [(mock_ps, mock_spell)]

        response = client.get("/stats/player_spells/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == [{
            "playerspellID": 1,
            "spellID": 1,
            "spellName": "Fireball",
            "spellElement": "fire"
        }]

def test_get_player_spells_not_found(client):
    with patch('app.routes.stats.db.session') as mock_session:
        mock_query = mock_session.query.return_value
        mock_query.join.return_value.filter.return_value.all.return_value = []

        response = client.get("/stats/player_spells/999", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 404
        assert response.get_json() == {"error": "No spells found for this player"}

def test_create_player_stats_success(client):
    payload = {
        "campaignID": 1,
        "attack": 10,
        "hp": 50,
        "mana": 30,
        "affinity": "fire"
    }

    with patch('app.routes.stats.PlayerStats') as MockPlayerStats, \
         patch('app.routes.stats.db.session') as mock_session:
        response = client.post(
            "/stats/create",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 200
        assert response.get_json() == {"message": "Player stats created successfully"}
        mock_session.add.assert_called_once()
        mock_session.commit.assert_called_once()

def test_delete_player_spell_success(client):
    with patch('app.routes.stats.PlayerSpells') as MockPlayerSpells, \
         patch('app.routes.stats.db.session') as mock_session:
        mock_spell = MagicMock()
        MockPlayerSpells.query.get.return_value = mock_spell

        response = client.delete("/stats/player_spells/delete/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == {"message": "Spell removed successfully"}
        mock_session.delete.assert_called_once_with(mock_spell)
        mock_session.commit.assert_called_once()

def test_delete_player_spell_not_found(client):
    with patch('app.routes.stats.PlayerSpells') as MockPlayerSpells:
        MockPlayerSpells.query.get.return_value = None

        response = client.delete("/stats/player_spells/delete/999", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 404
        assert response.get_json() == {"error": "Spell not found"}

def test_create_spell_success(client):
    payload = {
        "spellName": "Fireball",
        "description": "A fiery projectile",
        "spellElement": "fire"
    }

    with patch('app.routes.stats.Spell') as MockSpell, \
         patch('app.routes.stats.db.session') as mock_session:
        response = client.post(
            "/stats/spells/create",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 200
        assert response.get_json() == {"message": "Spell created successfully"}
        mock_session.add.assert_called_once()
        mock_session.commit.assert_called_once()

def test_replenish_mana_success(client):
    with patch('app.routes.stats.PlayerStats') as MockPlayerStats, \
         patch('app.routes.stats.db.session') as mock_session:
        mock_stats = MagicMock()
        mock_stats.campaignID = 1
        mock_stats.mana = 70
        MockPlayerStats.query.get.return_value = mock_stats

        payload = {"manaAmount": 20}
        response = client.patch(
            "/stats/replenish_mana/1",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 200
        assert response.get_json() == {"message": "Mana replenished successfully", "newMana": 90}
        assert mock_stats.mana == 90
        mock_session.commit.assert_called_once()

def test_replenish_mana_missing_amount(client):
    with patch('app.routes.stats.PlayerStats'):
        payload = {}
        response = client.patch(
            "/stats/replenish_mana/1",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 400
        assert response.get_json() == {"error": "Missing manaAmount"}

def test_replenish_mana_not_found(client):
    with patch('app.routes.stats.PlayerStats') as MockPlayerStats:
        MockPlayerStats.query.get.return_value = None

        payload = {"manaAmount": 20}
        response = client.patch(
            "/stats/replenish_mana/999",
            data=json.dumps(payload),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 404
        assert response.get_json() == {"error": "Player stats not found"}

def pytest_sessionfinish():
    os_getenv_patcher.stop()

if __name__ == '__main__':
    pytest.main()