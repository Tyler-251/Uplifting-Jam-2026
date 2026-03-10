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
    float xWins = 4;
    float oWins = 4;
    float bestOf = 8;



    public void RenderBar()
    {
        float totalHeight = (fullBar.GetComponent<RectTransform>().rect.height)*.98f;
        float xratio = xWins / bestOf;
        float oratio = oWins / bestOf;
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

        oWinsText.text = "O Wins: " + oWins;
        xWinsText.text = "X Wins: " + xWins;
        bestOfText.text = "Best of: " + bestOf;



    }



    // Update is called once per frame
    void Update()
    {
        
    }
}
