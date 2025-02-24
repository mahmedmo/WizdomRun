using System.Collections.Generic;
public class User
{
    public int UserID { get; set; }
    public string UserName { get; set; }
    public string CharName { get; set; }

    public List<Campaign> CampaignList { get; set; }

    public PlayerCharacter PlayerChar { get; set; }


}