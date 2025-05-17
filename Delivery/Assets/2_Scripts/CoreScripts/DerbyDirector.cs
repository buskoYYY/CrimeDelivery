using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[System.Serializable]
public class DerbyDirectorConfig
{
    public float maxSpeed = 50;
    public float pushCoefFront = 600;
    public float pushCoefUP = 5000;
    public float stunSpeed = 25;

    public float frontDamage = 50;
    public float sideDamage = 0;
    public float backDamage = 0;
    public float roofDamage = 0;

    public float frontDefence = 25;
    public float sideDefence = 0;
    public float backDefence = 0;
    public float roofDefence = 0;

    public float maxAirPushDevider = 5;
}

public static class DerbyDirector
{

    public class CalculatedHitInfo
    {
        public bool strongHit = false;
        public Vector3 pushForce;

        public float damage;

        public CalculatedHitInfo(Vector3 pushForce, float newDamage)
        {
            this.pushForce = pushForce;
            damage = newDamage;
        }
    }

    private static Vector3 ComputeIncidentVelocity(Rigidbody body, Collision collision, out Vector3 otherVelocity)
    {
        Vector3 impulse = collision.impulse;

        // Both participants of a collision see the same impulse, so we need to flip it for one of them.
        if (Vector3.Dot(collision.GetContact(0).normal, impulse) < 0f)
            impulse *= -1f;

        otherVelocity = Vector3.zero;

        // Static or kinematic colliders won't be affected by impulses.
        var otherBody = collision.rigidbody; 
        if (otherBody != null)
        {
            otherVelocity = otherBody.linearVelocity;
            if (!otherBody.isKinematic)
                otherVelocity += impulse / otherBody.mass;
        }

        //print($"{body.name} - {collision.impulse.magnitude}");
        //print($"{body.name} - {(body.velocity - impulse / body.mass).magnitude}");
        //print($"{body.name} - {(body.velocity - impulse / body.mass).magnitude}");


        return body.linearVelocity - impulse / body.mass;
    }

    //Reduce nitro force

    public static CalculatedHitInfo CalculatePushForce(HitInfo hitInfo)
    {
        Vector3 carPushVelosity = ComputeIncidentVelocity(hitInfo.pushCarComponents.carRigidbody, hitInfo.collision, out Vector3 carToHitVelosity);
        Vector3 carPushSpeed = hitInfo.pushCarComponents.carTrasform.InverseTransformDirection(carPushVelosity);
        Vector3 carToHitSpeed = hitInfo.pushCarComponents.carTrasform.InverseTransformDirection(carToHitVelosity);
        DerbyDirectorConfig derbyDirectorConfig = hitInfo.derbyDirectorConfig;
        float hitVelosityProcent;
        if (hitInfo.colliderToHit.colliderType == ColliderType.Back)
        {
            hitVelosityProcent = (Mathf.Clamp(Mathf.Abs(carPushSpeed.z / derbyDirectorConfig.maxSpeed) - Mathf.Abs(carToHitSpeed.z / derbyDirectorConfig.maxSpeed), 0, 1)) / 2;
            carPushSpeed = carPushSpeed * hitVelosityProcent;
        }
        else
            hitVelosityProcent = Mathf.Abs(carPushSpeed.magnitude / hitInfo.derbyDirectorConfig.maxSpeed);

        float damage = 0;
        float defence = 0;
        float pushColliderCoef = 0.01f;
        switch (hitInfo.pushCollider.colliderType)
        {
            case ColliderType.Front:
                damage = derbyDirectorConfig.frontDamage;
                pushColliderCoef = 1;
                break;
            case ColliderType.Side:
                damage = derbyDirectorConfig.sideDamage;
                break;
            case ColliderType.Roof:
                damage = derbyDirectorConfig.roofDamage;
                break;
            case ColliderType.Back:
                damage = derbyDirectorConfig.backDamage;
                break;
        }

        switch (hitInfo.colliderToHit.colliderType)
        {
            case ColliderType.Front:
                defence = derbyDirectorConfig.frontDefence;
                break;
            case ColliderType.Side:
                defence = derbyDirectorConfig.sideDefence;
                break;
            case ColliderType.Roof:
                defence = derbyDirectorConfig.roofDefence;
                break;
            case ColliderType.Back:
                defence = derbyDirectorConfig.backDefence;
                break;
        }

        float resultDamage = Mathf.Clamp(damage - defence, 0, 100) * hitVelosityProcent * hitInfo.pushCarComponents.hitDamageMultiplyer;
        //Vector3 pushFront =  new Vector3( Mathf.Sign(hitInfo.collision.impulse.x) * 200, Mathf.Sign(hitInfo.collision.impulse.y) * 200, Mathf.Sign(hitInfo.collision.impulse.z) * 200);


        /* //Speed
        Vector3 pushFront = (carPushVelosity / maxSpeed * pushCoefFront * Mathf.Clamp(hitVelosityProcent, 0.2f, 1) * pushColliderCoef) *
            Mathf.Clamp(hitInfo.pushCarComponents.pushForceFront - hitInfo.carToHitcarComponents.pushDefenceUp, 0, Mathf.Infinity);
        float pushUp = (carPushSpeed.z / maxSpeed * pushCoefUP * Mathf.Clamp(hitVelosityProcent, 0, 1) * pushColliderCoef) *
            Mathf.Clamp(hitInfo.pushCarComponents.pushForceUp - hitInfo.carToHitcarComponents.pushDefenceUp, 0, Mathf.Infinity);
        */


        //Impulse
        Vector3 pushFront = new Vector3(Mathf.Sign(hitInfo.collision.impulse.x) * derbyDirectorConfig.pushCoefFront, 
            Mathf.Sign(hitInfo.collision.impulse.y) * derbyDirectorConfig.pushCoefFront, 
                Mathf.Sign(hitInfo.collision.impulse.z) * derbyDirectorConfig.pushCoefFront) * pushColliderCoef * Mathf.Clamp(hitInfo.pushCarComponents.pushForceFront - hitInfo.carToHitcarComponents.pushDefenceUp, 0, Mathf.Infinity);
        float pushUp = derbyDirectorConfig.pushCoefUP * pushColliderCoef * Mathf.Clamp(hitInfo.pushCarComponents.pushForceFront - hitInfo.carToHitcarComponents.pushDefenceUp, 0, Mathf.Infinity);


        float AirPushDevider;
        if (hitInfo.carToHitcarComponents.vehicle.isGrounded)
            AirPushDevider = 1;
        else
            AirPushDevider = derbyDirectorConfig.maxAirPushDevider;
        Vector3 pushForce =  new Vector3(pushFront.x, pushUp, pushFront.z) / AirPushDevider;

        CalculatedHitInfo calculatedHitInfo = new CalculatedHitInfo(pushForce, resultDamage);
        if (carPushSpeed.z >= derbyDirectorConfig.stunSpeed && hitInfo.pushCollider.colliderType == ColliderType.Front) //����� ������� �������� ���������. ����� �� ��� ����� ������ ������� ����� �������
            calculatedHitInfo.strongHit = true;
        return calculatedHitInfo;
    }
}
