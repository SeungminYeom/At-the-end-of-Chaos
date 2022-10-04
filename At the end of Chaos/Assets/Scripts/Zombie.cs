using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    GameObject train;
    Rigidbody rigidbody;

    //공격할 대상
    [SerializeField] GameObject target;


    int health;
    float def;
    int speed;
    int attack;


    void Start()
    {
        train = GameObject.Find("TrainManager");
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        target = train.GetComponent<TrainManager>().GetTrain(GameManager.instance.trainCount);
        rigidbody.velocity = (target.transform.position - transform.position).normalized * 2;
    }

    void ZombieAttack()
    {

    }
}
