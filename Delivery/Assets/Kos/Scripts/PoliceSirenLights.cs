using UnityEngine;

public class PoliceSirenLights : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light blueLight;
    [SerializeField] private Light redLight;
    [SerializeField] private float flashSpeed = 0.5f; // Частота миганий в секундах
    [SerializeField] private bool startOnAwake = true;

    private bool _isActive;
    private float _timer;
    private bool _lightsState;

    private void Awake()
    {
        // Отключаем оба света на старте
        if (blueLight != null) blueLight.enabled = false;
        if (redLight != null) redLight.enabled = false;
        
        _isActive = startOnAwake;
    }

    private void Update()
    {
        if (!_isActive) return;

        _timer += Time.deltaTime;
        
        if (_timer >= flashSpeed)
        {
            _timer = 0f;
            ToggleLights();
        }
    }

    private void ToggleLights()
    {
        _lightsState = !_lightsState;
        
        if (blueLight != null)
            blueLight.enabled = _lightsState;
        
        if (redLight != null)
            redLight.enabled = !_lightsState; // Противоположное состояние для красного
    }

    // Методы для управления из других скриптов
    public void ActivateSiren(bool activate)
    {
        _isActive = activate;
        
        // Выключаем оба света при деактивации
        if (!activate)
        {
            if (blueLight != null) blueLight.enabled = false;
            if (redLight != null) redLight.enabled = false;
        }
    }

    public void SetFlashSpeed(float newSpeed)
    {
        flashSpeed = Mathf.Clamp(newSpeed, 0.1f, 2f);
    }
}
