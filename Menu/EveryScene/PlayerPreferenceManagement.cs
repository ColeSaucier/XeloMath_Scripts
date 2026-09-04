using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UI;
using TMPro;

public class PlayerPreferenceManagement : MonoBehaviour
{
    public string playerjsonFilePath;
    private string playerjsonString;
    private PlayerData playerObject;
    //public Image paceBarImage;

    public Toggle paceBarToggle;
    public Toggle leaderboardToggle;
    public Toggle timeToggle;

    public InputField userField;
    private string user;

    private bool settingsViewable = false;
    public CanvasGroup playerSettingsCanvasGroup;

    public SceneCompleteMenu sceneCompleteMenu_script;

    private GameObject UI_Bridge_Object = null;
    private UI_Scene_Bridge UI_Scene_Bridge__SCRIPT;
    public Toggle swipeHintToggle;


    private void Start()
    {
        playerObject = new PlayerData();
        LoadPlayerData();

        // Ensures all toggles reflect persistent player settings (on start)
        if (paceBarToggle != null)
        {
            paceBarToggle.isOn = playerObject.timeEnabled;
        }
        if (leaderboardToggle != null)
        {
            leaderboardToggle.isOn = playerObject.leaderboardEnabled;
        }
        if (timeToggle != null)
        {
            timeToggle.isOn = playerObject.timeEnabledNotPace;
        }

        if (playerObject.timeEnabled)
        {
            // Show the pace bar
            //paceBarImage.gameObject.SetActive(true);
            if (sceneCompleteMenu_script.beatScoreBar != null)
            {
                sceneCompleteMenu_script.HideShowPaceBarSC(true);
            }
        }
        else
        {
            // Hide the pace bar
            if (sceneCompleteMenu_script.beatScoreBar != null)
            {
                sceneCompleteMenu_script.HideShowPaceBarSC(false);
            }
        }

        if (playerObject.timeEnabledNotPace)
        {

        }
        else
        {
            if (sceneCompleteMenu_script.beatScoreBar != null)
            {
                // Hide the TextMeshPro objects
                sceneCompleteMenu_script.HideShowTimeTextsSC(false);
            }
        }

        if (playerObject.leaderboardEnabled)
        {

        }
        else
        {
            // Hide the TextMeshPro objects
            sceneCompleteMenu_script.leaderboardEnabled = false;
        }
        Find_UI_Bridge();

        if (swipeHintToggle != null)
        {
            swipeHintToggle.isOn = playerObject.swipeHint;
        }

    }
    public void Find_UI_Bridge()
    {
        UI_Bridge_Object = GameObject.Find("UI_Bridge");
        if (UI_Bridge_Object != null)
        {
            UI_Scene_Bridge__SCRIPT = UI_Bridge_Object.GetComponent<UI_Scene_Bridge>();
            if (UI_Scene_Bridge__SCRIPT == null)
            {
                //Debug.LogError("Bridge Failed");
            }
            else
            {
                //Sending saved SwipeHint Enabled bool to bridge
                //Match to setting
                UI_Scene_Bridge__SCRIPT.PassOnSwipeHint_ToAnimationSwipeHint(playerObject.swipeHint);
            }
        }
        else
        {
            //Debug.LogError("UI_Bridge object not found.");
        }
    }

    private void LoadPlayerData()
    {
        //Load the data
        string filePath = Path.Combine(Application.persistentDataPath, playerjsonFilePath);
        playerjsonString = File.ReadAllText(filePath);
        playerObject = JsonUtility.FromJson<PlayerData>(playerjsonString);
    }

    private void SavePlayerData()
    {
        //save the data
        string filePath = Path.Combine(Application.persistentDataPath, playerjsonFilePath);
        playerjsonString = JsonUtility.ToJson(playerObject);
        File.WriteAllText(filePath, playerjsonString);
    }

    public void ChangeSwipeHint_AndSave()
    {
        // Updates settings within AnimationSwipeHint to either have animation or not
        if (playerObject.swipeHint != swipeHintToggle.isOn)
        {
            playerObject.swipeHint = swipeHintToggle.isOn;
            UI_Scene_Bridge__SCRIPT.PassOnSwipeHint_ToAnimationSwipeHint(playerObject.swipeHint);
            SavePlayerData();            
        }
    }

    public void HideShowPaceBar()
    {
        if (paceBarToggle.isOn)
        {
            //Alter persistent data
            playerObject.timeEnabled = true;
            SavePlayerData();
            // Show the pace bar
            if (sceneCompleteMenu_script.beatScoreBar != null)
            {
                sceneCompleteMenu_script.HideShowPaceBarSC(true);
            }
            //paceBarImage.gameObject.SetActive(true);
        }
        else
        {
            //Alter persistent data
            playerObject.timeEnabled = false;
            SavePlayerData();
            if (sceneCompleteMenu_script.beatScoreBar != null)
            {
                sceneCompleteMenu_script.HideShowPaceBarSC(false);
            }
            //paceBarImage.gameObject.SetActive(false);
        }
    }

    public void HideShowTimeTexts()
    {
        //Alter persistent data
        if (timeToggle.isOn)
        {
            playerObject.timeEnabledNotPace = true;
            SavePlayerData();
            sceneCompleteMenu_script.HideShowTimeTextsSC(true);
        }
        else
        {
            playerObject.timeEnabledNotPace = false;
            SavePlayerData();
            sceneCompleteMenu_script.HideShowTimeTextsSC(false);
        }
    }

    public void HideShowLeaderboard()
    {
        //Alter persistent data
        if (leaderboardToggle.isOn)
        {
            playerObject.leaderboardEnabled = true;
            SavePlayerData();
            sceneCompleteMenu_script.leaderboardEnabled = true;
        }
        else
        {
            playerObject.leaderboardEnabled = false;
            SavePlayerData();
            sceneCompleteMenu_script.leaderboardEnabled = false;
        }
    }
    //NOT USED CURRENTLY
    public void UpdateUser()
    {
        if (userField != null)
        {
            playerObject.user = userField.text;
            SavePlayerData();
        }
    }

    public void HidePlayerSettingsCanvas()
    {
        settingsViewable = false;
        if (playerSettingsCanvasGroup != null)
        {
            playerSettingsCanvasGroup.interactable = false;
            playerSettingsCanvasGroup.alpha = 0f;
            playerSettingsCanvasGroup.blocksRaycasts = false;
        }
    }

    public void ShowPlayerSettingsCanvas()
    {
        if (!settingsViewable) //false
        {
            settingsViewable = true;
            if (playerSettingsCanvasGroup != null)
            {
                playerSettingsCanvasGroup.interactable = true;
                playerSettingsCanvasGroup.alpha = 1f;
                playerSettingsCanvasGroup.blocksRaycasts = true;
            }
        }
        else //true
        {
            HidePlayerSettingsCanvas();
        }
    }

    public void UpdateGold()
    {
        //playerObject.gemTotal 
        SavePlayerData();
    }

    [Serializable]
    public class PlayerData
    {
        public string user;
        public string menuText;
        public int gemTotal;
        public bool timeEnabled;
        public bool timeEnabledNotPace;
        public bool leaderboardEnabled;
        public bool swipeHint;
        public string swipeRight;
        public string swipeLeft;
        public string swipeDown;
        public string swipeUp;
    }
}