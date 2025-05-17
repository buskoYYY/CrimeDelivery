using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroSystem : MonoBehaviour
{
    [Header("Настройки нитро")]
    [SerializeField] private float maxNitro = 100f;
    [SerializeField] private float consumptionRate = 20f;
    [SerializeField] private KeyCode nitroKey = KeyCode.LeftShift;

    [SerializeField] private enum NitroActivationMode
    {
        Hold,
        Toggle,
        BurnUntilEmpty
    }
    [SerializeField] private NitroActivationMode activationMode = NitroActivationMode.Hold;

    [SerializeField] private float currentNitro;
    private bool isNitroActive;



    #region Public API

    public float CurrentNitro => currentNitro;
    public float MaxNitro => maxNitro;
    public bool IsUsingNitro => isNitroActive && currentNitro > 0;

    public void AddNitro(float amount)
    {
        currentNitro = Mathf.Clamp(currentNitro + amount, 0f, maxNitro);
    }

    #endregion


    private void Update()
    {
        HandleInput();
    }

    private void FixedUpdate()
    {
        ApplyNitroConsumption();
    }

    private void HandleInput()
    {
        switch (activationMode)
        {
            case NitroActivationMode.Hold:
                isNitroActive = Input.GetKey(KeyCode.LeftShift);
                break;

            case NitroActivationMode.Toggle:
                if (Input.GetKeyDown(KeyCode.LeftShift))
                    isNitroActive = !isNitroActive;
                break;

            case NitroActivationMode.BurnUntilEmpty:
                if (Input.GetKeyDown(KeyCode.LeftShift) && currentNitro > 0)
                    isNitroActive = true;
                break;
        }


    }

    private void ApplyNitroConsumption()
    {
        if (IsUsingNitro)
        {
            currentNitro -= consumptionRate * Time.deltaTime;
            currentNitro = Mathf.Max(currentNitro, 0f);
        }

        // Auto-disable for BurnUntilEmpty
        if (activationMode == NitroActivationMode.BurnUntilEmpty && currentNitro <= 0)
        {
            isNitroActive = false;
        }
    }
}
