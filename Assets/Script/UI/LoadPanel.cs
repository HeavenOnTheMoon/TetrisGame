using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadPanel : MonoBehaviour
{
    public Image SliderBg;
    public Image SliderImg;
    public Button StartButton;
    public Text SliderText;

    private bool isSlider;
    void Start()
    {
        isSlider = true;
        SliderImg.fillAmount = 0;
        SliderBg.gameObject.SetActive(true);
        StartButton.gameObject.SetActive(false);
        StartButton.onClick.AddListener(()=> {
            BlockManager.Instance.ShowOrHidePanel(PanelType.LoadPanel, false);
            BlockManager.Instance.ShowOrHidePanel(PanelType.GamePanel, true);
        });
        SliderText.text = "0%";
    }

    
    void Update()
    {
        if(isSlider)
        {
            SliderImg.fillAmount += Time.deltaTime / 3f;
            SliderText.text = (int)(SliderImg.fillAmount * 100) + "%";
            if (SliderImg.fillAmount >= 1)
            {
                //StartButton.gameObject.SetActive(true);
                BlockManager.Instance.ShowOrHidePanel(PanelType.LoadPanel, false);
                BlockManager.Instance.ShowOrHidePanel(PanelType.HomePanel,true);
                SliderBg.gameObject.SetActive(false);
                isSlider = false;
            }
        }
        
    }
}
