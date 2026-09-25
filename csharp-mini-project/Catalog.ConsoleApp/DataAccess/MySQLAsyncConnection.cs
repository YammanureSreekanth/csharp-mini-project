using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

public class MySQLAsyncConnection: IDisposable
{
    private readonly SqlConnection _connection;
    private bool _disposed;    
    private readonly ILogger<MySQLConnection> _logger;
    public MySQLAsyncConnection(string connectionString, ILogger<MySQLConnection> logger)
    {
        _logger = logger;
        _connection = new SqlConnection(connectionString);
        _connection.Open();
        _logger.LogDebug("Connection opened");
    }
    public async Task<List<T>> RunQueryAsync<T>(string Query, Dictionary<string, string> ParamsPlaceholders, Func<SqlDataReader, T> SqlDataReaderProcessor, CancellationToken cancellationToken)
    {
        List<T> results = new List<T>();

        using SqlCommand command = new SqlCommand(Query, _connection);

        foreach (KeyValuePair<string, string> de in ParamsPlaceholders)
        {
            command.Parameters.AddWithValue($"{de.Key}", de.Value);
        }

        try
        {
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken);   
            }

            using SqlDataReader reader = await command.ExecuteReaderAsync();

             _logger.LogInformation("Query returned {RowCount} rows", results.Count);

            while (await reader.ReadAsync())
            {
                results.Add(SqlDataReaderProcessor(reader));   
            }
        }
        catch (Exception ex)
        {
             _logger.LogError(ex, "Query failed: {Query}", Query);
            throw;
        }

        return results;
    }

    public async Task<List<T>> RunNonQueryAsync<T>(string Query, Func<SqlDataReader, T> SqlDataReaderProcessor, CancellationToken cancellationToken)
    {
        List<T> results = new List<T>();

        using SqlCommand command = new SqlCommand(Query, _connection);

        try
        {   
            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken);
            }

            using SqlDataReader reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                results.Add(SqlDataReaderProcessor(reader));   
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            throw;
        }

        return results;
    }

    public void Dispose()
    {
        if (_disposed) return;
        _connection.Dispose();
        _disposed = true;
        _logger.LogDebug("Connection disposed");
    }
}