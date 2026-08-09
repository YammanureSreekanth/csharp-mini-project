using Microsoft.Data.SqlClient;

public static class MySQLConnection
{
    private readonly static string connectionString =
            "Data Source=localhost;Initial Catalog=devdb;User ID=sreekanth;Password=Sreekanth@2031;Pooling=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Authentication=SqlPassword;Application Name=vscode-mssql;Application Intent=ReadWrite;Command Timeout=30";
     public static List<T> RunQuery<T>(string Query, Dictionary<string, string> ParamsPlaceholders, Func<SqlDataReader, T> SqlDataReaderProcessor)
    {
        List<T> results = new List<T>();
        // Create and open the connection in a using block. This
        // ensures that all resources will be closed and disposed
        // when the code exits.
        using (SqlConnection connection =
            new(connectionString))
        {
            // Create the Command and Parameter objects.
            SqlCommand command = new(Query, connection);
            // @TODO: Check better way here for 'de' type
            foreach (var de in ParamsPlaceholders)
            {
                command.Parameters.AddWithValue($"{de.Key}", de.Value);
            }

            // Open the connection in a try/catch block.
            // Create and execute the DataReader, writing the result
            // set to the console window.
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(SqlDataReaderProcessor(reader));
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return results;
        }
    }

    public static List<T> RunNonQuery<T>(string Query, Func<SqlDataReader, T> SqlDataReaderProcessor)
    {
        const string connectionString =
            "Data Source=localhost;Initial Catalog=devdb;User ID=sreekanth;Password=Sreekanth@2031;Pooling=False;Connect Timeout=30;Encrypt=False;Trust Server Certificate=True;Authentication=SqlPassword;Application Name=vscode-mssql;Application Intent=ReadWrite;Command Timeout=30";

        List<T> results = new List<T>();
        // Create and open the connection in a using block. This
        // ensures that all resources will be closed and disposed
        // when the code exits.
        using (SqlConnection connection =
            new(connectionString))
        {
            // Create the Command and Parameter objects.
            SqlCommand command = new(Query, connection);

            // Open the connection in a try/catch block.
            // Create and execute the DataReader, writing the result
            // set to the console window.
            try
            {
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    results.Add(SqlDataReaderProcessor(reader));
                    // Console.WriteLine($"\t{reader[0]}\t{reader[1]}\t{reader[2]}");
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return results;
        }
    }
}