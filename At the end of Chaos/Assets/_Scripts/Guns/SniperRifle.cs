using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperRifle : MonoBehaviour
{
    private void OnEnable()
    {
        transform.parent.SendMessage("EquipGun", GunType.SniperRifle);
    }
}
