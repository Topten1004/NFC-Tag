using BqsClinoTag.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using RestSharp;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Web.Http;

public class ChatHub : Hub
{
    private readonly CLINOTAGBQSContext _db;
    private static Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();

    public ChatHub(CLINOTAGBQSContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
        _apiKey = _configuration["OpenAiSettings:ApiKey"];

    }

    private readonly IConfiguration _configuration;
    private readonly string _apiKey;


    private async Task<string> TranslateTextAsync(string text)
    {
        var client = new RestClient("https://api.openai.com/v1/engines/davinci-codex/completions");
        var request = new RestRequest(Method.Post.ToString()); // Update this line

        request.AddHeader("Authorization", $"Bearer {_apiKey}");
        request.AddHeader("Content-Type", "application/json");

        var requestBody = new
        {
            prompt = $"Translate this text to English: {text}",
            max_tokens = 100
        };

        request.AddParameter("application/json", JsonConvert.SerializeObject(requestBody), ParameterType.RequestBody);

        var response = await client.ExecuteAsync(request);

        if (response.IsSuccessful)
        {
            var content = JsonConvert.DeserializeObject<dynamic>(response.Content);
            return content.choices[0].text;
        }
        else
        {
            throw new System.Exception("Translation API request failed.");
        }
    }


    public async Task SendMessageToRoom(string roomId, string message)
    {
        bool isNewRoom = false;
        if (!_rooms.ContainsKey(roomId))
        {
            _rooms[roomId] = new List<string>();
            isNewRoom = true;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        _rooms[roomId].Add(message);

        ChatHistory chatHistory = new ChatHistory();

        var translation = await TranslateTextAsync(message);

        chatHistory.CreatedTime = DateTime.Now;
        chatHistory.RoomName = roomId;
        chatHistory.Content = message;
        chatHistory.Checked = true;

        _db.ChatHistorys.Add(chatHistory);
        await _db.SaveChangesAsync();

        await Clients.Group(roomId).SendAsync("ReceiveMessage", new { roomId, text = translation });

        if (isNewRoom)
        {
            await Clients.All.SendAsync("ReceiveNewRoom", roomId);
        }
    }

    public async Task JoinRoom(string roomId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    public static List<string> GetRooms()
    {
        return new List<string>(_rooms.Keys);
    }

    public static List<string> GetMessages(string roomId)
    {
        if (_rooms.ContainsKey(roomId))
        {
            return _rooms[roomId];
        }
        return new List<string>();
    }

    public async Task GetRoomList()
    {
        var rooms = GetRooms();
        await Clients.Caller.SendAsync("ReceiveRoomList", rooms);
    }

    public async Task GetRoomMessages(string roomId)
    {
        var messages = GetMessages(roomId);
        await Clients.Caller.SendAsync("ReceiveRoomMessages", new { roomId, messages });
    }
}
