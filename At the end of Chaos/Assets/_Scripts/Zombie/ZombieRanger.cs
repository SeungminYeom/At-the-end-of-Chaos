using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRanger : Zombie
{
    bool isAttack;

    void Start()
    {
        health = ((int)ZombieManager.instance.health);
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
            if (Mathf.Pow(transform.position.x - target.transform.position.x, 2)
                + Mathf.Pow(transform.position.z - target.transform.position.z, 2) < 10f)
            {
                if (!isAttack)
                {
                    isAttack = true;
                    StartCoroutine(Attack());
                }
            }
            else isAttack = false;

            target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
            Vector3 zombieToTarget = target.transform.position - transform.position;
            zombieToTarget = zombieToTarget.normalized * speed;
            zombieToTarget.y = rigid.velocity.y;
            rigid.velocity = zombieToTarget;
        }
    }

    IEnumerator Attack()
    {
        yield return new WaitForSeconds(2f);


    }
}
    