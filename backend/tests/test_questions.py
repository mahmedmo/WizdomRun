import sys
import os
from unittest.mock import MagicMock, patch
import pytest
from flask import json, jsonify

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
    if key == "TEMP":  # For Windows temp dir in create_questions
        return os.path.join(os.path.abspath(os.path.dirname(__file__)), "temp")
    return default

# Apply os.getenv patch at module level
os_getenv_patcher = patch('os.getenv', side_effect=mock_getenv)
os_getenv_patcher.start()

@pytest.fixture
def app():
    with patch('app.routes.questions.verify_firebase_token', return_value={"uid": "test_user_id"}), \
         patch('app.firebase_auth.verify_firebase_token', return_value={"uid": "test_user_id"}):

        from app import create_app
        app = create_app()
        app.config["TESTING"] = True

        with app.app_context(), \
             patch('app.routes.questions.db') as mock_db, \
             patch('app.models.User.query') as mock_user_query:
            
            mock_user = MagicMock()
            mock_user.userID = "test_user_id"
            mock_filter = mock_user_query.filter_by.return_value
            mock_filter.first.return_value = mock_user

            yield app

@pytest.fixture
def client(app):
    return app.test_client()

def test_get_questions(client):
    with patch('app.routes.questions.Question') as MockQuestion:
        mock_question = MagicMock()
        mock_question.questionID = 1
        mock_question.difficulty = "easy"
        mock_question.questionStr = "What is 2+2?"
        mock_question.gotCorrect = False
        mock_question.wrongAttempts = 0
        MockQuestion.query.filter_by.return_value.all.return_value = [mock_question]

        response = client.get("/questions/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == [{
            "questionID": 1,
            "difficulty": "easy",
            "questionStr": "What is 2+2?",
            "gotCorrect": False,
            "wrongAttempts": 0
        }]

