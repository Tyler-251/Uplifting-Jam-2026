using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurtainBehavior : MonoBehaviour
{
    [SerializeField] private GameObject curtain;
    [SerializeField] private TMP_Text title;
    [SerializeField] private TMP_Text subTitle;

    public void OpenCurtain(string curtainTitle, string subTitle, MessageSO messageToPlayAfterCurtainFade)
    {
        curtain.SetActive(true);
        title.text = curtainTitle;
        this.subTitle.text = subTitle;
        StartCoroutine(OpenCurtainCoroutine(messageToPlayAfterCurtainFade));
    }

    private IEnumerator OpenCurtainCoroutine(MessageSO messageToPlayAfterCurtainFade)
    {
        Image backdropImage = curtain.GetComponent<Image>();
        TMP_Text titleText = title.GetComponent<TMP_Text>();
        TMP_Text subTitleText = subTitle.GetComponent<TMP_Text>();
        yield return new WaitForSeconds(3f); // Wait for 3 seconds before starting the fade
        float fadeDuration = 2f; // Duration of the fade effect
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            backdropImage.color = new Color(backdropImage.color.r, backdropImage.color.g, backdropImage.color.b, alpha);
            titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, alpha);
            subTitleText.color = new Color(subTitleText.color.r, subTitleText.color.g, subTitleText.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the final alpha is set to 0
        backdropImage.color = new Color(backdropImage.color.r, backdropImage.color.g, backdropImage.color.b, 0f);
        titleText.color = new Color(titleText.color.r, titleText.color.g, titleText.color.b, 0f);
        subTitleText.color = new Color(subTitleText.color.r, subTitleText.color.g, subTitleText.color.b, 0f);
        curtain.SetActive(false);

        // Play the message after the curtain fade
        if (messageToPlayAfterCurtainFade != null)
        {
            DialogueManager.instance.PlayMessage(messageToPlayAfterCurtainFade);
        }
    }
}
