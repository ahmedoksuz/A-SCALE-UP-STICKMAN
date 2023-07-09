using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(menuName = "GP Hive Objects/Audio Events/Random")]
public class RandomAudioEvent : AudioEvent
{
    public AudioClip[] Clips;

    public RangedFloat Volume;

    [MinMaxRange(0, 2)] public RangedFloat Pitch;


    public override void Play(AudioSource source)
    {
        if (Clips.Length == 0) return;

        source.clip = Clips[Random.Range(0, Clips.Length - 1)];
        source.volume = Random.Range(Volume.minValue, Volume.maxValue);
        source.pitch = Random.Range(Pitch.minValue, Pitch.maxValue);

        source.Play();
    }
}