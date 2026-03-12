using Unity.VisualScripting;
using UnityEngine;

public class SFXAudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip buttonClickClip;
    [SerializeField] private AudioClip coinFlipClip;
    [SerializeField] private AudioSource audioSource;

    public static SFXAudioManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayButtonClick()
    {
        PlayClip(buttonClickClip);
    }

    public void PlayCoinFlip()
    {
        PlayClip(coinFlipClip);
    }

    public void PlayClip(AudioClip clip, float volume = 1.0f)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }

        // Convert linear slider-like input to dB and back for predictable decibel behavior.
        float clampedLinear = Mathf.Clamp(volume, 0.0001f, 1.0f);
        float decibels = Mathf.Log10(clampedLinear) * 20.0f;
        float oneShotVolume = Mathf.Pow(10.0f, decibels / 20.0f);

        audioSource.PlayOneShot(clip, oneShotVolume);
    }
}
