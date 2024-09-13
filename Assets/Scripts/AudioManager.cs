using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [Header("Audio References")]
    public Slider volumeSlider;
    public Slider soundEffectslider;

    public AudioSource volumeSource;
    public AudioSource soundEffectsSource;

    private void Start()
    {
        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        soundEffectslider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        volumeSource.volume = volumeSlider.value;
        soundEffectsSource.volume = soundEffectslider.value;
    }

    public void PlaySoundEffect(AudioClip clip)
    {
        soundEffectsSource.PlayOneShot(clip);
    }

    public void SoundEffectVolumeChange()
    {
        soundEffectsSource.volume = soundEffectslider.value;
        PlayerPrefs.SetFloat("SFXVolume", soundEffectslider.value);
        PlayerPrefs.Save();
    }

    public void GameMusicChange()
    {
        volumeSource.volume = volumeSlider.value;

        PlayerPrefs.SetFloat("MusicVolume", volumeSlider.value);
        PlayerPrefs.Save();
    }
}
