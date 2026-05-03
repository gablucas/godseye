namespace GodsEye.WEB.Services.Interfaces
{
    public interface IWebService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync(int pageNumber, int pageSize);
        Task<T> GetById(int id);
    }
}
