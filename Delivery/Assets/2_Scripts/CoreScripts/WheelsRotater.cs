using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CarWheel
{
    public int skidId = -1;
    public Transform wheelTrasform;
    public float skidMarkOffset;
    public ParticleSystem wheelsSmoke;
}

public class WheelsRotater : MonoBehaviour
{

    private CarDriftController car;
    public List<CarWheel> wheelsBack = new List<CarWheel>();
    public List<CarWheel> wheelsFront = new List<CarWheel>();

    [SerializeField] private float intensityModifier = 1.5f;
    private Skidmarks skidMarkController;
    [SerializeField] private ParticleSystem wheelsSmokeParticles;
    public List<CarWheel> lastSkidId = new List<CarWheel>();
    public float skidMarkOffset = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        car = GetComponent<CarDriftController>();
        skidMarkController = FindFirstObjectByType<Skidmarks>().GetComponent<Skidmarks>();

        for (int i = 0; i < wheelsBack.Count; i++)
        {
            CarWheel skidId = new CarWheel();
            skidId.skidId = -1;
            skidId.wheelTrasform = wheelsBack[i].wheelTrasform;
            skidId.skidMarkOffset = skidMarkOffset;
            skidId.wheelsSmoke = Instantiate(wheelsSmokeParticles, new Vector3(skidId.wheelTrasform.parent.position.x, skidId.wheelTrasform.parent.position.y - skidId.skidMarkOffset, skidId.wheelTrasform.parent.position.z), wheelsSmokeParticles.transform.rotation, skidId.wheelTrasform.parent);
            //skidId.wheelTrasform.parent.position.x, skidId.wheelTrasform.parent.position.y - skidId.skidMarkOffset, skidId.wheelTrasform.parent.position.z) , wheelsSmokeParticles.transform.rotation, skidId.wheelTrasform.parent
            lastSkidId.Add(skidId);
        }

        for (int i = 0; i < wheelsFront.Count; i++)
        {
            CarWheel skidId = new CarWheel();
            skidId.skidId = -1;
            skidId.wheelTrasform = wheelsFront[i].wheelTrasform;
            skidId.skidMarkOffset = skidMarkOffset;
            skidId.wheelsSmoke = Instantiate(wheelsSmokeParticles, new Vector3(skidId.wheelTrasform.parent.position.x, skidId.wheelTrasform.parent.position.y - skidId.skidMarkOffset, skidId.wheelTrasform.parent.position.z) , wheelsSmokeParticles.transform.rotation, skidId.wheelTrasform.parent);
            lastSkidId.Add(skidId);
        }
        StartCoroutine(StartSkidMarks());
    }


    private void SkidMarks()
    {
        float intensity = car.rigidBody.linearVelocity.x / car.rigidBody.linearVelocity.z;
        if (intensity < 0)
            intensity = -intensity;

        if (car.inSlip && car.isGrounded)
        {
            for (int i = 0; i < lastSkidId.Count; i++)
            {
                lastSkidId[i].skidId = skidMarkController.AddSkidMark(new Vector3(lastSkidId[i].wheelTrasform.position.x, lastSkidId[i].wheelTrasform.position.y - lastSkidId[i].skidMarkOffset, lastSkidId[i].wheelTrasform.position.z), 
                    transform.up, intensity * intensityModifier, lastSkidId[i].skidId);
                
                if (lastSkidId[i].wheelsSmoke != null && !lastSkidId[i].wheelsSmoke.isPlaying)
                    lastSkidId[i].wheelsSmoke.Play();
            }
        }
        else
        {
            for (int i = 0; i < lastSkidId.Count; i++)
            {
                lastSkidId[i].skidId = -1;

                if (lastSkidId[i].wheelsSmoke != null && lastSkidId[i].wheelsSmoke.isPlaying)
                    lastSkidId[i].wheelsSmoke.Stop();
            }
        }
    }

    void LateUpdate()
    {
        WheelsRotation();
        if (startSkidMarks == false)
            SkidMarks();

    }

    private bool startSkidMarks = true;
    private IEnumerator StartSkidMarks()
    {
        float intensity = car.rigidBody.linearVelocity.x / car.rigidBody.linearVelocity.z;
        if (intensity < 0)
            intensity = -intensity;
        for (int i = 0; i < lastSkidId.Count; i++)
        {
            lastSkidId[i].skidId = skidMarkController.AddSkidMark(new Vector3(lastSkidId[i].wheelTrasform.position.x, lastSkidId[i].wheelTrasform.position.y - lastSkidId[i].skidMarkOffset, lastSkidId[i].wheelTrasform.position.z),
                transform.up, intensity * intensityModifier, lastSkidId[i].skidId);

            if (lastSkidId[i].wheelsSmoke != null && !lastSkidId[i].wheelsSmoke.isPlaying)
                lastSkidId[i].wheelsSmoke.Play();
        }

        yield return new WaitForSeconds(1.5f);
        startSkidMarks = false;
    }


    private float spinRotation;
    private void WheelsRotation()
    {
        spinRotation += car.pvel.z * Time.deltaTime * 100;
        float turnRotation = car.inTurn * 10;

        foreach (CarWheel wheel in wheelsBack)
            wheel.wheelTrasform.localRotation = Quaternion.Euler(spinRotation, 0, 0);

        foreach (CarWheel wheel in wheelsFront)
            SetCurrentRotation(turnRotation, wheel.wheelTrasform);
    }

    private void SetCurrentRotation(float rot, Transform wheelTrasform)
    {
        Mathf.Clamp(rot, -30, 30);
        wheelTrasform.localRotation = Quaternion.Euler(spinRotation, rot, 0);
    }


}

