using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace ModernDesktopApp;

/// <summary>
/// Helper class for SQLite database operations
/// </summary>
public class DatabaseHelper
{
    private readonly string _dbPath;

    public DatabaseHelper(string databasePath)
    {
        _dbPath = databasePath;
    }

    /// <summary>
    /// Get connection string for the database
    /// </summary>
    public string ConnectionString => $"Data Source={_dbPath}";

    /// <summary>
    /// Check if database file exists
    /// </summary>
    public bool DatabaseExists()
    {
        return File.Exists(_dbPath);
    }

    /// <summary>
    /// Get list of all tables in the database
    /// </summary>
    public List<string> GetTables()
    {
        var tables = new List<string>();

        if (!DatabaseExists())
            return tables;

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            tables.Add(reader.GetString(0));
        }

        return tables;
    }

    /// <summary>
    /// Get table schema information
    /// </summary>
    public List<ColumnInfo> GetTableSchema(string tableName)
    {
        var columns = new List<ColumnInfo>();

        if (!DatabaseExists())
            return columns;

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({tableName})";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            columns.Add(new ColumnInfo
            {
                ColumnId = reader.GetInt32(0),
                Name = reader.GetString(1),
                Type = reader.GetString(2),
                NotNull = reader.GetInt32(3) == 1,
                DefaultValue = reader.IsDBNull(4) ? null : reader.GetValue(4)?.ToString(),
                IsPrimaryKey = reader.GetInt32(5) == 1
            });
        }

        return columns;
    }

    /// <summary>
    /// Execute a SELECT query and return DataTable
    /// </summary>
    public DataTable ExecuteQuery(string query)
    {
        var dataTable = new DataTable();

        if (!DatabaseExists())
            return dataTable;

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = query;

        using var reader = command.ExecuteReader();
        dataTable.Load(reader);

        return dataTable;
    }

    /// <summary>
    /// Execute a non-query command (INSERT, UPDATE, DELETE)
    /// </summary>
    public int ExecuteNonQuery(string commandText)
    {
        if (!DatabaseExists())
            throw new InvalidOperationException("Database does not exist");

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = commandText;

        return command.ExecuteNonQuery();
    }

    /// <summary>
    /// Get row count for a table
    /// </summary>
    public long GetRowCount(string tableName)
    {
        if (!DatabaseExists())
            return 0;

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {tableName}";

        var result = command.ExecuteScalar();
        return result != null ? Convert.ToInt64(result) : 0;
    }

    /// <summary>
    /// Get database file info
    /// </summary>
    public DatabaseInfo GetDatabaseInfo()
    {
        if (!DatabaseExists())
            return new DatabaseInfo { Exists = false };

        var fileInfo = new FileInfo(_dbPath);
        var tables = GetTables();

        return new DatabaseInfo
        {
            Exists = true,
            FilePath = _dbPath,
            FileSize = fileInfo.Length,
            LastModified = fileInfo.LastWriteTime,
            TableCount = tables.Count,
            Tables = tables
        };
    }
}

/// <summary>
/// Column information for a table
/// </summary>
public class ColumnInfo
{
    public int ColumnId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool NotNull { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsPrimaryKey { get; set; }
}

/// <summary>
/// Database information
/// </summary>
public class DatabaseInfo
{
    public bool Exists { get; set; }
    public string FilePath { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime LastModified { get; set; }
    public int TableCount { get; set; }
    public List<string> Tables { get; set; } = new();
}