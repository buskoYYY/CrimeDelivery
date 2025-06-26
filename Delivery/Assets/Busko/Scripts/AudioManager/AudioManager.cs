using UnityEngine;

    public class AudioManager : MonoBehaviour
    {
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource _soundSource;
    [SerializeField] private AudioClip _defaultMusic;
    [SerializeField] private float _sqrMaxDistanceToSource = 100f;

    private Transform _listenerTransform;

    public bool CanBeHeard(Vector3 sourcePosition) => (sourcePosition - _listenerTransform.position).sqrMagnitude < _sqrMaxDistanceToSource;

    private void Awake()
    {
        PlayMusic(_defaultMusic);
        _musicSource.loop = true;

        _soundSource.playOnAwake = false;
        _soundSource.loop = false;
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.Stop();
        _musicSource.clip = clip;
        _musicSource.Play();
    }

    public void PlaySound(AudioClip clip)
    {
        _soundSource.PlayOneShot(clip);
    }
}
