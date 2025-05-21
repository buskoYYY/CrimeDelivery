using UnityEngine;

public class CarEffects : CarComponent
{
    public MeshRenderer meshRenderer;
    public Material destroyMaterial;
    public override void CarDestroy()
    {
        // �������� ������� ������ ����������
        Material[] materials = meshRenderer.materials;

        // �������� �� ������� ���� �� ������ ���������
        if (materials.Length > 0)
        {
            materials[0] = destroyMaterial;
            meshRenderer.materials = materials; // ����������� ������ �������!
        }
    }
}