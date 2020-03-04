using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Slider PlayerSlider;
    public Slider BotSlider;
    public Slider RoundsSlider;

    public Text PlayerValue;
    public Text BotValue;
    public Text RoundValue;

    public GameManager Manager;

    public GameObject PauseMenu;

    public void SelectGameMode(int GameMode)
    {

    }

    public void ApplyConfigSettings()
    {
        Debug.Log(PlayerSlider.value);
        Debug.Log(BotSlider.value);

        Manager.ApplyGameSettings(Mathf.RoundToInt(PlayerSlider.value), Mathf.RoundToInt(BotSlider.value), Mathf.RoundToInt(RoundsSlider.value));
        Manager.StartNewGame();
    }

    public void ReturnToMenu()
    {
        Manager.ReturnToMenu();
    }

    public void UpdateValueText()
    {
        PlayerValue.text = PlayerSlider.value.ToString();
        BotValue.text = BotSlider.value.ToString();
        RoundValue.text = RoundsSlider.value.ToString();

        if (PlayerSlider.value > 1)
        {
            BotSlider.minValue = 0;
        }
        else
        {
            BotSlider.minValue = 1;
        }

        if (PlayerSlider.value < 2)
        {
            BotSlider.maxValue = 7;
        }
        else
        {
            BotSlider.maxValue = 6;
        }
    }

    public void PauseGame(float _TimeScale)
    {
        Manager.SetGameTimeScale(_TimeScale);
    }

    public void QuitApplication()
    {
        #if UNITY_EDITOR

                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

}
