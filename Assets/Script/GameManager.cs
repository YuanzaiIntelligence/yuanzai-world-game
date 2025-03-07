using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : Singleton<GameManager>
{

    [Header("事件广播")]
    public StringEventSO getNewInfoEvent;

    public IEnumerator PostWebRequestSync(string apiUrl, string jsonData)
    {
        // 创建UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        // 设置请求头
        request.SetRequestHeader("Content-Type", "application/json");

        // 发送请求
        yield return request.SendWebRequest();

        // 处理响应
        if (request.result == UnityWebRequest.Result.ConnectionError ||
            request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError("API请求错误: " + request.error);
        }
        else
        {
            // 成功获取响应
            Debug.Log("API响应: " + request.downloadHandler.text);

            getNewInfoEvent.RaisEvent(request.downloadHandler.text, this);

        }
    }
}
