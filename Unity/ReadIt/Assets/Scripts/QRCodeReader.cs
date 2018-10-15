using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRCodeReader : MonoBehaviour
{
    public Action<string> CodeProcessor
    {
        get;
        set;
    }

    // Use this for initialization
    void Start()
    {
        Debug.Log("QRCodeReader start at " + DateTime.Now);
//#if !UNITY_EDITOR
//        Debug.Log("Start at " + DateTime.Now);
//        MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
//            result =>
//            {
//                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
//                {
//                    Debug.Log("Got result " + result + " at " + DateTime.Now);
//                    if(CodeProcessor != null)
//                    {
//                        CodeProcessor(result);
//                    }
//                }, false);
//            },
//            null);
//#else
        if (CodeProcessor != null)
        {
            CodeProcessor("MyCode");
        }
//#endif
    }
}
