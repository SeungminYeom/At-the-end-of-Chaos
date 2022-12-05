using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXPlayer : MonoBehaviourPun
{
    public enum vfx
    {
        gunSpark
    }

    public static VFXPlayer instance;
    public PhotonView pv;

    public List<ParticleSystem> particlesPool;


    [SerializeField] float destroyDelay = 2f;
    WaitForSeconds destroyTime;
    void Start()
    {
        instance = this;
        pv = GameServerManager.instance.pv;

        destroyTime = new WaitForSeconds(destroyDelay);

        particlesPool.Add(Resources.LoadAll<GameObject>("VFX/Gun/GunSpark")[0].GetComponent<ParticleSystem>());
    }

    [PunRPC] void PlayVFX(int _vfx, Vector3 _pos, Quaternion _rot)
    {
        ParticleSystem vfx = Instantiate(particlesPool[_vfx], _pos, _rot); //임시
        StartCoroutine(DestroyObject(vfx));
    }

    IEnumerator DestroyObject(ParticleSystem _obj)
    {
        yield return destroyTime;
        Destroy(_obj.gameObject);
    }
}
