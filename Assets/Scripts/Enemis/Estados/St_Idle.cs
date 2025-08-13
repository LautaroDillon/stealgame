using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class St_Idle : IState
{
    Guards guard;
    StateMachine _FSM;

    public float idleDuration = 3f; // Duration to stay idle
    private float idleTimer;

    public St_Idle(Guards guard, StateMachine fsm)
    {
        this.guard = guard;
        _FSM = fsm;
    }
    public void OnEnter()
    {
        Debug.Log("Entering Idle State");
        idleTimer = idleDuration; // Reset the timer when entering the idle state
    }

    public void OnExit()
    {
        Debug.Log("exit Idle");
        idleTimer = idleDuration; // Reset the timer when entering the idle state

    }

    public void Tick()
    {
        idleTimer -= Time.deltaTime; // Decrease the timer

        if (idleTimer <= 0f)
        {
            // Transition to the next state after the idle duration
            guard.isIdle = false;
            guard.isPatrolling = true; // Assuming you want to transition to patrolling after idle

        }
    }

}
