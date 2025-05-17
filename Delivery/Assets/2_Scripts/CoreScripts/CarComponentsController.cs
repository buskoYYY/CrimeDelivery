using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CarComponentsController : MonoBehaviour
{
    public bool isPlayer = false;

    public CarComponent[] carComponents;

    public Vehicle vehicle;
    public CarDamageHandler carDamageHandler;
    public CarPusher carPusher;
    public Rigidbody carRigidbody;
    public GameObject carGameobject;
    public Transform carTrasform;

    //public SceneConfig sceneConfig;

    
    public float CurrentNitroFuel { get; private set; } = 0;
    [SerializeField] private float currentNitroFuel;
    public float MaxNitroFuel { get; private set; } = 1f;

    public float hitDamageMultiplyer = 1;

    public float pushDefenceFront = 1;
    public float pushDefenceUp = 1;

    public float pushForceFront = 2;
    public float pushForceUp = 2;

    private void Awake()
    {
        carGameobject = gameObject;
        carTrasform = transform;
        vehicle = GetComponent<Vehicle>();
        carRigidbody = GetComponent<Rigidbody>();
        massAtStart = carRigidbody.mass;
        currentMass = massAtStart;

        carDamageHandler = GetComponent<CarDamageHandler>();
        
        if (carPusher != null)
            carPusher = GetComponent<CarPusher>();


        SetupComponents();
        carDamageHandler.OnUpdateHealthEvent += OnUpdateHealth;
        carDamageHandler.OnDestroyCarEvent += OnCarDestroy;
        carDamageHandler.OnCarReturnToLiveEvent += OnCarReturnToLive;
    }
    private void OnDestroy()
    {
        carDamageHandler.OnUpdateHealthEvent -= OnUpdateHealth;
        carDamageHandler.OnDestroyCarEvent -= OnCarDestroy;
        carDamageHandler.OnCarReturnToLiveEvent -= OnCarReturnToLive;
    }

    public void OnUpdateHealth(float health, float maxHealth)
    {
        for (int i = 0; i < carComponents.Length; i++)
            carComponents[i].UpdateHealth(health, maxHealth);
    }

    public void OnCarDestroy(CarComponentsController destroyedCar)
    {
        for (int i = 0; i < carComponents.Length; i++)
            carComponents[i].CarDestroy();
    }

    public void OnCarReturnToLive(CarComponentsController destroyedCar)
    {
        for (int i = 0; i < carComponents.Length; i++)
            carComponents[i].CarReturnToLive();

        if (onResetCoroutine == null)
            onResetCoroutine = StartCoroutine(ResetingCar());
        else
        {
            StopCoroutine(onResetCoroutine);
            onResetCoroutine = StartCoroutine(ResetingCar());
        }
    }

    public void SetupComponents()
    {
        carComponents = GetComponents<CarComponent>();
        for (int i = 0; i < carComponents.Length; i++)
            carComponents[i].SetupComponent();
    }

    public  void StartRace()
    { }
    public void FinishRace()
    { }

    private float resetTimer;
    private float timeToReset = 2;

    public bool activateStuckReseting;
    private void FixedUpdate()
    {
        BoostLogic();
        currentNitroFuel = CurrentNitroFuel;
        StuckReseting();

    }

    private void StuckReseting()
    {
        if (activateStuckReseting)
        {

            if (carRigidbody.linearVelocity.magnitude <= 1f)
            {
                resetTimer += Time.deltaTime;
                if (resetTimer >= timeToReset)
                {
                    resetTimer = 0;
                    ResetCar();
                }

            }
            else
                resetTimer = 0;
        }

    }

    private void ResetCar()
    {
        if (carDamageHandler.carAlive)
        {
            if (onResetCoroutine == null)
                onResetCoroutine = StartCoroutine(ResetingCar());
            else
            {
                StopCoroutine(onResetCoroutine);
                onResetCoroutine = StartCoroutine(ResetingCar());
            }
        }

    }

    public void ChangeThrottleCarComponent(float gass)
    {
        vehicle.ChangeThrottle(gass);
    }

    public void ChangeTurnCarComponent(float turn)
    {
        vehicle.ChangeTurn(turn);
    }

    private float massAtStart;
    private float currentMass;

    private bool boostStarted;
    private float addNitroDelay;
    private void BoostLogic() //boost ������
    {
        addNitroDelay -= Time.deltaTime;
        if (addNitroDelay < 0)
        {
            AddNitroFuel(Time.deltaTime / 5f);

            addNitroDelay = 0.01f;
        }
    }


    public void AddNitroFuel(float value)
    {
        CurrentNitroFuel = Mathf.Clamp(CurrentNitroFuel += value, 0, MaxNitroFuel);
    }

    private Coroutine inBoostCorutine;
    private IEnumerator InBoost()
    {
        CurrentNitroFuel = 0;
        vehicle.ActivateBoost();
        currentMass = massAtStart * 1.5f;
        carRigidbody.mass = currentMass;
        yield return new WaitForSeconds(MaxNitroFuel);

        currentMass = massAtStart;
        carRigidbody.mass = currentMass;
        vehicle.DeactivateBoost();
    }

    public void ActivateBoostCarComponent()
    {
        if (CurrentNitroFuel >= MaxNitroFuel)
        {
            if (inBoostCorutine != null)
            {
                StopCoroutine(inBoostCorutine);
                inBoostCorutine = StartCoroutine(InBoost());
            }
            else
            {
                inBoostCorutine = StartCoroutine(InBoost());
            }
        }

            
    }

    private Coroutine onResetCoroutine;
    public delegate void OnReset(bool inReset);
    public event OnReset OnResetEvent;

    private IEnumerator ResetingCar()
    {
        OnResetEvent?.Invoke(true);
        ChangeCarCollidersLayers(11, 12);
        yield return new WaitForSeconds(3f);
  
        ChangeCarCollidersLayers(9, 10);
        OnResetEvent?.Invoke(false);
    }


    [SerializeField] private Transform trasparetTrigger;
    public LayerMask otherCarsLayerMask;
    private bool CarInOtherObjects()
    {
        Collider[] hitColliders = Physics.OverlapBox(trasparetTrigger.position, trasparetTrigger.localScale / 2, trasparetTrigger.rotation, otherCarsLayerMask);

        if (hitColliders.Length > 0)
        {
            return true;
        }
        else
            return false;
    }


    [SerializeField] private GameObject[] carPusherColliders;
    [SerializeField] private GameObject[] carShapeColliders;
    private void ChangeCarCollidersLayers(int carPusherColliderIndex, int carShapeColliderIndex)
    {

        for (int i = 0; i < carPusherColliders.Length; i++)
        {
            carPusherColliders[i].layer = carPusherColliderIndex;
        }

        for (int i = 0; i < carShapeColliders.Length; i++)
            carShapeColliders[i].layer = carShapeColliderIndex;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Check that it is being run in Play Mode, so it doesn't try to draw this in Editor mode
        //Draw a cube where the OverlapBox is (positioned where your GameObject is as well as a size)
        //Gizmos.DrawWireCube(trasparetTrigger.position, trasparetTrigger.localScale);
    }
}
