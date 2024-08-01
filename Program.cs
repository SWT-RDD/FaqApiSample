using Newtonsoft.Json;

//主程式
var chatRoomVM = new ChatRoomVM
{
    ApiKey = "your_key",
    LogChatLogHistorySN = -1,
    ChatLogs = new List<ChatLog>
    {
        new ChatLog { HumanContent = "你好阿"},
    },
    ResponseFormat = (int)ChatRoomVMResponseFormat.Html
};
await PostChatRoomVM(chatRoomVM);

//HttpPost副程式
async Task PostChatRoomVM(ChatRoomVM chatRoomVM)
{
    var url = "https://www.sol-idea.com.tw/back/api/CompletionBot/SimplifiedFAQ";
    var jsonChatRoomVM = JsonConvert.SerializeObject(chatRoomVM);
    MultipartFormDataContent form = new MultipartFormDataContent();
    form.Add(new StringContent(jsonChatRoomVM), "jsonChatRoomVM");

    using (var client = new HttpClient())
    {
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
}
public class ChatLog
{
    public string HumanContent { get; set; }
    public string AIContent { get; set; }
}

