using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckLoader : MonoBehaviour
{
    [Header("Параметры укладки")]
    public Transform trunkOrigin; // точка в багажнике, откуда начинается укладка
    public int gridWidth = 3;
    public int gridDepth = 2;
    public float cellSize = 0.5f;
    public int maxLayers = 2;

    [Header("Анимация")]
    public float throwDuration = 1f;
    public AnimationCurve arcCurve;
    public float delayBetweenThrows = 0.2f;

    private bool hasLoaded = false;
    [SerializeField] private int loadedCount = 0;

    private bool hasUnloaded = false;

    public List<GameObject> loadedObjects = new List<GameObject>();


    private bool inWork;
    public void LoadObjects(List<GameObject> objectsToLoad)
    {
        if (hasLoaded) return;
        hasLoaded = true;
        StartCoroutine(LoadSequence(objectsToLoad, trunkOrigin, true));
    }

    public void UnloadObjects(Transform targetUnLoad, int unloadAmount)
    {
        if (hasUnloaded && loadedObjects.Count > 0) return;
        hasUnloaded = true;
        //List<GameObject> objectsToUnload = loadedObjects / unloadAmount;

        //loadedCount = 0;
        StartCoroutine(LoadSequence(loadedObjects, targetUnLoad, false));
    }

    private IEnumerator LoadSequence(List<GameObject> objectsToLoad, Transform targetLoad, bool isLoad)
    {
        foreach (var obj in objectsToLoad)
        {
            if (loadedCount >= gridWidth * gridDepth * maxLayers)
                break;

            Vector3 localOffset = GetStackLocalOffset(loadedCount);
            StartCoroutine(ThrowObject(obj, localOffset, targetLoad, isLoad));

            if (isLoad)
                loadedCount++;
            else
                loadedCount--;

            yield return new WaitForSeconds(delayBetweenThrows);
        }
        //loadedCount = 0;
        hasLoaded = false;
        hasUnloaded = false;
    }

    private Vector3 GetStackLocalOffset(int index)
    {
        int layer = index / (gridWidth * gridDepth);
        int rem = index % (gridWidth * gridDepth);
        int x = rem % gridWidth;
        int z = rem / gridWidth;

        return new Vector3(x * cellSize, layer * cellSize, z * cellSize);
    }

    private IEnumerator ThrowObject(GameObject obj, Vector3 localOffset, Transform targetTrasform, bool isLoad)
    {
        obj.transform.SetParent(null);
        //Collider col = obj.GetComponent<Collider>();
        //Rigidbody rb = obj.GetComponent<Rigidbody>();
        //if (col) col.enabled = false;
        //if (rb) rb.isKinematic = true;

        Vector3 startPos = obj.transform.position;
        float elapsed = 0f;

        while (elapsed < throwDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / throwDuration;
            float height = arcCurve.Evaluate(t);

            Vector3 targetPos = targetTrasform.TransformPoint(localOffset);
            Vector3 midPos = Vector3.Lerp(startPos, targetPos, t);
            midPos.y += height;

            obj.transform.position = midPos;
            yield return null;
        }

        obj.transform.position = targetTrasform.TransformPoint(localOffset);
        obj.transform.rotation = targetTrasform.rotation;
        obj.transform.SetParent(targetTrasform);
        if (isLoad)
            loadedObjects.Add(obj);
        //if (col) col.enabled = true;
        //if (rb) rb.isKinematic = false;
    }
}
