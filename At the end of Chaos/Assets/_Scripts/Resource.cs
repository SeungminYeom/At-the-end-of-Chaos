using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public Transform resource_piece_target;
    GameObject resource_piece;

    public int wood = 0;
    public int iron = 0;

    private void OnEnable()
    {
        if (tag == "WoodResource")
            wood = new System.Random().Next(3) + 2;
        else
            iron = new System.Random().Next(3) + 2;
    }

    private void OnDestroy()
    {
        if (resource_piece_target != null)
        {
            for (int i = 0; i < (wood + iron); i++)
            {
                resource_piece = transform.Find("Resources").GetChild(0).gameObject;
                resource_piece.gameObject.SetActive(true);
                resource_piece.GetComponent<ResourceMagnet>().flyingTarget = resource_piece_target;
                resource_piece.transform.parent = null;
                resource_piece.GetComponent<Rigidbody>().AddForce((resource_piece.transform.position - transform.position).normalized * 3, ForceMode.Impulse);
            }
        }
    }
}
