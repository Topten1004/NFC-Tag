using BqsClinoTag.Models;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class ChatHub : Hub
{
    private static Dictionary<string, List<string>> _rooms = new Dictionary<string, List<string>>();

    private readonly CLINOTAGBQSContext _db;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;

    public ChatHub(CLINOTAGBQSContext db, IConfiguration configuration)
    {
        _db = db;
        _configuration = configuration;
        _apiKey = _configuration["OpenAiSettings:ApiKey"];
    }

    // This method sends a message to the room and handles the translation
    public async Task SendMessageToRoom(string roomId, string message, string language)
    {
        bool isNewRoom = false;
        if (!_rooms.ContainsKey(roomId))
        {
            _rooms[roomId] = new List<string>();
            isNewRoom = true;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        _rooms[roomId].Add(message);

        var translationToEnglish = await TranslateTextAsync(message, language, "en");

        ChatHistory chatHistory = new ChatHistory
        {
            CreatedTime = DateTime.Now,
            RoomName = roomId,
            Content = message,
            Checked = true,
            Language = language // Store the customer's language
        };

        _db.ChatHistorys.Add(chatHistory);
        await _db.SaveChangesAsync();

        // Broadcast message in English to all admins in the room
        await Clients.Group(roomId).SendAsync("ReceiveMessage", new { roomId, text = translationToEnglish });

        if (isNewRoom)
        {
            await Clients.All.SendAsync("ReceiveNewRoom", roomId);
        }
    }

    // When a user joins a room
    public async Task JoinRoom(string roomId)
    {
        if (!_rooms.ContainsKey(roomId))
        {
            _rooms[roomId] = new List<string>();
        }
        await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
    }

    // When a user leaves a room
    public async Task LeaveRoom(string roomId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, roomId);
    }

    // Get the list of all rooms
    public static List<string> GetRooms()
    {
        return new List<string>(_rooms.Keys);
    }

    // Retrieve chat messages for a specific room
    public static List<string> GetMessages(string roomId, CLINOTAGBQSContext db)
    {
        var messages = db.ChatHistorys
                         .Where(ch => ch.RoomName == roomId)
                         .OrderBy(ch => ch.CreatedTime)
                         .Select(ch => ch.Content)
                         .ToList();

        return messages;
    }

    // Send room messages to the client
    public async Task GetRoomMessages(string roomId)
    {
        var messages = GetMessages(roomId, _db);
        await Clients.Caller.SendAsync("ReceiveRoomMessages", new { roomId, messages });
    }

    // Send admin's response to the room and translate it to the original language
    public async Task SendAdminResponse(string roomId, string message)
    {
        var chatHistory = _db.ChatHistorys
                             .Where(ch => ch.RoomName == roomId)
                             .OrderByDescending(ch => ch.CreatedTime)
                             .FirstOrDefault();

        if (chatHistory != null)
        {
            var translatedResponse = await TranslateTextAsync(message, "en", chatHistory.Language);
            await Clients.Group(roomId).SendAsync("ReceiveMessage", new { roomId, text = translatedResponse });
        }
    }

    // Send the list of rooms to the manager
    public async Task GetRoomList()
    {
        var rooms = GetRooms(); // Fetch all room IDs
        await Clients.Caller.SendAsync("ReceiveRoomList", rooms);
    }

    // Dummy translation method
    private async Task<string> TranslateTextAsync(string text, string fromLanguage, string toLanguage)
    {
        // For now, returning the original text. Replace this with actual translation logic.
        return text;

        var client = new RestClient("https://api.openai.com/v1/chat/completions");
        var request = new RestRequest
        {
            Method = Method.Post
        };
        request.AddHeader("Authorization", $"Bearer {_apiKey}");
        request.AddHeader("Content-Type", "application/json");

        var prompt = $"Translate the following message from {fromLanguage} to {toLanguage}: '{text}'";

        var body = new
        {
            model = "gpt-4-turbo",
            messages = new List<object>
            {
                new { role = "system", content = "You are a translation and language detection assistant." },
                new { role = "user", content = prompt }
            },
            max_tokens = 100
        };

        request.AddJsonBody(body);

        var response = await client.ExecuteAsync<RestResponse>(request);
        var content = response.Content;

        dynamic result = JsonConvert.DeserializeObject(content);
        string translatedText = result?.choices[0]?.message?.content?.ToString()?.Trim();

        return translatedText ?? text;
    }
}
