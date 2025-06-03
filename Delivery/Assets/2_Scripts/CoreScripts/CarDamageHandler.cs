//using SickscoreGames.HUDNavigationSystem;
using ArcadeBridge;
using System.Collections;
using UnityEngine;
using static DerbyDirector;

public class CarDamageHandler : MonoBehaviour
{
    public bool initAtStart = false; 
    public CarComponentsController carComponents;

    public delegate void OnUpdateHealth(float health, float maxHealth);
    public event OnUpdateHealth OnUpdateHealthEvent;

    public delegate void OnDestroyCar(CarComponentsController carComponents, RaceData.CompleteType completeType);
    public event OnDestroyCar OnDestroyCarEvent;

    public delegate void OnCarReturnToLive(CarComponentsController carComponents);
    public event OnCarReturnToLive OnCarReturnToLiveEvent;

    public delegate void OnEndLives(CarComponentsController carComponents, RaceData.CompleteType completeType);
    public event OnEndLives OnEndLivesEvent;

    public float CurrentHealth { get; private set; } = 100f;
    public float MaxHealth { get; private set; } = 100f;
    public int lives = 5;
    public bool unlimitedLives = false;

    private DerbyDirectorConfig derbyDirectorConfig;

    private void Start()
    {
        derbyDirectorConfig = new DerbyDirectorConfig();
        carComponents = GetComponent<CarComponentsController>();
        if (initAtStart)
            Initialize(false, lives, 100);
    }

    public void Initialize(bool new_unlimitedLives, int new_lives, float new_maxHealth)
    {
        lives = new_lives;
        unlimitedLives = new_unlimitedLives;
        ChangeMaxHealth(new_maxHealth);
        ChangeHealth(0);//����� ��� ������������� UI
    }

    public void ChangeMaxHealth(float maxHealth)
    {
        MaxHealth = maxHealth;
        CurrentHealth = MaxHealth;
    }

    private float hitDelay = 0.2f;
    private float currentHitDelay = 0;

    private void FixedUpdate()
    {
        if (currentHitDelay <= 0.2f)
            currentHitDelay += Time.deltaTime;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (currentHitDelay >= hitDelay)
        {
            if (!carComponents.isPlayer)
            {
                CarColliderType hittedCarCollider = collision.GetContact(0).otherCollider.GetComponent<CarColliderType>();
                if (hittedCarCollider == null)
                    ApplyDamage(DerbyDirector.CalculateHitStaticDamage(collision, derbyDirectorConfig, carComponents.isPlayer));
            }
            currentHitDelay = 0;
        }
    }

    private CarComponentsController lastHittedCar;
    public void HitCar(HitInfo hitInfo)
    {
        if (hitInfo.pushCarComponents.carDamageHandler.carAlive && hitInfo.carToHitcarComponents.carDamageHandler.carAlive)
        {
            hitInfo.derbyDirectorConfig = this.derbyDirectorConfig;   
            CalculatedHitInfo calculatedHitInfo = DerbyDirector.CalculatePushForce(hitInfo);
            carComponents.carRigidbody.AddForceAtPosition(calculatedHitInfo.pushForce, hitInfo.collision.GetContact(0).point, ForceMode.Impulse);

            lastHittedCar = hitInfo.pushCarComponents;
            lastHittedCar.carPusher.PushResult(calculatedHitInfo);

            if (lastHittedCarRemover == null)
                lastHittedCarRemover = StartCoroutine(ClearLastHittedCar());
            else
            {
                StopCoroutine(lastHittedCarRemover);
                lastHittedCarRemover = StartCoroutine(ClearLastHittedCar());
            }


            if (calculatedHitInfo.strongHit)
                carComponents.vehicle.StrongHit();

            CalculatedDamage calculatedDamage = DerbyDirector.CalculateDamage(hitInfo);

            ApplyDamage(calculatedDamage.damage);
        }

    }

    private Coroutine lastHittedCarRemover;
    private IEnumerator ClearLastHittedCar()
    {
        yield return new WaitForSeconds(5f);
        lastHittedCar = null;
    }

    public bool damageble = true;
    public void ApplyDamage(float damage)
    {
        if (carAlive && damageble)
            ChangeHealth(-damage);
    }

    public bool carAlive = true;
    private void ChangeHealth(float currentHealth)
    {
        this.CurrentHealth += currentHealth;
        OnUpdateHealthEvent?.Invoke(this.CurrentHealth, MaxHealth);
        if (this.CurrentHealth <= 0)
        {
            DestroyCar();
        }
    }

    private void DestroyCar()
    {
        if (carAlive)
        {
            carAlive = false;
            lives--;
            if (lives > 0 || unlimitedLives)
                Invoke(nameof(RetunToLive), 2);
            else
                EndOfLives();
            OnDestroyCarEvent?.Invoke(carComponents, RaceData.CompleteType.DESTROYED);

            if (lastHittedCar!= null)
                lastHittedCar.carPusher.OtherCarDestroyed();

            /*
            if (gameObject.TryGetComponent<HUDNavigationElement>(out HUDNavigationElement carHUDNavigation))
            {
                carHUDNavigation.enabled = false;
            }
            */
        }
    }


    public void RetunToLive()
    {
        if (!carAlive)
        {
            OnCarReturnToLiveEvent?.Invoke(carComponents);
            CurrentHealth = MaxHealth;
            carAlive = true;
        }
    }

    private void EndOfLives()
    {
        
        OnEndLivesEvent?.Invoke(carComponents, RaceData.CompleteType.DESTROYED);
        if (!carComponents.isPlayer)
            Destroy(gameObject, 5);
        /*
        var wheels = gameObject.GetComponent<VehicleCarDriftController>();

        foreach (WheelController wheel in wheels.wheels)
        {
            wheel.gameObject.SetActive(false);
        }
        */
    }
}
