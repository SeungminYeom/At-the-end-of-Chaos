using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Gun : MonoBehaviour
{
    [SerializeField] GunType _typeOnHand;
    [SerializeField] int rounds; //ÀÜÅº·®

    GunType typeOnHand
    {
        get { return _typeOnHand; }
        set
        {
            //if (GunManager.instance.IsGunUseable((int)value))
            //{
                _typeOnHand = value;
            //}
        }
    }

    void Start()
    {
        typeOnHand = GunType.pistol;
        rounds = 10;
    }

    void Update()
    {
        if (GameManager.instance.timeState == TimeState.night && rounds > 0)
        {
            if (CrossPlatformInputManager.GetButtonDown("Shoot"))
                Shoot();
        }
    }

    public void Shoot()
    {
        switch (typeOnHand)
        {
            case GunType.pistol:
                Debug.Log("pistol " + GunManager.instance.gunDamage.ToString());
                break;
            case GunType.shotgun:
                Debug.Log("shotgun " + GunManager.instance.gunDamage.ToString());
                break;
            case GunType.sniperRifle:
                Debug.Log("sniperRifle " + (GunManager.instance.gunDamage * 2).ToString());
                break;
            case GunType.assaultRifle:
                Debug.Log("assaultRifle " + (GunManager.instance.gunDamage * 0.5f).ToString());
                break;
        }

        if (--rounds <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    public void EquipGun(GunType t)
    {
        typeOnHand = t;
        rounds = GunManager.instance.GetGunRounds((int)typeOnHand);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(1f);
        rounds = GunManager.instance.GetGunRounds((int)typeOnHand);
    }
}
