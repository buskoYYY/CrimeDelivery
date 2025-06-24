using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    [SerializeField] private AudioClip _hitCarSound;
    [SerializeField] private AudioClip _hitObjectSound;

    private AudioManager _audioManager;

    private void Awake()
    {
        _audioManager = FindFirstObjectByType<AudioManager>();
    }

    public void PlayHitCarSound() => _audioManager.PlaySound(_hitCarSound);

    public void PlayHitObjectSound() => _audioManager.PlaySound(_hitObjectSound);
}