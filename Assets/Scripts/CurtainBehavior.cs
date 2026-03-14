using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CurtainBehavior : MonoBehaviour
{
    [SerializeField] private GameObject curtain;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text subTitle;
    [SerializeField] private AudioClip bellToll;
    private Coroutine curtainRoutine;

    public void OpenCurtain(string curtainTitle, string subTitle, MessageSO messageToPlayAfterCurtainFade)
    {
        if (curtainRoutine != null)
        {
            StopCoroutine(curtainRoutine);
        }

        curtain.SetActive(true);
        SFXAudioManager.instance.PlayClip(bellToll, .75f);
        title.text = curtainTitle;
        this.subTitle.text = subTitle;
        SetCurtainAlpha(1f);
        curtainRoutine = StartCoroutine(OpenCurtainCoroutine(messageToPlayAfterCurtainFade));
    }

    public void CloseCurtain(UnityAction onCurtainClosed = null)
    {
        if (curtainRoutine != null)
        {
            StopCoroutine(curtainRoutine);
        }

        curtain.SetActive(true);
        curtainRoutine = StartCoroutine(CloseCurtainCoroutine(onCurtainClosed));
    }

    private IEnumerator OpenCurtainCoroutine(MessageSO messageToPlayAfterCurtainFade)
    {
        yield return new WaitForSeconds(3f); // Wait for 3 seconds before starting the fade
        yield return FadeCurtainAlpha(0f, 2f);
        curtain.SetActive(false);
        curtainRoutine = null;

        // Play the message after the curtain fade
        if (messageToPlayAfterCurtainFade != null)
        {
            DialogueManager.instance.PlayMessage(messageToPlayAfterCurtainFade);
        }
    }

    private IEnumerator CloseCurtainCoroutine(UnityAction onCurtainClosed)
    {
        title.text = "";
        subTitle.text = "";
        yield return FadeCurtainAlpha(1f, 2f);
        curtainRoutine = null;
        onCurtainClosed?.Invoke();
    }

    private IEnumerator FadeCurtainAlpha(float targetAlpha, float duration)
    {
        Image backdropImage = curtain != null ? curtain.GetComponent<Image>() : null;
        float startAlpha = backdropImage != null ? backdropImage.color.a : 0f;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / duration);
            SetCurtainAlpha(alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetCurtainAlpha(targetAlpha);
    }

    private void SetCurtainAlpha(float alpha)
    {
        if (curtain != null)
        {
            Image backdropImage = curtain.GetComponent<Image>();
            if (backdropImage != null)
            {
                backdropImage.color = new Color(backdropImage.color.r, backdropImage.color.g, backdropImage.color.b, alpha);
            }
        }

        if (title != null)
        {
            title.color = new Color(title.color.r, title.color.g, title.color.b, alpha);
        }

        if (subTitle != null)
        {
            subTitle.color = new Color(subTitle.color.r, subTitle.color.g, subTitle.color.b, alpha);
        }
    }
}
