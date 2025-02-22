using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Supabase;
using Supabase.Gotrue;
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using UnityEngine;
using Client = Supabase.Client;
using Random = UnityEngine.Random;

public class DatabaseManager : MonoBehaviour
{
    public event Action Initialized;
    
    public static DatabaseManager Instance;
    
    public List<Player> PlayerRecords = new List<Player>();
    
    private Client supabaseClient;
    
    private Session supabaseSession;

    private async void Awake()
    {
        Instance = this;
        await Initialize();
        Debug.Log("Initialized database connection.");
        var name = "John Doe";
    }

    public async Task<string> SignIn(string email, string password, string name)
    {
        await SignIn(email, password);

        var result = await supabaseClient.From<Player>().Get();

        if (result.Model == null)
        {
            await DatabaseManager.Instance.AddPlayerRecord(new DatabaseManager.Player
            {
                User = supabaseSession.User.Id,
                Name = name,
                Coins = 0,
                Level = 1
            });

            Debug.Log($"Created new user {name}");
            return supabaseSession.User.Id;
        }
        
        Debug.Log(result.Model.Name);
        // await LogPlayers();
        return supabaseSession.User.Id;
    }

    public async Task Initialize()
    {
        var url = "https://cwxockowjgxjrhrwgbhi.supabase.co";
        var key = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImN3eG9ja293amd4anJocndnYmhpIiwicm9sZSI6ImFub24iLCJpYXQiOjE3NDAxNjc2NjIsImV4cCI6MjA1NTc0MzY2Mn0.SFwP6x-8AyfJ6e49QUhChovjUL5BPuVPT3gwg19DhvQ";
        
        var options = new SupabaseOptions
        {
            //AutoConnectRealtime = true
        };
        
        supabaseClient = new Client(url, key, options);
        await supabaseClient.InitializeAsync();
        
        
        // A result can be fetched like so.
        
    }

    private async Task LogPlayers()
    {
        var result = await supabaseClient.From<Player>().Get();
        PlayerRecords = result.Models;
        PlayerRecords.ForEach(p => Debug.Log($"{p.Name} - {p.Level}"));
    }

    public async Task AddPlayerRecord(Player record)
    {
        
        await supabaseClient.From<Player>().Insert(record);
    }

    public async Task SignUp(string email, string password)
    {
        supabaseSession = await supabaseClient.Auth.SignUp(Constants.SignUpType.Email, email, password);
    }
    
    public async Task SignIn(string email, string password)
    {
        supabaseSession = await supabaseClient.Auth.SignIn(email, password);
        
        //var attrs = new UserAttributes { Email = supabaseSession.User.Email };
        //var response = await supabaseClient.Auth.Update(attrs);
        
        UnityEngine.Debug.Log(supabaseSession.Expired());
    }
    
    [Table("players")]
    public class Player : BaseModel
    {
        [PrimaryKey("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string User { get; set; }
        
        [Column("name")]
        public string Name { get; set; }

        [Column("level")]
        public int Level { get; set; }
        
        [Column("coins")]
        public int Coins { get; set; }

        //... etc.
    }
}
