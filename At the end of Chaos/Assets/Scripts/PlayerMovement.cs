using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    PhotonView pv;

    Rigidbody playerRigid;
    Vector3 playerRotation;

    [SerializeField] float moveSpeed = 5f;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();
        playerRigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        Move();
        if (CrossPlatformInputManager.GetButton("JoystickBtn") && pv.IsMine)
        {
            gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(CrossPlatformInputManager.GetAxisRaw("Horizontal"),
                             CrossPlatformInputManager.GetAxisRaw("Vertical")) * Mathf.Rad2Deg, 0));
        }
            


    }

    private void Move()
    {
        if (GameManager.instance.timeState != TimeState.afternoon)
        {
            //transform.position = new Vector3(0, 2.0f, 0);
            TrainManager trainManager = GameObject.Find("TrainManager").GetComponent<TrainManager>();
            GameObject myPosObj = trainManager.GetTrain(GameManager.instance.trainCount);
            transform.position = myPosObj.transform.Find(gameObject.tag + "_Pos").position;
            return;
        }

        //transform.position += new Vector3(x, 0, z).normalized * moveSpeed * Time.deltaTime;
        //playerRigid.velocity = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0, 
        //                                    CrossPlatformInputManager.GetAxisRaw("Vertical")).normalized * moveSpeed;

        if (CrossPlatformInputManager.GetButton("JoystickBtn") && pv.IsMine)
        {
            playerRigid.velocity = transform.forward * moveSpeed;
        }
        else
            playerRigid.velocity = Vector3.zero;
    }
}
