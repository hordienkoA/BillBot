using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillBot.Database
{
    public class Chat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long ChatId { get; set; }
        
        public virtual List<UserStatus> UserStatuses { get; set; }
        /*public int UserId { get; set; }
        public virtual User User { get; set; }*/
    }
}