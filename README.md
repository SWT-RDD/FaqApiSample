# API 使用說明文件

## Non-Stream版本 API 概述
本 API 用於接收聊天記錄與設定，並返回機器人回應的結果。

### URL
Post https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedFAQ

### 請求格式
| KEY            | VALUE                |
| -------------- | -------------------- |
| Content-Type   | multipart/form-data  |

### 請求資料範例
#### Layer 1
| KEY            | VALUE                |
| -------------- | -------------------- |
| jsonChatRoomVM | json 字串化後的字典，範例 |
|                | {                   |
|                | "ApiKey": "your_key",|
|                | "ResponseFormat": 0,|
|                | "LogChatLogHistorySN": -1,|
|                | "ChatLogs": [{"HumanContent": "你好阿"}],|
|                | "RequireSearchResults": 1|
|                | }                   |

#### Layer 2
| KEY                   | VALUE                       |
| --------------------- | --------------------------- |
| ApiKey                | 你的 api key                |
| ResponseFormat        | 要markdown格式填0，Html標籤格式填1            |
| LogChatLogHistorySN   | 想要接續對話紀錄的對話編號，若是開新對話請填 -1 |
| ChatLogs              | 將要送給機器人的字串放在 HumanContent，最少需要3個字，最多200字     |
| RequireSearchResults  | 是否需要搜尋結果，不需要填0，需要填1            |

### curl 請求範例
```
curl https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedFAQ --form jsonChatRoomVM="{\"ApiKey\":\"your_key\", \"ResponseFormat\":0, \"LogChatLogHistorySN\":-1,\"ChatLogs\":[{\"HumanContent\": \"你好阿\", }]}"
```
記得換掉your_key
### 回應資料範例
#### Layer 1
| KEY        | VALUE                      |
| ---------- | -------------------------- |
| JsonFormat | json 格式回應，範例          |
|            | {                         |
|            | "JsonData": "{...}",       |
|            | "Message": "查詢成功",      |
|            | "Error": false             |
|            | }                         |

#### Layer 2 (JsonData部分)
| KEY        | VALUE                      |
| ---------- | -------------------------- |
| JsonData   | json 字串化後的資料，範例     |
|            | {                         |
|            | "ApiKey": "your_key",      |
|            | "ResponseFormat": 0,|
|            | "LogChatLogHistorySN": 1234,|
|            | "ChatLogs": [{"HumanContent": "你好阿", "AIContent": "你好！有什麼我可以幫助你的嗎？"}],|
|            | "SearchResults": [{"Name": "文件名稱", "Title": "文件標題", ...}]|
|            | }                         |

#### Layer 3
| KEY                  | VALUE                     |
| -------------------- | ------------------------- |
| ApiKey               | 你的 api key              |
| ResponseFormat       | markdown格式0，Html標籤格式1            |
| LogChatLogHistorySN  | 本次對話的對話編號，如果下次要接著問(保持歷史對話)需要記錄這個編號         |
| ChatLogs             | 機器人的回應會放在 AIContent |
| SearchResults        | 搜尋結果陣列               |

#### Layer 4 (SearchResults部分)
| KEY                  | VALUE                     |
| -------------------- | ------------------------- |
| Name                 | 文件名稱                  |
| Date                 | 文件日期                  |
| FileType             | 文件類型                  |
| Title                | 文件標題                  |
| Abstract             | 文件摘要                  |
| Image                | 文件圖片                  |
| IndexName            | 資料集名稱                  |
| CusField             | 自訂欄位陣列["自訂1","自訂2","自訂3"]              |

### 回應錯誤處理
使用 Http 400 Bad Request

#### 錯誤格式
| KEY                  | VALUE                     |
| -------------------- | ------------------------- |
| Code                 | 錯誤代碼                  |
| Message              | 錯誤訊息                  |

#### 錯誤列表
| Code                 | Message                   |
| -------------------- | ------------------------- |
| 3001                 | Json字串解析失敗                |
| 3002                 | ChatLogs內容不能為空                  |
| 3003                 | 輸入字串過長(超過200字)             |
| 3004                 | 輸入字串過短(少於3字)                  |
| 4001                 | ApiKey錯誤、不存在或過期                |
| 4002                 | 問答次數不足                  |
| 4004                 | 對話編號沒有權限或不存在                  |

## Stream版本 API 概述
本 API 用於接收聊天記錄與設定，並返回機器人回應的結果。

### URL
- Post https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedStreamingFAQ
- Get https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedStreamingFAQ/{sn}

### Post請求格式
| KEY            | VALUE                |
| -------------- | -------------------- |
| Content-Type   | multipart/form-data  |

### Post請求資料範例
#### Layer 1
| KEY            | VALUE                |
| -------------- | -------------------- |
| jsonChatRoomVM | json 字串化後的字典，範例 |
|                | {                   |
|                | "ApiKey": "your_key",|
|                | "ResponseFormat": 0,|
|                | "LogChatLogHistorySN": -1,|
|                | "ChatLogs": [{"HumanContent": "你好阿"}],|
|                | "RequireSearchResults": 1|
|                | }                   |

#### Layer 2
| KEY                   | VALUE                       |
| --------------------- | --------------------------- |
| ApiKey                | 你的 api key                |
| ResponseFormat        | 要markdown格式填0，Html標籤格式填1            |
| LogChatLogHistorySN   | 想要接續對話紀錄的對話編號，若是開新對話請填 -1 |
| ChatLogs              | 將要送給機器人的字串放在 HumanContent，最少需要3個字，最多200字     |
| RequireSearchResults  | 是否需要搜尋結果，不需要填0，需要填1            |

