using Newtonsoft.Json;
using System;
using System.Text;

//共用HttpClient
var handler = new HttpClientHandler();
handler.ServerCertificateCustomValidationCallback =
    (httpRequestMessage, cert, cetChain, policyErrors) =>
    {
        return true;
    };
HttpClient client = new HttpClient(handler); //如果碰到SSL憑證問題，可能可以嘗試加上或拿掉handler

//主程式
var chatRoomVM = new ChatRoomVM
{
    ApiKey = "your_key", //請在此替換成你的key
    LogChatLogHistorySN = -1,
    ChatLogs = new List<ChatLog>
    {
        new ChatLog { HumanContent = "你好嗎?"},
    },
    ResponseFormat = (int)ChatRoomVMResponseFormat.Html
};

//-----------在這裡切換原本的FAQ(解除NonStream註解)，或是Streaming版本(解除Stream Step1 Step2註解)。
//await PostChatRoomVM(chatRoomVM); //NonStream

int sn = await PostChatRoomVMStreaming(chatRoomVM); //Stream Step1
await GetStreamingResponse(sn); //Stream Step2



//HttpPost副程式(Stream Step1)
async Task<int> PostChatRoomVMStreaming(ChatRoomVM chatRoomVM)
{
    var url = "https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedStreamingFAQ";
    var jsonChatRoomVM = JsonConvert.SerializeObject(chatRoomVM);

    MultipartFormDataContent form = new MultipartFormDataContent();
    form.Add(new StringContent(jsonChatRoomVM), "jsonChatRoomVM");

    var response = await client.PostAsync(url, form);
    if (response.IsSuccessStatusCode)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonFormat = JsonConvert.DeserializeObject<JsonFormat>(responseContent);
        var data = JsonConvert.DeserializeObject<ChatRoomVM>(jsonFormat.JsonData);
        Console.WriteLine(data.LogChatLogHistorySN + "  " + data.FocusLogChatLogSN);
        return data.FocusLogChatLogSN;
    }
    else
    {
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<ECustomError>(errorContent);
            Console.WriteLine($"Handled Error: {error.Message}, Code: {error.Code}");
        }
        else
        {
            Console.WriteLine($"Failed to post chatRoomVM. Status code: {response.StatusCode}");
        }
    }
    return -1;
}

//HttpGet副程式(Stream Step2)
async Task GetStreamingResponse(int sn)
{
    Console.WriteLine("Streaming:");

    var url = $"https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedStreamingFAQ/{sn}";

    using (var request = new HttpRequestMessage(HttpMethod.Get, url))
    {
        using (var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
        {
            if (response.IsSuccessStatusCode)
            {
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        StringBuilder messageBuilder = new StringBuilder();

                        while (!reader.EndOfStream)
                        {
                            line = await reader.ReadLineAsync();

                            if (line.StartsWith("data: "))
                            {
                                string data = JsonConvert.DeserializeObject<string>(line.Substring(6)); // 為了demo，移除"data: "前綴。如果是要傳到前端可以直接傳 (參考EventSource範例)

                                if (data == "[END]")
                                {
                                    Console.WriteLine("\nStreaming End.");
                                    break;
                                }

                                Console.Write(data);
                                messageBuilder.Append(data);
                            }
                        }

                        Console.WriteLine("\n\n\nFull Response:");
                        Console.WriteLine(messageBuilder.ToString());
                    }
                }
            }
            else
            {
                if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    var error = JsonConvert.DeserializeObject<ECustomError>(errorContent);
                    Console.WriteLine($"Handled Error: {error.Message}, Code: {error.Code}");
                }
                else
                {
                    Console.WriteLine($"Failed to post chatRoomVM. Status code: {response.StatusCode}");
                }
            }
        }
    }
}

//HttpPost副程式(NonStream)
async Task PostChatRoomVM(ChatRoomVM chatRoomVM)
{
    var url = "https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedFAQ";
    var jsonChatRoomVM = JsonConvert.SerializeObject(chatRoomVM);
    MultipartFormDataContent form = new MultipartFormDataContent();
    form.Add(new StringContent(jsonChatRoomVM), "jsonChatRoomVM");

    var response = await client.PostAsync(url, form);
    if (response.IsSuccessStatusCode)
    {
        var responseContent = await response.Content.ReadAsStringAsync();
        var jsonFormat = JsonConvert.DeserializeObject<JsonFormat>(responseContent);
        var data = JsonConvert.DeserializeObject<ChatRoomVM>(jsonFormat.JsonData);
        Console.WriteLine(data.LogChatLogHistorySN + "  " + data.ChatLogs.Last().AIContent);
    }
    else
    {
        if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            var error = JsonConvert.DeserializeObject<ECustomError>(errorContent);
            Console.WriteLine($"Handled Error: {error.Message}, Code: {error.Code}");
        }
        else
        {
            Console.WriteLine($"Failed to post chatRoomVM. Status code: {response.StatusCode}");
        }
    }
}

//資料結構
public enum ChatRoomVMResponseFormat : int
{
    Markdown = 0,
    Html = 1
}

public class JsonFormat
{
    public string JsonData { get; set; }
}

public class ChatRoomVM
{
    public string ApiKey { get; set; }
    public int LogChatLogHistorySN { get; set; }
    public List<ChatLog> ChatLogs { get; set; }
    public int ResponseFormat { get; set; }
    public int FocusLogChatLogSN { get; set; }
}
public class ChatLog
{
    public string HumanContent { get; set; }
    public string AIContent { get; set; }
}
public class ECustomError
{
    public string Message { get; set; }
    public string Code { get; set; }
}

