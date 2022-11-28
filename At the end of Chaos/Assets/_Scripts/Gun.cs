using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] GunType _typeOnHand;
    [SerializeField] int rounds; //잔탄량
    [SerializeField] public static float range;
    [SerializeField] float gunShootingTime = 0.2f;

    GameObject fireLight;

    Transform pistolFireTransform;
    Transform shotgunFireTransform;
    Transform assaultRifleFireTransform;
    Transform sniperRifleFireTransform;

    EnemyFinder enemyFinder;
    LineRenderer bulletLine;

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
        fireLight = transform.Find("FireLight").gameObject;
        pistolFireTransform = transform.Find("Pistol").GetChild(0);
        shotgunFireTransform = transform.Find("Shotgun").GetChild(0);
        assaultRifleFireTransform = transform.Find("AssaultRifle").GetChild(0);
        sniperRifleFireTransform = transform.Find("SniperRifle").GetChild(0);
        enemyFinder = GetComponent<EnemyFinder>();
        bulletLine = GetComponent<LineRenderer>();
        typeOnHand = GunType.pistol;
        range = GunManager.instance.GetGunRange((int)typeOnHand);
        StartCoroutine(Reload());
    }

    void Update()
    {
        if (GameManager.instance.timeState == TimeState.night && rounds > 0)
        {
            if (CrossPlatformInputManager.GetButton("Shoot") && gameObject.GetComponent<PlayerMovement>().pv.IsMine)
            {
                //PlayerMovement의 View를 통해서 Shoot명령 전달
                gameObject.GetComponent<PlayerMovement>().pv.RPC("Shoot", Photon.Pun.RpcTarget.All);
                //Shoot();
            }
            //if (Input.GetKeyDown(KeyCode.Space))
                
        }
    }

    [PunRPC]
    public void Shoot()
    {
        switch (typeOnHand)
        {
            case GunType.pistol:
                //Debug.Log("pistol " + GunManager.instance.gunDamage.ToString());
                if (enemyFinder.target != null)
                {
                    StartCoroutine(FireVFX(pistolFireTransform.position, GunManager.instance.gunDamage, true));
                }
                else
                {
                    StartCoroutine(FireVFX(pistolFireTransform.position, 0, false));
                }
                break;
            case GunType.shotgun:
                //Debug.Log("shotgun " + GunManager.instance.gunDamage.ToString());
                if (enemyFinder.target != null)
                {
                    StartCoroutine(FireVFX(shotgunFireTransform.position, GunManager.instance.gunDamage, true));
                }
                else
                {
                    StartCoroutine(FireVFX(shotgunFireTransform.position, 0, false));
                }
                break;
            case GunType.sniperRifle:
                //Debug.Log("sniperRifle " + (GunManager.instance.gunDamage * 2).ToString());
                if (enemyFinder.target != null)
                {
                    StartCoroutine(FireVFX(sniperRifleFireTransform.position, GunManager.instance.gunDamage * 3, true));
                }
                else
                {
                    StartCoroutine(FireVFX(sniperRifleFireTransform.position, 0, false));
                }
                break;
            case GunType.assaultRifle:
                //Debug.Log("assaultRifle " + (GunManager.instance.gunDamage * 0.5f).ToString());
                if (enemyFinder.target != null)
                {
                    StartCoroutine(FireVFX(assaultRifleFireTransform.position, GunManager.instance.gunDamage, true));
                }
                else
                {
                    StartCoroutine(FireVFX(assaultRifleFireTransform.position, 0, false));
                }
                break;
        }

        if (--rounds <= 0)
        {
            StartCoroutine(Reload());
        }
    }

    //총이 활성화될 때 이 함수를 호출함
    public void EquipGun(GunType t)
    {
        typeOnHand = t;
        rounds = GunManager.instance.GetGunRounds((int)typeOnHand);
        range = GunManager.instance.GetGunRange((int)typeOnHand);
    }

    IEnumerator Reload()
    {
        yield return new WaitForSeconds(GunManager.instance.gunReloadTime);
        rounds = GunManager.instance.GetGunRounds((int)typeOnHand);
    }

    IEnumerator FireVFX(Vector3 firePos, float damage, bool isEnemy)
    {
        fireLight.SetActive(true);
        fireLight.transform.position = firePos;
        bulletLine.enabled = true;
        bulletLine.SetPosition(0, firePos);
        //bulletLine.SetPosition(1, transform.forward * 10f);
        bulletLine.SetPosition(1, transform.position + transform.forward * 10f);
        if (isEnemy)
        {
            bulletLine.SetPosition(1, enemyFinder.target.position);
            Vector3 knockBack = enemyFinder.target.position - transform.position;
            knockBack.y = 0;
            enemyFinder.target.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            enemyFinder.target.gameObject.GetComponent<Rigidbody>().AddForce(knockBack.normalized * 10f, ForceMode.Impulse);
            enemyFinder.target.gameObject.GetComponent<Zombie>().SendMessage("AttackFromPlayer", new float[]{ damage, 0 });
        }
        yield return new WaitForSeconds(gunShootingTime);
        bulletLine.enabled = false;
        fireLight.SetActive(false);
    }
}
