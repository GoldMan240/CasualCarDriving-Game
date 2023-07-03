using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class CheckpointCollider : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        CheckpointManager.Instance.TryPassCheckpoint(this);
    }

}
