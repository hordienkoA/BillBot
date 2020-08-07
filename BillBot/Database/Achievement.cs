using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace BillBot.Database
{
    public class Achievement
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PathToImage { get; set; }
        public virtual List<StatusWithAchievement> StatusWithAchievements { get; set; }

    }
}