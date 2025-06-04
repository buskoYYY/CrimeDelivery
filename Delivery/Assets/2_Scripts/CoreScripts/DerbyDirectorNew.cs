using UnityEngine;

public class CalculatedHitInfoNew
{
    public float damageToSelf;

    public CalculatedHitInfoNew(float damageToSelf)
    {
        this.damageToSelf = damageToSelf;
    }
}

public static class DerbyDirectorNew
{
    public static float damageMultiplier = 5f;
    public static float minImpactSpeed = 1f;

    public static CalculatedHitInfoNew CalculateHitDamage(HitInfo hitInfo)
    {
        Rigidbody thisRigidbody = hitInfo.pushCarComponents.carRigidbody;
        Transform thisTrasform = hitInfo.pushCarComponents.carTrasform;

        Rigidbody otherRigidbody = hitInfo.carToHitcarComponents.carRigidbody;
        Transform otherTrasform = hitInfo.carToHitcarComponents.carTrasform;

        // Направление столкновения: от другого объекта к нашему
        Vector3 collisionNormal = (thisTrasform.position - otherTrasform.position).normalized;

        // Относительная скорость
        Vector3 relativeVelocity = thisRigidbody.linearVelocity - otherRigidbody.linearVelocity;

        // Величина скорости вдоль нормали столкновения
        float impactSpeed = Vector3.Dot(relativeVelocity, collisionNormal);


        if (hitInfo.pushCarComponents.isPlayer)
        {
            //Debug.Log($"Me --- {(int)otherDamage}");
            TestHit2(hitInfo, "Me");
        }

        if (hitInfo.carToHitcarComponents.isPlayer)
        {
            // Debug.Log($"Other --- {(int)otherDamage}");
            TestHit2(hitInfo, "Other");
        }

        // Учитываем только когда объекты сближаются
        if (impactSpeed < minImpactSpeed)
        {
            return new CalculatedHitInfoNew(0);
        }

        // Направления движения машин
        Vector3 myDir = thisRigidbody.linearVelocity.normalized;
        Vector3 otherDir = otherRigidbody.linearVelocity.normalized;

        // Углы столкновения (меньше угол → больше урон)
        float myImpactFactor = 1;// Mathf.Clamp01(1 - Vector3.Angle(myDir, -collisionNormal) / 45f);
        float otherImpactFactor = 1; // Mathf.Clamp01(1 - Vector3.Angle(otherDir, collisionNormal) / 45f);

        // Относительные скорости вдоль нормали
        float myImpactVelocity = Vector3.Dot(thisRigidbody.linearVelocity, collisionNormal);
        float otherImpactVelocity = -Vector3.Dot(otherRigidbody.linearVelocity, collisionNormal);

        // Вклад каждого в столкновение (с положительным вкладом)
        float myContribution = Mathf.Max(0, myImpactVelocity);
        float otherContribution = Mathf.Max(0, otherImpactVelocity);

        float totalContribution = myContribution + otherContribution;
        if (totalContribution < 0.01f) totalContribution = 0.01f;

        // Чем ты медленнее в момент удара — тем больше урона получаешь
        float myDamageShare = otherContribution / totalContribution;
        float otherDamageShare = myContribution / totalContribution;

        float baseDamage = impactSpeed * damageMultiplier;

        float myDamage = baseDamage * myDamageShare;
        float otherDamage = baseDamage * otherDamageShare;




        return new CalculatedHitInfoNew(otherDamage);
    }

    public static void TestHit(HitInfo hitInfo, string car)
    {
        Rigidbody thisRigidbody = hitInfo.pushCarComponents.carRigidbody;
        Transform thisTrasform = hitInfo.pushCarComponents.carTrasform;

        Rigidbody otherRigidbody = hitInfo.carToHitcarComponents.carRigidbody;
        Transform otherTrasform = hitInfo.carToHitcarComponents.carTrasform;


        ContactPoint contact = hitInfo.collision.contacts[0];
        Vector3 collisionDirection = (contact.point - thisTrasform.position).normalized;

        Vector3 flatDirection = new Vector3(thisRigidbody.linearVelocity.x, 0, thisRigidbody.linearVelocity.z).normalized;
        Vector3 flatCollisionDirection = new Vector3(collisionDirection.x, 0, collisionDirection.z).normalized;

        float dot = Vector3.Dot(flatDirection, flatCollisionDirection);
        //float dot = Vector3.Dot(thisRigidbody.linearVelocity.normalized, collisionDirection);

        
        if (dot > 0.7f)
        {
            // Лобовой удар — мы врезались во что-то
            Debug.Log($"{car}: Наношу урон");
            // Тут можно вызвать у объекта, в который врезались: collision.gameObject.GetComponent<Car>()?.ApplyDamage(...)
        }
        else if (dot < -0.7f)
        {
            // Получили удар в зад
            Debug.Log($"{car}: удар в зад");
        }
        else
        {
            // Получили удар в бок
            Debug.Log($"{car}: удар в бок");
        }
    }
    public static void TestHit2(HitInfo hitInfo, string car)
    {
        Vector3 impulse = hitInfo.collision.impulse;

        // Вектор в противоположную сторону действует на другой объект
        Vector3 impulseThis = impulse;
        Vector3 impulseOther = -impulse;

        float magnitudeThis = impulseThis.magnitude;
        float magnitudeOther = impulseOther.magnitude;
        //Debug.Log($"{car} --- {magnitudeThis} ");
    }
}
