using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio References")]
    public Slider volumeSlider;
    public Slider soundEffectslider;

    public AudioSource volumeSource;
    public AudioSource soundEffectsSource;

    public void PlaySoundEffect(AudioClip clip)
    {
        soundEffectsSource.PlayOneShot(clip);
    }

    public void SoundEffectVolumeChange()
    {
        soundEffectsSource.volume = soundEffectslider.value;
    }

    public void GameMusicChange()
    {
        volumeSource.volume = volumeSlider.value;
    }
}
