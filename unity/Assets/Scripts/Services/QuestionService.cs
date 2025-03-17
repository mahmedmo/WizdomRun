using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.IO;
public class QuestionService : BaseService
{
    // Uploads a PDF file to create questions for a campaign.
    public IEnumerator CreateQuestionsFromPDF(string pdfFilePath, int campaignID, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/create";
        WWWForm form = new WWWForm();
        form.AddField("campaignID", campaignID.ToString());

        // Read file data.
        byte[] fileData = File.ReadAllBytes(pdfFilePath);
        string fileName = Path.GetFileName(pdfFilePath);
        // "file" is the key expected by your endpoint.
        form.AddBinaryData("file", fileData, fileName, "application/pdf");

        UnityWebRequest request = UnityWebRequest.Post(url, form);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

        yield return request.SendWebRequest();

#if UNITY_2020_1_OR_NEWER
            if(request.result != UnityWebRequest.Result.Success)
#else
        if (request.isNetworkError || request.isHttpError)
#endif
        {
            onError?.Invoke(request.error);
        }
        else
        {
            onSuccess?.Invoke();
        }
    }

    // Optionally, if you need to re-fetch questions:
    public IEnumerator GetQuestions(int campaignID, string firebaseToken, System.Action<List<Question>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/{campaignID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);

        yield return SendRequest(request,
            response =>
            {
                QuestionListWrapper wrapper = JsonUtility.FromJson<QuestionListWrapper>("{\"questions\":" + response + "}");
                onSuccess?.Invoke(wrapper.questions);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Answer a question.
    public IEnumerator AnswerQuestion(int questionID, bool gotCorrect, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/answer/{questionID}";
        string jsonData = $"{{\"gotCorrect\":{gotCorrect.ToString().ToLower()}}}";
        UnityWebRequest request = UnityWebRequest.Put(url, jsonData);
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Batch create questions (expects a JSON array string as input).
    public IEnumerator BatchCreateQuestions(string jsonBatchData, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/batch_create";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBatchData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Get a single question.
    public IEnumerator GetQuestion(int questionID, string firebaseToken, System.Action<Question> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/question/{questionID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                Question question = JsonUtility.FromJson<Question>(response);
                onSuccess?.Invoke(question);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Delete a question.
    public IEnumerator DeleteQuestion(int questionID, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/delete/{questionID}";
        UnityWebRequest request = UnityWebRequest.Delete(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Get answers for a question.
    public IEnumerator GetAnswers(int questionID, string firebaseToken, System.Action<List<Answer>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/answers/{questionID}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                AnswerListWrapper wrapper = JsonUtility.FromJson<AnswerListWrapper>("{\"answers\":" + response + "}");
                onSuccess?.Invoke(wrapper.answers);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Increment the wrong attempt counter for a question.
    public IEnumerator IncrementWrongAttempt(int questionID, string firebaseToken, System.Action onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/wrong_attempt/{questionID}";
        UnityWebRequest request = UnityWebRequest.Put(url, "");
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response => { onSuccess?.Invoke(); },
            error => { onError?.Invoke(error); }
        );
    }

    // Get unanswered questions.
    public IEnumerator GetUnansweredQuestions(string firebaseToken, System.Action<List<Question>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/unanswered";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                QuestionListWrapper wrapper = JsonUtility.FromJson<QuestionListWrapper>("{\"questions\":" + response + "}");
                onSuccess?.Invoke(wrapper.questions);
            },
            error => { onError?.Invoke(error); }
        );
    }

    // Get questions filtered by difficulty.
    public IEnumerator GetQuestionsByDifficulty(string difficulty, string firebaseToken, System.Action<List<Question>> onSuccess, System.Action<string> onError)
    {
        string url = $"{baseUrl}/questions/difficulty/{difficulty}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + firebaseToken);
        yield return SendRequest(request,
            response =>
            {
                QuestionListWrapper wrapper = JsonUtility.FromJson<QuestionListWrapper>("{\"questions\":" + response + "}");
                onSuccess?.Invoke(wrapper.questions);
            },
            error => { onError?.Invoke(error); }
        );
    }


    // Local classes for JSON parsing.
    [System.Serializable]
    public class Question
    {
        public int questionID;
        public int campaignID;
        public string difficulty;
        public int gotCorrect;
        public int wrongAttempts;
        public string questionStr;
        public List<Answer> answerList;
    }

    [System.Serializable]
    public class Answer
    {
        public int answerID;
        public int questionID;
        public string answerStr;
        public bool isCorrect;
    }

    [System.Serializable]
    public class QuestionListWrapper
    {
        public List<Question> questions;
    }

    [System.Serializable]
    public class AnswerListWrapper
    {
        public List<Answer> answers;
    }
}