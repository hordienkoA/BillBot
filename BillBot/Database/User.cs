using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillBot.Database
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public bool IsBot { get; set; }
        public string LanguageCode { get; set; }
        public virtual List<UserStatus> Statuses { get; set; }
        /*public int UserStatusId { get; set; }
        public UserStatus UserStatus { get; set; }*/
    }
}