using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class BulletTrail : MonoBehaviour
{
    [SerializeField] float delay = 0.03f;

    WaitForSeconds t;
    LineRenderer line;
    void Start()
    {
        t = new WaitForSeconds(delay);
        transform.SetParent(GameObject.Find("BulletTrailsPool").transform);
        line = GetComponent<LineRenderer>();
        gameObject.SetActive(false);
    }

    public void PlayEffect(ref Vector3 from, ref Vector3 to)
    {
        line.SetPosition(0, from);
        line.SetPosition(1, to);
        StartCoroutine(Pop(from, to));
    }

    IEnumerator Pop(Vector3 from, Vector3 to)
    {
        yield return t;
        gameObject.SetActive(false);
    }
}
