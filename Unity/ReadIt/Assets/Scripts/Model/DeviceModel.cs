using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DeviceModel {
    public DeviceModel()
    {
        SubSystems = new List<SubSystem>();
    }
    public string DeviceId;
    public string SystemName;
    public List<SubSystem> SubSystems { get; private set; }
}
