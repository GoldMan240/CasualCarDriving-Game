using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RaceStatsUI : MonoBehaviour
{
    public static RaceStatsUI Instance { get; private set; }

    [SerializeField] private TextMeshProUGUI currentCheckpointValueText;
    [SerializeField] private TextMeshProUGUI maxCheckpointsValueText;
    [SerializeField] private TextMeshProUGUI lapValueText;

    private int lapCount = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateRaceStats();
    }

    public void AddPassedLap()
    {
        lapCount++;
        UpdateRaceStats();
    }

    public void UpdateRaceStats()
    {
        currentCheckpointValueText.text = CheckpointManager.Instance.GetPassedCheckpointsCount().ToString();
        maxCheckpointsValueText.text = CheckpointManager.Instance.GetMaxCheckpointsCount().ToString();
        lapValueText.text = lapCount.ToString();
    }
}
