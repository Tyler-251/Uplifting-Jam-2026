using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButtonBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void OpenSettings()
    {
        
    }
    public void OpenCredits()
    {
        
    }
}
