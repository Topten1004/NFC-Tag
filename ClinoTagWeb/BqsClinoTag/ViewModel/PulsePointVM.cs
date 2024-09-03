namespace BqsClinoTag.ViewModel
{
    
    public record PulsePointVM
    {
        public string Message { get; set; }
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int Status { get; set; }
        public User? User { get; set; }
        public Project? Project { get; set; }
    }

    public record User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public int Status { get; set; }
        public int Role { get; set; }
    }

    public record Project
    {
        public int IsApply { get; set; }
        public DateTime PurchaseDate { get; set; }
        public int Periodicity { get; set; }
    }

}
