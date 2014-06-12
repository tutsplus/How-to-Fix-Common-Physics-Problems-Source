﻿using UnityEngine;
using System.Collections;

public class CCMovement : MonoBehaviour {
    public float speed = 6f;
    public float gravity = 1.5f;
    public float jumpStrength = 0.5f;

    private CharacterController characterController;
    private Vector3 acceleration;

    private float accelerationModifier = 1f;

    private bool isGrounded = false;
    private bool IsGrounded {
        get {
            return isGrounded;
        }
        set {
            // Reset the y acceleration if the player just landed back onto the ground
            if(isGrounded != value && value == true) {
                acceleration = new Vector3(acceleration.x, 0f, acceleration.z);
            }
            isGrounded = value;
        }
    }
    private int layerMask = 1 >> 8;
    private bool isJumping;

    void Start() {
        characterController = GetComponent<CharacterController>();
    }

    void FixedUpdate() {
        RaycastHit hitInfo;

        /**
        * Check if the player is grounded by seeing how much distance the player and the ground. Here the 
        * value 0.58 is just based on how many units there are between the center of the player and plus 
        * some collision error margins (based on the Min Penatration for Panalty multiplied by two, for the
        * two collisions).
        */
        if(Physics.Raycast(new Ray(transform.position, Vector3.down), out hitInfo)) {
            if((transform.position - hitInfo.point).y <= 0.58f) {
                IsGrounded = true;
            } else {
                IsGrounded = false;
            }
        } else {
            IsGrounded = false;
        }
        Debug.Log(acceleration.y);

        if(Input.GetAxisRaw("Horizontal") != 0f) {
            accelerationModifier = Mathf.Min(accelerationModifier + Time.fixedDeltaTime * 2f, 2f);
        } else {
            accelerationModifier = 1f;
        }

        // Set the horizontal acceleration
        acceleration = new Vector3(speed * accelerationModifier * Time.fixedDeltaTime * Input.GetAxisRaw("Horizontal"),
            acceleration.y, acceleration.z);

        // Add vertical acceleration if the player has jumped
        if(isJumping && isGrounded) {
            acceleration = new Vector3(acceleration.x, jumpStrength, 0f);
            isJumping = false;
        }

        // Apply gravity only when not grounded
        if(IsGrounded == false) {
            acceleration -= new Vector3(0f, gravity * Time.fixedDeltaTime, 0f);
        }

        characterController.Move(acceleration);
    }

    void Update() {
        isJumping = Input.GetButtonDown("Jump");
    }

    void OnDrawGizmos() {
        Gizmos.DrawRay(new Ray(transform.position, Vector3.down));
    }

    void OnGUI() {
        GUILayout.Label(isGrounded.ToString());
    }
}