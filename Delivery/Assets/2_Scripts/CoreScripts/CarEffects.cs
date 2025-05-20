using UnityEngine;

public class CarEffects : CarComponent
{
    public MeshRenderer meshRenderer;
    public Material destroyMaterial;
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
    }
}