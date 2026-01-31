using GodsEye.Domain.DTOs.Result;

namespace GodsEye.Application.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> ExecuteSqlAsync(string sql, IDictionary<string, object?> parameters, CancellationToken cancellationToken);
        Task<List<T>> QuerySqlAsync<T>(string sql, IDictionary<string, object?> parameters, CancellationToken cancellationToken) where T : class;
        Task<T> QuerySingleSqlAsync<T>(string sql, IDictionary<string, object?> parameters, CancellationToken cancellationToken) where T : class;
    }
}
