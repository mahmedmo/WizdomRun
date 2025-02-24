using System.Collections.Generic;

public class Campaign
{
    public int CampaignID { get; set; }
    public int UserID { get; set; }
    public string Title { get; set; }
    public CampaignLength CampaignLength { get; set; }
    public int CurrLevel { get; set; }
    public int RemainingTries { get; set; }
    public List<Achievement> AchievementList { get; set; }
    public List<Question> QuestionList { get; set; }


}
