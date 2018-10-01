using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunParticle : MonoBehaviour {

    ParticleSystem ps;

    List<ParticleSystem.Particle> enter = new List<ParticleSystem.Particle>();

    private void OnEnable()
    {
        ps = GetComponent<ParticleSystem>();
    }

    private void OnParticleTrigger()
    {

        //Gets the particles which enter the trigger this frame
        int numEnter = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);

        //Iterate through the particles which enter the trigger & deletes them
        for (int i = 0; i < numEnter; i ++)
        {
            ParticleSystem.Particle p = enter[i];
            p.startColor = new Color(0, 0, 0, 0);
            enter[i] = p;
        }

        //Re-assign the modified particles back into the particle system
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Enter, enter);
    }
}
