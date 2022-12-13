using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviourPun, IPunObservable
{
    GameObject train;
    GameObject ground;
    Rigidbody rigid;

    //공격할 대상
    [SerializeField] GameObject target;

    [SerializeField] public int health;
    [SerializeField] float def;
    [SerializeField] float speed;
    int attackPoint;

    //공격까지의 대기 시간
    float attackDelay = 1f;
    //현재 좀비의 대기 시간
    [SerializeField] float attackDelayTime;

    public bool targeting = true;

    public PhotonView pv;

    int takenDamage = 0;


    void Start()
    {
        health = ((int)ZombieManager.instance.health);
        def = ZombieManager.instance.def;
        speed *= ZombieManager.instance.speedMultiplier;
        train = GameObject.Find("TrainManager");
        rigid = GetComponent<Rigidbody>();
        pv = GetComponent<PhotonView>();
    }

    void Update()
    {
        //target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
        //rigid.velocity = (target.transform.position - transform.position).normalized * Time.deltaTime * speed;
        //Vector3 vec = transform.position;
        //vec.x -= Time.deltaTime * 2f;
        //transform.position = vec;
            target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
            Vector3 zombieToTarget = target.transform.position - transform.position;
            transform.rotation = Quaternion.Euler(new Vector3(0, Mathf.Atan2(zombieToTarget.x, zombieToTarget.z) * Mathf.Rad2Deg, 0));
        
        //if (GameManager.instance.timeState == TimeState.nightStart)
        //{
        //    Debug.Log("STR");
        //    Stronger();
        //}
        //else if (GameManager.instance.timeState != TimeState.night)
        //{
        //    Die();
        //}
    }

    private void FixedUpdate()
    {
        if (targeting)
        {
            target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
            Vector3 zombieToTarget = target.transform.position - transform.position;
            //transform.position += ((target.transform.position - transform.position).normalized * speed + Vector3.left) * Time.deltaTime;
            zombieToTarget = zombieToTarget.normalized * speed + Vector3.left;
            zombieToTarget.y = rigid.velocity.y;
            rigid.velocity = zombieToTarget;
        }

        //Vector3 vec = transform.position;
        //vec.x -= Time.deltaTime;
        //transform.position = vec;
    }

    private void OnCollisionStay(Collision collision)
    {
        //if (collision.gameObject == train)
        //{
        //    attackDelayTime += Time.deltaTime;
        //    if (attackDelayTime > attackDelay)
        //    {
        //        attackDelayTime = 0f;
        //        AttackFromZombie();
        //    }
        //}
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == train)
        {
            attackDelayTime = 0f;
        }
    }

    //void AttackFromPlayer(float damage, float defPen)
    //{
    //    health -= (int)((1f - def * (1f - defPen)) * damage);
    //    if (health <= 0) Die();
    //}

    public void Die()
    {
        StopAllCoroutines();
        PhotonNetwork.Destroy(gameObject);
    }


    public void AttackFromPlayer(float damage, float pierce, Vector3 vec)
    {
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().AddForce(vec.normalized * 10f, ForceMode.Impulse);

        takenDamage = (int)((1 - (def * (1 - pierce / 100)) / 100) * damage);
        health -= takenDamage;

        DamageDisplayManager.instance.Display(takenDamage, transform.position);

        //Debug.Log("Damage : " + "((1 - (" + def + " * (1 - " + pierce + " / 100)) / 100) * " + damage +")" + " =>(int) " + ((int)((1 - (def * (1 - pierce / 100)) / 100) * damage)) );
        //관통력 1 = 방어력의 1% 무시
        //방어력 1 = 공격력의 1% 무시
        //최종 데미지는 소숫점을 버림
        if (health <= 0) Die();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //throw new System.NotImplementedException();
    }
}
    