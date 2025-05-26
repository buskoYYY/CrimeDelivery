using UnityEngine;

public class CarEffects : CarComponent
{
    public MeshRenderer meshRenderer;
    public Material destroyMaterial;

    public GameObject expliosion;

    public Vector3 explosionForce;
    public Vector3 explosionTorque;

    [SerializeField] private ParticleSystem smokeParticles;
    [SerializeField] private ParticleSystem fireParticles;
    public override void CarDestroy()
    {
        // ѕолучаем текущий массив материалов
        Material[] materials = meshRenderer.materials;

        // ѕроверка на наличие хот€ бы одного материала
        if (materials.Length > 0)
        {
            materials[0] = destroyMaterial;
            meshRenderer.materials = materials; // ѕрисваиваем массив обратно!
        }

        GameObject explosionInstance = Instantiate(expliosion, transform.position, transform.rotation);
        carComponents.carRigidbody.AddForceAtPosition(transform.position, explosionForce, ForceMode.Acceleration);
        carComponents.carRigidbody.AddTorque(explosionTorque, ForceMode.Acceleration);

        Destroy(explosionInstance, 2);
    }

    public override void UpdateHealth(float carHeath, float maxHealth)
    {
        base.UpdateHealth(carHeath, maxHealth);
        if (smokeParticles != null && fireParticles != null)
        {
            if (carComponents.carDamageHandler.CurrentHealth <= carComponents.carDamageHandler.MaxHealth / 2)
            {
                smokeParticles.Play();
                fireParticles.Stop();
            }
            else if (carComponents.carDamageHandler.CurrentHealth <= carComponents.carDamageHandler.MaxHealth / 4)
            {
                smokeParticles.Stop();
                fireParticles.Play();
            }
            else
            {
                smokeParticles.Stop();
                fireParticles.Stop();
            }
        }

    }
}