namespace GodsEye.Application.Interfaces
{
    public interface IDapperContext
    {
        Task<int> ExecuteSqlAsync(string sql, object? parameters, CancellationToken cancellationToken);
        Task<T?> QuerySingleSqlAsync<T>(string sql, object? parameters, CancellationToken cancellationToken) where T : class;
        Task<List<T>> QuerySqlAsync<T>(string sql, CancellationToken cancellationToken) where T : class;
        Task<List<T>> QuerySqlAsync<T>(string sql, object? parameters, CancellationToken cancellationToken) where T : class;
        Task<int> ExecuteDeleteAsync(string sql, object? parameters, CancellationToken cancellationToken);
    }
}
