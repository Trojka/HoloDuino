﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QRCodeReader : MonoBehaviour
{
    bool _doScan = false;
    string _lastScannedCode;

    public Action<string> CodeProcessor
    {
        get;
        set;
    }

    public void StartScan()
    {
        _doScan = true;
        _lastScannedCode = string.Empty;
    }

    // Use this for initialization
    void Start()
    {
//        Debug.Log("QRCodeReader start at " + DateTime.Now);
////#if !UNITY_EDITOR
////        Debug.Log("Start at " + DateTime.Now);
////        MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
////            result =>
////            {
////                UnityEngine.WSA.Application.InvokeOnAppThread(() =>
////                {
////                    Debug.Log("Got result " + result + " at " + DateTime.Now);
////                    if(CodeProcessor != null)
////                    {
////                        CodeProcessor(result);
////                    }
////                }, false);
////            },
////            null);
////#else
//        if (CodeProcessor != null)
//        {
//            CodeProcessor("087E94E8-5A0F-43A9-B20F-B17CC3CBC9C5");
//        }
////#endif
    }

    private void Update()
    {
        if(_doScan)
        {
            Debug.Log("QRCodeReader start at " + DateTime.Now);
#if !UNITY_EDITOR
            Debug.Log("Start at " + DateTime.Now);
            MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
                result =>
                {
                    if (!string.IsNullOrEmpty(_lastScannedCode))
                        return;

                    _lastScannedCode = result;
                    UnityEngine.WSA.Application.InvokeOnAppThread(() =>
                    {
                        Debug.Log("Got result " + _lastScannedCode + " at " + DateTime.Now);
                        if (CodeProcessor != null)
                        {
                            CodeProcessor(result);
                        }
                    }, false);
                },
                TimeSpan.FromSeconds(15));
#else
            if (CodeProcessor != null)
            {
                CodeProcessor("087E94E8-5A0F-43A9-B20F-B17CC3CBC9C5");
            }
#endif

            _doScan = false;
        }
    }
}
