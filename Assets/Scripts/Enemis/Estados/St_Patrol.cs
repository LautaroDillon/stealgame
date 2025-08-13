using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class St_Patrol : IState
{
    Guards guard;
    StateMachine _FSM;
    private float _waitTimer;
    private float _waitDuration = 5f;

    public St_Patrol(Guards guard, StateMachine fsm)
    {
        this.guard = guard;
        _FSM = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Patrol State");
        guard.isPatrolling = true;
        _waitTimer = 0f;
        guard.isIdle = false;
        ChooseRandomNodeInZone();
    }

    public void Tick()
    {
        if (guard.path != null && guard.pathIndex < guard.path.Count)
        {
            var node = guard.path[guard.pathIndex];
            Vector3 dir = (node.transform.position - guard.transform.position).normalized;

            //TryStepUp(dir);

            Vector3 force = dir * guard.maxForce;
            guard.rb.AddForce(force, ForceMode.Acceleration);

            if (guard.rb.velocity.magnitude > guard.maxSpeed)
            {
                guard.rb.velocity = guard.rb.velocity.normalized * guard.maxSpeed;
            }

            if (force.sqrMagnitude > 0.001f)
            {
                Vector3 flat = new Vector3(force.x, 0, force.z).normalized;
                Quaternion rot = Quaternion.LookRotation(flat);
                guard.transform.rotation = Quaternion.Slerp(
                    guard.transform.rotation, rot, Time.deltaTime * 5f);
            }

            //Vector3 local = guard.transform.InverseTransformDirection(dir);
            // guard.anim.SetFloat("Horizontal", local.x, 0.1f, Time.deltaTime);
            //guard.anim.SetFloat("Vertical", local.z, 0.1f, Time.deltaTime);

            if (Vector3.Distance(guard.transform.position, node.transform.position) < guard.nodeReachDistance)
            {
                Debug.Log($"Guard reached node {guard.pathIndex} at {node.transform.position}");
                guard.pathIndex++;
            }
        }
        else
        {
            _waitTimer += Time.deltaTime;
            if (_waitTimer >= _waitDuration)
            {
                _waitTimer = 0f;
                ChooseRandomNodeInZone();
                guard.isPatrolling = true;
            }

            // guard.anim.SetFloat("Horizontal", 0f);
            // guard.anim.SetFloat("Vertical", 0f);
        }
    }

    public void OnExit()
    {
        guard.isPatrolling = false;
        guard.rb.velocity = Vector3.zero;
        // guard.anim.SetFloat("Horizontal", 0f);
        // guard.anim.SetFloat("Vertical", 0f);
    }

    private void ChooseRandomNodeInZone()
    {
        var zoneNodes = ManagerNode.Instance.GetNodesInZone(guard.zoneId);
        if (zoneNodes == null || zoneNodes.Count < 2)
            return;

        var start = ManagerNode.Instance.GetClosestNode(guard.transform.position, guard.zoneId);
        if (start == null)
            return;

        zoneNodes.Remove(start);
        if (zoneNodes.Count == 0)
            return;

        var dest = zoneNodes[Random.Range(0, zoneNodes.Count)];

        guard.path = ManagerNode.Instance.FindPath(start, dest);
        guard.pathIndex = 0;

        for (int i = 0; i < guard.path.Count - 1; i++)
        {
            Debug.DrawLine(guard.path[i].transform.position, guard.path[i + 1].transform.position, Color.green, 2f);
        }
    }

    /*private void TryStepUp(Vector3 dir)
    {
        if (guard.col == null) return;

        CapsuleCollider col = guard.col;

        // Origen desde la base del collider (ligeramente elevado para no tocar el suelo directamente)
        Vector3 rayOrigin = guard.rayFoot.position;
        float rayDistance = 1f;

        // Visualización del raycast
        Debug.DrawRay(rayOrigin, dir.normalized * rayDistance, Color.red);

        if (Physics.Raycast(rayOrigin, dir, out RaycastHit hit, rayDistance, ~LayerMask.GetMask("Enemy")))
        {
            Vector3 normal = hit.normal;
            float angle = Vector3.Angle(normal, Vector3.up);

            if (angle > 40f && angle < 80f)
            {
                // Aplica impulso vertical leve para subir la rampa/escalón
                Vector3 upwardBoost = Vector3.up * 4f + dir.normalized * 1f;
                guard.rb.AddForce(upwardBoost, ForceMode.VelocityChange);
            }
        }
    }*/
}
