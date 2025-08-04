using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Guards : MonoBehaviour
{
    [Header("Fov")]
    public float radius;
    [Range(0, 360)]
    public float angle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    public bool canSeePlayer;

    [Header("stats")]
    public float moveSpeed;
    public float rotationSpeed;
    public float attackRange;
    public float chasevelocity;

    [Header("hearing stats")]
    public float hearingRadius;
    public Vector3 EyeLocation => transform.position;
    public Vector3 EyeDirection => transform.forward;

    private void Start()
    {
        StartCoroutine(FOVRoutine());
    }


    IEnumerator FOVRoutine()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        // Debug.Log("busco player");
        while (true)
        {
            yield return wait;
            FieldOfViewCheck();
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directiontoTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, directiontoTarget) < angle / 2)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, directiontoTarget, distanceToTarget, obstacleMask))
                    canSeePlayer = true;
                else 
                    canSeePlayer = false;
            }
            else
                canSeePlayer = false;
        }
        else if (canSeePlayer)
        {
            canSeePlayer = false;
        }
    }


    public void ReportCanHear(Vector3 Location, EHearingSensosType category, float intensity)
    {

    }

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, transform.forward, 360, radius);

        Vector3 viewAngleA = directionFromAngle(transform.eulerAngles.y, -angle / 2);
        Vector3 viewAngleB = directionFromAngle(transform.eulerAngles.y, angle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(transform.position, transform.position + viewAngleA * radius);
        Handles.DrawLine(transform.position, transform.position + viewAngleB * radius);

    }

    private Vector3 directionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
