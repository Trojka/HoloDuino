using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemParser { 
    public SystemParser(JSONNode root)
    {
        System = new SubSystem();
        System.Id = root["id"].ToString();
        System.UI = new UIDescriptor();
        var uiTypeNode = root["ui"]["type"];
        var uiType = uiTypeNode.Value.ToString();
        switch (uiType)
        {
            case "toggle":
                var toggleButtonParser = new ToggleButtonParser(root["ui"]);
                System.UI = toggleButtonParser.Descriptor;
                break;
        }
    }

    public SubSystem System
    {
        get;
        private set;
    }
}
