using HoloToolkit.Examples.InteractiveElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFactory : MonoBehaviour
{

    public GameObject toggleTemplate;
    public GameObject sliderTemplate;

    //public GameObject CreateControlFromCode(string code)
    //{
    //    // get the descriptor and create the control
    //    // hardcoded for testing purposes:
    //    var toggleDescriptor = new ToggleDescriptor();
    //    toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.On, "1_on"));
    //    toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.On, "1_off"));

    //    return CreateControlFromDescriptor(toggleDescriptor);
    //}

    public GameObject CreateControlFromDescriptor(string deviceId, UIDescriptor descriptor)
    {
        switch(descriptor.UIType)
        {
            case UIType.Toggle:
                return CreateToggleButton(deviceId, descriptor as ToggleDescriptor);
            default:
                throw new System.Exception("Not supported UIType");
        }
    }

    public GameObject CreateToggleButton(string deviceId, ToggleDescriptor descriptor)
    {
        var toggle = Instantiate(toggleTemplate);
        toggle.transform.position = new Vector3(0, 0, 2);

        var toggleComponent = toggle.GetComponent<InteractiveToggle>();

        var toggleHandler = new ToggleHandler();
        toggleHandler.Initialize(deviceId, descriptor, toggleComponent);

        return toggle;
    }
}
