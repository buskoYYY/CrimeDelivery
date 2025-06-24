using UnityEngine;
using System;

namespace ArcadeBridge
{
    public class PoliceSounds : MonoBehaviour
    {
        [SerializeField] private AudioSource _source;
        [SerializeField] private AudioSource _explosionSourse;
        [SerializeField] private AudioClip _flashingLightsSound;
        [SerializeField] private AudioClip _explosianSound;
        [SerializeField] private float _maxDistanceToHear = 100f;
        [SerializeField] private float _maxDistanceToExplosian = 100f;

        private Player _player;

        private void Awake()
        {
            _player = FindFirstObjectByType<Player>();
        }

        private void Update()
        {
            PlayFlashLightSound();
        }

        public void PlayExplosianSound()
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (distance < _maxDistanceToHear)
            {
                    _explosionSourse.PlayOneShot(_explosianSound);               
            }
        }

        private void PlayFlashLightSound()
        {
            float distance = Vector3.Distance(transform.position, _player.transform.position);

            if (distance < _maxDistanceToHear)
            {
                if (!_source.isPlaying)
                {
                    _source.PlayOneShot(_flashingLightsSound);
                }
            }
            else
            {
                if (_source.isPlaying)
                {
                    _source.Stop(); 
                }
            }
        }
    }
}
