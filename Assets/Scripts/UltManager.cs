using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UltManager : MonoBehaviour
{
    public static UltManager instance;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private GameObject ultPanel;
    [SerializeField] private GameObject ultBar;
    [SerializeField] private Button ultButton;
    [SerializeField] private float barLerpSpeed = 10f;
    public bool isUltReady = false;
    private Vector2 targetPos;
    private Vector2 startingPos;
    private float ultCharge = 0f;
    private RectTransform ultBarRect;

    void Start()
    {
        if (ultBar != null)
        {
            ultBarRect = ultBar.GetComponent<RectTransform>();
            if (ultBarRect != null)
            {
                startingPos = ultBarRect.anchoredPosition;
                targetPos = startingPos;
            }
        }

        if (ultButton != null)
        {
            ultButton.interactable = false;
        }
    }

    void Update()
    {
        RenderBar();
    }

    public void ChargeUlt(float amount)
    {
        ultCharge += amount;
        if (ultCharge >= 1f)
        {
            ultCharge = 1f;
            isUltReady = true;
            ultButton.interactable = true;
        }
    }

    public void UseUlt()
    {
        if (isUltReady)
        {
            // Implement ult effect here
            Debug.Log("ULT Activated!");
            if (TTTManager.instance != null)
            {
                TTTManager.instance.MultiplyPot(1.5f);
            }
            ultCharge = 0f;
            isUltReady = false;
            ultButton.interactable = false;
        }
    }

    public void RenderBar()
    {
        if (ultBarRect == null)
        {
            return;
        }

        // Shift left as charge empties: 0 charge = full width left, 1 charge = start position.
        float xOffset = (1f - ultCharge) * ultBarRect.sizeDelta.x;
        targetPos = startingPos + Vector2.left * xOffset;
        ultBarRect.anchoredPosition = Vector2.Lerp(ultBarRect.anchoredPosition, targetPos, Time.deltaTime * barLerpSpeed);
    }

}
