using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR.WSA.Input;
using Newtonsoft.Json;

public class TheApplication : MonoBehaviour {

    KeyCode scanTriggerKey = KeyCode.N;

    Dictionary<string, ToggleDescriptor> registeredControls = new Dictionary<string, ToggleDescriptor>();

    ControlFactory _controlFactory;
    GestureRecognizer _gestureRecognizer;
    QRCodeReader _qrCodeReader;

    DeviceRegistry _registry;
    ControlManager _configuringControlManager;
    DeviceModel _configuringDevice;

    bool _isScanning = false;
    bool _finishedScanning = false;

    string RegisteryFolder
    {
        get { return string.Format("{0}/readit", Application.persistentDataPath); }
    }

    string RegisteryPath
    {
        get { return string.Format("{0}/deviceregistry.txt", RegisteryFolder); }
    }

    void FillRegisteredDevices()
    {
        if (UnityEngine.Windows.File.Exists(RegisteryPath))
        {
            byte[] data = UnityEngine.Windows.File.ReadAllBytes(RegisteryPath);
            if(data != null && data.Length > 0)
            {
                string text = Encoding.ASCII.GetString(data);
                JsonSerializerSettings settings = new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                };
                _registry = JsonConvert.DeserializeObject<DeviceRegistry>(text, settings);
                Debug.Log("Registered devices: " + _registry.Devices.Count);
            }
            else
            {
                _registry = new DeviceRegistry();
            }
        }
        else
        {
            _registry = new DeviceRegistry();
        }

    }

    void RegisterDevice(DeviceModel device)
    {
        _registry.Devices.Add(device);

        if (!UnityEngine.Windows.Directory.Exists(RegisteryFolder))
            UnityEngine.Windows.Directory.CreateDirectory(RegisteryFolder);

        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };
        var text = JsonConvert.SerializeObject(_registry, settings); //"de tekst om weg te schrijven";
        byte[] data = Encoding.ASCII.GetBytes(text);
        UnityEngine.Windows.File.WriteAllBytes(RegisteryPath, data);
    }

    void ProcessQRCode(string code)
    {
        StartCoroutine(IotHub.GetDeviceModelForCode(
            code, 
            d =>
            {
                var deviceUI = _controlFactory.CreateControlFromDescriptor(d.SubSystems[0].UI);
                deviceUI.SetActive(true);

                _configuringDevice = d;
                _configuringControlManager = deviceUI.GetComponent<ControlManager>();
                _configuringControlManager.State = ControlManager.ConfigState.StartPlacement;

                _finishedScanning = true;
            }));
        
    }

    public void StartScan()
    {
        Debug.Log("Start QR-code scan");
        _qrCodeReader.enabled = true;
        _isScanning = true;
    }

    public void StartUsage()
    {
        Debug.Log("Start usage");
        if (_configuringControlManager != null)
        {
            _configuringControlManager.State = ControlManager.ConfigState.StopPlacement;
            RegisterDevice(_configuringDevice);
            _isScanning = false;
            _finishedScanning = false;
        }
    }

    private void _gestureRecognizer_Tapped(TappedEventArgs obj)
    {
        Debug.Log("_gesureRecognizer_Tapped");

        if (!_isScanning)
            StartScan();
        else if (_finishedScanning)
            StartUsage();
    }

    // Use this for initialization
    void Start () {
        _controlFactory = this.GetComponent<ControlFactory>();
        _qrCodeReader = this.GetComponent<QRCodeReader>();
        _qrCodeReader.CodeProcessor = this.ProcessQRCode;

        _gestureRecognizer = new GestureRecognizer();
        _gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        _gestureRecognizer.Tapped += _gestureRecognizer_Tapped;
        _gestureRecognizer.StartCapturingGestures();

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
