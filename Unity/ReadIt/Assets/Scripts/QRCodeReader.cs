using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRCodeReader : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log("About to start at " + DateTime.Now);
#if !UNITY_EDITOR
        Debug.Log("Start at " + DateTime.Now);
        MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
            result =>
            {
                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
            {
                  Debug.Log("Got result " + result + " at " + DateTime.Now);
              },
            false);
            },
            null);
#endif
    }

    // Update is called once per frame
    void Update()
    {

    }
}
