using UnityEngine;

public class TestDialogue : MonoBehaviour
{
    [SerializeField] private MessageSO testMessageSO;
    public void OnClick()
    {
        DialogueManager.instance.PlayMessage(testMessageSO);
    }
}
