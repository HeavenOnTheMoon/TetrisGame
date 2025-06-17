using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HomePanel : MonoBehaviour
{
    public Button StartButton;
    void Start()
    {
        StartButton.onClick.AddListener(()=> {
            BlockManager.Instance.ShowOrHidePanel(PanelType.HomePanel, false);
            BlockManager.Instance.ShowOrHidePanel(PanelType.GamePanel, true);
        });
    }

    
}
