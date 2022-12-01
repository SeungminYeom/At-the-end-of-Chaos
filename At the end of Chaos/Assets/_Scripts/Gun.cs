using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;
using TMPro;

public class Gun : MonoBehaviour
{
    [SerializeField] GunType _typeOnHand;
    [SerializeField] int rounds; //��ź��
    [SerializeField] public static float range;
    [SerializeField] float gunShootingTime = 0.2f;
    [SerializeField] LayerMask layerMask;

    float hitOffset = 0.5f;

    GameObject fireLight;

    Transform pistolFireTransform;
    Transform shotgunFireTransform;
    Transform assaultRifleFireTransform;
    Transform sniperRifleFireTransform;

    EnemyFinder enemyFinder;
    LineRenderer bulletLine;

    RaycastHit hit;
    GameObject testSp;
    Ray r;
    Transform gun;
    Vector3 gunPos;
    Vector3 hitOffsetPos;

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

        testSp = GameObject.Find("Target");
        gun = GameServerManager.instance.player.transform.Find("Pistol");
        gunPos = gun.GetChild(0).position;
    }

    void Update()
    {
        Transform gun = GameServerManager.instance.player.transform.Find("Pistol");
        Vector3 gunPos = gun.GetChild(0).position;
        Vector3 linePos = new Vector3(gunPos.x, 0f, gunPos.z); //좀비는 아래에 있으니깐 아래에서 쏜다.
        if (Physics.Raycast(linePos, transform.forward, out hit, range, layerMask))
        {
            hitOffsetPos = hit.collider.transform.position + Vector3.up * hitOffset;
            Debug.DrawRay(gunPos, hitOffsetPos - gunPos, Color.red);
            //타겟이 있으면 좀비를 타겟으로 하여 표시해준다.
            testSp.transform.position = hit.collider.transform.position;
            gun.LookAt(hitOffsetPos);
            //총도 대상을 바라본다.
        } else {
            r = new Ray(transform.position, transform.forward);
            Vector3 rangeInGround = new Vector3(r.GetPoint(range).x, 0, r.GetPoint(range).z);

            testSp.transform.position = rangeInGround;
            Debug.DrawRay(gunPos, rangeInGround - gunPos, new Color(1, 0.2f, 0.2f, 0.5f));

            gun.LookAt(rangeInGround);
        }
        
        if (GameManager.instance.timeState == TimeState.night && rounds > 0)
        {
            if (CrossPlatformInputManager.GetButtonDown("Shoot") && gameObject.GetComponent<PlayerMovement>().pv.IsMine)
            {
                //PlayerMovement�� View�� ���ؼ� Shoot���� ����
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

    //���� Ȱ��ȭ�� �� �� �Լ��� ȣ����
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
            enemyFinder.target.gameObject.GetComponent<Zombie>().pv.RPC("AttackFromPlayer", RpcTarget.All, damage, 0f, knockBack);
        }
        yield return new WaitForSeconds(gunShootingTime);
        bulletLine.enabled = false;
        fireLight.SetActive(false);
    }
}
