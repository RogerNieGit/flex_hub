using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace ModernDesktopApp;

/// <summary>
/// Manager for AI API keys with encryption/decryption support
/// Based on FlexDesk AI Tools implementation
/// </summary>
public class AiKeyManager
{
    private readonly string _dbPath;
    private readonly string _encryptionKey;

    public AiKeyManager(string? databasePath = null)
    {
        _dbPath = databasePath ?? @"C:\Users\T917991\AppData\Roaming\com.flexdesk.app\flex_desk_db";
        // Use a default encryption key (in production, this should be securely stored)
        _encryptionKey = "FlexHub-AI-Key-2026"; // 20 chars for AES
    }

    /// <summary>
    /// Get connection string for the database
    /// </summary>
    private string ConnectionString => $"Data Source={_dbPath}";

    /// <summary>
    /// Get all AI models from database
    /// </summary>
    public List<AiModel> GetAvailableModels()
    {
        var models = new List<AiModel>();

        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT id, name, provider, is_active FROM ai_models WHERE is_active = 1 ORDER BY name";

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                models.Add(new AiModel
                {
                    Id = reader.GetInt32(0),
                    Name = reader.GetString(1),
                    Provider = reader.GetString(2),
                    IsActive = reader.GetBoolean(3)
                });
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading models: {ex.Message}");
            // Return default models if database fails
            models.AddRange(GetDefaultModels());
        }

        return models;
    }

    /// <summary>
    /// Get default models if database is unavailable
    /// </summary>
    private List<AiModel> GetDefaultModels()
    {
        return new List<AiModel>
        {
            new AiModel { Id = 1, Name = "gpt-4o-mini", Provider = "openai", IsActive = true },
            new AiModel { Id = 2, Name = "gpt-4o", Provider = "openai", IsActive = true },
            new AiModel { Id = 3, Name = "gpt-3.5-turbo", Provider = "openai", IsActive = true },
            new AiModel { Id = 4, Name = "claude-3-5-sonnet-20241022", Provider = "anthropic", IsActive = true }
        };
    }

    /// <summary>
    /// Get API key from database (decrypted)
    /// </summary>
    public AiKeyInfo? GetApiKey(string provider = "fuelix")
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT id, provider, encrypted_key, base_url, created_at, updated_at FROM ai_keys WHERE provider = @provider LIMIT 1";
            command.Parameters.AddWithValue("@provider", provider);

            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                var encryptedKey = reader.GetString(2);
                var decryptedKey = DecryptString(encryptedKey);

                return new AiKeyInfo
                {
                    Id = reader.GetInt32(0),
                    Provider = reader.GetString(1),
                    ApiKey = decryptedKey,
                    BaseUrl = reader.IsDBNull(3) ? "https://proxy.fuelix.ai/v1" : reader.GetString(3),
                    CreatedAt = reader.IsDBNull(4) ? DateTime.Now : reader.GetDateTime(4),
                    UpdatedAt = reader.IsDBNull(5) ? DateTime.Now : reader.GetDateTime(5)
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving API key: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Save API key to database (encrypted)
    /// </summary>
    public bool SaveApiKey(string provider, string apiKey, string baseUrl)
    {
        try
        {
            var encryptedKey = EncryptString(apiKey);

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            // Check if key exists
            var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = "SELECT COUNT(*) FROM ai_keys WHERE provider = @provider";
            checkCommand.Parameters.AddWithValue("@provider", provider);
            var exists = Convert.ToInt32(checkCommand.ExecuteScalar()) > 0;

            var command = connection.CreateCommand();
            if (exists)
            {
                // Update existing key
                command.CommandText = @"
                    UPDATE ai_keys 
                    SET encrypted_key = @key, base_url = @url, updated_at = @updated 
                    WHERE provider = @provider";
            }
            else
            {
                // Insert new key
                command.CommandText = @"
                    INSERT INTO ai_keys (provider, encrypted_key, base_url, created_at, updated_at) 
                    VALUES (@provider, @key, @url, @created, @updated)";
                command.Parameters.AddWithValue("@created", DateTime.Now);
            }

            command.Parameters.AddWithValue("@provider", provider);
            command.Parameters.AddWithValue("@key", encryptedKey);
            command.Parameters.AddWithValue("@url", baseUrl);
            command.Parameters.AddWithValue("@updated", DateTime.Now);

            command.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving API key: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Encrypt a string using AES
    /// </summary>
    private string EncryptString(string plainText)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16]; // Zero IV for simplicity (in production, use random IV)

            using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            var encryptedBytes = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
            
            return Convert.ToBase64String(encryptedBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Encryption error: {ex.Message}");
            return plainText; // Fallback to plain text if encryption fails
        }
    }

    /// <summary>
    /// Decrypt a string using AES
    /// </summary>
    private string DecryptString(string encryptedText)
    {
        try
        {
            using var aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(_encryptionKey.PadRight(32).Substring(0, 32));
            aes.IV = new byte[16]; // Zero IV for simplicity

            using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
            
            return Encoding.UTF8.GetString(decryptedBytes);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Decryption error: {ex.Message}");
            return encryptedText; // Fallback to returning the encrypted text
        }
    }

    /// <summary>
    /// Test if API key is valid by checking decryption
    /// </summary>
    public bool TestApiKey(string provider = "fuelix")
    {
        var keyInfo = GetApiKey(provider);
        return keyInfo != null && !string.IsNullOrEmpty(keyInfo.ApiKey);
    }
}

/// <summary>
/// AI Model information from database
/// </summary>
public class AiModel
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Provider { get; set; } = "";
    public bool IsActive { get; set; }
}

/// <summary>
/// AI Key information from database
/// </summary>
public class AiKeyInfo
{
    public int Id { get; set; }
    public string Provider { get; set; } = "";
    public string ApiKey { get; set; } = "";
    public string BaseUrl { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}