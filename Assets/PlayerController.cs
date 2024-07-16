using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MyLittleStore
{
    public class PlayerController : MonoBehaviour
    {
        public float MovementSpeed = 5;
        public float RotationSpeed = 15;

        private Rigidbody rb;
        private Quaternion GoalRotation = Quaternion.identity;

        private void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        private void Update()
        {
            rb.velocity = MovementSpeed * (transform.right * Input.GetAxis("Horizontal") + transform.up * Input.GetAxis("Height") + transform.forward * Input.GetAxis("Vertical"));

            if (Input.GetMouseButton(1))
            {
                GoalRotation.y += Input.GetAxis("Mouse X") * Time.deltaTime * RotationSpeed;
                transform.rotation = GoalRotation;
            }
        }
    }
}