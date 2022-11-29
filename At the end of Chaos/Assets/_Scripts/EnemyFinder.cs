using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyFinder : MonoBehaviour
{
    public float rayDeg;
    //[Range(0, 360)]
    //public float viewAngle;

    //public List<Transform> targetList = new List<Transform>();
    //public Transform targetPos;
    public Transform target;

    public LayerMask targetMask;

    //RaycastHit hit;

    Vector3 viewPos;
    public Vector3 dirToTarget;

    void Start()
    {
        // 0.2초 간격으로 코루틴 호출
        StartCoroutine(FindTargetsWithDelay(0.2f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            target = null;

            viewPos = new Vector3(transform.position.x, 0, transform.position.z);
            Physics.SphereCast(viewPos, rayDeg, transform.forward, out RaycastHit hit, Gun.range, targetMask);
            //Physics.Raycast(viewPos, transform.forward, out RaycastHit hit, Gun.range, targetMask);
            Debug.DrawRay(viewPos, transform.forward * Gun.range, Color.red);

            if (hit.collider != null)
                target = hit.transform;
            
            //FindVisibleTargets();
        }
    }

    //void FindVisibleTargets()
    //{
    //    targetList.Clear();
    //    targetPos = null;
    //    viewPos = new Vector2(transform.position.x, transform.position.z);

    //    // viewRadius를 반지름으로 한 구 영역 내 targetMask 레이어인 콜라이더를 모두 가져옴
    //    Collider[] targetsInViewRadius = Physics.OverlapSphere(viewPos, Gun.range, targetMask);
    //    Debug.Log(targetsInViewRadius.Length);
    //    for (int i = 0; i < targetsInViewRadius.Length; i++)
    //    {
    //        targetList.Add(targetsInViewRadius[i].transform);
    //        Transform target = targetsInViewRadius[i].transform;
    //        Vector2 tartgetPos = new Vector2(target.position.x, target.position.z);
    //        dirToTarget = (tartgetPos - viewPos).normalized;

    //        Vector2 transForward = new Vector2(transform.forward.x, transform.forward.z);

    //        // 플레이어와 forward와 target이 이루는 각이 설정한 각도 내라면
    //        if (Vector2.Angle(transForward, dirToTarget) < viewAngle / 2)
    //        {
    //            //targetList.Add(target);
    //            if (targetPos == null)
    //            {
    //                targetPos = target;
    //                return;
    //            }
    //            if (Vector2.Distance(target.position, viewPos) < Vector2.Distance(targetPos.position, viewPos))
    //                targetPos = target;
    //        }
    //    }
    //}

    //public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    //{
    //    if (!angleIsGlobal)
    //    {
    //        angleDegrees += transform.eulerAngles.y;
    //    }

    //    return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    //}
}

#if UNITY_EDITOR
    //[CustomEditor(typeof(EnemyFinder))]
    //public class FieldOfViewEditor : Editor
    //{
    //    void OnSceneGUI()
    //    {
    //        EnemyFinder fow = (EnemyFinder)target;
    //        Handles.color = Color.white;
    //        Vector3 viewPos = fow.transform.position;
    //        viewPos.y = 0.01f;
    //        Handles.DrawWireArc(viewPos, Vector3.up, Vector3.forward, 360, Gun.range);
    //        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
    //        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

    //        Handles.DrawLine(viewPos, viewPos + viewAngleA * Gun.range);
    //        Handles.DrawLine(viewPos, viewPos + viewAngleB * Gun.range);

    //        Handles.color = Color.red;
    //        //foreach (Transform visible in fow.target)
    //        //{
    //        //    Handles.DrawLine(viewPos, visible.transform.position);
    //        //}
    //        if (fow.targetPos != null)
    //            Handles.DrawLine(viewPos, fow.targetPos.position);
    //    }
    //}
#endif