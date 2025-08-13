using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class St_Chase : IState
{
    Guards guard;
    StateMachine _FSM;

    public St_Chase(Guards guard, StateMachine fsm)
    {
        this.guard = guard;
        _FSM = fsm;
    }

    public void OnEnter()
    {
       Debug.Log("Entering Chase State");
    }

    public void OnExit()
    {
       
    }

    public void Tick()
    {
       
    }

}
