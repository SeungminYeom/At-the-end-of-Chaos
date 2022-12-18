using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieGiant : Zombie
{
    void Start()
    {
        health = ((int)ZombieManager.instance.health) * 5;
        def = ZombieManager.instance.def;
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
            zombieToTarget = zombieToTarget.normalized * speed;
            zombieToTarget.y = rigid.velocity.y;
            rigid.velocity = zombieToTarget;
        }
    }
}
    