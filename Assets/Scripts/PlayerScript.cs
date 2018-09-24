using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//[RequireComponent(typeof(CharacterController))]
public class PlayerScript : MonoBehaviour {

    private Transform _absoluteTransform;

    private CharacterController _char;
    private Vector3 _velocity = Vector3.zero; // [m/s]
    private MeshRenderer _rend;
    private float _speed;
    Vector3 _inputMovement;

    void Start ()
        {
        //attach components
        _char = GetComponent<CharacterController>();
        _rend = transform.GetChild(0).GetComponent<MeshRenderer>();

        //dependency error
        #if DEBUG
        Assert.IsNotNull(_char, "DEPENDENCY ERROR: CharacterController missing from PlayerScript");
        #endif

        }

    void FixedUpdate ()
        {
        ApplyGround();
        ApplyGravity();
        ApplyMovement();

        DoMovement();
        }


    private void ApplyMovement()
        {
        //apply movement input
        _inputMovement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Horizontal"));
        _inputMovement *= _speed * Time.deltaTime;
        
        _velocity += _inputMovement;
        }

    private void DoMovement()
        {
        //do velocity / movement on character controller
        Vector3 movement = _velocity * Time.deltaTime;
        _char.Move(movement);
        }

    private void ApplyGravity()
        {
        if (!_char.isGrounded)
            {
            //apply gravity
            _velocity += Physics.gravity * Time.deltaTime;
            }
        }

    private void ApplyGround()
        {
        if (_char.isGrounded)
            {
            //ground velocity
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
            }
        }
    }
