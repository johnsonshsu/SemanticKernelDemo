using Microsoft.SemanticKernel;
using System.Text.RegularExpressions;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text;
using Plugins;

class Program
{
    static async Task Main()
    {
        // 建立 Kernel Builder
        var builder = Kernel.CreateBuilder();
        // 設定 OpenAI API 金鑰和模型 ID
        string modelId = "gpt-4o";
        // 請在此輸入您的 OpenAI API 金鑰
        string apiKey = "";
        // 未輸入 API 金鑰時，提示使用者輸入 
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            Console.WriteLine("請在程式碼中輸入您的 OpenAI API 金鑰。");
            return;
        }
        // 如果您使用的是 OpenAI，請取消註解以下代碼並填入以下參數
        builder.AddOpenAIChatCompletion(
            modelId: modelId,
            apiKey: apiKey
        );

        // 如果您使用的是 Azure OpenAI，請取消註解以下代碼並填入以下參數
        // builder.AddAzureOpenAIChatCompletion(
        //     deploymentName: "", // ，請在此輸入部署名稱
        //     modelId: modelId, // 請在此輸入模型 ID
        //     apiKey: apiKey, // 請在此輸入 Azure OpenAI API 金鑰
        //     endpoint: "" // 請在此輸入 Azure OpenAI 端點
        // );

        // 建立 Kernel
        var kernel = builder.Build();

        // 文字檔 Plugin
        var textPlugin = new TextFilePlugin("Datas\\demo.txt");
        // PDF 讀取 Plugin
        var pdfPlugin = new PdfReaderPlugin("Datas\\demo.pdf");
        // SQLite Plugin
        var sqlitePlugin = new SqlitePlugin("Datas\\demodb.sqlite");

        // 載入文字檔 Plugin
        kernel.ImportPluginFromObject(textPlugin, "TextFile");
        // 載入 PDF 讀取 Plugin
        kernel.ImportPluginFromObject(pdfPlugin, "PdfReader");
        // 載入 SQLite Plugin
        kernel.ImportPluginFromObject(sqlitePlugin, "Sqlite");

        // Prompt 模板：整合三種來源內容
        var prompt = @"
你是一位助理，根據以下資料回答問題，回答時請用繁體中文回答。

文字檔內容：
{{TextFile.ReadAll}}

PDF 內容：
{{PdfReader.ReadPdfText}}

SQLite 學生成績資料內容：
{{Sqlite.ReadScores}}

SQLite 指定使用者每日收支資料內容：
{{Sqlite.ReadDailyUserRecords userName=$userName}}

SQLite 所有人每日收支資料內容：
{{Sqlite.ReadDailyAllRecords}}

使用者問題：
{{$input}}

請根據以上內容，清楚且簡潔並以條列式方式回答。
並以純文字格式回答，不要加入任何額外的格式。
數字及日期資料與文字結合時前後要加一個空白來區隔。
在回答時不要顯示呼叫了那一個函式名稱。
在回答有關 Sqlite.ReadDailyUserRecords 函式時請注意以下規定：
1. 請顯示我輸入的使用者名稱，即 arguments 參數 userName 值。
2. 將 arguments 參數 userName 值傳給函式 userName 參數。
";

        // 建立函式，使用 Prompt 模板
        var askFunction = kernel.CreateFunctionFromPrompt(
            prompt,
            new OpenAIPromptExecutionSettings
            {
                Temperature = 0.7, // 控制回答的隨機性
                MaxTokens = 2000 // 控制回答的長度
            }
        );
        // 設定控制台編碼為 UTF-8 , 以支援中文輸入和輸出
        Console.InputEncoding = Encoding.UTF8; // 設定輸入編碼為 UTF-8
        Console.OutputEncoding = Encoding.UTF8; // 設定輸出編碼為 UTF-8
        // 取得使用者名稱
        Console.Write("\n請輸入使用者名稱 (輸入 exit 離開): ");
        var userNameInput = Console.ReadLine();
        string userName = userNameInput ?? ""; //避免 null 值
        if (string.IsNullOrEmpty(userName) || userName.ToLower() == "exit") return;

        // 問題迴圈
        while (true)
        {
            // 顯示歡迎訊息
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{userName} 您好，有什麼我可以為您服務的嗎？");
            Console.ResetColor();

            // 提示使用者輸入問題
            Console.Write("\n請輸入問題 (輸入 exit 離開): ");
            var question = Console.ReadLine();
            string questionData = question ?? ""; //避免 null 值
            if (string.IsNullOrEmpty(questionData) || questionData.ToLower() == "exit") break;

            // 設定問題參數
            var arguments = new KernelArguments()
            {
                ["input"] = questionData, // 使用者輸入的問題
                ["TextFile.ReadAll"] = await textPlugin.ReadAllAsync(), // 文字檔內容
                ["PdfReader.ReadPdfText"] = pdfPlugin.ReadPdfText(), // PDF 內容
                ["Sqlite.ReadScores"] = await sqlitePlugin.ReadScores(), // SQLite 學生成績資料內容
                ["Sqlite.ReadDailyAllRecords"] = await sqlitePlugin.ReadDailyAllRecords(), // 所有人每日收支資料內容
                ["Sqlite.ReadDailyUserRecords"] = await sqlitePlugin.ReadDailyUserRecords(userName), // 指定使用者每日收支資料內容
                ["userName"] = userName // 傳遞使用者名稱參數
            };

            // 呼叫函式並取得結果
            var result = await kernel.InvokeAsync(askFunction, arguments);
            // 顯示結果
            Console.ForegroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine($"\nAI 回答內容：\n{result}\n");
            // 重新設定文字顏色
            Console.ResetColor();
            // 顯示分隔線
            Console.WriteLine(new string('-', 50));
            // 等待使用者按鍵
            Console.WriteLine("按任意鍵繼續...");
            Console.ReadKey();
            // 清除控制台
            Console.Clear();
        }
    }
}
