using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Photon.Pun;
using TMPro;
using UnityEngine.Rendering.UI;
using Unity.VisualScripting;

public class Gun : MonoBehaviour
{
    [SerializeField] GunType _typeOnHand;
    [SerializeField] int rounds; //��ź��
    [SerializeField] public static float range;
    [SerializeField] float gunShootingTime = 0.2f;
    [SerializeField] LayerMask layerMask;

    PhotonView pv;
    float hitOffset = 0.5f;

    GameObject fireLight;

    Transform pistolFireTransform;
    Transform shotgunFireTransform;
    Transform assaultRifleFireTransform;
    Transform sniperRifleFireTransform;

    EnemyFinder enemyFinder;
    LineRenderer bulletLine;

    bool targeting = false;
    RaycastHit hit;
    Ray r = new Ray();
    Transform gun;
    Vector3 gunPos;
    Vector3 hitOffsetPos;

    public LineRenderer targetingLazer;
    public GameObject testSp;

    [SerializeField] Gradient r2b;
    [SerializeField] Gradient dr2b;


    GunType typeOnHand;

    void Start()
    {
        pv = GetComponent<PhotonView>();

        fireLight = transform.Find("FireLight").gameObject;
        pistolFireTransform = transform.Find("Pistol").GetChild(0);
        shotgunFireTransform = transform.Find("Shotgun").GetChild(0);
        assaultRifleFireTransform = transform.Find("AssaultRifle").GetChild(0);
        sniperRifleFireTransform = transform.Find("SniperRifle").GetChild(0);
        enemyFinder = GetComponent<EnemyFinder>();
        bulletLine = GetComponent<LineRenderer>();
        typeOnHand = GunType.pistol;
        range = GunManager.instance.GetGunRange((int)typeOnHand);
        
        testSp = GameObject.Find("Target");

        targetingLazer = GetComponent<LineRenderer>();

        StartCoroutine(Reload());
    }

    void Update()
    {
        if (pv.IsMine && rounds > 0)
        {
            gun = gameObject.transform.Find("Pistol");
            gunPos = gun.GetChild(0).position;
            Vector3 linePos = gunPos;
            linePos.y = 0; //좀비는 아래에 있으니깐 아래에서 판정선을 쏜다.
            targetingLazer.SetPosition(0, gunPos);
            r.origin = transform.position;
            if (
                GameManager.instance.timeState == TimeState.night &&
                rounds > 0 &&
                Physics.Raycast(linePos, transform.forward, out hit, range, layerMask)
                )
            {
                r.direction = hit.point;
                hitOffsetPos = hit.collider.transform.position + Vector3.up * hitOffset;
                targetingLazer.colorGradient = r2b;
                targetingLazer.SetPosition(1, hitOffsetPos);
                //Debug.DrawRay(gunPos, hitOffsetPos - gunPos, Color.red);
                //타겟이 있으면 좀비를 타겟으로 하여 표시해준다.
                testSp.transform.position = hitOffsetPos;
                gun.LookAt(hitOffsetPos);
                //총도 대상을 바라본다.

                if (CrossPlatformInputManager.GetButtonDown("Shoot"))
                {
                    BulletTrailManager.instance.pv.RPC("PlayEffect", RpcTarget.All, gunPos, hitOffsetPos);
                    VFXPlayer.instance.pv.RPC("PlayVFX", RpcTarget.All, ((int)VFXPlayer.vfx.gunSpark), gunPos, Quaternion.Euler(r.direction));
                    pv.RPC("Shoot", Photon.Pun.RpcTarget.All, hit.collider.gameObject.GetPhotonView().ViewID);
                }
            }
            else
            {
                r.direction = transform.forward;
                Vector3 rangeInGround = r.GetPoint(range);
                rangeInGround.y = 0;

                testSp.transform.position = rangeInGround;
                //Debug.DrawRay(gunPos, rangeInGround - gunPos, new Color(1, 0.2f, 0.2f, 0.5f));
                targetingLazer.colorGradient = dr2b;
                targetingLazer.SetPosition(1, rangeInGround);

                gun.LookAt(rangeInGround);

                if (CrossPlatformInputManager.GetButtonDown("Shoot"))
                {
                    pv.RPC("Shoot", Photon.Pun.RpcTarget.All, -1);
                    BulletTrailManager.instance.pv.RPC("PlayEffect", RpcTarget.All, gunPos, rangeInGround);
                    VFXPlayer.instance.pv.RPC("PlayVFX", RpcTarget.All, ((int)VFXPlayer.vfx.gunSpark), rangeInGround, Quaternion.Inverse(gun.rotation));
                    VFXPlayer.instance.pv.RPC("PlayVFX", RpcTarget.All, ((int)VFXPlayer.vfx.gunSpark), gunPos, gun.rotation);

                }
            }
        }
    }

