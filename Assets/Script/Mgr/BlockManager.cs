using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
{
    public static BlockManager Instance;
    //�����Ԥ��
    public List<GameObject> List_BlockPrefab;

    public List<GameObject> List_TriggerObj;
    //�����ͼƬ
    public List<Sprite> List_BlockImg;
    //������Ч��mat
    public List<Material> List_Material;

    public List<GameObject> List_Panel;

    public Dictionary<int, List<Vector2Int>>  Dic_BlockAnim;

    public GameObject SelectDragGameObject;
    //ѡ�еķ�������
    public int BlockNumber;
    //�Ƿ�ʹ���϶�
    public bool IsUseDrag;

    public int GameScore;

    public int _gameAllCoin;
    //�Ƿ���ʹ��
    public bool IsUseRevive;

    public event Action OnUpdateScoreEvent;

    public int GameAllCoin
    {
        get {
            _gameAllCoin = PlayerPrefs.GetInt("AllScore");
            return _gameAllCoin;
        }
        set {
            _gameAllCoin = value;
            PlayerPrefs.SetInt("AllScore",_gameAllCoin);
        }
    }

    private void Awake()
    {
        Instance = this;
        List_TriggerObj = new List<GameObject>();
        Dic_BlockAnim = new Dictionary<int, List<Vector2Int>>();
        IsUseDrag = true;
        GameScore = 0;
        IsUseRevive = true;
    }

    #region ί��
    public void UpdateScoreEvent()
    {
        OnUpdateScoreEvent?.Invoke();
    }
    #endregion


   
    /// <summary>
    /// ��ӿ�
    /// </summary>
    /// <param name="obj"></param>
    public void AddTriggerObj(GameObject obj) 
    {
        if (!List_TriggerObj.Contains(obj) && !obj.GetComponent<BlockBoardItem>().BlockImg.activeInHierarchy)
        {
            List_TriggerObj.Add(obj);
        }
        ClearTriggerObj();
    }

    /// <summary>
    /// ������
    /// </summary>
    /// <param name="obj"></param>
    public void SubTriggerObj(GameObject obj)
    {
        obj.GetComponent<BlockBoardItem>().BlockImg.SetActive(false);
        if (List_TriggerObj.Contains(obj))
        {
            List_TriggerObj.Remove(obj);
        }
        ClearTriggerObj();
    }

    /// <summary>
    /// ���Ч��
    /// </summary>
    public void ClearTriggerObj()
    {
        if (List_TriggerObj.Count == BlockNumber)
        {
            ChangeBlockImg(true);
            if (SelectDragGameObject != null)
            {
                SelectDragGameObject.GetComponent<DragMove>().ChangeImageAlpha(false);
            }
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int i = 0; i < List_TriggerObj.Count; i++)
            {
                var obj = List_TriggerObj[i].GetComponent<BlockBoardItem>();
                if (dic.ContainsKey(obj.x))
                {
                    dic[obj.x] = Mathf.Min(dic[obj.x], obj.y);
                }
                else
                {
                    dic.Add(obj.x,obj.y);
                }
            }
            foreach (var item in dic)
            {
                GamePanel.Instance.ChangeOpenLight(item.Key, item.Value);
            }
        }
        else
        {
            ChangeBlockImg(false);
            if (SelectDragGameObject != null)
            {
                SelectDragGameObject.GetComponent<DragMove>().ChangeImageAlpha(true);
            }
            GamePanel.Instance.ChangeCloseLight();
        }
    }

    /// <summary>
    /// �޸����ͼƬ��ʾ
    /// </summary>
    /// <param name="isShow"></param>
    public void ChangeBlockImg(bool isShow)
    {
        for (int i = 0; i < List_TriggerObj.Count; i++)
        {
            List_TriggerObj[i].GetComponent<BlockBoardItem>().ShowOrHideBlockImg(isShow);//.BlockImg.SetActive(false);
        }
    }

   

    public bool IsShowTrigger()
    {
        return List_TriggerObj.Count == BlockNumber && List_TriggerObj.Count !=0;
    }

    public void ShowTrigger()
    {
        Dic_BlockAnim.Clear();
        ChangeBlockImg(false);
        for (int i = 0; i < List_TriggerObj.Count; i++)
        {
            var obj = List_TriggerObj[i].GetComponent<BlockBoardItem>();
            obj.ChangeBoardState(true);
            if (!Dic_BlockAnim.ContainsKey(obj.x))
            {
                Dic_BlockAnim.Add(obj.x, new List<Vector2Int>());
            }
            Dic_BlockAnim[obj.x].Add(new Vector2Int(obj.x, obj.y));
        }
        List_TriggerObj.Clear();
    }


    public void AddScore(int score)
    {
        GameAllCoin = GameAllCoin + score;
        UpdateScoreEvent();
    }

    public void SubScore(int coin)
    {
        int num = GameAllCoin - coin;
        GameAllCoin = num;
        UpdateScoreEvent();
    }


    public void ShowOrHidePanel(PanelType type,bool isOpen)
    {
        List_Panel[(int)type].SetActive(isOpen);
    }

    public void BlockMoveMusic(int index)
    {
        A_AudioManager.Instance.PlaySound("Block" + index);
    }
}

public enum PanelType
{
    LoadPanel,GamePanel,GameOverPanel,SetPanel,RevivePanel,HomePanel
}