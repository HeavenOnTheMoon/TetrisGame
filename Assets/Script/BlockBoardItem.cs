using Coffee.UIExtensions;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BlockBoardItem : MonoBehaviour
{
    public GameObject BlockImg;
    public GameObject UIParticleObj;
    public bool IsUse;
    private float timer = 0f;
    private float checkInterval = 0.1f; // 检测间隔0.1秒

    public bool IsOpenCheck;
    public int x;
    public int y;
    public GameObject Block;
    public Image BlockBg;
    public Image BlockLight;

    public int colorIndex;

    private void Awake()
    {
        
        BlockBoardItemInit();
    }


    public void BlockBoardItemInit()
    {
        IsUse = false;
        colorIndex = -1;
        IsOpenCheck = false;
        BlockImg.SetActive(false);
        ChangeBlcokLight(false);
        if (Block != null)
        {
            Destroy(Block);
            Block = null;
        }
    }

    /// <summary>
    /// 显示/关闭block
    /// </summary>
    /// <param name="isShow"></param>
    public void ShowOrHideBlockImg(bool isShow)
    {
        BlockImg.SetActive(isShow);
    }
    /// <summary>
    /// 设置颜色
    /// </summary>
    /// <param name="obj"></param>
    public void SetBlockImgColor(GameObject obj)
    {
        obj.transform.GetComponent<Image>().sprite = BlockManager.Instance.List_BlockImg[colorIndex];
    }


    ///修改格子检测状态
    public void ChangeBoardState(bool isuse)
    {
        IsUse = isuse;
        if (isuse)
        {
            Block = Instantiate(BlockImg, transform);
            SetBlockImgColor(Block);
            Block.gameObject.SetActive(true);
            Block.transform.localPosition = new Vector3(0, 55,0);
            Block.transform.localScale  = Vector3.one ;
            if (Block.GetComponent<BlockImg>() != null)
            {
                Block.GetComponent<BlockImg>().BlockIndex = colorIndex;
            }
        }
    }

    public void PlayScaleBounce(float duration = 0.1f)
    {
        
        if (Block == null) return;
        Block.transform.DOKill(); // 停止现有动画
        DOTween.Sequence()
            .Append(Block.transform.DOScaleY(0.7f, duration).SetEase(Ease.OutBack))
            .Append(Block.transform.DOScaleY(1f, duration).SetEase(Ease.OutElastic))
            .SetTarget(Block.transform);
    }

    public void ShowUIParticle()
    {
        //Block.SetActive(false);
        UIParticleObj.SetActive(true);
        A_AudioManager.Instance.PlaySound("Delete");
        var psRenderer = UIParticleObj.GetComponent<UIParticle>().transform.GetChild(0).GetComponent<ParticleSystemRenderer>();
        
        if (psRenderer != null  && colorIndex < BlockManager.Instance.List_Material.Count && colorIndex >= 0)
        {
            psRenderer.material = BlockManager.Instance.List_Material[colorIndex];
        }
        UIParticleObj.GetComponent<UIParticle>().RefreshParticles();
        
        Block.transform.DOScale(0, 0.5f).OnComplete(() => {
            Destroy(Block);
            Block = null;
        });
        A_TimeManager.Instance.Delay(0.5f, () =>
        {
            UIParticleObj.SetActive(false);
            /*Destroy(Block);
            Block = null;*/
            IsUse = false;
            colorIndex = -1;
        });
    }
    void Update()
    {
        if (IsOpenCheck)
        {
           /* timer += Time.deltaTime;

            if (timer >= checkInterval)
            {*/
                timer = 0f; // 重置计时器
                if (!IsUse)
                {
                    DetectNearestGrid();
                }
            //}
        }
        if (Block != null  && colorIndex == -1)
        {
            colorIndex = Block.GetComponent<BlockImg>().BlockIndex;
        }
    }
   
    private void DetectNearestGrid()
    {
        // 直接检测当前鼠标位置是否有格子
        Vector2 mousePos = transform.position;
        Collider2D hit = Physics2D.OverlapPoint(mousePos, 1 << 6);
        Vector3 rayOrigin = Camera.main.ScreenToWorldPoint(mousePos);
        if (hit != null && hit.CompareTag("BlockItem"))
        {
            //BlockImg.SetActive(true);
            BlockManager.Instance.AddTriggerObj(gameObject);
            colorIndex = hit.transform.GetComponent<BlockChildItem>().ImgIndex;
            SetBlockImgColor(BlockImg);
        }
        else
        {
            BlockManager.Instance.SubTriggerObj(gameObject);
        }
    }

    public void ChangeBlcokLight(bool isOpen)
    {
        BlockLight.gameObject.SetActive(isOpen);
    }
}
