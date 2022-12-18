using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGuard : Zombie
{
    void Start()
    {
        health = ((int)ZombieManager.instance.health) * 2;
        def = ZombieManager.instance.def * 10;
        speed *= ZombieManager.instance.speedMultiplier;
        train = GameObject.Find("TrainManager");
        rigid = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    protected override void FixedUpdate()
    {
        if (targeting)
        {
            target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
            Vector3 zombieToTarget = target.transform.position - transform.position;
            zombieToTarget = zombieToTarget.normalized * speed + Vector3.left * 0.5f;
            zombieToTarget.y = rigid.velocity.y;
            rigid.velocity = zombieToTarget;
        }
    }
}
    