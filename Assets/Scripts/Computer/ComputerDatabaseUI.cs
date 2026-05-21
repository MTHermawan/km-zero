using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ComputerDatabaseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private NPCDatabase database;
    [SerializeField] private TMP_InputField searchInput;
    [SerializeField] private Button searchButton;
    [SerializeField] private Transform listContainer;
    [SerializeField] private GameObject npcListItemPrefab;

    [Header("Detail Panel")]
    [SerializeField] private GameObject detailPanel;
    [SerializeField] private Image detailPortrait;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailCode;
    [SerializeField] private TMP_Text detailGender;
    [SerializeField] private TMP_Text detailPlate;
    [SerializeField] private TMP_Text validationResultText;

    private NPCData _selectedNPC;

    void Awake()
    {
        searchButton.onClick.AddListener(() => OnSearch(searchInput.text));
    }

    void Start()
    {
        RefreshList(database.npcs);

        detailPanel.SetActive(false);
        listContainer.gameObject.SetActive(true);
    }

    void OnEnable()
    {
        RefreshList(database.npcs);

        detailPanel.SetActive(false);
        listContainer.gameObject.SetActive(true);

        _selectedNPC = null;
    }

    public void ShowList()
    {
        detailPanel.SetActive(false);
        listContainer.gameObject.SetActive(true);
    }

    private void OnSearch(string query)
    {
        // Hide detail ketika search
        ShowList();

        _selectedNPC = null;
        validationResultText.SetText("");

        List<NPCData> results =
            string.IsNullOrEmpty(query)
            ? database.npcs
            : database.SearchByName(query);

        RefreshList(results);
    }

    private void RefreshList(List<NPCData> npcs)
    {
        foreach (Transform child in listContainer)
            Destroy(child.gameObject);

        foreach (NPCData npc in npcs)
        {
            GameObject item =
                Instantiate(
                    npcListItemPrefab,
                    listContainer);

            item.GetComponentInChildren<TMP_Text>()
                .SetText(npc.fullName);

            item.GetComponent<Button>()
                .onClick.AddListener(() => SelectNPC(npc));
        }
    }

    private void SelectNPC(NPCData npc)
    {
        _selectedNPC = npc;

        detailPortrait.sprite = npc.portrait;

        detailName.SetText(npc.fullName);
        detailCode.SetText(npc.code);
        detailGender.SetText(npc.gender.ToString());
        detailPlate.SetText(npc.licensePlate);

        // Show detail → hide list
        listContainer.gameObject.SetActive(false);
        detailPanel.SetActive(true);

        validationResultText.SetText("");
    }

    public void OnValidateButton()
    {
        if (_selectedNPC == null)
        {
            validationResultText.SetText(
                "Pilih data NPC terlebih dahulu.");

            return;
        }

        ValidationResult result =
            CheckpointManager.Instance.Validate(_selectedNPC);

        validationResultText.SetText(
            result switch
            {
                ValidationResult.Valid
                    => "Data sesuai",

                ValidationResult.Mismatch
                    => "Data tidak cocok — kemungkinan anomali",

                ValidationResult.NoCar
                    => "Tidak ada kendaraan di pos",

                _ => ""
            });
    }
}