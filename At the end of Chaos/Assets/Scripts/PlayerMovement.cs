using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    }

    private void Move()
    {
        if (GameManager.instance.timeState != TimeState.afternoon)
        {
            transform.position = new Vector3(0, 2.5f, 0);
            return;
        }

        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        //transform.position += new Vector3(x, 0, z).normalized * moveSpeed * Time.deltaTime;
        playerRigid.velocity = new Vector3(x, 0, z).normalized * moveSpeed;
    }
}
