using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using static UnityEditor.Rendering.CameraUI;



public class DetectItems : MonoBehaviour
{
    public float PickupRange = 100f;
    public InteractuablesItems currentItem;

    public Camera mainCamera;
    public KeyCode code = KeyCode.E;

    private Dictionary<(interactionType, TypeProyectils), Action> actions;

    private void Start()
    {
        // Cache the reference to the main camera  
        mainCamera = Camera.main;
    }

    private void Update()
    {
        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found!");
            return;
        }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        Debug.DrawRay(ray.origin, ray.direction * PickupRange, Color.red);

        if (Physics.Raycast(ray, out hit, PickupRange))
        {
            // Logic for interacting with the hit object  
            InteractuablesItems obj = hit.collider.GetComponent<InteractuablesItems>();
            if (obj)
            {
                currentItem = obj;
            }
            else
            {
                currentItem = null;
            }
        }
        else
        {
            currentItem = null;
        }

        if (Input.GetKeyDown(code) && currentItem)
        {
            Debug.Log("using or picking up something" + currentItem.type);
            switch (currentItem.type)
            {
                case interactionType.None:
                    break;
                case interactionType.Pickup:
                    break;
                case interactionType.Use:
                    break;
                case interactionType.projectile:
                    switch (currentItem.typeProyectil)
                    {
                        case TypeProyectils.none:
                            break;
                        case TypeProyectils.chair:
                            // Here you can add the logic for using the chair projectile
                            break;
                        case TypeProyectils.knife:
                            // Here you can add the logic for using the knife projectile
                            break;
                        case TypeProyectils.book:
                            // Here you can add the logic for using the book projectile
                            break;
                    }

                    break;
            }

        }
    }
}
