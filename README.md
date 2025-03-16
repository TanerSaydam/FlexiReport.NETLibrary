# Flexi Report - Backend Library

This library is designed to support the backend functionalities required by the [Flexi Report](https://www.npmjs.com/package/flexi-report) Angular library.

## 📌 Features

- **Report Management**: Create, edit, and manage reports.
- **Dynamic SQL Queries**: Retrieve data from the database with dynamic queries.
- **Database Schema Retrieval**: List tables and columns from the database.
- **Flexible and Customizable**: Supports dynamic components generated on the UI side.

## 🚀 Installation

To add this package to your project, run:

```bash
dotnet add package FlexiReport
```

## 📖 Usage

### 1️⃣ Retrieve Database Schema

Retrieve all tables and columns from the connected database:

```csharp
public async Task<List<DatabaseSchemaDto>> GetDatabaseSchemaAsync(DbContext dbContext, CancellationToken cancellationToken = default)
```

**Example Usage:**

```csharp
var schema = await flexiReportService.GetDatabaseSchemaAsync(dbContext);
```

### 2️⃣ Execute SQL Query

Only `SELECT` queries are allowed.

```csharp
public async Task<Result<object>> ExecuteQueryAsync(QueryRequestDto request, DbContext dbContext, CancellationToken cancellationToken = default)
```

**Example Usage:**

```csharp
var request = new QueryRequestDto("SELECT * FROM Users");
var result = await flexiReportService.ExecuteQueryAsync(request, dbContext);
```

### 3️⃣ Report Model

The `Report` model contains the following properties:

```csharp
public sealed class Report
{
    public Guid Id { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Endpoint { get; set; }
    public string Name { get; set; }
    public string PageSize { get; set; }
    public string PageOrientation { get; set; }
    public string FontFamily { get; set; }
    public string SqlQuery { get; set; }
    public string BackgroundColor { get; set; }
    public List<RequestElement>? RequestElements { get; set; }
}
```

### 4️⃣ API Integration

To integrate with the frontend, use the following API structure:

```csharp
[HttpPost("execute-query")]
public async Task<IActionResult> ExecuteQuery([FromBody] QueryRequestDto request)
{
    var result = await _flexiReportService.ExecuteQueryAsync(request, _dbContext);
    return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
}
```

## 📌 DTO Models

### **QueryRequestDto**

```csharp
public sealed record QueryRequestDto(
    string SqlQuery);
```

### **ReportDto**

```csharp
public sealed record ReportDto(
    Guid? Id,
    string Content,
    string Endpoint,
    string Name,
    string PageSize,
    string PageOrientation,
    string FontFamily,
    string SqlQuery,
    string BackgroundColor,
    List<RequestElementDto> RequestElements);
```

## 📜 License

Distributed under the MIT License.

