namespace Infrastructure.Seeder;

public static class  SeederHelper
{
    public static async Task ExecuteSqlAsync(System.Data.Common.DbConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        await command.ExecuteNonQueryAsync();
    }


}
