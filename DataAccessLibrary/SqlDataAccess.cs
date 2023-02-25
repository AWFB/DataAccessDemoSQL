using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace DataAccessLibrary;

public class SqlDataAccess
{
    // Read operation
    public List<T> LoadData<T, U>(string sqlStatement, U parameters, string connectionString)
    {
        // using - always close connection to DB when done
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            List<T> rows = connection.Query<T>(sqlStatement, parameters).ToList();

            return rows;
        }
    }

    // Write operation
    public void SaveData<T>(string sqlStatement, T parameters, string connectionString)
    {
        using (IDbConnection connection = new SqlConnection(connectionString))
        {
            connection.Execute(sqlStatement, parameters);
        }
    }
}
