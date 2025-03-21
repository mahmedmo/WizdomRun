# WizdomRun\llm\tests\test_qa_app.py
import sys
import os
from unittest.mock import patch, MagicMock
import pytest

# Adjust sys.path to import from WizdomRun\llm
sys.path.insert(0, os.path.abspath(os.path.join(os.path.dirname(__file__), '..')))

@pytest.fixture(scope="session", autouse=True)
def setup_test_pdf():
    # Create a fully valid PDF file in the tests directory
    test_dir = os.path.dirname(__file__)
    test_pdf_path = os.path.join(test_dir, "test.pdf")
    valid_pdf = (
        b"%PDF-1.0\n"
        b"1 0 obj\n"
        b"<< /Type /Catalog /Pages 2 0 R >>\n"
        b"endobj\n"
        b"2 0 obj\n"
        b"<< /Type /Pages /Kids [3 0 R] /Count 1 >>\n"
        b"endobj\n"
        b"3 0 obj\n"
        b"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Contents 4 0 R /Resources << /Font << /F1 << /Type /Font /Subtype /Type1 /BaseFont /Helvetica >> >> >> >>\n"
        b"endobj\n"
        b"4 0 obj\n"
        b"<< /Length 44 >>\n"
        b"stream\n"
        b"BT /F1 12 Tf 100 700 Td (Sample content) Tj ET\n"
        b"endstream\n"
        b"endobj\n"
        b"xref\n"
        b"0 5\n"
        b"0000000000 65535 f \n"
        b"0000000010 00000 n \n"
        b"0000000055 00000 n \n"
        b"0000000100 00000 n \n"
        b"0000000220 00000 n \n"
        b"trailer\n"
        b"<< /Size 5 /Root 1 0 R >>\n"
        b"startxref\n"
        b"264\n"
        b"%%EOF\n"
    )
    with open(test_pdf_path, "wb") as f:
        f.write(valid_pdf)
    yield
    # Cleanup
    if os.path.exists(test_pdf_path):
        os.remove(test_pdf_path)

@pytest.fixture
def setup_env():
    os.environ["OPEN_AI_KEY"] = "test_key"
    yield
    os.environ.pop("OPEN_AI_KEY", None)

def test_load_paper(setup_env, setup_test_pdf):
    from qa_app import load_paper
    test_pdf_path = os.path.join(os.path.dirname(__file__), "test.pdf")
    result = load_paper(test_pdf_path)
    assert len(result) > 0  # Should load at least one page
    assert "Sample content" in result[0].page_content  # Check extracted text

def test_create_qa(setup_env):
    from qa_app import create_qa
    mock_response = MagicMock()
    mock_response.choices = [MagicMock(message=MagicMock(content="""Q1: What is X? (Medium)
A) A
B) B
C) C
D) D
Answer: C"""))]
    with patch('qa_app.client.chat.completions.create', return_value=mock_response):
        result = create_qa("Test context", 1, "Medium", ["What is Y?"])
        assert "Q1: What is X? (Medium)" in result
        assert "Answer: C" in result
        assert "What is Y?" not in result  # Avoids previous question

def test_run_qa_session_all_difficulties(setup_env):
    from qa_app import run_qa_session
    with patch('qa_app.load_paper') as mock_load, \
         patch('qa_app.create_qa') as mock_create:
        mock_load.return_value = [MagicMock(page_content="Test content")]
        mock_create.side_effect = [
            "Q1: Easy Q? (Easy)\nA) A\nB) B\nC) C\nD) D\nAnswer: B",
            "Q1: Med Q? (Medium)\nA) A\nB) B\nC) C\nD) D\nAnswer: C",
            "Q1: Hard Q? (Hard)\nA) A\nB) B\nC) C\nD) D\nAnswer: D"
        ]
        result = run_qa_session("fake_path.pdf", 1, "campaign123")
        assert len(result) == 3
        assert result[0]["difficulty"] == "easy"
        assert result[0]["questionStr"] == "Easy ?"  # Match processed output
        assert any(a["isCorrect"] and a["answerStr"] == "B" for a in result[0]["answers"])
        assert result[1]["difficulty"] == "medium"
        assert result[1]["questionStr"] == "Med ?"
        assert result[2]["difficulty"] == "hard"
        assert result[2]["questionStr"] == "Hard ?"
        assert result[0]["campaignID"] == "campaign123"

def test_run_qa_session_invalid_output(setup_env):
    from qa_app import run_qa_session
    with patch('qa_app.load_paper') as mock_load, \
         patch('qa_app.create_qa') as mock_create:
        mock_load.return_value = [MagicMock(page_content="Test content")]
        mock_create.return_value = "Q1: Bad Q? (Easy)\nA) A\nB) B\n"  # Malformed (too few options)
        result = run_qa_session("fake_path.pdf", 1, "campaign123")
        assert len(result) == 0  # Should skip malformed QA

if __name__ == '__main__':
    pytest.main()