namespace BqsClinoTag.ViewModel
{
    public record ScanTaskVM
    {
        public string Name { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public List<Byte[]> ImageData { get; set; } = new List<Byte[]>();
    }
}
