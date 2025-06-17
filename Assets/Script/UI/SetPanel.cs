using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetPanel : MonoBehaviour
{
    public Button MusicBtn;
    public Button SoundBtn;
    public Button CloseBtn;
    public Image MusicIcon;
    public Image SoundIcon;
    public Button HomeBtn;

    public Sprite MusicBGSprite;
    public Sprite SoundBGSprite;
    public Sprite OffBGSprite;
    public Sprite MusicOffSprite;
    public Sprite MusicOnSprite;
    public Sprite SoundOffSprite;
    public Sprite SoundOnSprite;
    private void Start()
    {
        Application.targetFrameRate = 60;

        MusicBtn.image.sprite = PlayerPrefs.GetInt("Music", 1) == 1 ? MusicBGSprite : OffBGSprite;
        MusicIcon.sprite = PlayerPrefs.GetInt("Music", 1) == 1 ? MusicOnSprite : MusicOffSprite;
        SoundBtn.image.sprite = PlayerPrefs.GetInt("Sound", 1) == 1 ? SoundBGSprite : OffBGSprite;
        SoundIcon.sprite = PlayerPrefs.GetInt("Music", 1) == 1 ? SoundOnSprite : SoundOffSprite;
        MusicBtn.onClick.AddListener(()=> {
            Music();
        });

        SoundBtn.onClick.AddListener(()=> {
            Sound();
        });

        CloseBtn.onClick.AddListener(()=> {
            BlockManager.Instance.ShowOrHidePanel(PanelType.SetPanel,false);
        });

        HomeBtn.onClick.AddListener(()=> {
            BlockManager.Instance.ShowOrHidePanel(PanelType.SetPanel, false);
            BlockManager.Instance.ShowOrHidePanel(PanelType.GamePanel, false);
            BlockManager.Instance.ShowOrHidePanel(PanelType.HomePanel, true);
        });
    }

    public void Music()
    {
        int music = PlayerPrefs.GetInt("Music", 1);
        music = music == 1 ? 0 : 1;
        PlayerPrefs.SetInt("Music", music);
        MusicBtn.image.sprite = music == 1 ? MusicBGSprite : OffBGSprite;
        MusicIcon.sprite = music == 1 ? MusicOnSprite : MusicOffSprite;
        A_AudioManager.Instance.ToggleMusic();
    }
    public void Sound()
    {
        int sound = PlayerPrefs.GetInt("Sound", 1);
        sound = sound == 1 ? 0 : 1;
        PlayerPrefs.SetInt("Sound", sound);
        SoundBtn.image.sprite = sound == 1 ? SoundBGSprite : OffBGSprite;
        SoundIcon.sprite = sound == 1 ? SoundOnSprite : SoundOffSprite;
        A_AudioManager.Instance.ToggleSound();
    }

    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }
}
