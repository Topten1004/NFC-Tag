namespace BqsClinoTag.ViewModel
{
    public record ChatMessage
    {
        public string RoomId { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
       
        public string Language {  get; set; } = string.Empty;

        public DateTime TimeStamp { get; set; } 

    }
}
