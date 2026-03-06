using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager instance;
    [SerializeField] private InputActionReference advanceDialogueAction;
    
    private Message currentMessage;
    private MessageSO currentMessageSO;
    private AudioSource audioSource;
    private Coroutine currentGurbleCoroutine;
    private bool skipRequested;

    [SerializeField] private TMP_Text titleTextField;
    [SerializeField] private TMP_Text messageTextField;
    [SerializeField] private Image profilePictureField;
    [SerializeField] private Button button1;
    [SerializeField] private Button button2;
    [SerializeField] private Button button3;
    [SerializeField] private GameObject dialoguePanel;
    
    [Header("Gurble Settings")]
    [SerializeField] private float charactersPerSecond = 20f;
    [SerializeField] private float gurbleInterval = 0.05f;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
        }
        EnsurePanelInactive();
    }

    void OnEnable()
    {
        if (advanceDialogueAction != null && advanceDialogueAction.action != null)
        {
            advanceDialogueAction.action.performed += OnAdvanceDialoguePerformed;
            advanceDialogueAction.action.Enable();
        }
    }

    void OnDisable()
    {
        if (advanceDialogueAction != null && advanceDialogueAction.action != null)
        {
            advanceDialogueAction.action.performed -= OnAdvanceDialoguePerformed;
            advanceDialogueAction.action.Disable();
        }
    }

    private void OnAdvanceDialoguePerformed(InputAction.CallbackContext context)
    {
        if (currentGurbleCoroutine != null)
        {
            skipRequested = true;
        }
    }

    void EnsurePanelActive()
    {
        if (dialoguePanel != null && !dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(true);
        }
    }
    void EnsurePanelInactive()
    {
        if (dialoguePanel != null && dialoguePanel.activeSelf)
        {
            dialoguePanel.SetActive(false);
        }
        
        if (currentGurbleCoroutine != null)
        {
            StopCoroutine(currentGurbleCoroutine);
            currentGurbleCoroutine = null;
        }

        skipRequested = false;
    }

    /// <summary>
    /// Plays a dialogue message and displays its choices
    /// </summary>
    /// <param name="message">The message to display</param>
    public void PlayMessage(Message message)
    {
        if (message == null)
        {
            Debug.LogWarning("Attempted to play null message");
            return;
        }

        // Stop any existing gurble coroutine
        if (currentGurbleCoroutine != null)
        {
            StopCoroutine(currentGurbleCoroutine);
        }

        skipRequested = false;

        currentMessage = message;

        // Display the message
        Debug.Log($"[{message.Name}]: {message.Text}");

        // Log available choices
        if (message.Choices != null && message.Choices.Count > 0)
        {
            Debug.Log($"Available choices: {message.Choices.Count}");
            for (int i = 0; i < message.Choices.Count; i++)
            {
                Debug.Log($"  {i + 1}. {message.Choices[i].Text}");
            }
        }

        // Update title
        if (titleTextField != null)
        {
            titleTextField.text = message.Name;
        }

        // Update profile picture
        if (profilePictureField != null)
        {
            profilePictureField.sprite = message.ProfilePicture;
        }

        // Start gurbling the text
        if (messageTextField != null)
        {
            currentGurbleCoroutine = StartCoroutine(GurbleText(message.Text, message.Gurble));
        }
    }

    /// <summary>
    /// Coroutine that displays text character by character with gurble audio
    /// </summary>
    private IEnumerator GurbleText(string fullText, AudioClip gurbleClip)
    {
        messageTextField.text = "";
        HideChoiceButtons();
        
        float timeBetweenChars = 1f / charactersPerSecond;
        float timeSinceLastGurble = 0f;

        for (int i = 0; i <= fullText.Length; i++)
        {
            if (skipRequested)
            {
                messageTextField.text = fullText;
                break;
            }

            messageTextField.text = fullText.Substring(0, i);

            // Play gurble sound at intervals (skip spaces)
            if (gurbleClip != null && audioSource != null && timeSinceLastGurble >= gurbleInterval && i > 0 && fullText[i - 1] != ' ')
            {
                // Randomly shift pitch slightly up or down
                float originalPitch = audioSource.pitch;
                audioSource.pitch = Random.Range(0.9f, 1.1f);
                audioSource.PlayOneShot(gurbleClip);
                timeSinceLastGurble = 0f;
            }

            if (i < fullText.Length)
            {
                yield return new WaitForSeconds(timeBetweenChars);
                timeSinceLastGurble += timeBetweenChars;
            }
        }

        skipRequested = false;
        UpdateChoiceButtons();
        currentGurbleCoroutine = null;
    }

    /// <summary>
    /// Hides all choice buttons
    /// </summary>
    private void HideChoiceButtons()
    {
        Button[] buttons = { button1, button2, button3 };
        foreach (Button button in buttons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Plays a MessageSO scriptable object dialogue
    /// </summary>
    /// <param name="messageSO">The MessageSO to display</param>
    public void PlayMessage(MessageSO messageSO)
    {
        if (messageSO == null)
        {
            Debug.LogWarning("Attempted to play null MessageSO");
            return;
        }

        // Stop any existing gurble coroutine
        if (currentGurbleCoroutine != null)
        {
            StopCoroutine(currentGurbleCoroutine);
        }

        skipRequested = false;

        currentMessageSO = messageSO;
        currentMessage = null; // Clear old message

        EnsurePanelActive();

        // Display the message
        Debug.Log($"[{messageSO.speakerName}]: {messageSO.messageText}");

        // Log available choices
        if (messageSO.choices != null && messageSO.choices.Count > 0)
        {
            Debug.Log($"Available choices: {messageSO.choices.Count}");
            for (int i = 0; i < messageSO.choices.Count; i++)
            {
                Debug.Log($"  {i + 1}. {messageSO.choices[i].choiceText}");
            }
        }

        // Update title
        if (titleTextField != null)
        {
            titleTextField.text = messageSO.speakerName;
        }

        // Update profile picture
        if (profilePictureField != null)
        {
            profilePictureField.sprite = messageSO.profilePicture;
        }

        // Start gurbling the text
        if (messageTextField != null)
        {
            currentGurbleCoroutine = StartCoroutine(GurbleText(messageSO.messageText, messageSO.gurbleClip));
        }
    }

    /// <summary>
    /// Updates the choice buttons based on current MessageSO
    /// </summary>
    private void UpdateChoiceButtons()
    {
        if (currentMessageSO == null) return;

        Button[] buttons = { button1, button2, button3 };
    
        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i] != null)
            {
                if (i < currentMessageSO.choices.Count)
                {
                    buttons[i].gameObject.SetActive(true);
                    var buttonText = buttons[i].GetComponentInChildren<TMP_Text>();
                    if (buttonText != null)
                    {
                        buttonText.text = currentMessageSO.choices[i].choiceText;
                    }
                
                    // Set up button listener
                    int choiceIndex = i;
                    buttons[i].onClick.RemoveAllListeners();
                    buttons[i].onClick.AddListener(() => SelectChoiceSO(choiceIndex));
                }
                else
                {
                    buttons[i].gameObject.SetActive(false);
                }
            }
        }
    }

    /// <summary>
    /// Selects a choice from the current MessageSO
    /// </summary>
    /// <param name="choiceIndex">The index of the choice to select</param>
    public void SelectChoiceSO(int choiceIndex)
    {
        if (currentMessageSO == null)
        {
            Debug.LogWarning("No active MessageSO");
            return;
        }

        if (currentMessageSO.choices == null || choiceIndex < 0 || choiceIndex >= currentMessageSO.choices.Count)
        {
            Debug.LogWarning($"Invalid choice index: {choiceIndex}");
            return;
        }

        ChoiceSO selectedChoice = currentMessageSO.choices[choiceIndex];
        selectedChoice.onChoose?.Invoke();

        MessageSO nextMessage = selectedChoice.nextMessage;
    
        if (nextMessage != null)
        {
            PlayMessage(nextMessage);
        }
        else
        {
            Debug.Log("End of dialogue tree");
            EnsurePanelInactive();
        }
    }
}
