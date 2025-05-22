using System.Collections;
using UnityEngine;
public class CarConfig : MonoBehaviour
{
    public string id;
    public CarSettings carSettings;
}

[System.Serializable]
public class CarSettings
{
    public DriftControllerSettings driftControllerSettings;
    public string carNarrativeName;
    public bool isDefault = false; //если true, то машина открыта сразу и её не нужно покупать
    public int carPrice = 100;
}

[System.Serializable]
public class CarData
{
    public string id;
    public int openingProcent; // если 100, то машина открыта
    public CarSettings carSettingsData;
    public int engineLevel;
    public int capacityLevel;
    public int healthLevel;
}