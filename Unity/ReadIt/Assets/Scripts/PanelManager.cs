using HoloToolkit.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PanelManager : MonoBehaviour {

    public string AnchorName = "D20180809T203100_000";
    public GameObject PanelObject;

    GestureRecognizer _gestureRecognizer;

    bool toggle = false;

    bool updatePosition = false;

    // Use this for initialization
    void Start () {
        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        _gestureRecognizer.Tapped += _gestureRecognizer_Tapped;
        _gestureRecognizer.StartCapturingGestures();
        Debug.Log("GestureRecognizer initialized");

        WorldAnchorManager.Instance.AttachAnchor(this.PanelObject, AnchorName);
        Debug.Log("Anchor attached for: " + this.PanelObject.name + " - AnchorID: " + AnchorName);
    }



    int i = 0;
    // Update is called once per frame
    void Update () {
		if(updatePosition)
        {
            try
            {
                float x = -2;
                float y = 0;
                float z = 0;

                int step = i % 3;

                if(step == 0)
                {
                    WorldAnchorManager.Instance.RemoveAnchor(this.PanelObject);
                    Debug.Log("Anchor removed for: " + this.PanelObject.name + " - AnchorID: " + AnchorName);
                }

                if (step == 1)
                {
                    PanelObject.transform.Translate(x, y, z);
                    Debug.Log("Object: " + this.PanelObject.name + " translated");
                }

                if (step == 2)
                {
                    WorldAnchorManager.Instance.AttachAnchor(this.PanelObject, AnchorName);
                    Debug.Log("Anchor reattached for: " + this.PanelObject.name + " - AnchorID: " + AnchorName);
                }

                i++;
            }
            catch (Exception ex)
            {
            }
            finally
            {
                updatePosition = false;
            }

        }
    }

    private void _gestureRecognizer_Tapped(TappedEventArgs obj)
    {
        Debug.Log("_gesureRecognizer_Tapped");

        //#if !UNITY_EDITOR
        //        toggle = !toggle;
        //        ReadItLib.ActivateIt.ExecLedOn(toggle);
        //#endif

        updatePosition = true;

    }
}
