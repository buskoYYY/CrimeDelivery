//using SickscoreGames.HUDNavigationSystem;
using System.Collections;
using UnityEngine;
using static DerbyDirector;

public class CarDamageHandler : MonoBehaviour
{
    public bool initAtStart = false; 
    public CarComponentsController carComponents;

    public delegate void OnUpdateHealth(float health, float maxHealth);
    public event OnUpdateHealth OnUpdateHealthEvent;

    public delegate void OnDestroyCar(CarComponentsController carComponents);
    public event OnDestroyCar OnDestroyCarEvent;

    public delegate void OnCarReturnToLive(CarComponentsController carComponents);
    public event OnCarReturnToLive OnCarReturnToLiveEvent;

    public delegate void OnEndLives(CarComponentsController carComponents);
    public event OnEndLives OnEndLivesEvent;

    [SerializeField] private float currentHealth = 100;
    [SerializeField] private float maxHealth = 100;
    public int lives = 5;
    public bool unlimitedLives = false;

    private void Start()
    {
        carComponents = GetComponent<CarComponentsController>();
        if (initAtStart)
            Initialize(false, lives, 100);
    }

    public void Initialize(bool new_unlimitedLives, int new_lives, float new_maxHealth)
    {
        lives = new_lives;
        unlimitedLives = new_unlimitedLives;
        maxHealth = new_maxHealth;
        currentHealth = maxHealth;
        ChangeHealth(0);//Нужно для инициализации UI
    }

    private CarComponentsController lastHittedCar;
    public void HitCar(HitInfo hitInfo)
    {
        if (hitInfo.pushCarComponents.carDamageHandler.carAlive && hitInfo.carToHitcarComponents.carDamageHandler.carAlive)
        {
            
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

            ApplyDamage(calculatedHitInfo.damage);
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
        this.currentHealth += currentHealth;
        OnUpdateHealthEvent?.Invoke(this.currentHealth, maxHealth);
        if (this.currentHealth <= 0)
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
            OnDestroyCarEvent?.Invoke(carComponents);

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
            currentHealth = maxHealth;
            carAlive = true;
        }
    }

    private void EndOfLives()
    {
        
        OnEndLivesEvent?.Invoke(carComponents);
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
