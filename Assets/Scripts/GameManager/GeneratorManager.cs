using System.Collections.Generic;
using UnityEngine;

public class GeneratorManager : MonoBehaviour
{
    [SerializeField] private List<PowerGenerator> generators = new();
    [SerializeField] private Light[] lightsToControl = new Light[0];

    private float blackoutTimer = 30f;

    void Start()
    {
        blackoutTimer = GetBlackoutDuration();
    }

    void Update()
    {
        BlackoutLogic();
    }

    public void ResetGenerators()
    {
        foreach (var generator in generators)
        {
            generator.DeactivateGenerator();
        }
    }

    public bool AreAllGeneratorsActive()
    {
        foreach (var generator in generators)
        {
            if (!generator.IsActive) return false;
        }
        return true;
    }

    public float GetBlackoutDuration()
    {
        return Random.Range(60f, 80f);
    }

    public void BlackoutLogic()
    {

        bool allActive = AreAllGeneratorsActive();
        foreach (var light in lightsToControl)
        {
            light.enabled = allActive;
        }

        if (allActive) return;
        blackoutTimer -= Time.deltaTime;
        if (blackoutTimer <= 0f)
        {
            ResetGenerators();
            blackoutTimer = GetBlackoutDuration();
        }
    }
}
