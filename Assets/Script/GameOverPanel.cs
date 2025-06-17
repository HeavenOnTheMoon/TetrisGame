using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    public Button ADButton;
    public Button RewardButton;
    public Text IconText;
    public Text ScoreText;
    void Start()
    {
        ADButton.onClick.AddListener(()=> {
            //int score = BlockManager.Instance.GameScore * 2;
            //GetReward(score);
            
        });

        RewardButton.onClick.AddListener(()=> {
            int score = BlockManager.Instance.GameScore / 100;
            GetReward(score);
        });
    }

    private void OnEnable()
    {
        SetCoinInit();
    }

    public void SetCoinInit()
    {
        int score = BlockManager.Instance.GameScore / 100;
        IconText.text = score.ToString();
        ScoreText.text = BlockManager.Instance.GameScore.ToString();
    }

    public void GetReward(int score)
    {
        BlockManager.Instance.AddScore(score);
        BlockManager.Instance.ShowOrHidePanel(PanelType.GameOverPanel, false);
        GamePanel.Instance.GameInit();
    }

    
}
