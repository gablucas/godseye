namespace GodsEye.WEB.Helpers
{
    public static class ImageHelper
    {
        public static string HasPersonImage(string? url, string? Image)
        {
            if (Image is not null)
            {
                return $"{url}/{Image}";
            }
            else
            {
                return "/person-no-image.png";
            }
        }
    }
}