    [PunRPC] public void Shoot(int _target)
    {
        SoundPlayer.instance.PlaySound(SoundPlayer.instance.pistolFire, gunPos);
        if (_target == -1) { //아무도 못맞췄을떄
            if (--rounds <= 0)
            {
                StartCoroutine(Reload());
            }
            return;
        }

        float damage;
        float knockbackMul;
        GameObject target = PhotonView.Find(_target).gameObject;
        Vector3 knockBack = target.transform.position - transform.position;
        knockBack.y = 0;
        switch (typeOnHand)
        {
            case GunType.pistol:
                damage = 30f;
                knockbackMul = 1f;
                break;
            case GunType.shotgun:
                damage = 30f;
                knockbackMul = 1f;
                break;
            case GunType.sniperRifle:
                damage = 30f;
                knockbackMul = 1f;
                break;
            case GunType.assaultRifle:
                damage = 30f;
                knockbackMul = 1f;
                break;
            default:
                damage = 0f;
                knockbackMul = 0;
                break;
        }

        target.GetComponent<Zombie>().AttackFromPlayer(damage * (GunManager.instance.damageMultiplier / 100), GunManager.instance.pierceAdd, knockBack * knockbackMul);
        if (--rounds <= 0)
        {
            StartCoroutine(Reload());
        }
    }
    //}public void Shoot()
    //{
    //    switch (typeOnHand)
    //    {
    //        case GunType.pistol:
    //            //Debug.Log("pistol " + GunManager.instance.gunDamage.ToString());
    //            if (enemyFinder.target != null)
    //            {
    //                StartCoroutine(FireVFX(pistolFireTransform.position, GunManager.instance.gunDamage, true));
    //            }
    //            else
    //            {
    //                StartCoroutine(FireVFX(pistolFireTransform.position, 0, false));
    //            }
    //            break;
    //        case GunType.shotgun:
    //            //Debug.Log("shotgun " + GunManager.instance.gunDamage.ToString());
    //            if (enemyFinder.target != null)
    //            {
    //                StartCoroutine(FireVFX(shotgunFireTransform.position, GunManager.instance.gunDamage, true));
    //            }
    //            else
    //            {
    //                StartCoroutine(FireVFX(shotgunFireTransform.position, 0, false));
    //            }
    //            break;
    //        case GunType.sniperRifle:
    //            //Debug.Log("sniperRifle " + (GunManager.instance.gunDamage * 2).ToString());
    //            if (enemyFinder.target != null)
    //            {
    //                StartCoroutine(FireVFX(sniperRifleFireTransform.position, GunManager.instance.gunDamage * 3, true));
    //            }
    //            else
    //            {
    //                StartCoroutine(FireVFX(sniperRifleFireTransform.position, 0, false));
    //            }
    //            break;
    //        case GunType.assaultRifle:
    //            //Debug.Log("assaultRifle " + (GunManager.instance.gunDamage * 0.5f).ToString());
    //            if (enemyFinder.target != null)
    //            {
    //                StartCoroutine(FireVFX(assaultRifleFireTransform.position, GunManager.instance.gunDamage, true));
    //            }
    //            else
    //            {
    //                StartCoroutine(FireVFX(assaultRifleFireTransform.position, 0, false));
    //            }
    //            break;
    //    }

    //    if (--rounds <= 0)
    //    {
    //        StartCoroutine(Reload());
    //    }
    //}

    //���� Ȱ��ȭ�� �� �� �Լ��� ȣ����

    //public void EquipGun(GunType t)
    //{
    //    typeOnHand = t;
    //    rounds = GunManager.instance.GetGunRounds((int)typeOnHand);
    //    range = GunManager.instance.GetGunRange((int)typeOnHand);
    //}

    IEnumerator Reload()
    {
        if (pv.IsMine)
        {
            targetingLazer.enabled = false;
            testSp.SetActive(false);
        }

        float reloadTime = GunManager.instance.gunReloadTime * (1 - GunManager.instance.reloadMultiplier / 100);


        switch (typeOnHand)
        {
            case GunType.pistol:
                WaitForSeconds time = new WaitForSeconds(reloadTime / 3);
                SoundPlayer.instance.PlaySound(SoundPlayer.instance.pistolRM, gunPos);
                yield return time;
                SoundPlayer.instance.PlaySound(SoundPlayer.instance.pistolIM, gunPos);
                yield return time;
                SoundPlayer.instance.PlaySound(SoundPlayer.instance.pistolCocking, gunPos);
                yield return time;
                break;
            case GunType.shotgun:
                break;
            case GunType.sniperRifle:
                break;
            case GunType.assaultRifle:
                break;
            default:
                break;
        }
        rounds = (int)(GunManager.instance.GetGunRounds(typeOnHand) * (GunManager.instance.ammoMultiplier / 100));
        targetingLazer.enabled = true;
        testSp.SetActive(true);
    }

    //IEnumerator FireVFX(Vector3 firePos, float damage, bool isEnemy)
    //{
    //    fireLight.SetActive(true);
    //    fireLight.transform.position = firePos;
    //    bulletLine.enabled = true;
    //    bulletLine.SetPosition(0, firePos);
    //    //bulletLine.SetPosition(1, transform.forward * 10f);
    //    bulletLine.SetPosition(1, transform.position + transform.forward * 10f);
    //    if (isEnemy)
    //    {
    //        bulletLine.SetPosition(1, enemyFinder.target.position);
    //        Vector3 knockBack = enemyFinder.target.position - transform.position;
    //        knockBack.y = 0;
    //        enemyFinder.target.gameObject.GetComponent<Zombie>().pv.RPC("AttackFromPlayer", RpcTarget.All, damage, 0f, knockBack);
    //    }
    //    yield return new WaitForSeconds(gunShootingTime);
    //    bulletLine.enabled = false;
    //    fireLight.SetActive(false);
    //}

    public void Armoury(bool _armoury)
    {
        if (!_armoury)
        {
            testSp.SetActive(false);
            targetingLazer.enabled = false;
        } else
        {
            Reload();
        }
    }
}
