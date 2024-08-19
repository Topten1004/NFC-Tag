namespace BqsClinoTag.Services
{
    public static class ImageHelper
    {
        public static string ConvertImageToBase64(byte[]? imageBytes)
        {
            if (imageBytes == null || imageBytes.Length == 0)
                return string.Empty;

            string base64String = Convert.ToBase64String(imageBytes);
            return $"data:image/png;base64,{base64String}";
        }
    }

}
