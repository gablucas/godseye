using Dapper;
using GodsEye.API.Interfaces;
using MySqlConnector;
using System.Data;

namespace GodsEye.API.Services
{
    public class DapperContext : IDapperContext
    {
        private readonly IDbConnection _connection;

        public DapperContext(IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("MySQLConnection");
            _connection = new MySqlConnection(connectionString);
        }

        public async Task<int> ExecuteSqlAsync(string sql, object? parameters, CancellationToken cancellationToken)
        {
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            return await _connection.ExecuteAsync(command);
        }

        public async Task<T?> QuerySingleSqlAsync<T>(string sql, object? parameters, CancellationToken cancellationToken)
            where T : class
        {
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            return await _connection.QueryFirstOrDefaultAsync<T>(command);
        }

        public async Task<List<T>> QuerySqlAsync<T>(string sql, object? parameters, CancellationToken cancellationToken)
            where T : class
        {
            var command = new CommandDefinition(sql, parameters, cancellationToken: cancellationToken);
            var result = await _connection.QueryAsync<T>(command);
            return result.ToList();
        }

        public async Task<List<T>> QuerySqlAsync<T>(string sql, CancellationToken cancellationToken)
            where T : class
            => await QuerySqlAsync<T>(sql, null, cancellationToken);

        public async Task<int> ExecuteDeleteAsync(string sql, object? parameters, CancellationToken cancellationToken)
            => await ExecuteSqlAsync(sql, parameters, cancellationToken);

        public void Dispose()
        {
            _connection?.Dispose();
        }
    }
}
