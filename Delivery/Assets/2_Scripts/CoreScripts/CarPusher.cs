using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static DerbyDirector;

[System.Serializable]
public class HitInfo
{
    public Collision collision;

    public CarColliderType pushCollider;
    public CarComponentsController pushCarComponents;

    public CarColliderType colliderToHit;
    public CarComponentsController carToHitcarComponents;

    public DerbyDirectorConfig derbyDirectorConfig;

    public HitInfo SetHitInfo(Collision newContact, CarColliderType pushCollider, CarComponentsController pushCarComponents, CarColliderType colliderToHit, CarComponentsController carToHitcarComponents, 
        DerbyDirectorConfig derbyDirectorConfig = null)
    {
        collision = newContact;

        this.pushCollider = pushCollider;
        this.pushCarComponents = pushCarComponents;

        this.colliderToHit = colliderToHit;
        this.carToHitcarComponents = carToHitcarComponents;
        if (derbyDirectorConfig != null)
            this.derbyDirectorConfig = derbyDirectorConfig;
        return this;
    }
}
public class CarPusher : MonoBehaviour
{
    private CarComponentsController carComponents;
    private float maxHitSpeed = 50;
    [SerializeField] private HitInfo hitInfo;

    public delegate void OnPushResult(CalculatedHitInfo pushResult);
    public event OnPushResult OnPushResultEvent;

    public delegate void OnOtherCarDestroyed();
    public event OnOtherCarDestroyed OnOtherCarDestroyedEvent;
    private void Start()
    {
        carComponents = GetComponent<CarComponentsController>();
    }
    private float pushDelay = 0.1f;
    public float pushDelayCurrent = 0;
    private void FixedUpdate()
    {
        if (pushDelayCurrent <= pushDelay)
        {
            pushDelayCurrent += Time.deltaTime;
        }
    }


    private void OnCollisionEnter(Collision collision)
    {
        
        if (pushDelayCurrent >= pushDelay)
        {
            
            CarColliderType pushCollider = collision.GetContact(0).thisCollider.GetComponent<CarColliderType>();
            CarColliderType colliderToHit = collision.GetContact(0).otherCollider.GetComponent<CarColliderType>();


            if (pushCollider != null && colliderToHit != null)
            {

                pushDelayCurrent = 0;
                hitInfo = hitInfo.SetHitInfo(collision, pushCollider, carComponents,
                                            colliderToHit, colliderToHit.carComponentsController);
                colliderToHit.HitCarCollider(hitInfo);
            }

        } 
    }

    public void PushResult(CalculatedHitInfo pushResult)
    {
        OnPushResultEvent?.Invoke(pushResult);
    }

    public void OtherCarDestroyed()
    {
        OnOtherCarDestroyedEvent?.Invoke();
    }
}