### curl 請求範例
```
curl https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedStreamingFAQ --form jsonChatRoomVM="{\"ApiKey\":\"your_key\", \"ResponseFormat\":0, \"LogChatLogHistorySN\":-1,\"ChatLogs\":[{\"HumanContent\": \"你好阿\", }]}"
```
記得換掉your_key
### Post回應資料範例
#### Layer 1
| KEY        | VALUE                      |
| ---------- | -------------------------- |
| JsonFormat | json 格式回應，範例          |
|            | {                         |
|            | "JsonData": "{...}",       |
|            | "Message": "查詢成功",      |
|            | "Error": false             |
|            | }                         |

#### Layer 2 (JsonData部分)
| KEY        | VALUE                      |
| ---------- | -------------------------- |
| JsonData   | json 字串化後的資料，範例     |
|            | {                         |
|            | "ApiKey": "your_key",      |
|            | "ResponseFormat": 0,|
|            | "LogChatLogHistorySN": 1234,|
|            | "FocusLogChatLogSN": 654321,|
|            | "ChatLogs": [{"HumanContent": "你好阿",}],|
|            | "SearchResults": [{"Name": "文件名稱", "Title": "文件標題", ...}]|
|            | }                         |

#### Layer 3
| KEY                  | VALUE                     |
| -------------------- | ------------------------- |
| ApiKey               | 你的 api key              |
| ResponseFormat       | markdown格式0，Html標籤格式1            |
| LogChatLogHistorySN  | 本次對話的對話編號，如果下次要接著問(保持歷史對話)需要記錄這個編號         |
| FocusLogChatLogSN    | 用來呼叫Get api的編號         |
| ChatLogs             | 機器人的回應不會放在這裡，需要用Get api取得 |
| SearchResults        | 搜尋結果陣列               |

#### Layer 4 (SearchResults部分)
| KEY                  | VALUE                     |
| -------------------- | ------------------------- |
| Name                 | 文件名稱                  |
| Date                 | 文件日期                  |
| FileType             | 文件類型                  |
| Title                | 文件標題                  |
| Abstract             | 文件摘要                  |
| Image                | 文件圖片                  |
| IndexName            | 資料集名稱                  |
| CusField             | 自訂欄位陣列["自訂1","自訂2","自訂3"]              |

### Get請求方法
- Get https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedStreamingFAQ/{FocusLogChatLogSN}
- 可以使用後端(此範例程式)，或前端(見EventSource)讀取資料

### 回應錯誤處理
使用 Http 400 Bad Request

#### 錯誤格式
| KEY                  | VALUE                     |
| -------------------- | ------------------------- |
| Code                 | 錯誤代碼                  |
| Message              | 錯誤訊息                  |

#### 錯誤列表
| Code                 | Message                   |
| -------------------- | ------------------------- |
| 3001                 | Json字串解析失敗                |
| 3002                 | ChatLogs內容不能為空                  |
| 3003                 | 輸入字串過長(超過200字)             |
| 3004                 | 輸入字串過短(少於3字)                  |
| 4001                 | ApiKey錯誤、不存在或過期                |
| 4002                 | 問答次數不足                  |
| 4004                 | 對話編號沒有權限或不存在                  |

## Rating版本 API 概述
本 API 用於對機器人的回答進行評分（按讚、按倒讚）。

### URL
Patch https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedRating

### 請求格式
| KEY            | VALUE                |
| -------------- | -------------------- |
| Content-Type   | multipart/form-data  |

### 請求資料範例
#### Layer 1
| KEY            | VALUE                |
| -------------- | -------------------- |
| jsonRatingVM   | json 字串化後的字典，範例 |
|                | {                   |
|                | "ApiKey": "your_key",|
|                | "LogChatLogSN": 123456,|
|                | "RatingType": 1,|
|                | "RatingFeedback": "回答很有幫助"|
|                | }                   |

#### Layer 2
| KEY            | VALUE                       |
| -------------- | --------------------------- |
| ApiKey         | 你的 api key                |
| LogChatLogSN   | 對話紀錄編號（從 Stream API 的 FocusLogChatLogSN 取得）|
| RatingType     | 評分類型：1=按讚、2=按倒讚、3=未評分 |
| RatingFeedback | 評分回饋（選填，最多500字）  |

### curl 請求範例
```
curl -X PATCH "https://gufofaq.gufolab.com/api/CompletionBot/SimplifiedRating" --form jsonRatingVM="{\"ApiKey\":\"your_key\", \"LogChatLogSN\":123456, \"RatingType\":1, \"RatingFeedback\":\"回答很有幫助\"}"
```
記得換掉your_key和123456

### 回應資料範例
#### Layer 1
| KEY        | VALUE                      |
| ---------- | -------------------------- |
| JsonFormat | json 格式回應，範例          |
|            | {                         |
|            | "Message": "評分更新成功",  |
|            | "Error": false             |
|            | }                         |

#### Layer 2
| KEY        | VALUE                      |
| ---------- | -------------------------- |
| Message    | 回應訊息                    |
| Error      | 是否有錯誤，成功為false      |

### 回應錯誤處理
使用 Http 400 Bad Request

#### 錯誤格式
| KEY                  | VALUE                     |
| -------------------- | ------------------------- |
| Code                 | 錯誤代碼                  |
| Message              | 錯誤訊息                  |

#### 錯誤列表
| Code                 | Message                   |
| -------------------- | ------------------------- |
| 3001                 | Json字串解析失敗                |
| 4001                 | ApiKey錯誤、不存在或過期                |
| 4004                 | 對話編號沒有權限或不存在                  |
| 5001                 | 評分類型異常，必須是1(按讚)、2(按倒讚)或3(未評分) |
| 5002                 | 評分回饋過長，最多500字                  |

