using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum interactionType
{
    None,
    Pickup,
    Use,
    projectile
}

public class InteractuablesItems : MonoBehaviour
{
    public interactionType type;
    public TypeProyectils typeProyectil;
    public int amount;

}
