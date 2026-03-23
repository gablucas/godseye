namespace GodsEye.WEB.Helpers
{
    public static class ImageHelper
    {
        public static string HasPersonImage(string? Image)
        {
            if (Image is not null)
            {
                return $"https://localhost:7010/{Image}";
            }
            else
            {
                return "/person-no-image.png";
            }
        }
    }
}
