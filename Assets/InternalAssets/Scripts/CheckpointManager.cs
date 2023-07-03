using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> finishParticlesList = new List<GameObject>();
    [SerializeField] private AudioSource finishSound;
    [SerializeField] private List<CheckpointCollider> checkpointsList = new List<CheckpointCollider>();
    [SerializeField] private AudioSource checkpointSound;

    public static CheckpointManager Instance { get; private set; }

    private List<CheckpointCollider> passedCheckpointsList = new List<CheckpointCollider>();

    private void Awake()
    {
        Instance = this;
    }

    public void TryPassCheckpoint(CheckpointCollider checkpoint)
    {
        if (checkpoint == checkpointsList[passedCheckpointsList.Count])
        {
            passedCheckpointsList.Add(checkpoint);
            checkpointSound.Play();

            RaceStatsUI.Instance.UpdateRaceStats();
        }

        if (passedCheckpointsList.Count == checkpointsList.Count)
        {
            foreach (GameObject particleGameObject in finishParticlesList)
            {
                particleGameObject.SetActive(true);
            }
            finishSound.Play();

            passedCheckpointsList.Clear();
            RaceStatsUI.Instance.AddPassedLap();
        }
    }

    public int GetPassedCheckpointsCount()
    {
        return passedCheckpointsList.Count;
    }

    public int GetMaxCheckpointsCount()
    {
        return checkpointsList.Count;
    }
}
