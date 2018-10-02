using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    NotSupported,
    Toggle
}

public enum UIToggleEvent
{
    On,
    Off
}

public class UIAction<EventType>
{
    public UIAction(EventType eventType, string method)
    {
        Event = eventType;
        Method = method;
    }

    public EventType Event { get; private set; }
    public string Method { get; private set; }
}

public class UIDescriptor
{
    public UIDescriptor() { UIType = UIType.NotSupported; }
    public UIType UIType { get; protected set; }
}

public class ToggleDescriptor : UIDescriptor
{
    public ToggleDescriptor()
    {
        UIType = UIType.Toggle;
        Actions = new List<UIAction<UIToggleEvent>>();
    }
    public List<UIAction<UIToggleEvent>> Actions { get; private set; }
}

public class SubSystem
{
    public string Id { get; set; }
    public UIDescriptor UI { get; set; }
}
