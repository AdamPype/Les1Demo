using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour {

    private CharacterController _char;

	void Start () {
        _char = GetComponent<CharacterController>();

#if DEBUG

        Assert.IsNotNull(_char, "DEPENDENCY ERROR: CharacterController missing from PlayerScript");

#endif

        }

    void Update () {
	    
	}
}
