using FlexiReport.Dtos;
using Microsoft.EntityFrameworkCore;
using TS.Result;

namespace FlexiReport;

public sealed class FlexiReportService
{
    public async Task<List<DatabaseSchemaDto>> GetDatabaseSchemaAsync(DbContext dbContext, CancellationToken cancellationToken = default)
    {
        using var connection = dbContext.Database.GetDbConnection();
        await connection.OpenAsync();

        var query = @"
        SELECT TABLE_NAME, COLUMN_NAME
        FROM INFORMATION_SCHEMA.COLUMNS
        ORDER BY TABLE_NAME, ORDINAL_POSITION";

        var schema = new Dictionary<string, List<string>>();

        using var command = connection.CreateCommand();
        command.CommandText = query;

        using var reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync())
        {
            string tableName = reader.GetString(0);
            string columnName = reader.GetString(1);

            if (!schema.ContainsKey(tableName))
                schema[tableName] = new List<string>();

            schema[tableName].Add(columnName);
        }

        var respons = schema.Where(p => p.Key != "__EFMigrationsHistory").Select(s => new DatabaseSchemaDto(s.Key, s.Value)).ToList();
        return respons;
    }

    public async Task<Result<object>> ExecuteQueryAsync(QueryRequestDto request, DbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.SqlQuery))
            return Result<object>.Failure("SQL query cannot be empty");

        if (!request.SqlQuery.TrimStart().StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
            return Result<object>.Failure("You can only use SELECT queries");

        try
        {
            using var connection = dbContext.Database.GetDbConnection();
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = request.SqlQuery;

            using var reader = await command.ExecuteReaderAsync();
            var result = new List<Dictionary<string, object>>();

            while (await reader.ReadAsync())
            {
                var row = new Dictionary<string, object>();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    row[reader.GetName(i)] = await reader.IsDBNullAsync(i) ? null : reader.GetValue(i);
                }
                result.Add(row);
            }

            return result;
        }
        catch (Exception ex)
        {
            return Result<object>.Failure(ex.Message);
        }
    }
}