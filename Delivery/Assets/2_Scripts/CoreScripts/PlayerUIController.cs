using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerUIController: MonoBehaviour
{
    public CarComponentsController player;
    private Driver playerDriver;

    public EventTrigger turnLeft;
    public EventTrigger turnRight;

    public bool initAtStart;
    public void Initialize(CarComponentsController player)
    {
        foreach (CarComponent carComponent in player.carComponents)
        {
            if (carComponent is Driver)
                playerDriver = carComponent as Driver;
        }

        AddPointerEnterEvent(turnLeft, -1);
        AddPointerEnterEvent(turnRight, 1);
    }

    public void Start()
    {
        if (initAtStart)
            Initialize(player);
    }

    public void Turn(int dir)
    {
        playerDriver.Turn(dir);
    }

    private void AddPointerEnterEvent(EventTrigger trigger, int dir)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };

        // Используем лямбду, чтобы передать аргумент dir
        entry.callback.AddListener((eventData) => Turn(dir));

        trigger.triggers.Add(entry);

        // PointerExit — при уходе курсора передаём 0
        EventTrigger.Entry exit = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerExit
        };
        exit.callback.AddListener((eventData) => Turn(0));
        
        trigger.triggers.Add(exit);
    }
}