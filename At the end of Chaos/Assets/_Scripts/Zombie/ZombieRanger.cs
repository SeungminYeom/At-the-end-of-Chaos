using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieRanger : Zombie
{
    bool isAttack;
    public GameObject acid;

    IEnumerator ie;

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
        target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);

        if (targeting)
        {
            if (Mathf.Pow(transform.position.x - target.transform.position.x, 2)
                + Mathf.Pow(transform.position.z - target.transform.position.z, 2) < 100f)
            {
                if (!isAttack)
                {
                    isAttack = true;
                    ie = Attack();
                    StartCoroutine(ie);
                }
            }
            else
            {
                isAttack = false;

                if (ie != null)
                {
                    ie = null;
                    StopCoroutine(ie);
                }    

                Vector3 zombieToTarget = target.transform.position - transform.position;
                zombieToTarget = zombieToTarget.normalized * speed;
                zombieToTarget.y = rigid.velocity.y;
                rigid.velocity = zombieToTarget;
            }
        }
    }

    protected override IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            GameObject spitAcid = Instantiate(acid, transform.position + Vector3.up * 2, Quaternion.identity);
            spitAcid.GetComponent<Acid>().SetPos(spitAcid.transform.position, target.transform.position);
            yield return null;
        }
    }
}
    