using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HAK_MAUI_Hybrid_Startertemplate.Models
{
    [Table("Message")]
    internal class Message : BaseModel
    {
        // Supabase möchte kleine Anfangsbuchstaben
        [PrimaryKey("id")]
        public int id { get; set; }

        [Column("text")]
        public string text { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.Now;


        // Foreign Key
        public int user_id { get; set; }

        [Reference(typeof(User))] // Wichtig damit Abfragen auch funktionieren wenn keien Nachricht vorhanden ist
        public User User { get; set; }
    }
}
