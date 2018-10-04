using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class DeviceParser {

    public DeviceParser(string json)
    {
        var root = JSON.Parse(json);
        Initialize(root.AsArray[0]);
    }

    public DeviceParser(JSONNode root)
    {
        Initialize(root);
    }

    private void InitializeFromTag(JSONNode root)
    {
        Initialize(root["tags"]);
    }

    private void Initialize(JSONNode root)
    {
        Device = new DeviceModel();

        var systemNameNode = root["systems"]["name"];
        var systemName = systemNameNode.Value.ToString();
        Device.SystemName = systemName;

        var sysCount = root["systems"]["count"].AsInt;
        for (int i = 1; i <= sysCount; i++)
        {
            var sysTag = string.Format("sys{0}", i);
            var systemParser = new SystemParser(root["systems"][sysTag]);
        }

    }

    public DeviceModel Device
    {
        get;
        private set;
    }
}
