namespace BillBot.Database
{
    public class StatusWithAchievement
    {
        public int UserStatusId { get; set; }
        public virtual UserStatus UserStatus { get; set; }
        
        public int AchievementId { get; set; }
        public virtual Achievement Achievement { get; set; }
    }
}