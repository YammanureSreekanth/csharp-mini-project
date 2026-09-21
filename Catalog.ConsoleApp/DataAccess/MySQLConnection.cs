using Microsoft.Data.SqlClient;

public class MySQLConnection: IDisposable
{
    private readonly SqlConnection _connection;
    private bool _disposed;
    private readonly static string connectionString =
            "Data Source=localhost;Initial Catalog=CatalogDb;User ID=sa;Password=PraticeApp@2031;Pooling=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Authentication=SqlPassword;Application Name=vscode-mssql;Application Intent=ReadWrite;Command Timeout=30";
    
    public MySQLConnection(string connectionString)
    {
        _connection = new SqlConnection(connectionString);
        _connection.Open();
    }
    public List<T> RunQuery<T>(string Query, Dictionary<string, string> ParamsPlaceholders, Func<SqlDataReader, T> SqlDataReaderProcessor)
    {
        List<T> results = new List<T>();

        using SqlCommand command = new SqlCommand(Query, _connection);

        // @TODO: Check better way here for 'de' type
        foreach (var de in ParamsPlaceholders)
        {
            command.Parameters.AddWithValue($"{de.Key}", de.Value);
        }

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
    }
}