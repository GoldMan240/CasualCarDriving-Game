using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem particle;

    private void Update()
    {
        if (!particle.IsAlive())
        {
            gameObject.SetActive(false);
        }
    }
}
