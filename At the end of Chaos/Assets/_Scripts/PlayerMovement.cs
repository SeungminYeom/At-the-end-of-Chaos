using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerMovement : MonoBehaviour
{
    [DoNotSerialize]
    public PhotonView pv;

    Rigidbody playerRigid;
    Vector3 playerRotation;

    [SerializeField] float moveSpeed = 5f;

    void Start()
    {
        pv = gameObject.GetComponent<PhotonView>();
        playerRigid = GetComponent<Rigidbody>();

        //System.Random rand1 = new System.Random(10);
        //System.Random rand2 = new System.Random(20);
        //System.Random rand3 = new System.Random(10);

        //Debug.Log(rand1.Next(10));
        //Debug.Log(rand2.Next());
        //Debug.Log(rand3.Next());
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
            
            transform.position = myPosObj.transform.Find("Player_"+ pv.CreatorActorNr + "_Pos").position;
            return;
        }

        //transform.position += new Vector3(x, 0, z).normalized * moveSpeed * Time.deltaTime;
        //playerRigid.velocity = new Vector3(CrossPlatformInputManager.GetAxisRaw("Horizontal"), 0, 
        //                                    CrossPlatformInputManager.GetAxisRaw("Vertical")).normalized * moveSpeed;

        if (CrossPlatformInputManager.GetButton("JoystickBtn") && pv.IsMine)
        {
            playerRigid.velocity = new Vector3(transform.forward.x * moveSpeed, playerRigid.velocity.y, (transform.forward.z * moveSpeed));
        }
        else
        {
            //playerRigid.velocity = Vector3.zero;
        }
    }
}
