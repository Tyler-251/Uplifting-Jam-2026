using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Serializable choice for dialogue with reference to next message
/// </summary>
[System.Serializable]
public class ChoiceSO
{
    [Tooltip("The text displayed for this choice")]
    public string choiceText;
    
    [Tooltip("Optional UnityEvent invoked when this choice is selected")]
    public UnityEvent onChoose;
    
    [Tooltip("The next message to play when this choice is selected")]
    public MessageSO nextMessage;
}

/// <summary>
/// ScriptableObject representing a dialogue message with choices
/// </summary>
[CreateAssetMenu(fileName = "NewMessage", menuName = "Scriptable Objects/MessageSO")]
public class MessageSO : ScriptableObject
{
    [Header("Message Content")]
    [Tooltip("The name/title of the speaker")]
    public string speakerName;
    
    [Tooltip("The dialogue text to display")]
    [TextArea(3, 10)]
    public string messageText;
    
    [Tooltip("The speaker's profile picture")]
    public Sprite profilePicture;
    
    [Header("Audio")]
    [Tooltip("The gurble audio clip to play while displaying text")]
    public AudioClip gurbleClip;
    
    [Header("Choices")]
    [Tooltip("Available choices for this message")]
    public List<ChoiceSO> choices = new List<ChoiceSO>();
}
