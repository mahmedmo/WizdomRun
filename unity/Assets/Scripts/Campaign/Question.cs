using System.Collections.Generic;
public class Question
{
    public int QuestionID { get; set; }
    public int CampaignID { get; set; }
    public QuestionDifficulty Difficulty { get; set; }
    public int GotCorrect { get; set; }
    public int WrongAttempts { get; set; }
    public string QuestionStr { get; set; }
    public List<Answer> AnswerList { get; set; }
}
