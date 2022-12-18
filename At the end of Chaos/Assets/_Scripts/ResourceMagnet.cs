using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceMagnet : MonoBehaviour
{
    public Transform flyingTarget;

    private void OnEnable()
    {
        StartCoroutine(Retrieve());
    }

    IEnumerator Retrieve()
    {
        yield return new WaitForSeconds(1.5f);

        GetComponent<SphereCollider>().enabled = true;
        Destroy(GetComponent<Rigidbody>());
        //rigidbody.useGravity = false;

        while (true)
        {
            transform.position = Vector3.Lerp(transform.position, flyingTarget.position, Time.deltaTime * 10);
            //rigidbody.AddForce((flyingTarget.transform.position - transform.position).normalized);
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == flyingTarget.gameObject)
        {
            Destroy(gameObject);
        }
    }
}
