using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : MonoBehaviour
{
    private void OnEnable()
    {
        if (!gameObject.activeSelf)
            transform.parent.SendMessage("EquipGun", GunType.Pistol);
    }
}
