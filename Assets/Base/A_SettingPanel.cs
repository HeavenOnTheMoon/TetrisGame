using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>  </summary>
public class A_SettingPanel : MonoBehaviour
{
    public Sprite On;
    public Sprite Off;
    public Button MusicBtn;
    public Button SoundBtn;
    public Text VersionText;

    private void Start()
    {
        Application.targetFrameRate = 60;

        MusicBtn.image.sprite = PlayerPrefs.GetInt("Music", 1) == 1 ? On : Off;
        SoundBtn.image.sprite = PlayerPrefs.GetInt("Sound", 1) == 1 ? On : Off;

        VersionText.text = "Version : " + Application.version;
    }

    public void Music()
    {
        int music = PlayerPrefs.GetInt("Music", 1);
        music = music == 1 ? 0 : 1;
        PlayerPrefs.SetInt("Music", music);
        MusicBtn.image.sprite = music == 1 ? On : Off;
        A_AudioManager.Instance.ToggleMusic();
    }
    public void Sound()
    {
        int sound = PlayerPrefs.GetInt("Sound", 1);
        sound = sound == 1 ? 0 : 1;
        PlayerPrefs.SetInt("Sound", sound);
        SoundBtn.image.sprite = sound == 1 ? On : Off;
        A_AudioManager.Instance.ToggleSound();
    }
}
