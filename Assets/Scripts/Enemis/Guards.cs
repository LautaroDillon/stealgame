using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Guards : MonoBehaviour
{
    public Rigidbody rb;

    public int zoneId;
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

    [Header("Fsm")]
    StateMachine fsm;

    [Header("Pathfinding")]
    public NodePathfinding initialNode;
    public NodePathfinding goalNode;
    public List<NodePathfinding> path;
    public int pathIndex;
    public float nodeReachDistance = 0.3f;

    public float maxSpeed;
    public float arriveRadius;
    public float maxForce;
    [HideInInspector] public Vector3 velocity;

    [Header("Bools")]
    public bool isPatrolling, isIdle;

    private void Start()
    {
        StartCoroutine(FOVRoutine());

        fsm = new StateMachine();

        var stPatrol = new St_Patrol(this, fsm);
        var stChase = new St_Chase(this, fsm);
        var St_Idle = new St_Idle(this, fsm);
        var St_Serach = new St_Serach(this, fsm);

        //defino las distintas tranciciones de la fsm
        at(St_Idle, stPatrol, () => isPatrolling && !canSeePlayer);
        at(St_Idle, stChase, () => canSeePlayer);

        //cambion en el estado patrol
        at(stPatrol, stChase, () => canSeePlayer);
        at(stPatrol, St_Idle, () => !isPatrolling && !canSeePlayer);

        //cambios en el estado chase
        at(stChase, stPatrol, () => !canSeePlayer);
        at(stChase, St_Idle, () => !canSeePlayer && !isPatrolling);

        fsm.SetState(St_Idle);
    }

    void at(IState from, IState to, Func<bool> condition) => fsm.AddTransition(from, to, condition);
    void any(IState to, Func<bool> condition) => fsm.AddAnyTransition(to, condition);

    private void Update()
    {
        fsm.Tick();
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

    #region Movement
    public Vector3 Seek(Vector3 targetSeek)
    {
        var desired = targetSeek - transform.position;
        desired.Normalize();
        desired *= maxSpeed;
        return CalculateSteering(desired);
    }
    public Vector3 CalculateSteering(Vector3 desired)
    {
        var steering = desired - velocity;
        steering = Vector3.ClampMagnitude(steering, maxForce);
        return steering;
    }

    public void AddForce(Vector3 dir)
    {
        velocity += dir;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);
    }
    #endregion


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

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, hearingRadius);
    }

    private Vector3 directionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;

        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
