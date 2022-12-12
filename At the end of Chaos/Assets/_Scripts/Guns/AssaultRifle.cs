using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssaultRifle : MonoBehaviour
{
    private void OnEnable()
    {
        transform.parent.SendMessage("EquipGun", GunType.AssaultRifle);
    }
}
