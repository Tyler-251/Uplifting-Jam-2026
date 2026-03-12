using UnityEngine;
using TMPro;

public class VertBarBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public GameObject fullBar;
    public GameObject xBar;
    public GameObject oBar;
    public TMP_Text bestOfText;
    public TMP_Text xWinsText;
    public TMP_Text oWinsText;
    public int xWins = 0;
    public int oWins = 0;
    public int bestOf = 1;
    public int draws = 0;
    public int totalGames = 0;

    public void EnableChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
    }

    public void DisableChildren()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }

    public void NextBestOf()
    {
        bestOf += 2;
        RenderBar();
    }

    public void RenderBar()
    {
        float totalHeight = (fullBar.GetComponent<RectTransform>().rect.height)*.98f;
        float xratio = bestOf > 0 ? (float)xWins / bestOf : 0f;
        float oratio = bestOf > 0 ? (float)oWins / bestOf : 0f;
        float xHeight = totalHeight * xratio;
        float oHeight = totalHeight * oratio;
        xBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, xHeight);
        oBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, oHeight);
        if(xBar.GetComponent<RectTransform>().rect.height < 50f)
        {
            xBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);
        }
        if(oBar.GetComponent<RectTransform>().rect.height < 50f)
        {
            oBar.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 50f);
        }

        oWinsText.text = oWins.ToString();
        xWinsText.text = xWins.ToString();
        bestOfText.text = bestOf.ToString();
    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
