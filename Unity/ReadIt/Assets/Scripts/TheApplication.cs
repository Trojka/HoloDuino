using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows.Speech;

public class TheApplication : MonoBehaviour {

    KeyCode scanTriggerKey = KeyCode.N;

    Dictionary<string, ToggleDescriptor> registeredControls = new Dictionary<string, ToggleDescriptor>();

    ControlFactory _controlFactory;

    QRCodeReader _qrCodeReader;

    void FillRegisteredDevices()
    {
        //var toggleDescriptor = new ToggleDescriptor();
        //toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.On, "1_on"));
        //toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.Off, "1_off"));

        //registeredControls.Add("reg1", toggleDescriptor);
    }

    void ProcessQRCode(string code)
    {
        StartCoroutine(IotHub.GetDeviceModelForCode(
            code, 
            d =>
            {
                var deviceUI = _controlFactory.CreateControlFromDescriptor(d.SubSystems[0].UI);
                deviceUI.SetActive(true);
            }));
        
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
        _qrCodeReader.CodeProcessor = this.ProcessQRCode;

        FillRegisteredDevices();

        foreach(var ctrl in registeredControls)
        {
            var deviceUI = _controlFactory.CreateControlFromDescriptor(ctrl.Value);
            deviceUI.SetActive(true);
        }
    }
	
	// Update is called once per frame
	void Update () {
 #if UNITY_EDITOR
       if (Input.GetKeyDown(scanTriggerKey))
        {
            StartScan();
        }
#endif
    }
}
