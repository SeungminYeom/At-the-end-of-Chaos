using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EnemyFinder : MonoBehaviour
{
    public float viewRange; //radius
    [Range(0, 360)]
    public float viewAngle;

    public List<Transform> targetList = new List<Transform>();
    public Transform targetPos;

    public LayerMask targetMask;


    Vector2 viewPos;
    public Vector3 dirToTarget;

    void Start()
    {
        // 0.2�� �������� �ڷ�ƾ ȣ��
        StartCoroutine(FindTargetsWithDelay(1f));
    }

    IEnumerator FindTargetsWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void FindVisibleTargets()
    {
        targetList.Clear();
        targetPos = null;
        viewPos = new Vector2(transform.position.x, transform.position.z);

        // viewRadius�� ���������� �� �� ���� �� targetMask ���̾��� �ݶ��̴��� ��� ������
        Collider[] targetsInViewRadius = Physics.OverlapSphere(viewPos, Gun.range, targetMask);
        Debug.Log(targetsInViewRadius.Length);
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            targetList.Add(targetsInViewRadius[i].transform);
            Transform target = targetsInViewRadius[i].transform;
            Vector2 tartgetPos = new Vector2(target.position.x, target.position.z);
            dirToTarget = (tartgetPos - viewPos).normalized;

            Vector2 transForward = new Vector2(transform.forward.x, transform.forward.z);

            // �÷��̾�� forward�� target�� �̷�� ���� ������ ���� �����
            if (Vector2.Angle(transForward, dirToTarget) < viewAngle / 2)
            {
                //targetList.Add(target);
                if (targetPos == null)
                {
                    targetPos = target;
                    return;
                }
                if(Vector2.Distance(target.position, viewPos) < Vector2.Distance(targetPos.position, viewPos))
                    targetPos = target;
            }
        }
    }

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }

        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }
}

#if UNITY_EDITOR
    [CustomEditor(typeof(EnemyFinder))]
    public class FieldOfViewEditor : Editor
    {
        void OnSceneGUI()
        {
            EnemyFinder fow = (EnemyFinder)target;
            Handles.color = Color.white;
            Vector3 viewPos = fow.transform.position;
            viewPos.y = 0.01f;
            Handles.DrawWireArc(viewPos, Vector3.up, Vector3.forward, 360, Gun.range);
            Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
            Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

            Handles.DrawLine(viewPos, viewPos + viewAngleA * Gun.range);
            Handles.DrawLine(viewPos, viewPos + viewAngleB * Gun.range);

            Handles.color = Color.red;
            //foreach (Transform visible in fow.target)
            //{
            //    Handles.DrawLine(viewPos, visible.transform.position);
            //}
            if (fow.targetPos != null)
                Handles.DrawLine(viewPos, fow.targetPos.position);
        }
    }
#endif