def test_answer_question_success(client):
    with patch('app.routes.questions.Question') as MockQuestion, \
         patch('app.routes.questions.db.session') as mock_session:
        mock_question = MagicMock()
        mock_question.gotCorrect = False
        MockQuestion.query.get.return_value = mock_question

        response = client.put(
            "/questions/answer/1",
            data=json.dumps({"gotCorrect": True}),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 200
        assert response.get_json() == {"message": "Answer recorded successfully"}
        assert mock_question.gotCorrect is True
        mock_session.commit.assert_called_once()

def test_answer_question_not_found(client):
    with patch('app.routes.questions.Question') as MockQuestion:
        MockQuestion.query.get.return_value = None

        response = client.put(
            "/questions/answer/999",
            data=json.dumps({"gotCorrect": True}),
            content_type="application/json",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 404
        assert response.get_json() == {"error": "Question not found"}

from functools import wraps

def test_batch_create_questions_success(app):
    payload = [{
        "campaignID": 1,
        "difficulty": "easy",
        "questionStr": "What is 2+2?",
        "answers": [
            {"answerStr": "4", "isCorrect": True},
            {"answerStr": "5", "isCorrect": False}
        ]
    }]

    # Get the original function without the decorator
    from app.routes.questions import batch_create_questions
    original_func = batch_create_questions.__wrapped__  # Access the undecorated function

    with patch('app.routes.questions.Question') as MockQuestion, \
         patch('app.routes.questions.Answer') as MockAnswer, \
         patch('app.routes.questions.db.session') as mock_session:
        mock_question = MockQuestion.return_value
        mock_question.questionID = 1
        mock_session.begin_nested.return_value.__enter__.return_value = None

        with app.app_context():  # For DB session
            response = original_func({"uid": "test_user_id"}, payload)

        assert response[1] == 201
        assert response[0].get_json() == {"message": "Questions and answers created successfully"}
        mock_session.add.assert_called_once()
        mock_session.bulk_save_objects.assert_called_once()
        mock_session.commit.assert_called_once()

def test_batch_create_questions_missing_fields(app):
    payload = [{"campaignID": 1}]  # Missing required fields

    with patch('app.routes.questions.Question'), \
         patch('app.routes.questions.Answer'), \
         patch('app.routes.questions.db.session'):
        from app.routes.questions import batch_create_questions
        original_func = batch_create_questions.__wrapped__
        with app.app_context():
            response = original_func({"uid": "test_user_id"}, payload)

        assert response[1] == 400
        assert response[0].get_json() == {"error": "Missing required fields in one or more questions"}

def test_get_question_success(client):
    with patch('app.routes.questions.Question') as MockQuestion:
        mock_question = MagicMock()
        mock_question.questionID = 1
        mock_question.campaignID = 1
        mock_question.difficulty = "easy"
        mock_question.questionStr = "What is 2+2?"
        mock_question.gotCorrect = False
        mock_question.wrongAttempts = 0
        MockQuestion.query.get.return_value = mock_question

        response = client.get("/questions/question/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == {
            "questionID": 1,
            "campaignID": 1,
            "difficulty": "easy",
            "questionStr": "What is 2+2?",
            "gotCorrect": False,
            "wrongAttempts": 0
        }

def test_delete_question_success(client):
    with patch('app.routes.questions.Question') as MockQuestion, \
         patch('app.routes.questions.db.session') as mock_session:
        mock_question = MagicMock()
        MockQuestion.query.get.return_value = mock_question

        response = client.delete("/questions/delete/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == {"message": "Question deleted successfully"}
        mock_session.delete.assert_called_once_with(mock_question)
        mock_session.commit.assert_called_once()

def test_get_answers_success(client):
    with patch('app.routes.questions.Answer') as MockAnswer:
        mock_answer = MagicMock()
        mock_answer.answerID = 1
        mock_answer.questionID = 1
        mock_answer.answerStr = "4"
        mock_answer.isCorrect = True
        MockAnswer.query.filter_by.return_value.all.return_value = [mock_answer]

        response = client.get("/questions/answers/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == [{
            "answerID": 1,
            "questionID": 1,
            "answerStr": "4",
            "isCorrect": True
        }]

def test_increment_wrong_attempt_success(client):
    with patch('app.routes.questions.Question') as MockQuestion, \
         patch('app.routes.questions.db.session') as mock_session:
        mock_question = MagicMock()
        mock_question.wrongAttempts = 0
        MockQuestion.query.get.return_value = mock_question

        response = client.put("/questions/wrong_attempt/1", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == {"message": "Wrong attempt recorded", "questionID": 1, "wrongAttempts": 1}
        assert mock_question.wrongAttempts == 1
        mock_session.commit.assert_called_once()

def test_get_unanswered_questions(client):
    with patch('app.routes.questions.Question') as MockQuestion:
        mock_question = MagicMock()
        mock_question.questionID = 1
        mock_question.campaignID = 1
        mock_question.difficulty = "easy"
        mock_question.questionStr = "What is 2+2?"
        mock_question.wrongAttempts = 0
        MockQuestion.query.filter_by.return_value.all.return_value = [mock_question]

        response = client.get("/questions/unanswered", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == [{
            "questionID": 1,
            "campaignID": 1,
            "difficulty": "easy",
            "questionStr": "What is 2+2?",
            "wrongAttempts": 0
        }]

def test_get_questions_by_difficulty_success(client):
    with patch('app.routes.questions.Question') as MockQuestion:
        mock_question = MagicMock()
        mock_question.questionID = 1
        mock_question.campaignID = 1
        mock_question.questionStr = "What is 2+2?"
        mock_question.gotCorrect = False
        mock_question.wrongAttempts = 0
        MockQuestion.query.filter_by.return_value.all.return_value = [mock_question]

        response = client.get("/questions/difficulty/easy", headers={"Authorization": "Bearer test-token"})
        assert response.status_code == 200
        assert response.get_json() == [{
            "questionID": 1,
            "campaignID": 1,
            "questionStr": "What is 2+2?",
            "gotCorrect": False,
            "wrongAttempts": 0
        }]

def test_create_questions_success(client):
    with patch('app.routes.questions.Campaign') as MockCampaign, \
         patch('app.routes.questions.run_qa_session') as mock_run_qa, \
         patch('app.routes.questions.batch_create_questions') as mock_batch_create, \
         patch('os.path.exists', return_value=True), \
         patch('os.remove') as mock_remove, \
         patch('os.makedirs') as mock_makedirs, \
         patch('werkzeug.datastructures.FileStorage.save') as mock_save:
        mock_campaign = MagicMock()
        mock_campaign.userID = "test_user_id"
        mock_campaign.campaignID = "1"
        mock_campaign.campaignLength = "quest"
        MockCampaign.query.filter_by.return_value.first.return_value = mock_campaign
        mock_run_qa.return_value = [{"campaignID": 1, "difficulty": "easy", "questionStr": "Test?", "answers": [{"answerStr": "Yes", "isCorrect": True}, {"answerStr": "No", "isCorrect": False}]}]
        mock_batch_create.return_value = (jsonify({"message": "Questions created"}), 201)

        response = client.post(
            "/questions/create",
            data={"file": (open(os.path.join(os.path.dirname(__file__), "test.pdf"), "rb"), "test.pdf"), "campaignID": "1"},
            content_type="multipart/form-data",
            headers={"Authorization": "Bearer test-token"}
        )
        assert response.status_code == 201
        assert response.get_json() == {"message": "Questions created"}
        mock_run_qa.assert_called_once()
        mock_batch_create.assert_called_once()
        mock_remove.assert_called_once()

def pytest_sessionfinish():
    os_getenv_patcher.stop()
    temp_dir = os.path.join(os.path.abspath(os.path.dirname(__file__)), "temp")
    test_pdf_path = os.path.join(temp_dir, "test.pdf")
    if os.path.exists(test_pdf_path):
        os.remove(test_pdf_path)
    if os.path.exists(temp_dir) and not os.listdir(temp_dir):
        os.rmdir(temp_dir)

if __name__ == '__main__':
    pytest.main()