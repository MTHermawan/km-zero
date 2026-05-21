using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    private CarController _currentCar;

    public CarController CurrentCar => _currentCar;
    public IDCardController IDCard;

    private static CheckpointManager s_instance;
    public static CheckpointManager Instance
    {
        get
        {
            if (s_instance == null) s_instance = FindFirstObjectByType<CheckpointManager>();
            return s_instance;
        }
    }

    public void OnCarArrived(CarController car)
    {
        _currentCar = car;
        IDCard?.gameObject.SetActive(true);
        IDCard?.ShowCard(car.InstanceData);
    }

    public void OnCarLeft()
    {
        _currentCar = null;
        IDCard?.gameObject.SetActive(false);
        IDCard?.HideCard();
    }

    // Dipanggil dari komputer untuk validasi
    public ValidationResult Validate(NPCData selectedFromComputer)
    {
        if (_currentCar == null) return ValidationResult.NoCar;

        CarInstanceData id = _currentCar.InstanceData;

        // Cocokkan data ID card yang ditampilkan dengan data asli NPC yang dipilih player
        bool nameMatch        = id.displayName         == selectedFromComputer.fullName;
        bool codeMatch        = id.displayCode         == selectedFromComputer.code;
        bool genderMatch      = id.displayGender       == selectedFromComputer.gender;
        bool plateMatch       = id.displayLicensePlate == selectedFromComputer.licensePlate;
        bool portraitMatch    = id.displayPortrait     == selectedFromComputer.portrait;

        bool allMatch = nameMatch && codeMatch && genderMatch && plateMatch && portraitMatch;

        return allMatch ? ValidationResult.Valid : ValidationResult.Mismatch;
    }
}

public enum ValidationResult { NoCar, Valid, Mismatch }