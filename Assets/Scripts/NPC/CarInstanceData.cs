using UnityEngine;

public class CarInstanceData
{
    public NPCData sourceData;   // data asli dari database
    public bool isAnomaly;

    // Field yang ditampilkan — bisa teracak jika anomali
    public string displayName;
    public Sprite displayPortrait;
    public string displayCode;
    public Gender displayGender;
    public string displayLicensePlate;

    // Warna mobil (selalu acak)
    public Color carColor;

    public static CarInstanceData Create(NPCData source, bool isAnomaly, NPCDatabase database)
    {
        CarInstanceData d = new()
        {
            sourceData = source,
            isAnomaly = isAnomaly,
            carColor = Random.ColorHSV(0f, 1f, 0.4f, 0.9f, 0.6f, 1f),
        };

        if (!isAnomaly)
        {
            // Data sesuai dengan data set
            d.displayName = source.fullName;
            d.displayPortrait = source.portrait;
            d.displayCode = source.code;
            d.displayGender = source.gender;
            d.displayLicensePlate = source.licensePlate;
        }
        else
        {
            // Acak salah satu atau beberapa field dari NPC lain
            d.displayName = source.fullName;
            d.displayPortrait = source.portrait;
            d.displayCode = source.code;
            d.displayGender = source.gender;
            d.displayLicensePlate = source.licensePlate;
            RandomizeSomeFields(d, source, database);
        }

        return d;
    }

    private static void RandomizeSomeFields(CarInstanceData d, NPCData source, NPCDatabase db)
    {
        // Pilih NPC lain sebagai "donor" data palsu
        var others = db.npcs.FindAll(n => n != source);
        if (others.Count == 0) return;

        NPCData donor = others[Random.Range(0, others.Count)];

        // Acak 1-2 field secara random agar tidak selalu sama polanya
        int anomalyCount = Random.Range(1, 3);
        var fields = new System.Collections.Generic.List<int> { 0, 1, 2, 3, 4 };
        Shuffle(fields);

        for (int i = 0; i < anomalyCount; i++)
        {
            switch (fields[i])
            {
                case 0: d.displayName = donor.fullName; break;
                case 1: d.displayPortrait = donor.portrait; break;
                case 2: d.displayCode = donor.code; break;
                case 3: d.displayGender = donor.gender; break;
                case 4: d.displayLicensePlate = donor.licensePlate; break;
            }
        }
    }

    private static void Shuffle(System.Collections.Generic.List<int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}