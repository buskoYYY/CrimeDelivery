using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using ArcadeBridge;
using System;

public class Indicator : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Image barLight;
    [SerializeField] private Image clickableImage; // Ссылка на изображение, по которому будем кликать
    public float recoveryLife = 100;
    public float rechargeRate;
    bool isActive;
    [SerializeField] private BoostJump boostJump;
    [SerializeField] private BoostSpawner boostSpawner;

    private void Start()
    {
        recoveryLife = 100;

        // Добавляем обработчик клика, если изображение не назначено в инспекторе
        if (clickableImage == null)
        {
            clickableImage = GetComponent<Image>();
        }

        // Делаем изображение взаимодействующим
        if (clickableImage != null)
        {
            clickableImage.raycastTarget = true;
        }

    }

    void Update()
    {
        if (isActive)
        {
            RechargeBattery();
        }

        UpdateIndicator();
    }

    // Вызывается при нажатии на изображение
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isActive)
        {
            DrainBattery();
        }
    }

    public void DrainBattery()
    {
        if (recoveryLife == 100)
        {
            recoveryLife = 0;
            isActive = true;
        }

        if (boostJump != null)
        {
            boostJump.Jump();
        }

        if (boostSpawner != null)
        {
            boostSpawner.SpawnWeapon();
        }
    }

    private void RechargeBattery()
    {
        if (isActive && recoveryLife < 100)
        {
            recoveryLife += rechargeRate * Time.deltaTime;
            if (recoveryLife > 100)
            {
                StartCoroutine(ScaleAnimation());
                isActive = false;
                recoveryLife = 100;
            }
        }
    }

    private void UpdateIndicator()
    {
        if (barLight != null)
        {
            barLight.fillAmount = recoveryLife / 100f;
        }
    }

    private IEnumerator ScaleAnimation()
    {
        if (barLight == null) yield break;

        Vector3 originalScale = barLight.transform.localScale;
        Vector3 targetScale = originalScale * 1.1f;

        float time = 0f;
        while (time < 0.2f)
        {
            time += Time.deltaTime;
            barLight.transform.localScale = Vector3.Lerp(originalScale, targetScale, time / 0.1f);
            yield return null;
        }

        time = 0f;
        while (time < 0.4f)
        {
            time += Time.deltaTime;
            barLight.transform.localScale = Vector3.Lerp(targetScale, originalScale, time / 0.2f);
            yield return null;
        }

        barLight.transform.localScale = originalScale;
    }
}
