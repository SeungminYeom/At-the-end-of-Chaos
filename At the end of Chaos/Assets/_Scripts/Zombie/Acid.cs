using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Acid : MonoBehaviour
{
    [SerializeField] Vector3 startPos;
    [SerializeField] Vector3 peakPos;
    [SerializeField] Vector3 EndPos;
    float u;

    void Update()
    {
        Vector3 A = Vector3.Lerp(startPos, peakPos, (Time.time - u) / 1);
        Vector3 B = Vector3.Lerp(peakPos, EndPos, (Time.time - u) / 1);
        transform.position = Vector3.Lerp(A, B, (Time.time - u) / 1);
    }

    public void SetPos(Vector3 s, Vector3 e)
    {
        startPos = s;
        peakPos = (s + e) * 0.5f + Vector3.up * 5f;
        EndPos = e;
        u = Time.time;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Train")
        {
            other.GetComponent<Train>().Attacked();
            SoundPlayer.instance.PlaySound(SoundPlayer.instance.TrainAttacked, transform.position);
            Destroy(gameObject);
        }
    }
}
