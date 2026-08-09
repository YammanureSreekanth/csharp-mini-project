using Microsoft.Data.SqlClient;

namespace Catalog.ConsoleApp.CustomExtensions;
public static class SqlDataReaderExtensions
{
    extension(SqlDataReader dataReader)
    {
        public bool isValueNullInDB(string propName)
        {
            object? value = dataReader.GetValue(dataReader.GetOrdinal(propName));
            Console.WriteLine($"Propname {propName} and isDBNUll {value?.GetType().Name} and value {value?.ToString()}");
            return value is DBNull;
        }
    }
}