using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class PanelManager : MonoBehaviour {

    public string AnchorName = "D20180809T203100_000";
    public GameObject PanelObject;

    GestureRecognizer _gestureRecognizer;

    // Use this for initialization
    void Start () {
        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        _gestureRecognizer.Tapped += _gestureRecognizer_Tapped;
        _gestureRecognizer.StartCapturingGestures();
        Debug.Log("GestureRecognizer initialized");

        WorldAnchorManager.Instance.AttachAnchor(this.PanelObject, AnchorName);
        Debug.Log("Anchor attached for: " + this.gameObject.name + " - AnchorID: " + AnchorName);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void _gestureRecognizer_Tapped(TappedEventArgs obj)
    {
        Debug.Log("_gesureRecognizer_Tapped");

        WorldAnchorManager.Instance.RemoveAnchor(this.PanelObject);

        PanelObject.transform.Translate(1, 0, 0);

        WorldAnchorManager.Instance.AttachAnchor(this.PanelObject, AnchorName);
        Debug.Log("Anchor reattached for: " + this.gameObject.name + " - AnchorID: " + AnchorName);
    }
}
