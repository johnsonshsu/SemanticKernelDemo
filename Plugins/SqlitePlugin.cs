using Microsoft.SemanticKernel;
using System.ComponentModel;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using Plugin.Models;

namespace Plugins;

/// <summary>
/// 提供從 SQLite 資料庫讀取學生成績及每日收支記錄的功能。
/// </summary>
public class SqlitePlugin
{
    /// <summary>
    /// 資料庫檔案的路徑
    /// </summary>
    private readonly string _dbPath;

    /// <summary>
    /// 初始化 SqlitePlugin 類別的新實例。
    /// </summary>
    /// <param name="dbPath">資料庫檔案的路徑。</param>
    public SqlitePlugin(string dbPath)
    {
        _dbPath = dbPath;
    }

    [KernelFunction]
    [Description("從 SQLite 資料庫讀取學生成績列表，並以 JSON 格式返回。JSON 結構範例：[{\"StudentName\":\"學生A\",\"Score\":90},{\"StudentName\":\"學生B\",\"Score\":85}]")]
    public async Task<string> ReadScores()
    {
        var scores = new List<Scores>();
        if (!System.IO.File.Exists(_dbPath))
        {
            // 更好的錯誤處理，例如拋出異常或返回空列表並記錄錯誤
            Console.WriteLine($"錯誤：找不到資料庫檔案：{_dbPath}");
            return JsonSerializer.Serialize(scores).ToString();
        }

        try
        {
            // 建立 SqliteConnection 物件
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            // 開啟連線
            await connection.OpenAsync();
            // 建立命令物件
            using var command = connection.CreateCommand();
            // 構建 SQL 查詢語句
            command.CommandText = "SELECT StudentName, Score FROM Scores";
            // 執行 SQL 查詢
            using var reader = await command.ExecuteReaderAsync();
            // 讀取查詢結果
            while (await reader.ReadAsync())
            {
                scores.Add(new Scores
                {
                    StudentName = reader.GetString(0),
                    Score = reader.GetInt32(1)
                });
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"資料庫錯誤 (ReadScores): {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發生未知錯誤 (ReadScores): {ex.Message}");
        }
        if (scores.Count == 0)
        {
            Console.WriteLine("警告：沒有找到任何學生成績記錄。");
        }
        // 將結果序列化為 JSON 字串並返回
        return JsonSerializer.Serialize(scores).ToString();
    }

    [KernelFunction]
    [Description("從 SQLite 資料庫讀取所有人每日收支記錄列表，並以 JSON 格式返回。JSON 結構範例：[{\"UserName\":\"使用者A\",\"RecordDate\":\"2023-01-01\",\"TypeName\":\"收入\",\"ItemName\":\"薪水\",\"Amount\":30000,\"Remark\":\"\"}]")]
    public async Task<string> ReadDailyAllRecords()
    {
        // 傳入空字串以讀取所有使用者的每日收支記錄
        return await ReadDailyUserRecords(""); 
    }

    [KernelFunction]
    [Description("從 SQLite 資料庫讀取指定使用者名稱的每日收支記錄列表，並以 JSON 格式返回。JSON 結構範例：[{\"UserName\":\"使用者A\",\"RecordDate\":\"2023-01-01\",\"TypeName\":\"收入\",\"ItemName\":\"薪水\",\"Amount\":30000,\"Remark\":\"\"}]")]
    public async Task<string> ReadDailyUserRecords([Description("要查詢的使用者名稱")] string userName)
    {
        var records = new List<DailyRecords>();
        if (!System.IO.File.Exists(_dbPath))
        {
            Console.WriteLine($"錯誤：找不到資料庫檔案：{_dbPath}");
            return JsonSerializer.Serialize(records).ToString();
        }

        try
        {
            // 建立 SqliteConnection 物件
            using var connection = new SqliteConnection($"Data Source={_dbPath}");
            // 開啟連線 
            await connection.OpenAsync();
            // 建立命令物件
            using var command = connection.CreateCommand();
            // 構建 SQL 查詢語句
            command.CommandText = "SELECT UserName, RecordDate, TypeName, ItemName, Amount, Remark FROM DailyRecords ";
            // 如果 userName 不為空，則添加 WHERE 條件
            if (!string.IsNullOrEmpty(userName))
            {
                command.CommandText += "WHERE UserName = @UserName ";
                command.Parameters.AddWithValue("@UserName", userName);
            }
            // 執行 SQL 查詢
            using var reader = await command.ExecuteReaderAsync();
            // 讀取查詢結果
            while (await reader.ReadAsync())
            {
                records.Add(new DailyRecords
                {
                    UserName = reader.GetString(0),
                    RecordDate = reader.GetString(1),
                    TypeName = reader.GetString(2),
                    ItemName = reader.GetString(3),
                    Amount = reader.GetInt32(4),
                    Remark = reader.GetString(5)
                });
            }
        }
        catch (SqliteException ex)
        {
            Console.WriteLine($"資料庫錯誤 (ReadDailyRecords): {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"發生未知錯誤 (ReadDailyRecords): {ex.Message}");
        }
        if (records.Count == 0)
        {
            Console.WriteLine("警告：沒有找到任何每日收支記錄。");
        }
        // 將結果序列化為 JSON 字串並返回
        return JsonSerializer.Serialize(records).ToString();
    }
}