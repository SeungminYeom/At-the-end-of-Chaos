using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shotgun : MonoBehaviour
{
    private void OnEnable()
    {
        transform.parent.SendMessage("EquipGun", GunType.Shotgun);
    }
}
