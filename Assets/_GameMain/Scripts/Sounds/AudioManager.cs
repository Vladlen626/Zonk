using System;
using System.Collections.Generic;
using FMODUnity;
using Mirror;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct SoundItem
{
   public string name;
   public EventReference eventReference;
}

public class AudioManager : NetworkBehaviour
{
   [SerializeField] private SoundItem[] sounds;
   
   public static AudioManager inst;
   public SoundItem[] Sounds => sounds;
   private Dictionary<string, EventReference> soundDictionary;
   
   private void Awake()
   {
      if (inst == null)
      {
         inst = this;
      }
   }
   
   [ClientRpc]
   private void RpcInitSoundDictionary()
   {
      soundDictionary = new Dictionary<string, EventReference>();
      foreach (var sound in sounds)
      {
         soundDictionary[sound.name] = sound.eventReference;
      }
   }

   private void Start()
   {
      soundDictionary = new Dictionary<string, EventReference>();
      foreach (var sound in sounds)
      {
         soundDictionary[sound.name] = sound.eventReference;
      }

      if (isServer)
      {
         RpcInitSoundDictionary();
      }
   }

   [ClientRpc]
   public void PlaySound(string soundName)
   {  
      if (soundDictionary.TryGetValue(soundName, out var eventRef))
      {
         RuntimeManager.PlayOneShot(eventRef);
      }
      else
      {
         Debug.LogWarning($"Sound with name {soundName} not found!");
      }
   }

   public void PlayMusic(EventReference musicEvent)
   {
      RuntimeManager.PlayOneShot(musicEvent);
   }
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioManager))]
public class AudioManagerEditor : Editor
{
   public override void OnInspectorGUI()
   {
      base.OnInspectorGUI();
        
      GUILayout.Space(10);
        
      if (GUILayout.Button("Generate Sound Names", GUILayout.Height(30)))
      {
         SoundNamesGenerator.Generate();
      }
   }
}
#endif


