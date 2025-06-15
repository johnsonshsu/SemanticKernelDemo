using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;

namespace Plugins;

/// <summary>
/// 提供讀取指定文字檔全部內容的功能。
/// </summary>
public class TextFilePlugin
{
    /// <summary>
    /// 文字檔案的路徑。
    /// </summary>
    private readonly string _filePath;

    /// <summary>
    /// 初始化 TextFilePlugin 類別的新實例。
    /// </summary>
    /// <param name="filePath">文字檔案的路徑。</param>
    public TextFilePlugin(string filePath)
    {
        _filePath = filePath;
    }

    [KernelFunction]
    [Description("讀取指定文字檔全部內容")]
    public async Task<string> ReadAllAsync()
    {
        try
        {
            if (!File.Exists(_filePath))
            {
                Console.WriteLine($"檔案不存在: {_filePath}");
                return $"找不到檔案: {_filePath}";
            }
            // 使用 File.ReadAllTextAsync 讀取檔案內容
            return await File.ReadAllTextAsync(_filePath);
        }
        catch (FileNotFoundException ex)
        {
            Console.WriteLine($"讀取檔案失敗，檔案不存在: {_filePath}. 錯誤: {ex.Message}");
            return $"讀取檔案失敗，檔案不存在: {_filePath}. 錯誤: {ex.Message}";
        }
        catch (IOException ex)
        {
            Console.WriteLine($"讀取檔案時發生I/O錯誤: {_filePath}. 錯誤: {ex.Message}");
            return $"讀取檔案時發生I/O錯誤: {_filePath}. 錯誤: {ex.Message}";
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"讀取檔案時發生權限不足錯誤: {_filePath}. 錯誤: {ex.Message}");
            return $"讀取檔案時發生權限不足錯誤: {_filePath}. 錯誤: {ex.Message}";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"讀取檔案時發生未知錯誤: {_filePath}. 錯誤: {ex.Message}");
            return $"讀取檔案時發生未知錯誤: {_filePath}. 錯誤: {ex.Message}";
        }
    }
}
