using HoloToolkit.Examples.InteractiveElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleHandler {
    string _deviceId;
    string onSelectionMethod;
    string onDeselectionMethod;

    public void Initialize(string deviceId, ToggleDescriptor descriptor, InteractiveToggle toggleComponent)
    {
        _deviceId = deviceId;
        toggleComponent.OnSelection.AddListener(this.OnSelectionHandler);
        toggleComponent.OnDeselection.AddListener(this.OnDeselectionHandler);

        foreach(var action in descriptor.Actions)
        {
            if (action.Event == UIToggleEvent.On)
                onSelectionMethod = action.Method;
            if (action.Event == UIToggleEvent.Off)
                onDeselectionMethod = action.Method;
        }
    }

    private void OnSelectionHandler()
    {
        IotHub.SendMethod(_deviceId, onSelectionMethod);
    }

    private void OnDeselectionHandler()
    {
        IotHub.SendMethod(_deviceId, onDeselectionMethod);
    }
}
