using GodsEye.API.Interfaces;

namespace GodsEye.API.Services
{
    public class FolderService : IFolderService
    {
        private readonly string _PersonPhotoPath = "photos";

        public async Task SavePersonPhoto(byte[] photo, string fileName, CancellationToken cancellationToken)
        {
            var folder = Path.Combine("wwwroot", _PersonPhotoPath);

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var filePath = Path.Combine("wwwroot", _PersonPhotoPath, fileName);

            await File.WriteAllBytesAsync(filePath, photo, cancellationToken);
        }

        public string GeneratePersonPhotoPath(string fileName)
        {
            return Path.Combine(_PersonPhotoPath, fileName);
        }
    }
}
