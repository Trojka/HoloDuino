using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour {

    public enum ConfigState
    {
        None,
        StartPlacement,
        Placing,
        StopPlacement,
        Using
    }

    ConfigState _state = ConfigState.None;
    TwoHandManipulatable _placementComponent;
    InteractiveToggle _interactivity;

    public ConfigState State
    {
        get { return _state; }
        set { _state = value; }
    }

    // Use this for initialization
    void Start () {
        _placementComponent = this.GetComponent<TwoHandManipulatable>();
        _placementComponent.enabled = false;
        _interactivity = this.GetComponent<InteractiveToggle>();
        _interactivity.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		switch(_state)
        {
            case ConfigState.StartPlacement:
                _placementComponent.enabled = true;

                State = ConfigState.Placing;
                break;
            case ConfigState.StopPlacement:
                _placementComponent.enabled = false;

                State = ConfigState.Placing;
                break;
        }
    }
}
