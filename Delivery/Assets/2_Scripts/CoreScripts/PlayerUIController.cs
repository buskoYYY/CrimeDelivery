using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerUIController: MonoBehaviour
{
    public Driver player;

    public EventTrigger turnLeft;
    public EventTrigger turnRight;

    public void Start()
    {
        player.Throttle(1);
        AddPointerEnterEvent(turnLeft, -1);
        AddPointerEnterEvent(turnRight, 1);
    }

    public void Turn(int dir)
    {
        player.Turn(dir);
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