using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR.WSA.Input;
using Newtonsoft.Json;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity;

public class TheApplication : MonoBehaviour {

    GestureRecognizer _gestureRecognizer;
    ControlFactory _controlFactory;
    QRCodeReader _qrCodeReader;

    Dictionary<string, DeviceInstance> instantiatedDevices = new Dictionary<string, DeviceInstance>();
    DeviceRegistry _registry;

    ControlManager _configuringControlManager;
    DeviceModel _configuringDevice;
    GameObject _configuringUI;

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
                foreach(var device in _registry.Devices)
                {
                    RestoreDevice(device);
                }
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

    void RestoreDevice(DeviceModel device)
    {
        var deviceUI = _controlFactory.CreateControlFromDescriptor(device.DeviceId, device.SubSystems[0].UI);
        deviceUI.SetActive(true);

        //string[] allAnchors = WorldAnchorManager.Instance.AnchorStore.GetAllIds();

        instantiatedDevices.Add(device.DeviceId, new DeviceInstance() { DeviceUI = device.SubSystems[0].UI, Instance = deviceUI });

        var controlManager = deviceUI.GetComponent<ControlManager>();
        controlManager.State = ControlManager.ConfigState.Using;
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
        var text = JsonConvert.SerializeObject(_registry, settings);
        byte[] data = Encoding.ASCII.GetBytes(text);
        UnityEngine.Windows.File.WriteAllBytes(RegisteryPath, data);
    }

    void ProcessQRCode(string code)
    {
        code = "My314";
        StartCoroutine(IotHub.GetDeviceModelForCode(
            code, 
            d =>
            {
                GameObject deviceUI = null;

                if (instantiatedDevices.ContainsKey(d.DeviceId))
                {
                    Debug.Log("Known device: prepare placement");
                    deviceUI = instantiatedDevices[d.DeviceId].Instance;
                    //WorldAnchorManager.Instance.RemoveAnchor(deviceUI);

                    var distanceInFront = GazeManager.Instance.GazeOrigin + GazeManager.Instance.GazeNormal * 2.0f;
                    deviceUI.transform.position = distanceInFront;
                }
                else
                {
                    Debug.Log("Unknown device: prepare placement");
                    deviceUI = _controlFactory.CreateControlFromDescriptor(d.DeviceId, d.SubSystems[0].UI);
                    deviceUI.SetActive(true);
                }

                _configuringUI = deviceUI;
                _configuringDevice = d;
                _configuringControlManager = deviceUI.GetComponent<ControlManager>();
                _configuringControlManager.State = ControlManager.ConfigState.PreparePlacement;

                _finishedScanning = true;
            }));
        
    }

    public void StartScan()
    {
        Debug.Log("Start QR-code scan");
        _qrCodeReader.StartScan();
        _isScanning = true;
    }

    public void StartUsage()
    {
        Debug.Log("Start usage");
        if (_configuringControlManager != null)
        {
            _configuringControlManager.State = ControlManager.ConfigState.StopPlacement;

            bool isAlreadyRegistered = false;
            foreach (var d in _registry.Devices)
            {
                if (d.DeviceId == _configuringDevice.DeviceId)
                    isAlreadyRegistered = true;
            }

            if (!isAlreadyRegistered)
                RegisterDevice(_configuringDevice);

            if(!instantiatedDevices.ContainsKey(_configuringDevice.DeviceId))
                instantiatedDevices.Add(_configuringDevice.DeviceId, new DeviceInstance() { DeviceUI = _configuringDevice.SubSystems[0].UI, Instance = _configuringUI });

            //WorldAnchorManager.Instance.AttachAnchor(_configuringUI, _configuringDevice.DeviceId);

            _isScanning = false;
            _finishedScanning = false;

            _configuringUI = null;
            _configuringDevice = null;
            _configuringControlManager = null;
        }
    }

    public void CancelConfiguration()
    {
        Debug.Log("Cancel configuration");

        Destroy(_configuringControlManager.gameObject);

        _configuringControlManager = null;
        _configuringDevice = null;
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

        //_gestureRecognizer = new GestureRecognizer();
        //_gestureRecognizer.SetRecognizableGestures(GestureSettings.Tap);
        //_gestureRecognizer.Tapped += _gestureRecognizer_Tapped;
        //_gestureRecognizer.StartCapturingGestures();

        FillRegisteredDevices();

        //foreach (var ctrl in instantiatedDevices)
        //{
        //    var deviceUI = _controlFactory.CreateControlFromDescriptor(ctrl.Value.DeviceUI);
        //    deviceUI.SetActive(true);
        //}
    }
	
	// Update is called once per frame
	void Update () {
// #if UNITY_EDITOR
//       if (Input.GetKeyDown(scanTriggerKey))
//        {
//            StartScan();
//        }
//#endif
    }
}
