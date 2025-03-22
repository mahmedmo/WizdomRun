using System.Collections.Generic;
public class PlayerStats
{
    public int HP { get; set; }
    public int Mana { get; set; }
    public int Gold { get; set; }
    public PlayerClass? Affinity { get; set; }
    public List<int> Slots { get; set; } = new List<int>(4);

}