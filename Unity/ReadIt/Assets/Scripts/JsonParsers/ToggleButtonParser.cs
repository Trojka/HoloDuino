using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleButtonParser {
    public ToggleButtonParser(JSONNode root)
    {
        Descriptor = new ToggleDescriptor();

        var numberOfUIActions = root["actions"]["count"].AsInt;
        for (int i = 1; i <= numberOfUIActions; i++)
        {
            var actionTag = string.Format("action{0}", i);

            var actionNode = root["actions"][actionTag];

            var actionEvent = actionNode["event"].Value.ToString();
            var actionMethod = actionNode["method"].Value.ToString();

            var eventType = UIToggleEvent.On;
            switch ( actionEvent)
            {
                case "toggleon":
                    eventType = UIToggleEvent.On;
                    break;
                case "toggleoff":
                    eventType = UIToggleEvent.Off;
                    break;
            }

            var actionDefinition = new UIAction<UIToggleEvent>(eventType, actionMethod);
            Descriptor.Actions.Add(actionDefinition);
        }
    }

    public ToggleDescriptor Descriptor
    {
        get;
        private set;
    }
}
