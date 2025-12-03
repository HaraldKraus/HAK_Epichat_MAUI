using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Services.Maps;

namespace HAK_MAUI_Hybrid_Startertemplate.Models
{
    [Table("User")]
    internal class User : BaseModel
    {
        [PrimaryKey("id")]
        public int id { get; set; }

        [Column("username")]
        [Required(ErrorMessage = "Der Benutzername ist erforderlich")]
        public string username { get; set; } = string.Empty;

        [Column("password")]
        [Required(ErrorMessage = "Das Passwort ist erforderlich")]
        [MinLength(6, ErrorMessage = "Das Passwort muss mindestens aus 6 Zeichen bestehen")]
        public string password { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime created_at { get; set; } = DateTime.Now;

        [Column("last_login")]
        public DateTime last_login { get; set; } = DateTime.Now;

        [Reference(typeof(Message), useInnerJoin: false)] // Wichtig damit Abfragen auch funktionieren wenn keine Nachricht vorhanden ist
        public List<Message> Messages { get; set; } = new();
    }
}
