using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EHearingSensosType
{
    Efootstep,
    Ejump,
    Ethrow,

}

public class HearingManager : MonoBehaviour
{
    public static HearingManager instance { get; private set; } = null;

    public List<HearingSensos> Allsensors { get; private set; } = new List<HearingSensos>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Multiple instances of HearingManager found!" + gameObject.name);
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {

    }


    public void RegisterSensor(HearingSensos sensor)
    {
        // if (!Allsensors.Contains(sensor))
        {
            Allsensors.Add(sensor);
        }
    }

    public void UnregisterSensor(HearingSensos sensor)
    {
        //if (Allsensors.Contains(sensor))
        {
            Allsensors.Remove(sensor);
        }
    }

    public void OnSoundoEmited(Vector3 location, EHearingSensosType type, float intensity)
    {
        //notificate all sensors
        foreach (var sensor in Allsensors)
        {
            sensor.OnHeardSound(location, type, intensity);
        }
    }
}
