namespace GodsEye.WEB.Services.Interfaces
{
    public interface IWebService<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetById(int id);
    }
}
