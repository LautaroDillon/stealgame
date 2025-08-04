using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum TypeProyectils
{
    chair,
    knife,
    book,
}

public class Throwing : MonoBehaviour
{
    public Transform cam;
    public Transform attackpoint;


    public KeyCode code = KeyCode.Mouse0;

    [Header("Lista de proyectiles")]
    [SerializeField] private List<ProyectilData> Proyectails;

    //dictionary to hold the projectile prefabs and data
    private Dictionary<TypeProyectils, ProyectilData> dataDict;


    public TypeProyectils selectedType;
    bool canThrow;

    private void Awake()
    {
        // build the dictionary from the list of proyectail prefabs
        dataDict = Proyectails.ToDictionary(d => d.type, d => d);
    }

    // Start is called before the first frame update
    void Start()
    {
        canThrow = true;
    }

    // Update is called once per frame
    void Update()
    {
        changeEnum();

        var data = dataDict[selectedType];

        if (Input.GetKeyDown(code) && canThrow && data.totalProjectiles > 0)
        {
            Throw(data);
        }
    }

    // This method is a placeholder for changing the selected projectile type
    void changeEnum()
    {
        if (Input.anyKeyDown)
        {
            switch (Input.inputString)  // inputString devuelve la tecla como string ("1","2","3",...)
            {
                case "1":
                    selectedType = TypeProyectils.chair;
                    break;
                case "2":
                    selectedType = TypeProyectils.knife;
                    break;
            }
        }
    }


    // in this method we instantiate the projectile prefab, set its position and rotation and throw it in the direction of the camera's forward vector
    private void Throw(ProyectilData Data)
    {
        canThrow = false;
        //instantiate the projectile at the attack point position and rotation
        GameObject projectile = Instantiate(Data.projectile, attackpoint.position, cam.rotation);

        //get the rigidbody component of the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        //calculate the direction 
        Vector3 forceDirection = cam.transform.forward;

        RaycastHit hit;

        if (Physics.Raycast(cam.position, cam.forward, out hit, 500f))
        {
            forceDirection = (hit.point - attackpoint.position).normalized;
        }

        //add force to the projectile in the direction of the camera's forward vector
        Vector3 forcetoAdd = forceDirection * Data.throwForce + transform.up * Data.throwUpwardForce;

        rb.AddForce(forcetoAdd, ForceMode.Impulse);

        Data.totalProjectiles--;

        //Start the cooldown coroutine
        Invoke(nameof(Reset), Data.throwCooldown);
    }

    private void Reset()
    {
        canThrow = true;
    }

    // This method is a placeholder for adding the projectile to a list of throwable items
    public void addToThrows(int a)
    {
        dataDict[selectedType].totalProjectiles = dataDict[selectedType].totalProjectiles + a;
    }
}

[System.Serializable]
public class ProyectilData
{
    public TypeProyectils type;

    [Header("Prefab del proyectil")]
    public GameObject projectile;

    [Header("Cantidad disponible")]
    public int totalProjectiles;

    [Header("Ajustes de lanzamiento")]
    public float throwCooldown;
    public float throwUpwardForce;
    public float throwForce;
}