using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playerRigid;

    [SerializeField] float moveSpeed = 5f;

    void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        Rotate();
    }

    private void Move()
    {
        if (GameManager.instance.timeState != TimeState.afternoon)
        {
            transform.position = new Vector3(0, 2.5f, 0);
            return;
        }

        //transform.position += new Vector3(x, 0, z).normalized * moveSpeed * Time.deltaTime;
        //playerRigid.velocity = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0, 
        //                                    CrossPlatformInputManager.GetAxisRaw("Vertical")).normalized * moveSpeed;

        if (CrossPlatformInputManager.GetButton("JoystickBtn"))
        {
            playerRigid.velocity = transform.forward * moveSpeed;
        }
        else
            playerRigid.velocity = Vector3.zero;
    }

    private void Rotate()
    {
        if (CrossPlatformInputManager.GetButton("JoystickBtn"))
            transform.localRotation = Quaternion.Euler(new Vector3(0,  Mathf.Atan2(CrossPlatformInputManager.GetAxisRaw("Horizontal"),
                                                                        CrossPlatformInputManager.GetAxisRaw("Vertical")) * Mathf.Rad2Deg, 0));
    }
}
