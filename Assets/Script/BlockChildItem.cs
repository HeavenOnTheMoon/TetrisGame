using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BlockChildItem : MonoBehaviour
{
    public int ImgIndex;
    private Image ItemImg;

    private void Start()
    {
        ItemImg = transform.GetComponent<Image>();
        SetBlockImg();
    }

    public void SetBlockImg()
    {
        int colorIndex = Random.Range(0, BlockManager.Instance.List_BlockImg.Count);
        ImgIndex = colorIndex;
        ItemImg.sprite = BlockManager.Instance.List_BlockImg[colorIndex];
    }

    public void ChangeImageAlpha(bool isOpen)
    {
        var colorA = isOpen ? 255 : 0;
        ItemImg.color = new Color(255, 255, 255, colorA);
    }
}
