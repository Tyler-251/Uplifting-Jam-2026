using UnityEngine;
using System.Collections.Generic;
using System.IO;
using TMPro;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class JsonToSOChainTool : MonoBehaviour
{
    [System.Serializable]
    public class SpeakerAssetData
    {
        public string speakerName;
        public Sprite profilePicture;
        public AudioClip audioBlurb;
    }

    [System.Serializable]
    public class ChoiceData
    {
        public string text;
        public int target_id;
    }

    [System.Serializable]
    public class DialogueData
    {
        public int id;
        public string character;
        public string text;
        public List<ChoiceData> choices;
        public bool is_start;
        public bool is_end;
    }

    [System.Serializable]
    public class DialogueJsonWrapper
    {
        public List<DialogueData> dialogues;
    }

    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private List<SpeakerAssetData> speakerAssets = new List<SpeakerAssetData>();

#if UNITY_EDITOR
    public void OnClick() {
        string jsonText = inputField.text;
        ParseAndSaveDialogue(jsonText);
    }
#endif

    private bool TryGetSpeakerAssets(string speakerTitle, out SpeakerAssetData matchedData)
    {
        matchedData = null;

        if (string.IsNullOrWhiteSpace(speakerTitle) || speakerAssets == null)
        {
            return false;
        }

        string targetName = speakerTitle.Trim();
        for (int i = 0; i < speakerAssets.Count; i++)
        {
            SpeakerAssetData current = speakerAssets[i];
            if (current == null || string.IsNullOrWhiteSpace(current.speakerName))
            {
                continue;
            }

            if (string.Equals(current.speakerName.Trim(), targetName, System.StringComparison.OrdinalIgnoreCase))
            {
                matchedData = current;
                return true;
            }
        }

        return false;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Parses JSON text from an input field and creates MessageSO assets
    /// </summary>
    /// <param name="jsonText">The JSON string to parse</param>
    /// <returns>The path to the created export folder</returns>
    public string ParseAndSaveDialogue(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText))
        {
            Debug.LogError("JSON text is empty!");
            return null;
        }

        try
        {
            // Unity's JsonUtility doesn't support dictionaries, so we extract just the dialogues array
            // Find the dialogues array in the JSON
            int dialoguesStart = jsonText.IndexOf("\"dialogues\"");
            if (dialoguesStart == -1)
            {
                Debug.LogError("'dialogues' array not found in JSON!");
                return null;
            }

            // Extract just the dialogues array and wrap it
            int arrayBracketStart = jsonText.IndexOf('[', dialoguesStart);
            int arrayBracketEnd = jsonText.LastIndexOf(']');
            
            if (arrayBracketStart == -1 || arrayBracketEnd == -1)
            {
                Debug.LogError("Could not find dialogues array brackets!");
                return null;
            }

            string dialoguesJson = jsonText.Substring(arrayBracketStart, arrayBracketEnd - arrayBracketStart + 1);
            string wrappedJson = "{\"dialogues\":" + dialoguesJson + "}";

            // Parse JSON
            DialogueJsonWrapper data = JsonUtility.FromJson<DialogueJsonWrapper>(wrappedJson);

            if (data == null || data.dialogues == null || data.dialogues.Count == 0)
            {
                Debug.LogError("Failed to parse JSON or no dialogues found!");
                return null;
            }

            // Find next available folder number
            string exportsPath = "Assets/Resources/Exports";
            int folderNumber = GetNextFolderNumber(exportsPath);
            string exportFolderPath = $"{exportsPath}/{folderNumber}";

            // Create export folder
            if (!AssetDatabase.IsValidFolder(exportFolderPath))
            {
                AssetDatabase.CreateFolder(exportsPath, folderNumber.ToString());
            }

            // Dictionary to store created MessageSO by dialogue ID
            Dictionary<int, MessageSO> messagesByID = new Dictionary<int, MessageSO>();

            // First pass: Create all MessageSO assets
            foreach (var dialogue in data.dialogues)
            {
                MessageSO messageSO = ScriptableObject.CreateInstance<MessageSO>();
                messageSO.speakerName = dialogue.character;
                messageSO.messageText = dialogue.text;

                if (TryGetSpeakerAssets(dialogue.character, out SpeakerAssetData speakerData))
                {
                    messageSO.profilePicture = speakerData.profilePicture;
                    messageSO.gurbleClip = speakerData.audioBlurb;
                }

                // Initialize choices list
                messageSO.choices = new List<ChoiceSO>();
                if (dialogue.choices != null)
                {
                    foreach (var choice in dialogue.choices)
                    {
                        ChoiceSO choiceSO = new ChoiceSO
                        {
                            choiceText = choice.text
                        };
                        messageSO.choices.Add(choiceSO);
                    }
                }

                // Save the asset
                string assetPath = $"{exportFolderPath}/Message_{dialogue.id}.asset";
                AssetDatabase.CreateAsset(messageSO, assetPath);

                // Store reference
                messagesByID[dialogue.id] = messageSO;
            }

            // Second pass: Link choices to their target messages
            foreach (var dialogue in data.dialogues)
            {
                if (!messagesByID.ContainsKey(dialogue.id))
                    continue;

                MessageSO messageSO = messagesByID[dialogue.id];

                int sourceChoiceCount = dialogue.choices != null ? dialogue.choices.Count : 0;
                for (int i = 0; i < sourceChoiceCount && i < messageSO.choices.Count; i++)
                {
                    int targetID = dialogue.choices[i].target_id;
                    if (messagesByID.ContainsKey(targetID))
                    {
                        messageSO.choices[i].nextMessage = messagesByID[targetID];
                    }
                    else
                    {
                        Debug.LogWarning($"Target message ID {targetID} not found for choice in dialogue {dialogue.id}");
                    }
                }

                // Mark the asset as dirty to save changes
                EditorUtility.SetDirty(messageSO);
            }

            // Save all changes
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Debug.Log($"Successfully created {data.dialogues.Count} MessageSO assets in {exportFolderPath}");
            return exportFolderPath;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error parsing JSON: {e.Message}\n{e.StackTrace}");
            return null;
        }
    }

    /// <summary>
    /// Gets the next available folder number in the Exports directory
    /// </summary>
    private int GetNextFolderNumber(string exportsPath)
    {
        if (!Directory.Exists(exportsPath))
        {
            return 0;
        }

        var directories = Directory.GetDirectories(exportsPath);
        int maxNumber = -1;

        foreach (var dir in directories)
        {
            string folderName = Path.GetFileName(dir);
            if (int.TryParse(folderName, out int number))
            {
                if (number > maxNumber)
                {
                    maxNumber = number;
                }
            }
        }

        return maxNumber + 1;
    }
#endif
}
