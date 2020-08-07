using System.ComponentModel.DataAnnotations.Schema;

namespace BillBot.Database
{
    public class Phrase
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Text { get; set; }
    }
}