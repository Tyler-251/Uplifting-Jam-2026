using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TTTRenderer : MonoBehaviour
{
    [SerializeField] private Sprite xSprite;
    [SerializeField] private Sprite oSprite;
    [SerializeField] private Sprite xxSprite;
    [SerializeField] private Sprite ooSprite;
    [SerializeField] private Color oColor = Color.blue;
    [SerializeField] private Color xColor = Color.red;
    [SerializeField] private List<Image> slotImages; // 00, 01, 02, 10, 11, 12, 20, 21, 22
    public static TTTRenderer instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void RefreshBoard(TTTBoard board) {
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                var slot = board.GetSlot(row, col);
                var imageIndex = row * 3 + col;
                var image = slotImages[imageIndex];
                
                switch (slot.Piece.Type)
                {
                    case Piece.PieceType.X:
                        image.sprite = xSprite;
                        image.color = xColor;
                        break;
                    case Piece.PieceType.O:
                        image.sprite = oSprite;
                        image.color = oColor;
                        break;
                    case Piece.PieceType.XX:
                        image.sprite = xxSprite;
                        image.color = xColor;
                        break;
                    case Piece.PieceType.OO:
                        image.sprite = ooSprite;
                        image.color = oColor;
                        break;
                    default:
                        image.sprite = null;
                        image.color = new Color(1, 1, 1, 0.0001f); // Nearly invisible but still accepts raycasts
                        break;
                }
            }
        }
    }
}
