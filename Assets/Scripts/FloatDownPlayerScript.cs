using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//[RequireComponent(typeof(CharacterController))]
public class FloatDownPlayerScript : MonoBehaviour {

    
    [SerializeField] private float _acceleration;
    [SerializeField] private float _drag;
    [SerializeField] private float _maximumXZVelocity = (30 * 1000) / (60 * 60); //[m/s] 30km/h
    [SerializeField] private float _jumpHeight;
    [SerializeField] private float _floatSpeed;

    private Vector3 _velocity = Vector3.zero; // [m/s]
    private Vector3 _inputMovement;
    private bool _jump;
    private float _nonGroundedTimer = 0;
    private bool _isJumping;

    private Transform _absoluteTransform;
    private CharacterController _char;
    private MeshRenderer _rend;

    void Start ()
        {
        //attach components
        _char = GetComponent<CharacterController>();
        _rend = transform.GetChild(0).GetComponent<MeshRenderer>();
        _absoluteTransform = Camera.main.transform;

        //dependency error
        #if DEBUG
        Assert.IsNotNull(_char, "DEPENDENCY ERROR: CharacterController missing from PlayerScript");
        #endif

        }

    private void Update()
        {
        _inputMovement = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));  //.normalized;
        if (Input.GetButtonDown("Jump") && _nonGroundedTimer < 3)
            {
            _jump = true;
            }

        DoRainbow();
        }

    void FixedUpdate ()
        {
        CalculateStableGrounded();
        ApplyGround();
        ApplyGravity();
        ApplyMovement();
        ApplyDragOnGround();
        ApplyJump();
        LimitXZVelocity();

        DoMovement();
        }

    private void CalculateStableGrounded()
        {
        //make sure grounded is correct
        if (!_char.isGrounded)
            {
            _nonGroundedTimer++;
            }
        else
            {
            _nonGroundedTimer = 0;
            _isJumping = false;
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

    private void ApplyGravity()
        {
        if (!_char.isGrounded)
            {
            if (_velocity.y < 0 && _isJumping)
                {
                //apply low gravity
                _velocity.y = -_floatSpeed;
                }
            else
                {
                //apply gravity
                _velocity += Physics.gravity * Time.deltaTime;
                }
            }
        }

    private void ApplyMovement()
        {
        if (_char.isGrounded || (_isJumping && _velocity.y < 0))
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
        if (_char.isGrounded || (_isJumping && _velocity.y < 0))
            {
            float saveY = _velocity.y;
            //drag
            _velocity = _velocity * (1 - _drag * Time.deltaTime); //same as lerp
            _velocity.y = saveY;
            }
        }

    private void ApplyJump()
        {
        if (_nonGroundedTimer < 3 && _jump)
            {
            _velocity.y += Mathf.Sqrt(2 * Physics.gravity.magnitude * _jumpHeight);
            _jump = false;
            _isJumping = true;
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
