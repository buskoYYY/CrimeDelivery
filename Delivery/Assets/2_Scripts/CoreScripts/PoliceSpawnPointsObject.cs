using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PoliceSpawnPointsRay
{
    public Transform spawnPosition;
    public Transform startPosition;
}

public class PoliceSpawnPointsObject : MonoBehaviour
{
    public List<PoliceSpawnPointsRay> spawnRays = new List<PoliceSpawnPointsRay>();

    public List<Transform> policeTargets = new List<Transform>();
}
