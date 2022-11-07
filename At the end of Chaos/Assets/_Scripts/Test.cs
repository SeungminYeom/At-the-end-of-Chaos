using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Test : MonoBehaviour
{
    Rigidbody playerRigid;

    [SerializeField] float moveSpeed = 5f;

    void Start()
    {
        playerRigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //Move();
        //Rotate();
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
    }

    private void Rotate()
    {
        if (CrossPlatformInputManager.GetButton("JoystickBtn"))
            transform.localRotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(CrossPlatformInputManager.GetAxisRaw("Horizontal"),
                                                                        CrossPlatformInputManager.GetAxisRaw("Vertical")) * Mathf.Rad2Deg, 0));
    }
}
