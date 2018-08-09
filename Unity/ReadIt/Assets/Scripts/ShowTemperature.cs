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

    }

    IEnumerator PostRequest(string url, string json)
    {
        var uwr = new UnityWebRequest(url, "POST");
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        uwr.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        uwr.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        uwr.SetRequestHeader("Authorization", WebAPIHeaderAuthorization);
        uwr.SetRequestHeader("Content-Type", "application/json");

        //Send the request then wait here until it returns
        yield return uwr.SendWebRequest();

        string result = "Waiting...";
        if (uwr.isNetworkError)
        {
            result = "Error While Sending: " + uwr.error;
        }
        else
        {
            result = "Received: " + uwr.downloadHandler.text;
        }

        Debug.Log(result);
        _textToDisplay = result;
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
}
