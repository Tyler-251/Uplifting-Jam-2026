using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject sfxVolumeSlider;
    public float sfxVolume;
    public GameObject musicVolumeSlider;
    public float musicVolume;
    [SerializeField] private AudioMixer audioMixer;
    void Start()
    {
        
    }

    void OnEnable()
    {
        sfxVolume = PlayerPrefs.GetFloat("sfxVolume", 1f);
        musicVolume = PlayerPrefs.GetFloat("musicVolume", 1f);
        sfxVolumeSlider.GetComponent<UnityEngine.UI.Slider>().value = sfxVolume;
        musicVolumeSlider.GetComponent<UnityEngine.UI.Slider>().value = musicVolume;
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(sfxVolume) * 20);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
    }


    public void SetSFXVolume()
    {

        sfxVolume = sfxVolumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        audioMixer.SetFloat("sfxVolume", Mathf.Log10(sfxVolume) * 20);

    }
    public void SetMusicVolume()
    {
        musicVolume = musicVolumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        audioMixer.SetFloat("musicVolume", Mathf.Log10(musicVolume) * 20);
    }
    
}
