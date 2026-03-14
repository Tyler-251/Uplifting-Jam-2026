using UnityEngine;

public class BackToMenu : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GO()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("TitleScene");
    }
}
