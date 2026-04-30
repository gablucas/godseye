namespace GodsEye.API.Interfaces
{
    public interface IFolderService
    {
        Task SavePersonPhoto(byte[] photo, string fileName, CancellationToken cancellationToken);
        string GeneratePersonPhotoPath(string fileName);
    }
}
