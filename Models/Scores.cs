
using System.Text.Json.Serialization;

namespace Plugin.Models;

/// <summary>
/// 學生成績資料類別
/// 用於存儲學生成績的基本資訊。
/// </summary>
public class Scores
{
    /// <summary>
    /// 學生姓名
    /// </summary>
    [JsonPropertyName("studentName")]
    public string StudentName { get; set; } = "";
    /// <summary>
    /// 學生成績分數
    /// </summary>
    [JsonPropertyName("score")]
    public int Score { get; set; } = 0;
}