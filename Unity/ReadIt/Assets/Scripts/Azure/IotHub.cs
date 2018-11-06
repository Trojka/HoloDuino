using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public partial class IotHub {

    //static string descriptor = "" +
    //  "{\"systems\":{" +
    //  "\"name\":\"Buttons\"," +
    //  "\"count\":1," +
    //  "\"sys1\":{" +
    //    "\"id\":1," +
    //    "\"ui\":{" +
    //      "\"type\":\"toggle\"," +
    //      "\"actions\":{" +
    //        "\"count\":2," +
    //        "\"action1\":{\"event\":\"toggleon\",\"method\":\"1_on\"}," +
    //        "\"action2\":{\"event\":\"toggleoff\",\"method\":\"1_off\"}" +
    //      "}" +
    //    "}}," +
    //  //"\"sys2\":{" +
    //  //  "\"id\":2," +
    //  //  "\"ui\":{" +
    //  //    "\"type\":\"toggle\"," +
    //  //    "\"actions\":{" +
    //  //      "\"count\":2," +
    //  //      "\"action1\":{\"event\":\"toggleon\",\"method\":\"2_on\"}," +
    //  //      "\"action2\":{\"event\":\"toggleoff\",\"method\":\"2_off\"}" +
    //  //    "}" +
    //  //  "}}" +
    //  "}}";

    //static string SelectDeviceQuery = "{\"query\":\"select * from devices\"}";
    static string SelectDeviceQuery = "{{\"query\":\"select * from devices where deviceid='{0}'\"}}";

    public static IEnumerator GetDeviceModelForCode(string code, Action<DeviceModel> resultAction)
    {
        var qry = string.Format(SelectDeviceQuery, code);
        var response = PostRequest(WebAPIEndpoint, qry);
        yield return response;

        string result = "Waiting...";
        if (response.webRequest.isNetworkError)
        {
            result = "Error While Sending: " + response.webRequest.error;
        }
        else
        {
            result = /*"Received: " +*/ response.webRequest.downloadHandler.text;
        }

        Debug.Log(result);
        var parser = new DeviceParser(result);
        parser.Device.DeviceId = code;
        resultAction(parser.Device);
    }

    public static void SendMethod(string deviceId, string method)
    {
        Debug.Log("sending to [" + deviceId + "] method: " + method);
    }

    static UnityWebRequestAsyncOperation PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Authorization", WebAPIHeaderAuthorization);
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        return uwr.SendWebRequest();

        string result = "Waiting...";
        if (uwr.isNetworkError)
        {
            result = "Error While Sending: " + uwr.error;
        }
        else
        {
            result = /*"Received: " +*/ uwr.downloadHandler.text;
        }

        Debug.Log(result);

        //List<DeviceModel> listOfModels = new List<DeviceModel>() { new DeviceModel() { deviceId = "dev_id" } };
        //string jsonString = ShowTemperature.ToJson(listOfModels.ToArray());


        string formattedJson = "{\"Received\"" + result.Substring("Received".Length) + "}";

        var N = JSON.Parse(result);
        var cnt = N.AsArray.Count;
        var sysCount = N.AsArray[0]["tags"]["systems"]["count"];

        //DeviceModel[] devices = ShowTemperature.FromJson<DeviceModel>(formattedJson);

        //_textToDisplay = devices[0].deviceId;
    }
}
