using System;
using DG.Tweening;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

public class NetworkParticleController : NetworkBehaviour
{
    [SerializeField] private ParticleSystem particleEffect; 

    public void StartRotationAndScale()
    {
        CmdPlayParticleEffect();
    }
    
    public void StopEffects()
    {
        CmdStopParticleEffect();
    }

    [Command(requiresAuthority = false)]
    private void CmdPlayParticleEffect()
    {
        RpcPlayParticleEffect();
    }

    [ClientRpc]
    private void RpcPlayParticleEffect()
    {
        if (particleEffect != null)
        {
            particleEffect.Play();
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdStopParticleEffect()
    {
        RpcStopParticleEffect();
    }

    [ClientRpc]
    private void RpcStopParticleEffect()
    {
        if (particleEffect != null)
        {
            particleEffect.Stop();
        }
    }
 
}
