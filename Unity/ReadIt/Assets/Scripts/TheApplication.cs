using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Windows.Speech;
using UnityEngine.XR.WSA.Input;

public class TheApplication : MonoBehaviour {

    KeyCode scanTriggerKey = KeyCode.N;

    Dictionary<string, ToggleDescriptor> registeredControls = new Dictionary<string, ToggleDescriptor>();

    ControlFactory _controlFactory;
    GestureRecognizer _gestureRecognizer;
    QRCodeReader _qrCodeReader;

    ControlManager _configuringControlManager;

    void FillRegisteredDevices()
    {
        //var toggleDescriptor = new ToggleDescriptor();
        //toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.On, "1_on"));
        //toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.Off, "1_off"));

        //registeredControls.Add("reg1", toggleDescriptor);

        string path = string.Format("{0}/readit/{1}.txt", Application.persistentDataPath, "deviceregistry");
        if (UnityEngine.Windows.File.Exists(path))
        {
            byte[] data = UnityEngine.Windows.File.ReadAllBytes(path);
            string text = Encoding.ASCII.GetString(data);


            Debug.Log("Registered devices: " + text);
        }

    }

    void RegisterDevice()
    {
        var text = "de tekst om weg te schrijven";
        string folder = string.Format("{0}/readit", Application.persistentDataPath);
        string path = string.Format("{0}/readit/{1}.txt", Application.persistentDataPath, "deviceregistry");

        byte[] data = Encoding.ASCII.GetBytes(text);
        UnityEngine.Windows.Directory.CreateDirectory(folder);
        UnityEngine.Windows.File.WriteAllBytes(path, data);

        //UnityEngine.Windows.File.Exists();
    }

    //public MyObject ReadData(string filename)
    //{
    //    string path = string.Format("{0}/mydata/{1}.json", Application.persistentDataPath, filename);

    //    byte[] data = UnityEngine.Windows.File.ReadAllBytes(path);
    //    string json = Encoding.ASCII.GetString(data);

    //    MyObject obj = JsonConvert.DeserializeObject(json);

    //    return obj;
    //}

    //public void SaveData(string filename, MyObject obj)
    //{
    //    string path = string.Format("{0}/mydata/{1}.json", Application.persistentDataPath, filename);

    //    string json = JsonConvert.SerializeObject(obj);
    //    byte[] data = Encoding.ASCII.GetBytes(json);

    //    UnityEngine.Windows.File.WriteAllBytes(path, data);
    //}

    void ProcessQRCode(string code)
    {
        StartCoroutine(IotHub.GetDeviceModelForCode(
            code, 
            d =>
            {
                var deviceUI = _controlFactory.CreateControlFromDescriptor(d.SubSystems[0].UI);
                deviceUI.SetActive(true);
                _configuringControlManager = deviceUI.GetComponent<ControlManager>();
                _configuringControlManager.State = ControlManager.ConfigState.StartPlacement;
            }));
        
    }

    public void StartScan()
    {
        Debug.Log("Start QR-code scan");
        _qrCodeReader.enabled = true;
    }

    public void StartUsage()
    {
        Debug.Log("Start usage");
        if (_configuringControlManager != null)
        {
            _configuringControlManager.State = ControlManager.ConfigState.StopPlacement;
            RegisterDevice();
        }
    }

    private void _gestureRecognizer_Tapped(TappedEventArgs obj)
    {
        Debug.Log("_gesureRecognizer_Tapped");

        //StartScan();
        RegisterDevice();
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
