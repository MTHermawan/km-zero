using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Lobotomization : MonoBehaviour
{
    public List<UnityEvent> lobotomizedMethods = new();
    [Button(nameof(Lobotomize), CustomName = "Lobotomize!")]
    [SerializeField] private bool _lobotomizeButton;

    void Lobotomize()
    {
        if (lobotomizedMethods == null) return;

        foreach (UnityEvent lm in lobotomizedMethods)
        {
            if (lm == null) continue;
            lm.Invoke();
        }
    }
}
