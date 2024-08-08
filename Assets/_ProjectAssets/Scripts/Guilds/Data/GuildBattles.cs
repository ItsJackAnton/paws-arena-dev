using System;

[Serializable]
public class GuildBattles
{
    public DateTime StartingDate = DateTime.MinValue;
    public DateTime EndingDate = DateTime.MaxValue;
    public DateTime ReviewDate = DateTime.MaxValue;
    
    public bool IsActive => DateTime.UtcNow > StartingDate && DateTime.UtcNow < EndingDate;
    public bool IsReviewDate => DateTime.UtcNow < ReviewDate;
}
