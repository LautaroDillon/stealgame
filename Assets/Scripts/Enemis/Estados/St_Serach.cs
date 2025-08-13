using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class St_Serach : IState
{
    Guards guard;
    StateMachine _FSM;

    public St_Serach(Guards guard, StateMachine fsm)
    {
        this.guard = guard;
        _FSM = fsm;
    }

    public void OnEnter()
    {
        Debug.Log("Entering Search State");
    }

    public void OnExit()
    {
        
    }

    public void Tick()
    {
        
    }
}
