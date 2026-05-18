using UnityEngine;

public class PowerGenerator : Interactable
{
    [SerializeField] private bool isActive = false;

    [Header("Visuals")]
    [SerializeField] private Transform switchPivot;
    public float switchRotationAngle = 175f;
    public float rotationSpeed = 3f;
    private float targetRotation = 0f;
    private float initialRotation = 0f;


    public bool IsActive => isActive;

    protected override void Start()
    {
        base.Start();
        if (switchPivot != null)
        {
            initialRotation = switchPivot.localEulerAngles.x;
        }
    }

    protected override void Update()
    {
        base.Update();
        if (switchPivot == null) return;

        switchPivot.localRotation = Quaternion.Lerp(switchPivot.localRotation, Quaternion.Euler(targetRotation, 0f, 0f), rotationSpeed * Time.deltaTime);
    }

    public void ActivateGenerator()
    {
        isActive = true;
        targetRotation = initialRotation + switchRotationAngle;
    }

    public void DeactivateGenerator()
    {
        isActive = false;
        targetRotation = initialRotation;
    }

    private void ToggleGenerator()
    {
        if (isActive)
        {
            DeactivateGenerator();
        }
        else
        {
            ActivateGenerator();
        }
    }

    public override void Interact()
    {
        // if (isActive) return;

        // ActivateGenerator();
        ToggleGenerator();
    }
}
