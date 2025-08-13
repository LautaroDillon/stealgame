using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockPick_S : MonoBehaviour
{
    public float pickPosition;
    public float pickposition
    {
        get { return pickPosition; }
        set
        {
            pickPosition = value;
            pickPosition = Mathf.Clamp(pickPosition, 0, 1);
        }
    }

    public float CilinderPosition;
    public float cilinderposition
    {
        get { return CilinderPosition; }
        set
        {
            CilinderPosition = value;
            CilinderPosition = Mathf.Clamp(CilinderPosition, 0, _MaxRotationDistance);
        }
    }

    public Animator animator;
    public float pickSpeed = 0.5f;
    public float cilinderRotationSpeed = 0.5f;
    public float cilinderRetentionSpeed = 0.5f;

    bool _Paused = false;
    bool _Shakin = false;

    public float targetPosition;
    [SerializeField] float leanency = 0.1f;
    float _MaxRotationDistance
    {
        get
        {
            return 1f - Mathf.Abs(targetPosition - pickposition) + leanency;

        }
    }

    float _Tension;
    [SerializeField] float _TensionMultiplicator;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        init();
        //Paused = false;
        // UpdateAnimator();
    }

    void init()
    {
        Reset();

        targetPosition = UnityEngine.Random.value;
    }

    private void Reset()
    {
        cilinderposition = 0f;
        pickPosition = 0f;
        _Tension = 0f;
        _Paused = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (_Paused)
            return;
        if (Input.GetAxisRaw("Mouse Y") != 0)
            Pick();
        ShakinMetod();
        cilinder();
        UpdateAnimator();
    }

    private void ShakinMetod()
    {
        _Shakin = _MaxRotationDistance - cilinderposition < 0.03f;
        if (_Shakin)
        {
            _Tension += Time.deltaTime * _TensionMultiplicator;
            if (_Tension > 1)
            {
                BreakPiclock();
            }
        }
    }

    private void BreakPiclock()
    {
        Debug.Log("Lock Pick Broke!");
        Reset();
    }

    private void cilinder()
    {
        cilinderposition -= cilinderRetentionSpeed * Time.deltaTime;
        cilinderposition += Math.Abs(Input.GetAxisRaw("Mouse y")) * Time.deltaTime * cilinderRotationSpeed;

        if (cilinderposition > 0.98f)
        {
            complete();
        }
    }

    private void complete()
    {
        _Paused = true;
        Debug.Log("Lock Picked!");
    }

    private void UpdateAnimator()
    {
        animator.SetFloat("PickPosition", pickPosition);
        animator.SetFloat("LockOpen", cilinderRotationSpeed);
        animator.SetBool("Shakin", _Shakin);
    }

    private void Pick()
    {
        pickPosition += Input.GetAxisRaw("Mouse X") * Time.deltaTime * pickSpeed;
    }
}
