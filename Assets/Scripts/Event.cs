using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Subject : MonoBehaviour {
    private List<IObserver> observers = new List<IObserver>();

    public void AddObserver(IObserver observer) {
        observers.Add(observer);
    }

    public void RemoveObserver(IObserver observer) {
        observers.Remove(observer);
    }

    protected void NotifyObservers(EventData data) {
        foreach(IObserver observer in observers) observer.OnNotify(data);
    }
}

public interface IObserver {
    public void OnNotify(EventData data);
}

public class EventData {
    string name;
    string content;

    public EventData(string name, string content) {
        this.name = name;
        this.content = content;
    }

    public static EventData PlayerStart() {
        return new EventData("PlayerStart", "Nothing");
    }
}
