using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevivePanel : MonoBehaviour
{
    public Button AdButton;
    public Button CloseButton;
    public GameObject Coin;
    public GameObject AD;
    private bool IsCoin;
    void Start()
    {
        AdButton.onClick.AddListener(()=> {
            if (IsCoin)
            {
                BlockManager.Instance.SubScore(100);
                BlockManager.Instance.ShowOrHidePanel(PanelType.RevivePanel, false);
                GamePanel.Instance.GameResurrection();
            }
            else
            {
                A_ADManager.Instance.playRewardVideo((ok) => {
                    if (ok)
                    {
                        BlockManager.Instance.ShowOrHidePanel(PanelType.RevivePanel, false);
                        GamePanel.Instance.GameResurrection();
                    }
                });
               
            }
        });

        CloseButton.onClick.AddListener(()=> {
            BlockManager.Instance.ShowOrHidePanel(PanelType.RevivePanel, false);
            BlockManager.Instance.ShowOrHidePanel(PanelType.GameOverPanel, true);
        });
    }


    private void OnEnable()
    {
        int coin = BlockManager.Instance.GameAllCoin;
        Coin.SetActive((coin >= 100));
        AD.SetActive((coin < 100));
        IsCoin = coin >= 100;
    }

    private void OnDisable()
    {
        BlockManager.Instance.IsUseDrag = true;
        BlockManager.Instance.IsUseRevive = false;
    }
}
