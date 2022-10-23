using System.Data;

using Microsoft.Data.Sqlite;

namespace Presentation.Cli;

public class Storage
{
    private readonly SqliteConnection _dbConnection;
    private int _id = 1;

    public Storage(string database) => _dbConnection = new SqliteConnection($"Data Source={database};Cache=Shared");

    public async Task<Request?> GetNextAsync()
    {
        if (_dbConnection.State != ConnectionState.Open) await _dbConnection.OpenAsync();

        var getCommand = _dbConnection.CreateCommand();
        getCommand.CommandText = @"
SELECT
    *
FROM
    Requests
WHERE
    Id = @id
";
        getCommand.Parameters.AddWithValue("@id", _id);

        _id += 1;
        
        var reader = await getCommand.ExecuteReaderAsync();
        reader.Read();
        if (reader.HasRows)
            return new Request(
                reader.GetString(1),
                reader.GetString(2),
                reader.GetString(3),
                reader.GetString(4),
                reader.GetDateTime(5)
            );

        return null;
    }
}

public record Request(string Method, string Path, string Headers, string Body, DateTime Time);