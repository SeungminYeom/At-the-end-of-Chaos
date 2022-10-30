using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraMovement : MonoBehaviour
{
    public GameObject player;
    Vector3 cameraPos;
    [SerializeField] float cameraSpeed = 1f;
    
    void Start()
    {
        cameraPos = transform.position;
    }

    void Update()
    {
        //if (GameManager.instance.timeState != TimeState.afternoon)
        //{
        //    return;
        //}

        //transform.position = Vector3.Lerp(cameraPos, player.transform.position, Time.deltaTime * 5f);
    }

    private void LateUpdate()
    {
        if (GameManager.instance.timeState == TimeState.afternoon)
        {
            transform.position = Vector3.Lerp(transform.position,
                new Vector3(player.transform.position.x + cameraPos.x, cameraPos.y, player.transform.position.z + cameraPos.z),
                Time.deltaTime * cameraSpeed);
            return;
        }
        if (GameManager.instance.timeState == TimeState.night && CrossPlatformInputManager.GetButton("JoystickBtn"))
        {
            //Vector3 playerFrontView = (player.transform.forward - player.transform.position).normalized * Gun.range / 2;
            Vector3 playerFrontView = new Vector3(player.transform.forward.x * Gun.range + cameraPos.x,
                                                    cameraPos.y,
                                                    player.transform.forward.z * Gun.range + cameraPos.z);
            //playerFrontView.y = cameraPos.y;
            transform.position = Vector3.Lerp(transform.position, playerFrontView, Time.deltaTime * cameraSpeed * 8f);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, cameraPos, Time.deltaTime * cameraSpeed);
        }
    }
}
