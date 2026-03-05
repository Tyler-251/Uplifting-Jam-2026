using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a choice option in dialogue
/// </summary>
public class Choice
{
    public string Text { get; set; }
    public Action OnChoose { get; set; }

    public Choice(string text, Action onChoose)
    {
        Text = text;
        OnChoose = onChoose;
    }
}

/// <summary>
/// Represents a dialogue message with optional choices
/// </summary>
public class Message
{
    public string Name { get; set; }
    public string Text { get; set; }
    public Sprite ProfilePicture { get; set; }
    public List<Choice> Choices { get; set; }
    public AudioClip Gurble { get; set; }

    public Message(string name, string text, Sprite profilePicture = null, List<Choice> choices = null, AudioClip gurble = null)
    {
        Name = name;
        Text = text;
        ProfilePicture = profilePicture;
        Choices = choices ?? new List<Choice>();
        Gurble = gurble;
    }
}
