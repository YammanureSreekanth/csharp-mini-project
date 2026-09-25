using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

public class MySQLConnection: IDisposable
{
    private readonly SqlConnection _connection;
    private bool _disposed;    
    private readonly ILogger<MySQLConnection> _logger;
    public MySQLConnection(string connectionString, ILogger<MySQLConnection> logger)
    {
        _logger = logger;
        _connection = new SqlConnection(connectionString);
        _connection.Open();
        _logger.LogDebug("Connection opened");
    }
    public List<T> RunQuery<T>(string Query, Dictionary<string, string> ParamsPlaceholders, Func<SqlDataReader, T> SqlDataReaderProcessor)
    {
        List<T> results = new List<T>();

        using SqlCommand command = new SqlCommand(Query, _connection);

        foreach (KeyValuePair<string, string> de in ParamsPlaceholders)
        {
            command.Parameters.AddWithValue($"{de.Key}", de.Value);
        }

        try
        {
            using SqlDataReader reader = command.ExecuteReader();

             _logger.LogInformation("Query returned {RowCount} rows", results.Count);

            while (reader.Read())
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

    public List<T> RunNonQuery<T>(string Query, Func<SqlDataReader, T> SqlDataReaderProcessor)
    {
        List<T> results = new List<T>();

        using SqlCommand command = new SqlCommand(Query, _connection);

        try
        {
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
                results.Add(SqlDataReaderProcessor(reader));
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