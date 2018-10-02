using HoloToolkit.Examples.InteractiveElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlFactory : MonoBehaviour
{

    public GameObject toggleTemplate;
    public GameObject sliderTemplate;

    Dictionary<string, object> registeredControls = new Dictionary<string, object>();

    public void CreateControlWithCode(string code)
    {
        // get the descriptor and create the control
        // hardcoded for testing purposes:
        var toggleDescriptor = new ToggleDescriptor();
        toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.On, "1_on"));
        toggleDescriptor.Actions.Add(new UIAction<UIToggleEvent>(UIToggleEvent.On, "1_off"));

        CreateToggleButton(code, toggleDescriptor);
    }

    public void CreateToggleButton(string code, ToggleDescriptor descriptor)
    {
        var toggle = Instantiate(toggleTemplate);
        toggle.transform.position = new Vector3(0, 0, 2);

        var toggleComponent = toggle.GetComponent<InteractiveToggle>();

        var toggleHandler = new ToggleHandler();
        toggleHandler.Initialize(descriptor, toggleComponent);

        registeredControls.Add(code, toggleHandler);

        toggle.SetActive(true);
    }
}
