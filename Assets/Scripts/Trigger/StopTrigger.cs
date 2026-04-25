using System.Collections.Generic;
using UnityEngine;

public class StopTrigger : MonoBehaviour
{
    private List<Collider> _enteredCollider = new();

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.gameObject.TryGetComponent(out NPC car))
            {
                car.DisableMove();
            }
        }
        if (!_enteredCollider.Contains(other) && other != null) _enteredCollider.Add(other);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Car"))
        {
            if (other.gameObject.TryGetComponent(out NPC car))
            {
                car.EnableMove();
            }
        }
    }

    void OnDisable()
    {
        foreach (Collider col in _enteredCollider)
        {
            if (col == null) continue;
            
            OnTriggerExit(col);
        }
        _enteredCollider.Clear();
    }
}
