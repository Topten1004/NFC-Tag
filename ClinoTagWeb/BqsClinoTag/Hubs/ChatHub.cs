using BqsClinoTag.Models;
using BqsClinoTag.ViewModel;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Crmf;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenAI_API;
using OpenAI_API.Chat;

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
    public async Task SendMessageToRoom(string roomId, string userName, string message, string language)
    {
        bool isNewRoom = false;
        if (!_rooms.ContainsKey(roomId))
        {
            _rooms[roomId] = new List<string>();
            isNewRoom = true;
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
        }
        _rooms[roomId].Add(message);

        // Translate customer's message to English for the manager

        string? translationToEnglish = message;

        if (language != "English")
            translationToEnglish = await TranslateTextAsync(message, language, "English");

        ChatHistory chatHistory = new ChatHistory
        {
            CreatedTime = DateTime.Now,
            RoomName = roomId,
            Content = message,
            Checked = true,
            UserName = userName,
            Language = language // Store the customer's language
        };

        _db.ChatHistorys.Add(chatHistory);
        await _db.SaveChangesAsync();

        // Send the original message in French to the customer only (i.e., the caller)
        await Clients.Caller.SendAsync("ReceiveMessage", new { roomId, text = userName + ": " +  message, created = DateTime.Now.ToString("hh:mm") });

        // Send the translated message in English to the admin (manager group)
        await Clients.OthersInGroup(roomId).SendAsync("ReceiveMessage", new { roomId, text = userName + ": " + translationToEnglish, created = DateTime.Now.ToString("hh:mm") });

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
            await Clients.All.SendAsync("ReceiveNewRoom", roomId); // Notify all clients about the new room
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
    public static List<ChatLog> GetMessages(string roomId, CLINOTAGBQSContext db)
    {
        var history = db.ChatHistorys
                         .Where(ch => ch.RoomName == roomId && ch.CreatedTime.Date == DateTime.Now.Date)
                         .OrderBy(ch => ch.CreatedTime)
                         .ToList();

        List<ChatLog> messages = new List<ChatLog>();

        foreach (var item in history)
        {
            ChatLog log = new ChatLog();

            log.message = item.UserName + ": " + item.Content;
            log.roomId = roomId;
            log.created = item.CreatedTime.ToString("hh:mm");

            messages.Add(log);
        }

        return messages;
    }

    // Send room messages to the client
    public async Task GetRoomMessages(string roomId)
    {
        var messages = GetMessages(roomId, _db);
        await Clients.Caller.SendAsync("ReceiveRoomMessages", new { roomId, messages });
    }

    public async Task SendAdminResponse(string roomId, string userName, string message)
    {
        // Fetch the chat history to get the customer's language
        var chatHistory = _db.ChatHistorys
                             .Where(ch => ch.RoomName == roomId)
                             .OrderByDescending(ch => ch.CreatedTime)
                             .FirstOrDefault();

        if (chatHistory != null)
        {
            string translatedResponse = message;

            if(chatHistory.Language != "English")
                translatedResponse = await TranslateTextAsync(message, "English", chatHistory.Language);

            ChatHistory history = new ChatHistory
            {
                CreatedTime = DateTime.Now,
                RoomName = roomId,
                Content = message,
                Checked = true,
                UserName = userName,
                Language = "English" // Manager language
            };

            _db.ChatHistorys.Add(history);
            await _db.SaveChangesAsync();

            // Send the original message (in English) to the manager (admin group)
            await Clients.Caller.SendAsync("ReceiveMessage", new { roomId, text = "Manager: " + message, created = DateTime.Now.ToString("hh:mm") });

            // Send the translated message (in French) to the customer
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveMessage", new { roomId, text =  "Manager: " + translatedResponse, created = DateTime.Now.ToString("hh:mm") });
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
        var apiModel = "gpt-3.5-turbo"; // Updated model
        OpenAIAPI api = new OpenAIAPI(new APIAuthentication(_apiKey));

        // Prepare the chat completion request
        var chatRequest = new ChatRequest()
        {
            Messages = new List<ChatMessage>()
        {
            new ChatMessage { Role = OpenAI_API.Chat.ChatMessageRole.System, Content = @"You are a translator. Translate from " + fromLanguage + " to " + toLanguage + "." },
            new ChatMessage { Role = OpenAI_API.Chat.ChatMessageRole.User, Content = text }
        },
            Model = apiModel,
            Temperature = 0.3,
            MaxTokens = 1000
        };

        string translatedText = string.Empty;

        try
        {
            // Attempt to call the API and translate the text
            var result = await api.Chat.CreateChatCompletionAsync(chatRequest);

            foreach (var choice in result.Choices)
            {
                translatedText = choice.Message.Content;
            }

            return translatedText;
        }
        catch (HttpRequestException httpEx)
        {
            Console.WriteLine("HTTP request failed: " + httpEx.Message);
            return "Error: Failed to connect to the translation service.";
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
            return "Error: An unexpected error occurred.";
        }
    }

    public record ChatLog
    {
        public string message { get; set; }

        public string roomId { get; set; }

        public string created { get; set; }
    }
}
