using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class TheApplication : MonoBehaviour {

    Dictionary<string, ToggleDescriptor> registeredControls = new Dictionary<string, ToggleDescriptor>();

    ControlFactory _controlFactory;

    QRCodeReader _qrCodeReader;

    void FillRegisteredDevices()
    {
        var toggleDescriptor = new ToggleDescriptor();
        toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.On, "1_on"));
        toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.Off, "1_off"));

        registeredControls.Add("reg1", toggleDescriptor);
    }

    public void StartScan()
    {
        Debug.Log("Start QR-code scan");
        _qrCodeReader.enabled = true;
    }

    // Use this for initialization
    void Start () {
        _controlFactory = this.GetComponent<ControlFactory>();
        _qrCodeReader = this.GetComponent<QRCodeReader>();

        FillRegisteredDevices();

        foreach(var ctrl in registeredControls)
        {
            var deviceUI = _controlFactory.CreateControlFromDescriptor(ctrl.Value);
            deviceUI.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
