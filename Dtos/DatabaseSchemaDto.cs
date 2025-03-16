namespace FlexiReport.Dtos;

public sealed record DatabaseSchemaDto(
    string TableName,
    List<string> Columns);
