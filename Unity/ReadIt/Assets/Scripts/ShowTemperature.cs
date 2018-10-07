using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public partial class ShowTemperature : MonoBehaviour {

    Text _textWidget;

    string _textToDisplay;
    List<string> _deviceIds;
    bool _deviceIdsFilled;

    // Use this for initialization
    /*async*/ void Start () {
        Debug.Log("Starting");


        _deviceIdsFilled = false;

        _textWidget = GetComponentInChildren<Text>();
#if !UNITY_EDITOR
        //await ReadItLib.ReadIt.Start(this.SetValue);
        //await ReadItLib.ReadIt.updateDeviceIdsComboBoxes(this.ShowDevices);
#else
        _textToDisplay = "Unity text";
        //await TimeSpan.FromSeconds(1);
#endif

        //StartCoroutine(PostRequest(WebAPIEndpoint, "{\"query\":\"select * from devices\"}"));

        SetParseResult();
    }

    //IEnumerator PostRequest(string url, string json)
    //{
    //    var uwr = new UnityWebRequest(url, "POST");
    //    byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
    //    uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
    //    uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
    //    uwr.SetRequestHeader("Authorization", WebAPIHeaderAuthorization);
    //    uwr.SetRequestHeader("Content-Type", "application/json");

    //    //Send the request then wait here until it returns
    //    yield return uwr.SendWebRequest();

    //    string result = "Waiting...";
    //    if (uwr.isNetworkError)
    //    {
    //        result = "Error While Sending: " + uwr.error;
    //    }
    //    else
    //    {
    //        result = /*"Received: " +*/ uwr.downloadHandler.text;
    //    }

    //    Debug.Log(result);

    //    //List<DeviceModel> listOfModels = new List<DeviceModel>() { new DeviceModel() { deviceId = "dev_id" } };
    //    //string jsonString = ShowTemperature.ToJson(listOfModels.ToArray());


    //    string formattedJson = "{\"Received\"" + result.Substring("Received".Length) + "}";

    //    var N = JSON.Parse(result);
    //    var cnt = N.AsArray.Count;
    //    var sysCount = N.AsArray[0]["tags"]["systems"]["count"];

    //    //DeviceModel[] devices = ShowTemperature.FromJson<DeviceModel>(formattedJson);

    //    //_textToDisplay = devices[0].deviceId;
    //}

    public void SetParseResult()
    {
        //var parser = new DeviceParser();
    }

    // Update is called once per frame
    void Update () {
		_textWidget.text = _textToDisplay;
        //if(_deviceIdsFilled)
        //{
        //    if (_deviceIds != null && _deviceIds.Count > 0)
        //        _textWidget.text = string.Join(",", _deviceIds);
        //    else
        //        _textWidget.text = "No devices found";
        //}
	}

    void SetValue(string theValue)
    {
        _textToDisplay = theValue;
    }

    //void ShowDevices(bool ready, List<string> deviceIds)
    //{
    //    _deviceIds = deviceIds;
    //    _deviceIdsFilled = ready;
    //}


    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Received;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Received = array;
        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Received;
    }

}
