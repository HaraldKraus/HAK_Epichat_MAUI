using HAK_MAUI_Hybrid_Startertemplate.Models;
using Supabase;
using Supabase.Postgrest.Responses;

namespace HAK_MAUI_Hybrid_Startertemplate.Services
{
    internal class DataService
    {
        // Der SupaBase Client um auf die Datenbank zuzugreifen
        private readonly Client _supabaseClient;

        public User? LoggedInUser { get; private set; } = null;

        public DataService(Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        // Users
        public async Task<(string color, string message)> RegisterUser(User newUser)
        {
            // Prüfen ob Benutzername bereits existiert
            int userExists = await _supabaseClient
                .From<User>()
                .Where(u => u.username == newUser.username)
                .Count(Supabase.Postgrest.Constants.CountType.Exact);

            if(userExists > 0)
            {
                return ("pico-background-red-500", "Der Benutzername existiert bereits.");
            }

            // Passwort verschlüsseln, muss in eigene Variable sonst sieht der User es
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(newUser.password);

            newUser.password = hashedPassword;
            ModeledResponse<User> response = await _supabaseClient
                .From<User>()
                .Insert(newUser);

            newUser = new User();

            if (response.Model == null)
            {
                return ("pico-background-red-500", "Der Benutzer konnte nicht angelegt werden.");
            }
            return ("pico-background-jade-500", "Der Benutzer wurde erfolgreich registriert.");
        }
        public async Task<bool> LoginUser(User myUser)
        {
            User? dbUser = await _supabaseClient
                .From<User>()
                .Where(u => u.username == myUser.username)
                .Single();
                          
            if (dbUser == null)
            {
                return false;
            }

            if (BCrypt.Net.BCrypt.Verify(myUser.password, dbUser.password) == false)
            {
                return false;
            }

            dbUser.last_login = DateTime.Now;

            await _supabaseClient
                .From<User>()
                .Update(dbUser);

            LoggedInUser = dbUser;
            LoggedInUser.password = string.Empty;
            return true;
        }
        public async Task LogoutUser()
        {
            // Hole User aus DB
            this.LoggedInUser = null;
        }
        public async Task<bool> ResetPassword(string newPassword)
        {
            if (this.LoggedInUser == null)
            {
                return false;
            }

            this.LoggedInUser.password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            
            await _supabaseClient
                .From<User>()
                .Update(this.LoggedInUser);

            return true;
        }
        public async Task<bool> DeleteAccount()
        {
            if (this.LoggedInUser == null)
            {
                return false;
            }

            await _supabaseClient
                .From<User>()
                .Where(u => u.id == this.LoggedInUser.id)
                .Delete();

            this.LoggedInUser = null;
            return true;
        }

        // Messages
        public async Task<int> GetMessageCountForLoggedinUser()
        {
            if (this.LoggedInUser == null)
            {
                return 0;
            }

            int count = await _supabaseClient
                .From<Message>()
                .Where(m => m.user_id == this.LoggedInUser.id)
                .Count(Supabase.Postgrest.Constants.CountType.Exact);

            return count;
        }
        public async Task<List<Message>> GetMessagesAsync()
        {
            if (this.LoggedInUser == null)
            {
                return new List<Message>();
            }

            ModeledResponse<Message> messages = await _supabaseClient
                .From<Message>()                
                .Order(m => m.created_at, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            return messages.Models;
        }
        public async Task<bool> SendMessage(Message newMessage)
        {
            if (this.LoggedInUser == null)
            {
                return false;
            }

            newMessage.user_id = this.LoggedInUser.id;

            await _supabaseClient
                .From<Message>()
                .Insert(newMessage);

            return true;
        }
    }
}
