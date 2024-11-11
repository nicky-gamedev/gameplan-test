using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ButtonReferenceContainer : MonoBehaviour
{
    [Serializable]
    public struct ButtonActionPair
    {
        public Button button;
        public string name;
    }

    public List<ButtonActionPair> actions;

    Dictionary<string, Button> buttonActionPair;

    void Awake()
    {
        buttonActionPair = actions.ToDictionary(x => x.name, x => x.button);
    }

    public void SubscribeToEvent(string eventName, UnityAction callback) 
    {
        buttonActionPair[eventName].onClick.AddListener(callback);
    }

    public void UnsubscribeToEvent(string eventName, UnityAction callback)
    {
        buttonActionPair[eventName].onClick.RemoveListener(callback);
    }

    public void CleanButton(string eventName)
    {
        buttonActionPair[eventName].onClick.RemoveAllListeners();
    }
}
