using System.Collections;
using UnityEngine;

public class MusicAudioManager : MonoBehaviour
{
    public static MusicAudioManager instance;
    [SerializeField] private AudioClip mainMusic;
    [SerializeField] private AudioSource audioSource;

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

    public void FadeInMainMusic(float fadeDuration = 1f)
    {
        StartCoroutine(FadeIn(audioSource, mainMusic, fadeDuration));
    }

    private IEnumerator FadeIn(AudioSource source, AudioClip newClip, float duration)
    {
        if (source.clip == newClip)
        {
            yield break; // Already playing the desired clip
        }

        if (source.isPlaying)
        {
            // Fade out current music
            float startVolume = source.volume;
            for (float t = 0; t < duration; t += Time.deltaTime)
            {
                source.volume = Mathf.Lerp(startVolume, 0f, t / duration);
                yield return null;
            }
            source.Stop();
        }

        // Switch to new clip and fade in
        source.clip = newClip;
        source.Play();
        for (float t = 0; t < duration; t += Time.deltaTime)
        {
            source.volume = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }
        source.volume = 1f; // Ensure volume is fully set at the end
    }
}
