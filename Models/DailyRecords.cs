using System.Text.Json.Serialization;

namespace Plugin.Models;

/// <summary>
/// 每日收支記錄資料類別
/// 用於存儲每日收支記錄的基本資訊。
/// </summary>
public class DailyRecords
{
    /// <summary>
    /// 使用者姓名
    /// </summary>
    [JsonPropertyName("userName")]
    public string UserName { get; set; } = "";
    /// <summary>
    /// 記錄日期
    /// </summary>
    [JsonPropertyName("recordDate")]
    public string RecordDate { get; set; } = DateTime.Now.ToString("yyyy-MM-dd");
    /// <summary>
    /// 收支類型名稱
    /// </summary>
    [JsonPropertyName("typeName")]
    public string TypeName { get; set; } = "";
    /// <summary>
    /// 項目名稱
    /// </summary>
    [JsonPropertyName("itemName")]
    public string ItemName { get; set; } = "";
    /// <summary>
    /// 金額
    /// </summary>
    [JsonPropertyName("amount")]
    public int Amount { get; set; } = 0;
    /// <summary>
    /// 備註
    /// </summary>
    [JsonPropertyName("remark")]
    public string Remark { get; set; } = "";
}