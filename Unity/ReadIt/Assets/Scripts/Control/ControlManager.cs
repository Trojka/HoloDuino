using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity.InputModule.Utilities.Interactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlManager : MonoBehaviour {

    public enum ConfigState
    {
        None,
        PreparePlacement,
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

    public void PrepareForPlacement()
    {
        Debug.Log("ControlManager.PrepareForPlacement");
        State = ConfigState.PreparePlacement;
    }

    public void StopPlacement()
    {
        Debug.Log("ControlManager.PrepareForPlacement");
        State = ConfigState.StopPlacement;
    }

    // Use this for initialization
    void Start () {
        _placementComponent = this.GetComponent<TwoHandManipulatable>();
        _placementComponent.enabled = false;
        _interactivity = this.GetComponent<InteractiveToggle>();
        //_interactivity.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		switch(_state)
        {
            case ConfigState.PreparePlacement:
                _placementComponent.enabled = true;
                _interactivity.IsEnabled = false;

                State = ConfigState.StartPlacement;
                break;
            case ConfigState.StartPlacement:
                _interactivity.enabled = false;

                State = ConfigState.Placing;
                break;
            case ConfigState.StopPlacement:
                _placementComponent.enabled = false;
                _interactivity.enabled = true;
                _interactivity.IsEnabled = true;

                State = ConfigState.Using;
                break;
        }
    }
}
