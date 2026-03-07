using UnityEngine;

public class Settings : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public GameObject sfxVolumeSlider;
    public float sfxVolume;
    public GameObject musicVolumeSlider;
    public float musicVolume;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        

    }

    public void SetSFXVolume()
    {

        sfxVolume = sfxVolumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);

    }
    public void SetMusicVolume()
    {
        musicVolume = musicVolumeSlider.GetComponent<UnityEngine.UI.Slider>().value;
        PlayerPrefs.SetFloat("MusicVolume", musicVolume);
    }
    
}
