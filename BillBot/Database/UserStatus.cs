using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillBot.Database
{
    public class UserStatus
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int FugCount { get; set; }
        public int UserId { get; set; }
        public virtual User User { get; set; }
        /*public virtual List<User> Users { get; set; }*/
        public int ChatId { get; set; }
        public virtual Chat Chat { get; set; }
        /*public virtual List<Chat> Chats { get; set; }*/
        public virtual List<StatusWithAchievement> StatusWithAchievements { get; set; }
    }
}