using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    GameObject train;
    Rigidbody rigid;

    //공격할 대상
    [SerializeField] GameObject target;


    int health;
    float def;
    [SerializeField] int speed;
    int attackPoint;

    //공격까지의 대기 시간
    float attackDelay = 1f;
    //현재 좀비의 대기 시간
    [SerializeField] float attackDelayTime;


    void Start()
    {
        train = GameObject.Find("TrainManager");
        rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        //target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
        //rigid.velocity = (target.transform.position - transform.position).normalized * Time.deltaTime * speed;
        //Vector3 vec = transform.position;
        //vec.x -= Time.deltaTime * 2f;
        //transform.position = vec;

        if (GameManager.instance.timeState == TimeState.nightStart)
        {
            Stronger();
        }
        else if (GameManager.instance.timeState != TimeState.night)
        {
            Die();
        }
    }

    private void FixedUpdate()
    {
        target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
        //rigid.AddForce((target.transform.position - transform.position).normalized * 56f);
        //rigid.ve
        //rigid.velocity = rigid.velocity.normalized * speed;
        //rigid.velocity = (target.transform.position - transform.position).normalized * Time.deltaTime * speed;

        transform.position += ((target.transform.position - transform.position).normalized * speed + Vector3.left) * Time.deltaTime;

        //Vector3 vec = transform.position;
        //vec.x -= Time.deltaTime;
        //transform.position = vec;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject == train)
        {
            attackDelayTime += Time.deltaTime;
            if (attackDelayTime > attackDelay)
            {
                attackDelayTime = 0f;
                AttackFromZombie();
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == train)
        {
            attackDelayTime = 0f;
        }
    }

    void AttackFromPlayer(int damage, float defPen)
    {
        health -= (int)((1f - def * (1f - defPen)) * damage);
        if (health <= 0) Die();
    }

    void AttackFromZombie()
    {
        Debug.Log("기차 공격!");
    }

    void Die()
    {
        StopAllCoroutines();
        Destroy(gameObject);
    }

    void Stronger()
    {
        health = (int)(health * 1.1f);
        def *= 1.1f;
    }
}
