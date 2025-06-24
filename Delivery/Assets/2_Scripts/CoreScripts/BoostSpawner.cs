using UnityEngine;
public class BoostSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] boostPrefabs;
    [SerializeField] private Transform spawnPosition;
    void Start()
    {
        Instantiate(boostPrefabs[Random.Range(0, boostPrefabs.Length)], spawnPosition.position, spawnPosition.rotation, spawnPosition); //
    }
}
