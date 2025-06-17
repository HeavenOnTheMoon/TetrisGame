using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragMove : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    public int BlockNumber;
    public int BlockVerticalNumber;
    private Vector3 offset;

    public List<Vector2Int> List_BlockVector;

    public List<Image> List_BlockImage;

    private bool isDrag;

    void Start()
    {
        isDrag = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (BlockManager.Instance.IsUseDrag)
        {
            BlockManager.Instance.BlockNumber = BlockNumber;
            BlockManager.Instance.SelectDragGameObject = gameObject;
            transform.localScale = Vector3.one ;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            offset = Vector3.zero;//transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, screenPoint.z));
         }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (BlockManager.Instance.IsUseDrag)
        {
            isDrag = true;
            BlockManager.Instance.SelectDragGameObject = gameObject;
            BlockManager.Instance.BlockNumber = BlockNumber;
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 currentScreenPoint = new Vector3(eventData.position.x, eventData.position.y, screenPoint.z);
            Vector3 camarePos = Camera.main.ScreenToWorldPoint(currentScreenPoint);
            float y = camarePos.y > GamePanel.Instance.BoardPoint.transform.position.y ? GamePanel.Instance.BoardPoint.transform.position.y : camarePos.y;
            transform.position = new Vector3(camarePos.x,y,camarePos.z) + offset;
            transform.localScale = Vector3.one  * 1.7f ;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDrag)
        {
            BlockManager.Instance.IsUseDrag = false;
            if (BlockManager.Instance.IsShowTrigger())
            {
                BlockManager.Instance.ShowTrigger();
                Destroy(transform.gameObject);
            }
            else
            {
                // 吸附逻辑将在这里实现
                transform.localPosition = new Vector3(0,(BlockVerticalNumber - 1) * -50,0);
                transform.localScale = Vector3.one ;
            }
            BlockManager.Instance.BlockNumber = 0;
            GamePanel.Instance.CombNumber = 0;
            GamePanel.Instance.CurDeleteNumber = 0;
            GamePanel.Instance.FloatBlocksUp(0);
            GamePanel.Instance.IsShowCombScore = true;
            BlockManager.Instance.SelectDragGameObject = null;
            GamePanel.Instance.ChangeCloseLight();
        }
    }


    public void ChangeImageAlpha(bool isOpen)
    {
        for (int i = 0; i < List_BlockImage.Count; i++)
        {
            var colorA = isOpen ? 255 : 0;
            List_BlockImage[i].color = new Color(255,255,255,colorA);
        }
    }

}
