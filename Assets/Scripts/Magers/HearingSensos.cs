using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Guards))]
public class HearingSensos : MonoBehaviour
{
    Guards Guard;

    // Start is called before the first frame update
    void Start()
    {
        Guard = GetComponent<Guards>();
        HearingManager.instance.RegisterSensor(this);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnHeardSound(Vector3 Location, EHearingSensosType category, float intensity)
    {
        //out of hearing range, ignore sound
        if (Vector3.Distance(Location, Guard.EyeLocation) > Guard.hearingRadius)
            return;

        Guard.ReportCanHear(Location, category, intensity);
    }

    void OnDestroy()
    {
        HearingManager.instance.UnregisterSensor(this);
    }
}
