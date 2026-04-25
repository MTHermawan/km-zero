using UnityEngine;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private GameObject crosshair;

    public void EnableCrosshair() => crosshair?.SetActive(true);
    public void DisableCrosshair() => crosshair?.SetActive(false);

    private static PlayerUIController s_instance;
    public static PlayerUIController Instance
    {
        get
        {
            if (s_instance == null)
            {
                s_instance = FindFirstObjectByType<PlayerUIController>();
            }
            return s_instance;
        }
    } 
}
