using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//[RequireComponent(typeof(CharacterController))]
public class GraceJumpPlayerScript : MonoBehaviour {

    
    [SerializeField] private float _acceleration;
    [SerializeField] private float _drag;
    [SerializeField] private float _maximumXZVelocity = (30 * 1000) / (60 * 60); //[m/s] 30km/h
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _graceTime;

    private Vector3 _velocity = Vector3.zero; // [m/s]
    private Vector3 _inputMovement;
    private bool _jump;
    private float _graceTimer;

    private Transform _absoluteTransform;
    private CharacterController _char;
    private MeshRenderer _rend;
    private bool _canGrace;

    void Start ()
        {
        //attach components
        _char = GetComponent<CharacterController>();
        _rend = transform.GetChild(0).GetComponent<MeshRenderer>();
        _absoluteTransform = Camera.main.transform;

        //set grace timer
        _graceTimer = _graceTime;

        //dependency error
#if DEBUG
        Assert.IsNotNull(_char, "DEPENDENCY ERROR: CharacterController missing from PlayerScript");
        #endif

        }

    private void Update()
        {
        _inputMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));  //.normalized;
        if (Input.GetButtonDown("Jump"))
            {
            _jump = true;
            }

        DoRainbow();
        }

    void FixedUpdate ()
        {
        ApplyGround();
        ApplyGravity();
        ApplyMovement();
        ApplyDragOnGround();
        SetGraceTime();
        ApplyJump();
        LimitXZVelocity();

        DoMovement();
        }

    private void ApplyGround()
        {
        if (_char.isGrounded)
            {
            //ground velocity
            _velocity -= Vector3.Project(_velocity, Physics.gravity);
            }
        }

    private void ApplyGravity()
        {
        if (!_char.isGrounded)
            {
            //apply gravity
            _velocity += Physics.gravity * Time.deltaTime;
            }
        }

    private void ApplyMovement()
        {
        if (_char.isGrounded)
            {
            //get relative rotation from camera
            Vector3 xzForward = Vector3.Scale(_absoluteTransform.forward, new Vector3(1, 0, 1));
            Quaternion relativeRot = Quaternion.LookRotation(xzForward);

            //move in relative direction
            Vector3 relativeMov = relativeRot * _inputMovement;
            _velocity += relativeMov * _acceleration * Time.deltaTime;
            }

        }

    private void LimitXZVelocity()
        {
        Vector3 yVel = Vector3.Scale(_velocity, Vector3.up);
        Vector3 xzVel = Vector3.Scale(_velocity, new Vector3(1, 0, 1));

        xzVel = Vector3.ClampMagnitude(xzVel, _maximumXZVelocity);

        _velocity = xzVel + yVel;
        }

    private void ApplyDragOnGround()
        {
        if (_char.isGrounded)
            {
            //drag
            _velocity = _velocity * (1 - _drag * Time.deltaTime); //same as lerp
            }
        }

    private void SetGraceTime()
        {
        if (_char.isGrounded && !_jump)
            {
            _graceTimer = _graceTime;
            }
        else
            {
            _graceTimer -= Time.deltaTime;
            }
        }

    private void ApplyJump()
        {
        if ((_char.isGrounded || _graceTimer > 0) && _jump)
            {
            _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            _jump = false;
            _graceTimer = 0;
            }
        }

    private void DoMovement()
        {
        //do velocity / movement on character controller
        Vector3 movement = _velocity * Time.deltaTime;
        _char.Move(movement);
        }

    private void DoRainbow()
        {
        float h;
        float s;
        float v;
        Color.RGBToHSV(_rend.material.color, out h, out s, out v);
        h += Time.deltaTime;
        if (h > 1)
            {
            h = 0;
            }
        _rend.material.color = Color.HSVToRGB(h, 0.8f, 1);
        }
    }
