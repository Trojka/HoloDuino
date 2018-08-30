using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentConfiguration : MonoBehaviour {

    public GameObject ToggleObject;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnToggleSelection()
    {
        if(ToggleObject != null)
            ToggleObject.transform.Translate(1, 0, 0);
    }

    public void OnToggleDeselection()
    {
        if (ToggleObject != null)
            ToggleObject.transform.Translate(-1, 0, 0);
    }
}
