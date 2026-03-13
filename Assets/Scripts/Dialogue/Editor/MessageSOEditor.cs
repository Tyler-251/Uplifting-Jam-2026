using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MessageSO))]
public class MessageSOEditor : Editor
{
    private const float PreviewMaxHeight = 220f;

    private SerializedProperty speakerNameProp;
    private SerializedProperty messageTextProp;
    private SerializedProperty profilePictureProp;
    private SerializedProperty graphicPictureProp;
    private SerializedProperty clickThroughProp;
    private SerializedProperty gurbleClipProp;
    private SerializedProperty choicesProp;

    private void OnEnable()
    {
        speakerNameProp = serializedObject.FindProperty("speakerName");
        messageTextProp = serializedObject.FindProperty("messageText");
        profilePictureProp = serializedObject.FindProperty("profilePicture");
        graphicPictureProp = serializedObject.FindProperty("graphicPicture");
        clickThroughProp = serializedObject.FindProperty("clickThrough");
        gurbleClipProp = serializedObject.FindProperty("gurbleClip");
        choicesProp = serializedObject.FindProperty("choices");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Message Content", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(speakerNameProp);
        EditorGUILayout.PropertyField(messageTextProp);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Visuals", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(profilePictureProp);
        DrawSpritePreview(profilePictureProp, "Profile Picture Preview", true);

        EditorGUILayout.PropertyField(graphicPictureProp);
        DrawSpritePreview(graphicPictureProp, "Graphic Picture Preview", false);

        EditorGUILayout.PropertyField(clickThroughProp);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(gurbleClipProp);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Choices", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(choicesProp, true);

        serializedObject.ApplyModifiedProperties();
    }

    private static void DrawSpritePreview(SerializedProperty spriteProp, string label, bool forceSquare)
    {
        if (!(spriteProp.objectReferenceValue is Sprite sprite) || sprite.texture == null)
        {
            return;
        }

        EditorGUILayout.LabelField(label, EditorStyles.miniBoldLabel);

        float inspectorWidth = EditorGUIUtility.currentViewWidth - 40f;
        Rect previewRect;

        if (forceSquare)
        {
            float squareSize = Mathf.Min(PreviewMaxHeight, inspectorWidth);
            previewRect = GUILayoutUtility.GetRect(squareSize, squareSize, GUILayout.ExpandWidth(false));
        }
        else
        {
            float spriteAspect = sprite.rect.width / Mathf.Max(1f, sprite.rect.height);
            float previewHeight = Mathf.Min(PreviewMaxHeight, inspectorWidth / Mathf.Max(0.01f, spriteAspect));
            previewRect = GUILayoutUtility.GetRect(inspectorWidth, previewHeight, GUILayout.ExpandWidth(true));
        }

        Texture2D texture = sprite.texture;
        Rect textureRect = sprite.textureRect;
        Rect sampleRect = textureRect;

        if (forceSquare)
        {
            if (sampleRect.width > sampleRect.height)
            {
                float crop = (sampleRect.width - sampleRect.height) * 0.5f;
                sampleRect.x += crop;
                sampleRect.width = sampleRect.height;
            }
            else if (sampleRect.height > sampleRect.width)
            {
                float crop = (sampleRect.height - sampleRect.width) * 0.5f;
                sampleRect.y += crop;
                sampleRect.height = sampleRect.width;
            }
        }

        Rect uvRect = new Rect(
            sampleRect.x / texture.width,
            sampleRect.y / texture.height,
            sampleRect.width / texture.width,
            sampleRect.height / texture.height
        );

        GUI.DrawTextureWithTexCoords(previewRect, texture, uvRect, true);
        EditorGUILayout.Space(4f);
    }
}
