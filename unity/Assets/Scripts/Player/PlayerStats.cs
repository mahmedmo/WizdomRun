using System.Collections.Generic;
public class PlayerStats
{
    public int PlayerID { get; set; }
    public float Attack { get; set; }
    public int HP { get; set; } = 10;
    public int Mana { get; set; }
    public PlayerClass Affinity { get; set; }

    public List<Spell> list { get; set; }

}