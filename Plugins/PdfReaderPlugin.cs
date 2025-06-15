using Microsoft.SemanticKernel;
using System.ComponentModel;
using UglyToad.PdfPig;
using System.Text; // 新增：用於 StringBuilder

namespace Plugins;

/// <summary>
/// 提供讀取 PDF 檔案文字的功能。
/// </summary>
public class PdfReaderPlugin
{
    /// <summary>
    /// PDF 檔案的路徑。
    /// </summary>
    private readonly string _pdfPath;

    /// <summary>
    /// 初始化 PdfReaderPlugin 類別的新實例。
    /// </summary>
    /// <param name="pdfPath">PDF 檔案的路徑。</param>
    public PdfReaderPlugin(string pdfPath)
    {
        // 1. 程式碼可讀性和可維護性：參數驗證
        // 2. 錯誤處理和邊緣情況：參數驗證
        if (string.IsNullOrWhiteSpace(pdfPath))
        {
            Console.WriteLine("PDF 檔案路徑不能為 null 或空白。");
            throw new ArgumentNullException(nameof(pdfPath), "PDF 檔案路徑不能為 null 或空白。");
        }
        _pdfPath = pdfPath;
    }

    /// <summary>
    /// 讀取 PDF 檔案中的所有文字。
    /// </summary>
    /// <returns>PDF 檔案中的所有文字，如果檔案不存在或讀取失敗則返回錯誤訊息。</returns>
    [KernelFunction]
    [Description("讀取 PDF 檔案中的文字")]
    public string ReadPdfText()
    {
        // 錯誤處理和邊緣情況：更明確的檔案不存在處理
        if (!File.Exists(_pdfPath))
        {
            Console.WriteLine($"錯誤：找不到指定的 PDF 檔案 '{_pdfPath}'。請檢查路徑是否正確。");
            return $"錯誤：找不到指定的 PDF 檔案 '{_pdfPath}'。請檢查路徑是否正確。";
        }

        try
        {
            // 使用 PdfPig 讀取 PDF 檔案
            using var document = PdfDocument.Open(_pdfPath);

            // 使用 StringBuilder 處理大量文字拼接
            var textBuilder = new StringBuilder();
            // 使用迴圈遍歷 PDF 檔案每一頁並提取文字
            foreach (var page in document.GetPages())
            {
                textBuilder.AppendLine(page.Text);
            }
            // 返回提取的文字
            if (textBuilder.Length == 0)
            {
                Console.WriteLine($"警告: PDF 檔案 '{_pdfPath}' 中沒有可讀取的文字。");
                return $"警告: PDF 檔案 '{_pdfPath}' 中沒有可讀取的文字。";
            }
            return textBuilder.ToString();
        }
        catch (IOException ex)
        {
            // 錯誤處理和邊緣情況：捕獲 IO 相關異常（例如檔案鎖定、權限問題）
            System.Console.Error.WriteLine($"錯誤：讀取 PDF 檔案 '{_pdfPath}' 時發生 IO 錯誤：{ex.Message}");
            return $"錯誤：讀取 PDF 檔案 '{_pdfPath}' 時發生 IO 錯誤：{ex.Message}";
        }
        catch (Exception ex)
        {
            // 錯誤處理和邊緣情況：捕獲其他未預期的異常
            System.Console.Error.WriteLine($"錯誤：讀取 PDF 檔案 '{_pdfPath}' 時發生未預期的錯誤：{ex.Message}");
            return $"錯誤：讀取 PDF 檔案 '{_pdfPath}' 時發生未預期的錯誤：{ex.Message}";
        }
    }
}
