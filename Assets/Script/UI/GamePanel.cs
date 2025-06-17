using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class GamePanel : MonoBehaviour
{
    public static GamePanel Instance;
    public GameObject BoardParent;
    public GameObject BlockCollItem;
    public Text Text_Score;
    public Button Button_Set;
    public Text Text_ShowScore;
    public Text Text_Comb;
    public GameObject CombObj;
    public Text Text_CombScore;
    public GameObject CombParent;
    public GameObject CombScoreParent;
    public GameObject BoardPoint;

    public List<GameObject> List_PointParent;
    public GameObject[] Array_Block;
    public Dictionary<int , List<BlockBoardItem>>  Dic_AllDelete;

    public GameObject[,] gridArray = new GameObject[9, 9];

    public int CombNumber;
    public int CurDeleteNumber;
    private GameObject CombScoreObj;

    public bool IsShowCombScore;

    private void Awake()
    {
        Instance = this;
        IsShowCombScore = true;
        BlockManager.Instance.OnUpdateScoreEvent += OnUpdateScore;

        Button_Set.onClick.AddListener(()=> {
            BlockManager.Instance.ShowOrHidePanel(PanelType.SetPanel,true);
        });
        Text_ShowScore.text = "0";
        Array_Block = new GameObject[3];
    }

    private void OnUpdateScore()
    {
        Text_Score.text = BlockManager.Instance.GameAllCoin.ToString();
    }

    void Start()
    {
        

    }

    private void OnEnable()
    {
        
        GameInit();
        OnUpdateScore();
    }

    public void GameInit()
    {
        for (int i = 0; i < Array_Block.Length; i++)
        {
            if (Array_Block[i] != null)
            {
                Destroy(Array_Block[i]);
                Array_Block[i] = null;
            }
        }
        BlockManager.Instance.GameScore = 0;
        BlockManager.Instance.IsUseRevive = true;
        Text_ShowScore.text = "0";
        CreateBoard(); 
    }

    public void CreateBoard()
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (gridArray[x, y] == null)
                {
                    GameObject boardObj = Instantiate(BlockCollItem, BoardParent.transform);
                    gridArray[x, y] = boardObj;
                    boardObj.GetComponent<BlockBoardItem>().x = x;
                    boardObj.GetComponent<BlockBoardItem>().y = y;
                }
                else
                {
                    gridArray[x, y].GetComponent<BlockBoardItem>().BlockBoardItemInit();
                }
                if (y >= 0)//6)
                {
                    gridArray[x, y].GetComponent<BlockBoardItem>().IsOpenCheck = true;
                }
            }
        }
        CreateThreeBlock();
    }
    //创建面板
    public void CreateThreeBlock()
    {
        for (int i = 0; i < Array_Block.Length; i++)
        {
            int index = Random.Range(0,BlockManager.Instance.List_BlockPrefab.Count);
            GameObject obj = Instantiate(BlockManager.Instance.List_BlockPrefab[index], List_PointParent[i].transform);
            obj.transform.localPosition = new Vector3(0,-300,0);
            obj.transform.localScale = Vector3.one;
            Array_Block[i] = obj;
            int vecY = obj.GetComponent<DragMove>().BlockVerticalNumber - 1;
            obj.transform.DOLocalMove(new Vector3(0,-50* vecY,0), 0.5f);
        }

        CheckGameFinish();
    }

    public void CheckGameFinish()
    {
        if (!CheckThreeBlockHavePos())
        {
            BlockManager.Instance.IsUseDrag = false;
            //GameInit();
            Debug.Log("=============  游戏失败  ===============");
            FinishAnim(() =>
            {
                if (BlockManager.Instance.IsUseRevive)
                {
                    BlockManager.Instance.ShowOrHidePanel(PanelType.RevivePanel, true);
                    
                }
                else
                {
                    BlockManager.Instance.ShowOrHidePanel(PanelType.GameOverPanel, true);
                }
            });
        }
        else
        {
            BlockManager.Instance.IsUseDrag = true;
        }
    }

    public void FinishAnim(Action action)
    {
        Sequence mySequence = DOTween.Sequence();
        
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (gridArray[x, y] != null)
                {
                    var grid = gridArray[x, y].GetComponent<BlockBoardItem>();
                    // 4次"往返"（Yoyo）等于2个完整循环
                    mySequence.Join(grid.BlockBg.DOFade(1f, 0.5f).SetLoops(4, LoopType.Yoyo).SetEase(Ease.Linear));
                }
            }
        }
        mySequence.OnComplete(()=> {
            for (int y = 0; y < 9; y++)
            {
                for (int x = 0; x < 9; x++)
                {
                    if (gridArray[x, y] != null)
                    {
                        var grid = gridArray[x, y].GetComponent<BlockBoardItem>();
                        grid.BlockBg.DOFade(0, 0.1f);
                    }
                }
            }
            action?.Invoke();
        });
    }


    /// <summary>
    /// 检测是否创建拖动块
    /// </summary>
    public void CheckThreeCreate()
    {
        bool isNull = true;
        for (int i = 0; i < Array_Block.Length; i++)
        {
            if (Array_Block[i] != null)
            {
                isNull = false;
            }
        }
        if (isNull)
        {
            CreateThreeBlock();
        }
    }

    /// <summary>
    /// 上浮效果
    /// </summary>
    public void FloatBlocksUp(float time)
    {
        StartCoroutine(IE_FloatBlocksUp(time));
    }

    bool isPlayAudio = false;
    IEnumerator IE_FloatBlocksUp(float time)
    {
        yield return new WaitForSeconds(time);
        BlockBoardItem block;
        int index = 1;
        int y = 0;
        int audioImdex = 0;

        Dictionary<int, Vector2Int> dic = new Dictionary<int, Vector2Int>();
        foreach (var item in gridArray)
        {
            block = item.GetComponent<BlockBoardItem>();
            if (block.Block != null)
            {
                y = Mathf.Max(y, block.y);
            }
        }
        var list = GetSeletBlockMaxY();
        while (index <= 8)
        {
            foreach (var item in gridArray)
            {
                block = item.GetComponent<BlockBoardItem>();
                if (block.Block != null)
                {
                    FloatUpDotween(block.x, block.y, block.Block,dic, list);
                }
            }
            if (audioImdex < y && isPlayAudio)
            {
                BlockManager.Instance.BlockMoveMusic(audioImdex);
                audioImdex++;
            }
            yield return new WaitForSeconds(0.03f);
            index++;
            isPlayAudio = false;
        }
       
        StartCoroutine(nameof(IE_DeleteBlock));
    }

    IEnumerator IE_DeleteBlock()
    {
        yield return new WaitForSeconds(0.2f);
        CheckDeleteBlock();
    }
    
    /// <summary>
    /// 上浮动画
    /// </summary>
    public void FloatUpDotween(int x,int y,GameObject obj, Dictionary<int, Vector2Int> dic, List<Vector2Int> list)
    {
        if (x >= 0 && y - 1 >= 0)
        {
            if (gridArray[x, y - 1].GetComponent<BlockBoardItem>()?.Block == null)
            {
                isPlayAudio = true;
                gridArray[x, y - 1].GetComponent<BlockBoardItem>().Block = obj;
                gridArray[x, y - 1].GetComponent<BlockBoardItem>().IsUse = true;
                gridArray[x, y - 1].GetComponent<BlockBoardItem>().Block.transform.SetParent(gridArray[x, y - 1].gameObject.transform);
                gridArray[x, y - 1].GetComponent<BlockBoardItem>().Block.transform.localPosition = new Vector3(0,55,0);
                gridArray[x, y - 1].GetComponent<BlockBoardItem>().colorIndex = gridArray[x, y].GetComponent<BlockBoardItem>().Block.GetComponent<BlockImg>().BlockIndex;
                    //gridArray[x, y].GetComponent<BlockBoardItem>().colorIndex;

                gridArray[x, y].GetComponent<BlockBoardItem>().Block = null;
                gridArray[x, y].GetComponent<BlockBoardItem>().IsUse = false;
                gridArray[x, y].GetComponent<BlockBoardItem>().colorIndex = -1;
              
                if (dic.ContainsKey(x))
                {
                    dic[x] = new Vector2Int(x, y - 1);
                }
                else
                {
                    dic.Add(x,new Vector2Int(x,y-1));
                }
                for (int i = 0; i < list.Count; i++)
                {
                    if (x == list[i].x && (y - 1) == list[i].y)
                    {
                        gridArray[x, y - 1].GetComponent<BlockBoardItem>().PlayScaleBounce();
                    }
                }
            }
        }
    }

    public void CheckBlockIsBottom(Dictionary<int, Vector2Int> dic)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        foreach (var item in BlockManager.Instance.Dic_BlockAnim)
        {
            var start = item.Value;
            int startIndex = 0;
            for (int i = 0; i < start.Count; i++)
            {
                if (dic.ContainsKey(start[i].x))
                {
                    var end = dic[start[i].x];
                    list.Add(new Vector2Int(end.x, end.y + startIndex));
                    startIndex -= 1;
                }
            }
        }

        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].x >= 0  && list[i].y>=0 && gridArray[list[i].x, list[i].y] != null)
            {
                gridArray[list[i].x, list[i].y].GetComponent<BlockBoardItem>().PlayScaleBounce();
            }
        }
    }

    public List<Vector2Int> GetSeletBlockMaxY()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        foreach (var item in BlockManager.Instance.Dic_BlockAnim)
        {
            for (int i = 0; i < item.Value.Count; i++)
            {
                int min = 1000;
                for (int j = 0; j <= 8; j++)
                {
                    if (gridArray[item.Value[i].x, j] != null
                        && gridArray[item.Value[i].x, j].GetComponent<BlockBoardItem>().Block == null)
                    {
                        min = Mathf.Min(min, j);
                    }
                }
                Vector2Int temp = new Vector2Int(item.Value[i].x, min);
                if (!list.Contains(temp))
                {
                    list.Add(temp);
                }
                else
                {
                    list.Add(new Vector2Int(temp.x,temp.y+1));
                }
                
            }
        }
        return list;
    }

    

    /// <summary>
    /// 检测删除方块
    /// </summary>
    public void CheckDeleteBlock()
    {
        Dic_AllDelete = new Dictionary<int, List<BlockBoardItem>>();
        BlockBoardItem block;
        foreach (var item in gridArray)
        {
            block = item.GetComponent<BlockBoardItem>();
            if (block.Block != null)
            {
                var list = new List<BlockBoardItem>();
                CheckBlockRoundColor(block, list);
                if (list.Count >= 3)
                {
                    if (!Dic_AllDelete.ContainsKey(list[0].colorIndex))
                    {
                        Dic_AllDelete.Add(list[0].colorIndex, list);
                    }
                }
            }
        }
        int allScoreValue = 0;
        if (Dic_AllDelete.Count > 0)
        {
            CombNumber += 1;
            foreach (var item in Dic_AllDelete)
            {
                allScoreValue += item.Value.Count;
                CurDeleteNumber += item.Value.Count;
                
                //Text_ShowScore.text = BlockManager.Instance.GameScore.ToString();
                for (int i = 0; i < item.Value.Count; i++)
                {
                    item.Value[i].ShowUIParticle();

                }
            }
            //Debug.Log("============   Comb  " + CombNumber );
            CreateComb();
            CreateCombScore((CurDeleteNumber * 5), () => {
                
            });
            /*CreateCombScore((allScoreValue * 5));
            BlockManager.Instance.GameScore += ((allScoreValue * 5) * CombNumber);
            ChangeTextAnim(Text_ShowScore, BlockManager.Instance.GameScore);*/
            FloatBlocksUp(0.5f);
        }
        else
        {
            if (CurDeleteNumber != 0 && IsShowCombScore)
            {
                BlockManager.Instance.GameScore += ((CurDeleteNumber * 5) * CombNumber);
                ChangeTextAnim(Text_ShowScore, BlockManager.Instance.GameScore);
                /*CreateCombScore((CurDeleteNumber * 5),()=> {
                    
                });*/
            }
            CheckThreeCreate();
            CheckGameFinish();
        }
    }
    //          1,0
    // 0,1      1,1        2,1
    //          1,2
    public void CheckBlockRoundColor(BlockBoardItem block, List<BlockBoardItem> list)
    {
        CheckBlockRoundOneColor(block.x, (block.y - 1), block, list);
        CheckBlockRoundOneColor((block.x - 1), block.y, block, list);
        CheckBlockRoundOneColor((block.x + 1), block.y, block, list);
        CheckBlockRoundOneColor(block.x, (block.y + 1), block, list);
    }
    /// <summary>
    /// 检查一个方块周围的颜色
    /// </summary>
    public void CheckBlockRoundOneColor(int x,int y,BlockBoardItem block, List<BlockBoardItem> list)
    {
        if (x >= 0 && y >= 0 && x < 9 && y < 9)
        {
            BlockBoardItem roundItem = gridArray[x, y].GetComponent<BlockBoardItem>();
            if (roundItem.Block != null  && block.colorIndex == roundItem.colorIndex && !list.Contains(roundItem))
            {
                list.Add(roundItem);
                CheckBlockRoundColor(roundItem, list);
            }
        }
    }

    /// <summary>
    /// 检测三个块是否有放置空间
    /// </summary>
    public bool CheckThreeBlockHavePos()
    {
        for (int i = 0; i < Array_Block.Length; i++)
        {
            if (Array_Block[i] != null)
            {
                var isHave = CheckThreeBlockOneHavePos(Array_Block[i]);
                if (isHave)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 检测三个生成块的其中一个是否有位置
    /// </summary>
    public bool CheckThreeBlockOneHavePos(GameObject obj)
    {
        foreach (var item in gridArray)
        {
            var block = item.GetComponent<BlockBoardItem>();
            if (block.Block == null)
            {
                var isPos =  CheckBlockPos(new Vector2Int(block.x, block.y), obj.GetComponent<DragMove>().List_BlockVector);
                if (isPos)
                {
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 渐层方块位置
    /// </summary>
    public bool CheckBlockPos(Vector2Int start, List<Vector2Int> vec)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        for (int i = 0; i < vec.Count; i++)
        {
            list.Add(new Vector2Int(start.x + vec[i].x, start.y + vec[i].y));
        }
        int index = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].x >= 0 && list[i].x < 9 && list[i].y >= 0 && list[i].y < 9 )
            {
                if (gridArray[list[i].x, list[i].y].GetComponent<BlockBoardItem>().Block == null)
                {
                    index++;
                }
            }
        }
        return index == list.Count;
    }
    /// <summary>
    /// 游戏复活
    /// </summary>
    public void GameResurrection()
    {
        for (int y = 0; y < 3; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (gridArray[x, y] != null  && gridArray[x,y].GetComponent<BlockBoardItem>().Block!= null)
                {
                    gridArray[x, y].GetComponent<BlockBoardItem>().ShowUIParticle();

                }
            }
        }
        IsShowCombScore = false;
        FloatBlocksUp(1);
        
    }

    //============== Comb =============
    public void CreateComb()
    {
        if (CombNumber != 1)
        {
            GameObject obj = Instantiate(CombObj.gameObject, CombParent.transform);
            obj.SetActive(true);
            obj.transform.localPosition = new Vector3(-342, 0, 0);
            obj.transform.localScale = Vector3.one;
            obj.transform.GetChild(0).GetComponent<Text>().text = "X " + CombNumber;
            Sequence seq = DOTween.Sequence();
            seq.Join(obj.transform.DOLocalMoveX(45, 0.5f));
            seq.AppendInterval(0.5f); // 添加1秒等待
            seq.OnComplete(() =>
            {
                Destroy(obj);
            });
        }
        

    }
    public void CreateCombScore(int score,Action action)
    {
        if (CombScoreObj != null)
        {
            CombScoreObj.transform.DOKill();
            Destroy(CombScoreObj);
            CombScoreObj = null;
        }
        GameObject obj = Instantiate(Text_CombScore.gameObject, CombScoreParent.transform);
        obj.SetActive(true);
        obj.transform.localPosition = Vector3.zero;
        obj.transform.localScale = Vector3.one;
        string str = CombNumber == 1?"": " X " + CombNumber; ;
        obj.GetComponent<Text>().text = score + str;
        CombScoreObj = obj;
        Sequence punchSequence = DOTween.Sequence();
        punchSequence.Append(obj.transform.DOPunchPosition(Vector3.up * 20, 0.5f, 1, 1));
        //punchSequence.AppendInterval(0.2f);
        punchSequence.Append(obj.transform.DOPunchPosition(Vector3.up * 20, 0.5f, 1, 1));
        punchSequence.OnComplete(() => {
            Destroy(obj);
            CombScoreObj = null;
            action?.Invoke();
        });
    }
    /// <summary>
    /// 改变分数Text动画
    /// </summary>
    /// <param name="text"></param>
    /// <param name="score"></param>
    public void ChangeTextAnim(Text text,int score)
    {
        int start = int.Parse(text.text);
        // 初始值
        int initialValue = start;
        text.text = initialValue.ToString();

        // 创建数值变化动画
        DOTween.To(() => initialValue,
                  x => {
                      text.text = Mathf.FloorToInt(x).ToString();
                  },
                  ( score), 0.3f);
    }
    /// <summary>
    /// 开启光效
    /// </summary>
    public void ChangeOpenLight(int objX,int objY)
    {
        for (int y = 0; y < 9; y++)
        {
            for (int x = 0; x < 9; x++)
            {
                if (gridArray[x, y]!=null && !gridArray[x, y].GetComponent<BlockBoardItem>().IsUse && x== objX && y< objY)
                {
                    gridArray[x, y].GetComponent<BlockBoardItem>().ChangeBlcokLight(true);
                }
            }
        }
    }

    public void ChangeCloseLight()
    {
        foreach (var item in gridArray)
        {
            item.GetComponent<BlockBoardItem>().ChangeBlcokLight(false);
        }
    }

}


public class FloatUpBlock
{
    public int x;
    public int y;
    public GameObject block;

    public FloatUpBlock(int x, int y, GameObject block)
    {
        this.x = x;
        this.y = y;
        this.block = block;
    }
}