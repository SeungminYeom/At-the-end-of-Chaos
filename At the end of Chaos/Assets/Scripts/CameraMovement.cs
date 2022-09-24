using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        //if (GameManager.instance.timeState != TimeState.afternoon)
        //{
        //    return;
        //}

        transform.position = Vector3.Lerp(transform.position, 
            new Vector3(player.transform.position.x + cameraPos.x, cameraPos.y, player.transform.position.z + cameraPos.z),
            Time.deltaTime * cameraSpeed);
    }
}
