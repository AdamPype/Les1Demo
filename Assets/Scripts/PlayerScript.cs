using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour {

    private CharacterController _char;
    private Vector3 _velocity = Vector3.zero; // [m/s]


	void Start () {
        _char = GetComponent<CharacterController>();

#if DEBUG

        Assert.IsNotNull(_char, "DEPENDENCY ERROR: CharacterController missing from PlayerScript");

#endif

        }

    void Update () {

        //apply gravity
        if (!_char.isGrounded)
            {
            _velocity += Physics.gravity * Time.deltaTime;
            }
        else
            {

            }
        

        //do velocity / movement
        Vector3 movement = _velocity * Time.deltaTime;
        _char.Move(movement);

	}
}
