using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Reflection;
using AssetKits.ParticleImage;

public class SceneCompleteMenu : MonoBehaviour
{
    public Button Button1;
    public Button Button2;
    public Button Button3;
    public TextMeshProUGUI ratingText;
    public TextMeshProUGUI timeText;
    //public TextMeshProUGUI besttimeText;
    //public GameObject SceneCompleteCanvas;
    public TextMeshProUGUI repCounter;
    public CanvasGroup sceneCanvasGroup;
    public CanvasGroup buttonCanvasGroup;
    public int completionRating;

    public bool SceneComplete = false;
    public bool SceneWASCompleted = false;
    public float startTime;

    //For Data Collection/Saving
    public SceneData sceneObject;
    public string scenejsonFilePath;
    private string sceneJsonString;
    private VariableData variableObject;
    public string variablejsonFilePath;
    private string variableJsonString;
    private AllSceneRatingsData allSceneRatingObject;
    public string allSceneRatingsjsonFilePath;
    private string allSceneRatingsJsonString;
    private string filePath; 
    public string completedLevelTextFilePath;//"1.01 SubtractionV"

    List<string> levelOrder = new List<string> {"NumberCounting", "NumberCountingScattered", "BasicAdditionV", "BasicSubtractionV", "ShapePatterns", "SmallerOrBigger", "PlaceValues", "Clock", "AdditionV", "AdditionFunctionBox", "SubtractionFunctionBox", "MultiplicationV", "DivisionV", "NormalAddition", "NormalSubtraction", "LongMultiplication", "FractionFromShape", "FractionEqualize", "FractionEqualizeHard", "FractionReduction", "PercentEqualize", "LongDivision", "PEMDAS", "PemdasHard", "Exponent", "LineFormulation", "Factoring", "RollingHardProblems"};
    public string currentScene;// = text.gameObject.name;

    //Pace time bar vars
    public GameObject barHolder;
    public Image beatScoreBar;
    //public Image backgroundBar;
    public float repPaceTime;
    public float totalTimeToBeatScore;
    public float elapsedTime;
    public float elapsedTimeFinal;

    //scene complete animations + leaderboard vars
    public GameObject improveAnimation1;
    public GameObject improveAnimation2;
    public GameObject correctAnimation;
    public LeaderboardManager leaderboardScript;
    public float rounded_time;
    public bool leaderboardEnabled = true;

    //Animations/transition vars
    public SwipeHandler swipeHandler;
    //int levelIndex;

    public GameObject star1;
    public GameObject star2;
    public GameObject star3;

    public Image heartImage;
    public Sprite fullheartsprite;
    public Sprite heartsprite;

    // Start is called before the first frame update
    void Start()
    {
        sceneCanvasGroup.interactable = false;

        // Retrieve data and create dataObject
        sceneObject = new SceneData();
        LoadSceneData();
        //Debug.LogError($"sceneObject.bestRating {sceneObject.bestRating}");

        // Retrieve data and create dataObject
        variableObject = new VariableData();
        LoadVariableData();

        // Check if Animation Scene needs to be loaded
        CheckIfUiNeeded();
        DefineSwipe_AnimationHandler(); // Call the new function here

        // Updates repCounter, current scene, and starts counter
        repCounter.text = $"{variableObject.counterScene}/{sceneObject.numRepetitions}";
        currentScene = SceneManager.GetSceneByBuildIndex(swipeHandler.levelIndex).name;
        variableObject.currentScene = currentScene;
        SaveVariableData();

        // Wait for 3 seconds for animation/starttime
        bool enableOptionalWait = true;
        if (enableOptionalWait && variableObject.counterScene == 0)
        {
            StartCoroutine(WaitThreeSeconds());
        }

        // Vars for beatScoreBar
        startTime = Time.time;
        totalTimeToBeatScore = sceneObject.bestTime - variableObject.timeElapsed;
        repPaceTime = totalTimeToBeatScore / (sceneObject.numRepetitions - variableObject.counterScene);
        AssignImprovementWindowAttributes();
    }
    IEnumerator WaitThreeSeconds()
    {
        yield return new WaitForSeconds(2.99f);
        // Code to be executed after the wait, if any, would go here
    }

    private bool paceBarBool = true;
    // Update is called once per frame
    void Update()
    {
        if (SceneComplete)
        {
            // Used in Data collector, to determine if scene was completed
            SceneWASCompleted = true;
            SceneComplete = false;
            paceBarBool = false;

            elapsedTimeFinal = Time.time - startTime;
            variableObject.timeElapsed += elapsedTimeFinal;//(int)
            //Green shift for correct input
            Color32 shiftColor = new Color32(20, 210, 20, 50);
            StartCoroutine(ShowColoredImage(shiftColor, 0.35f));
        }
        else if (paceBarBool)
        {
            // checks if variable 
            elapsedTime = Time.time - startTime;
            // beatScoreBar Updating
            if (elapsedTime > totalTimeToBeatScore)
            {
                beatScoreBar.color = new Color32(210, 0, 0, 255);
                beatScoreBar.fillAmount = (float)(0.1);
            }
            else if (elapsedTime > repPaceTime)
            {
               beatScoreBar.color = Color.yellow;
            }
            else
            {
                //Debug.LogError((string)(0.1 + 0.9 * ((repPaceTime - elapsedTime) / repPaceTime)));
                beatScoreBar.fillAmount = (float)(0.1 + 0.9 * ((repPaceTime - elapsedTime) / repPaceTime));
            }
        }
    }

    public void DefineSwipe_AnimationHandler()
    {
        GameObject transitionAnimationsObject = GameObject.Find("TransitionAnimations");
        if (transitionAnimationsObject != null)
        {
            swipeHandler = transitionAnimationsObject.GetComponent<SwipeHandler>();
            if (swipeHandler == null)
            {
                //Debug.LogError("Animator component not found");
            }
            else
            {
                //Debug.LogError("Animator component found");
            }
        }
        else
        {
            //Debug.LogError("transitionAnimationsObject not found.");
        }
    }

    // Method to start the coroutine that creates a colored image
    public void DisplayColoredImage(Color32 color, float duration)
    {
        StartCoroutine(ShowColoredImage(color, duration));
    }

    public Sprite background;
    IEnumerator ShowColoredImage(Color32 color, float duration)
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("TemporaryCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 15;

        // Add a Panel (which is just an Image component with a default rect)
        GameObject panel = new GameObject("ColoredPanel");
        panel.transform.SetParent(canvas.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        
        // Load the "Background" sprite
        Sprite backgroundSprite = background; // Assumes the sprite is in a Resources folder

        // Set the sprite to the panel's Image component
        panelImage.sprite = backgroundSprite;
        panelImage.color = color;

        // Position and size the panel (you might adjust these values)
        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
        panelRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height); // Full screen size, adjust as needed

        // Wait for the duration
        yield return new WaitForSeconds(duration);
        sceneComplete();

        // Clean up
        Destroy(canvasObject);
    }


    void sceneComplete()
    {
        //Record the duration of one scene repetition, cumalatively into variableObject
        //Raise rep counter
        //variableObject.counterScene = 0;
        variableObject.counterScene++;
        repCounter.text = $"{variableObject.counterScene}/{sceneObject.numRepetitions}";

        //Determine if Reps are completed or to restart
        if (variableObject.counterScene != sceneObject.numRepetitions)
        {
            //saveData
            SaveVariableData();
            swipeHandler.triggerTransitionByUpdate = 0;
        }
        if (variableObject.counterScene == sceneObject.numRepetitions)
        {
            RepsCompleted();
        }
    }

    private Color32 ratingColor;
    async void RepsCompleted()
    {
        float current_time = variableObject.timeElapsed;
        rounded_time = (float)Math.Round(current_time, 2);

        // Record time in scene json
        if (current_time < sceneObject.bestTime)
        {
            sceneObject.bestTime = current_time;
            ImprovementWindowBool = true;
            //timeText.color = new Color32(135, 206, 235, 255); // Light Blue oorr new Color32(0, 0, 128, 255)

            improveAnimation1.SetActive(true);
            improveAnimation2.SetActive(true);
        }
        else
        {
            correctAnimation.SetActive(true);
        }


        // Determine Rating (+record it)
        completionRating = 1;
        star1.SetActive(true);
        Improvstar1.SetActive(true);
        if (sceneObject.bestTime <= sceneObject.goldTime)
        {
            star2.SetActive(true);
            Improvstar2.SetActive(true);
            completionRating = 2;
            //ratingText.color = new Color32(255, 165, 0, 255); // Orange
            ratingColor = new Color32(255, 165, 0, 255);

        }
        else if (sceneObject.bestTime <= sceneObject.perfTime)
        {
            Improvstar2.SetActive(true);
            Improvstar3.SetActive(true);
            star2.SetActive(true);
            star3.SetActive(true);
            completionRating = 3;
            //ratingText.color = new Color32(128, 0, 128, 255); // Purple
            ratingColor = new Color32(128, 0, 128, 255);
        }
        else
        {
            ratingColor = new Color32(210, 0, 0, 255);
        }

        UpdateValue_AllSceneRatings(currentScene, completionRating);

        //Assigns scene complete menu variables. This is the Set time.
        sceneObject.bestRating = completionRating.ToString();
        //ratingText.text = completionRating; 
        float minutes = variableObject.timeElapsed / 60;
        float seconds = variableObject.timeElapsed % 60;
        string timeString = $"{(int)minutes}min:{Math.Round(seconds, 2)}sec";
        timeText.text = timeString;

        //This is the Best time.
        minutes = sceneObject.bestTime / 60; //sceneObject.bestTime
        seconds = sceneObject.bestTime % 60;
        timeString = $"{(int)minutes}:{Math.Round(seconds, 2)}";
        //besttimeText.text = timeString;

        AssignImprovementWindowAttributes();

        //Reset variableObject
        variableObject.counterScene = 0;
        variableObject.timeElapsed = 0;
        variableObject.setId = variableObject.setId + 1;

        //saveData
        SaveVariableData(); 
        SaveSceneData();

        //save completed string
        filePath = Path.Combine(Application.persistentDataPath, completedLevelTextFilePath);
        AddCompletedScene(currentScene);
        //Debug.LogError("This didnt work");

        //Introduce the scene canvas
        sceneCanvasGroup.alpha = 1f;
        sceneCanvasGroup.interactable = true;
        buttonCanvasGroup.blocksRaycasts = true;
        //SceneCompleteCanvas.SetActive(true);

        if (leaderboardEnabled)
        {
            await leaderboardScript.SceneCompleteInsert();
            await leaderboardScript.SceneCompleteSupabase();
        }
        else
        {
            await leaderboardScript.SceneCompleteInsert();
            leaderboardScript.leaderboard.gameObject.SetActive(false);
        }
    }
    public void RestartScene()
    {
        //Reset variableObject
        variableObject.counterScene = 0;
        variableObject.timeElapsed = 0;
        SaveVariableData();
        swipeHandler.triggerTransitionByUpdate = 0;
    }
    public void GoToMenu()
    {
        variableObject = new VariableData();
        LoadVariableData();
        //Reset variableObject
        ResetVariableData();
        SceneManager.LoadScene("Menu");
    }
    public void GoToLevelSelect()
    {
        //Reset variableObject
        variableObject.counterScene = 0;
        variableObject.timeElapsed = 0;
        variableObject.currentScene = SceneManager.GetActiveScene().name;
        SaveVariableData();
        DefineSwipe_AnimationHandler(); 
        swipeHandler.triggerTransitionByUpdate_SceneName = "speed";

        //Debug.LogError("Nigga");
    }

    public void BeginPreviousScene()
    {
        ResetVariableData();
        swipeHandler.triggerTransitionByUpdate = -1;
        //StartCoroutine(swipeHandler.LoadLevel_SceneComplete(-1));
    }

    public void BeginNextScene()
    {
        /*
        if (MixIt.mixBool)
        {
            //Same as MixIt.StartRandomMixLevel() (In home screen)
            if (MixIt.mixList.Count > 0)
            {
                // Generate a random index within the bounds of mixList
                int randomIndex = UnityEngine.Random.Range(0, MixIt.mixList.Count);

                // Access the random entry from mixList
                string nextScene = MixIt.mixList[randomIndex];

                MixIt.mixList.Remove(nextScene);

                SceneManager.LoadScene(nextScene);
            }
            else
            {
                //go back to menu
                SceneManager.LoadScene("Speed");
            }
        }
        else
        {
            */
        //Update
        ResetVariableData();
        swipeHandler.triggerTransitionByUpdate = 1;
        /*
        filePath = Path.Combine(Application.persistentDataPath, completedLevelTextFilePath);
        List<string> completedLevelTextList = GetStringListFromFile();
        foreach (string level in completedLevelTextList)
        {
            //Debug.LogError(level);
        }
        string thisScene = SceneManager.GetActiveScene().name;
        //Debug.LogError(thisScene);
        int index = levelOrder.IndexOf(thisScene);
        string nextScene = null;

        // Check if the string was found and has a next element
        if (index != -1 && index < levelOrder.Count - 1)
        {
            // Get the next string in the list
            nextScene = levelOrder[index + 1];
        }
        // IF PLAYER HAS COMPLETED the scene
        if (completedLevelTextList.Contains(thisScene) || thisScene == "NumberCounting")
        {
            // If player has completed the current scene, load the next scene
            ResetVariableData();
            StartCoroutine(LoadLevel(nextScene));
        }
        else
        {
            //Debug.LogError("Scene not completed");
            // If player has not completed the scene, trigger animation
            // Else player has to complete this level

            //SceneManager.LoadScene(nextScene);
        }
        */
    }
    void ResetVariableData()
    {
        variableObject.counterScene = 0;
        variableObject.timeElapsed = 0;
        SaveVariableData();
    }
    /*
    IEnumerator LoadLevel(string levelIndex)
    {
        //play animation
        transition.SetTrigger("Start");

        //wait
        yield return new WaitForSeconds(0.1f);

        //load scene
        SceneManager.LoadScene(levelIndex);
    }
    */
    void AddCompletedScene(string sceneName)
    {
        //Adds the scene name to .txt list, after reps completed
        List<string> completedLevelTextList = GetStringListFromFile();
        completedLevelTextList.Add(sceneName);
        File.WriteAllLines(filePath, completedLevelTextList);
    }

    List<string> GetStringListFromFile()
    {
        if (File.Exists(filePath))
        {
            return new List<string>(File.ReadAllLines(filePath));
        }
        return new List<string>();
    }

    public void HideShowTimeTextsSC(bool enabled)
    {
        //Alter persistent data
        if (enabled)
        {
            // Show the TextMeshPro objects
            //besttimeText.gameObject.SetActive(true);
            timeText.gameObject.SetActive(true);
        }
        else
        {
            // Hide the TextMeshPro objects
            //besttimeText.gameObject.SetActive(false);
            timeText.gameObject.SetActive(false);
        }
    }
    public void HideShowPaceBarSC(bool enabled)
    {
        if (enabled)
        {
            // Show the pace bar
            beatScoreBar.gameObject.SetActive(true);
            //backgroundBar.color = new Color32(31, 31, 31, 255); //"#2B2B2B";
            
        }
        else
        {
            // Hide the pace bar
            beatScoreBar.gameObject.SetActive(false);
            //backgroundBar.color = new Color32(255, 255, 255, 255);
        }
    }

    void CheckIfUiNeeded()
    {
        int sceneCount = SceneManager.sceneCount;

        // Check if there's only one scene loaded
        if (sceneCount == 1)
        {
            // Load the scene at build index 1 additively, THIS IS MENU
            SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
            StartCoroutine(StartUp_UI(0));
        }
        else
        {
        }
        //Debug.LogError($"{SceneManager.GetActiveScene().name} is the current active scene.");
    }
    IEnumerator StartUp_UI(int levelIndex)
    {
        Scene newScene = SceneManager.GetSceneByBuildIndex(0);
        while (!newScene.isLoaded)
        {
            // If the scene is not loaded, wait for it or handle this situation appropriately
            yield return null; // Wait for the load operation if you're not already doing so
        }
    }

    //PopUp Improvement Window Vars
    public GameObject ImprovementWindow;
    public GameObject Improvstar1;
    public GameObject Improvstar2;
    public GameObject Improvstar3;
    public TextMeshProUGUI ImprovBestTime;
    public TextMeshProUGUI ImprovPrevTime;
    public TextMeshProUGUI ImprovPrevTime_StaticHeader;
    public TextMeshProUGUI ImprovPercentage;
    public TextMeshProUGUI ImprovBonusGoldText;
    private float PrevTime; //For storage to calculate percentage now
    public PlayerPreferenceManagement playerData_script; //Used to collect gold
    //public ParticleImage coinsssssss;
    private bool ImprovementWindowBool = false;
    public BoxCollider ImprovamentWindowcollider;

    private Color32 ratingColorPrevious;
    private int previousRatingInteger;

    //Star Highlights (on improvement of rating)
    public GameObject starHighlight1;
    public GameObject starHighlight2;
    public GameObject starHighlight3;


    void AssignImprovementWindowAttributes()
    {
        //PopUp Improvement Window Vars
        //Logic for before scene ever completed and getting variables 
        //Then executing the image post completeion ELSE
        if (!SceneWASCompleted)
        {
            //storage for calc later
            PrevTime = sceneObject.bestTime;
            float minutes = sceneObject.bestTime / 60; //sceneObject.bestTime
            float seconds = sceneObject.bestTime % 60;
            if ((int)minutes == 0)
                ImprovPrevTime.text = $"{Math.Round(seconds, 2)}sec";
            else
                ImprovPrevTime.text = $"{(int)minutes}min {Math.Round(seconds, 2)}sec";

            if (minutes != 10)
            {
                ratingColorPrevious = new Color32(207, 52, 35, 255);
                previousRatingInteger = 1;
            }
            else if (PrevTime <= sceneObject.goldTime)
            {
                ratingColorPrevious = new Color32(255, 165, 0, 255);
                previousRatingInteger = 2;
            }
            else if (PrevTime <= sceneObject.perfTime)
            {
                ratingColorPrevious = new Color32(128, 0, 128, 255);
                previousRatingInteger = 3;
            }
            else
            {
                ratingColorPrevious = new Color32(255, 255, 255, 255);
                previousRatingInteger = 0;
                ImprovPrevTime.text = "n/a";
                //ImprovPrevTime.color = new Color(ImprovPrevTime.color.r, ImprovPrevTime.color.g, ImprovPrevTime.color.b, 0);
                //ImprovPrevTime_StaticHeader.color = new Color(ImprovPrevTime_StaticHeader.color.r, ImprovPrevTime_StaticHeader.color.g, ImprovPrevTime_StaticHeader.color.b, 0);
            }
        }
        else
        {
            if (ImprovementWindowBool)
            {
                //Calculate and set displayed improvement texts and improvement bars
                float PercentageImprovement = (PrevTime - sceneObject.bestTime) / PrevTime * 100;

                float minutes = sceneObject.bestTime / 60; //sceneObject.bestTime
                float seconds = sceneObject.bestTime % 60;
                if ((int)minutes == 0)
                    ImprovBestTime.text = $"{Math.Round(seconds, 2)}sec";
                else
                    ImprovBestTime.text = $"{(int)minutes}min {Math.Round(seconds, 2)}sec";
                //ImprovBestTime.color = ratingColor;
                toggle_ImprovementWindow();

                //Debug.LogError("previousRatingInteger: "+ previousRatingInteger +"completionRating: "+ completionRating);

                //ImprovPercentage.text = $"Improvement: {Math.Round(PercentageImprovement, 2)}%";
                
                //float ImprovBonusGold = PercentageImprovement * 10;
                
                ////////// Show the bonus gold animation
                //ImprovBonusGoldText.text = $"Bonus Gold: {(int)ImprovBonusGold}";
                //coinsssssss.rateOverLifetime = 20;
                //coinsssssss.Play();
                //playerDataJson.gemTotal += ImprovBonusGold;
                Generate_improvementBars(improvementPercentage: PercentageImprovement);

                //Determine if rating has improved, to enable highlight in improvement window
                if (previousRatingInteger != completionRating)
                {
                    // Activate stars from the previous rating up to the new rating
                    for (int i = previousRatingInteger + 1; i <= completionRating; i++)
                    {
                        switch (i)
                        {
                            case 1:
                                starHighlight1.SetActive(true);
                                break;
                            case 2:
                                starHighlight2.SetActive(true);
                                break;
                            case 3:
                                starHighlight3.SetActive(true);
                                break;
                        }
                    }
                }
            }
        }
    }
    public void toggle_ImprovementWindow()
    {
        if (ImprovementWindow.activeSelf == true)
        {
            ImprovementWindow.SetActive(false);
        }
        else
        {
            ImprovementWindow.SetActive(true);
        }
    }
    


    // testing program for Improvement window
    void Testing_ImprovementBars()
    {
        sceneCanvasGroup.alpha = 1f;
        sceneCanvasGroup.interactable = true;
        buttonCanvasGroup.blocksRaycasts = true;
        
        ImprovementWindow.SetActive(true);

        //ImprovPercentage.text = $"Improvement: 15%";
        //float ImprovBonusGold = 10;
        //ImprovBonusGoldText.text = $"Bonus Gold: {(int)ImprovBonusGold}";
        //coinsssssss.rateOverLifetime = 20;
        //coinsssssss.Play();
        //playerDataJson.gemTotal += ImprovBonusGold;
        Generate_improvementBars(improvementPercentage: 15);
    }

    //Generate GameObjects
    public GameObject parentOfImprovementBars; // Assign this in the Unity Inspector or script
    private float variableHeight = 500f; // You can adjust this value
    private float width = 30f; // Static width for the example
    public Sprite BarsImprovementThing; // Assign a sprite in the inspector or script
    private Vector2 offset = new Vector2(-233.2f, 313f); // Offset from center
    private float xOffset = 265; // Not an x Offset
    //private 

    void Generate_improvementBars(float improvementPercentage)
    {
        float improvement = 1 - (improvementPercentage / 100);

        //Debug.LogError($"improvement {improvement}");
        // Create the image GameObject
        GameObject newImageObject = new GameObject("Improvement Bar Reference");


        
        // Add Image to Improvement Bar
        Image imageComponent = newImageObject.AddComponent<Image>();
        imageComponent.sprite = BarsImprovementThing; // Assign the sprite
        imageComponent.color = new Color32(255, 112, 67, 255);//ratingColorPrevious;

        // Set the transform for Improvement Bar
        RectTransform rectTransform = newImageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(variableHeight, width); // Set the size
        rectTransform.anchorMin = new Vector2(0.5f, 0f); // Center the image in the canvas
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        rectTransform.localScale = Vector3.one;

        // Set the parent of the new image object
        newImageObject.transform.SetParent(parentOfImprovementBars.transform, false);

        // Position the image within the parent canvas with an offset
        rectTransform.anchoredPosition = offset; // Apply the offset

        // Create the image GameObject
        GameObject diffImageObject = new GameObject("Improvement Bar ");
        
        // Add Image component
        imageComponent = diffImageObject.AddComponent<Image>();
        imageComponent.type = Image.Type.Filled;
        imageComponent.fillMethod = Image.FillMethod.Horizontal;
        imageComponent.fillAmount = improvement;
        imageComponent.sprite = BarsImprovementThing; // Assign the sprite
        imageComponent.color = new Color32(255, 112, 67, 255);//ratingColor;
        
        // Set the rect transform for the image
        rectTransform = diffImageObject.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(variableHeight, width); // Set the size
        rectTransform.anchorMin = new Vector2(0.5f, 0f); // Center the image in the canvas
        rectTransform.anchorMax = new Vector2(0.5f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.localEulerAngles = new Vector3(0, 0, 0);
        rectTransform.localScale = Vector3.one;

        // Set the parent of the new image object
        diffImageObject.transform.SetParent(parentOfImprovementBars.transform, false);

        // Position the image within the parent canvas with an offset
        //y offset is actually x offset
        //float yAdjustmentForFloor = ((variableHeight - (variableHeight * improvement))/2)*-1;
        rectTransform.anchoredPosition = offset + new Vector2(0, xOffset);
        //Debug.LogError($"Impotysny: {offset + new Vector2(0f - yAdjustmentForFloor, xOffset)}");

        //adjust for height

    }

    // This is the method the button will call
    public void UpdateHeart()
    { 
        bool heartfull = !sceneObject.heartScene;
        if (heartfull)
        {
            heartImage.sprite = fullheartsprite;
        }
        else
        {
            heartImage.sprite = heartsprite;
            //Debug.LogWarning("ImageChanger: Missing Image or Sprite reference!");
        }
        sceneObject.heartScene = heartfull;
        SaveSceneData();
    }
    [Serializable]
    public class SceneData
    {
        public float bestTime;
        public string bestRating;
        public int goldTime;
        public int perfTime;
        public int numRepetitions;
        public bool heartScene;
    }
    public void LoadSceneData()
    {
        filePath = Path.Combine(Application.persistentDataPath, scenejsonFilePath);
        sceneJsonString = File.ReadAllText(filePath);
        sceneObject = JsonUtility.FromJson<SceneData>(sceneJsonString);
    }
    public void SaveSceneData()
    {
        filePath = Path.Combine(Application.persistentDataPath, scenejsonFilePath);
        string sceneJsonString = JsonUtility.ToJson(sceneObject);
        File.WriteAllText(filePath, sceneJsonString);
    }

    [Serializable]
    public class VariableData
    {
        public string user;
        public string currentScene;
        public int counterScene;
        public float timeElapsed;
        public int setId;
    }
    public void LoadVariableData()
    {
        filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        variableJsonString = File.ReadAllText(filePath);
        variableObject = JsonUtility.FromJson<VariableData>(variableJsonString);
        //Debug.LogError("Load - variableJsonString: " + variableJsonString);
    }
    public void SaveVariableData()
    {
        filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        string variableJsonString = JsonUtility.ToJson(variableObject);
        File.WriteAllText(filePath, variableJsonString);
        //Debug.LogError("Saving - variableJsonString: " + variableJsonString);
    }

    [Serializable]
    public class AllSceneRatingsData
    {
        public string NumberCounting;
        public string NumberCountingScattered;
        public string BasicAddition;
        public string BasicSubtraction;
        public string ShapePatterns;
        public string SmallerOrBigger;
        public string Clock;
        public string PlaceValues;
        public string AdditionV;
        public string AdditionFunctionBox;
        public string SubtractionFunctionBox;
        public string NormalAddition;
        public string NormalSubtraction;
        public string MultiplicationV;
        public string DivisionV;
        public string LongMultiplication;
        public string FractionFromShape;
        public string FractionEqualize;
        public string FractionEqualizeHard;
        public string PercentEqualize;
        public string FractionReduction;
        public string LongDivision;
        public string PEMDAS;
        public string PemdasHard;
        public string Exponent;
        public string LineFormulation;
        public string Factoring;
        public string RollingHardProblems;
    }
    
    //For AllSceneRatingsData
    void UpdateValue_AllSceneRatings(string key, int rating)
    {
        //retrieve data and create dataObject
        allSceneRatingObject = new AllSceneRatingsData();
        filePath = Path.Combine(Application.persistentDataPath, allSceneRatingsjsonFilePath); // Combine with the Assets folder
        allSceneRatingsJsonString = File.ReadAllText(filePath);
        allSceneRatingObject = JsonUtility.FromJson<AllSceneRatingsData>(allSceneRatingsJsonString);

        Dictionary<string, string> allSceneRatingDictionary = new Dictionary<string, string>
        {
            { "NumberCounting", allSceneRatingObject.NumberCounting},
            { "NumberCountingScattered", allSceneRatingObject.NumberCountingScattered},
            { "BasicAdditionV", allSceneRatingObject.BasicAddition},
            { "BasicSubtractionV", allSceneRatingObject.BasicSubtraction},
            { "ShapePatterns", allSceneRatingObject.ShapePatterns},
            { "SmallerOrBigger", allSceneRatingObject.SmallerOrBigger},
            { "Clock", allSceneRatingObject.Clock},
            { "PlaceValues", allSceneRatingObject.PlaceValues},
            { "AdditionV", allSceneRatingObject.AdditionV},
            { "AdditionFunctionBox", allSceneRatingObject.AdditionFunctionBox},
            { "SubtractionFunctionBox", allSceneRatingObject.SubtractionFunctionBox},
            { "NormalAddition", allSceneRatingObject.NormalAddition},
            { "NormalSubtraction", allSceneRatingObject.NormalSubtraction},
            { "MultiplicationV", allSceneRatingObject.MultiplicationV},
            { "DivisionV", allSceneRatingObject.DivisionV},
            { "LongMultiplication", allSceneRatingObject.LongMultiplication},
            { "FractionFromShape", allSceneRatingObject.FractionFromShape},
            { "FractionEqualize", allSceneRatingObject.FractionEqualize},
            { "FractionEqualizeHard", allSceneRatingObject.FractionEqualizeHard},
            { "PercentEqualize", allSceneRatingObject.PercentEqualize},
            { "FractionReduction", allSceneRatingObject.FractionReduction},
            { "LongDivision", allSceneRatingObject.LongDivision},
            { "PEMDAS", allSceneRatingObject.PEMDAS},
            { "PemdasHard", allSceneRatingObject.PemdasHard},
            { "Exponent", allSceneRatingObject.Exponent},
            { "LineFormulation", allSceneRatingObject.LineFormulation},
            { "Factoring", allSceneRatingObject.Factoring},
            { "RollingHardProblems", allSceneRatingObject.RollingHardProblems}
        };

        allSceneRatingDictionary[key] = rating.ToString();
        ConvertDictionaryToJson__SaveIt(allSceneRatingDictionary);
    }

    public void ConvertDictionaryToJson__SaveIt(Dictionary<string, string> dictionary)
    {
        allSceneRatingObject.NumberCounting = dictionary["NumberCounting"];
        allSceneRatingObject.NumberCountingScattered = dictionary["NumberCountingScattered"];
        allSceneRatingObject.BasicAddition = dictionary["BasicAdditionV"];
        allSceneRatingObject.BasicSubtraction = dictionary["BasicSubtractionV"];
        allSceneRatingObject.ShapePatterns = dictionary["ShapePatterns"];
        allSceneRatingObject.SmallerOrBigger = dictionary["SmallerOrBigger"];
        allSceneRatingObject.Clock = dictionary["Clock"];
        allSceneRatingObject.PlaceValues = dictionary["PlaceValues"];
        allSceneRatingObject.AdditionV = dictionary["AdditionV"];
        allSceneRatingObject.AdditionFunctionBox = dictionary["AdditionFunctionBox"];
        allSceneRatingObject.SubtractionFunctionBox = dictionary["SubtractionFunctionBox"];
        allSceneRatingObject.NormalAddition = dictionary["NormalAddition"];
        allSceneRatingObject.NormalSubtraction = dictionary["NormalSubtraction"];
        allSceneRatingObject.MultiplicationV = dictionary["MultiplicationV"];
        allSceneRatingObject.DivisionV = dictionary["DivisionV"];
        allSceneRatingObject.LongMultiplication = dictionary["LongMultiplication"];
        allSceneRatingObject.FractionFromShape = dictionary["FractionFromShape"];
        allSceneRatingObject.FractionEqualize = dictionary["FractionEqualize"];
        allSceneRatingObject.FractionEqualizeHard = dictionary["FractionEqualizeHard"];
        allSceneRatingObject.PercentEqualize = dictionary["PercentEqualize"];
        allSceneRatingObject.FractionReduction = dictionary["FractionReduction"];
        allSceneRatingObject.LongDivision = dictionary["LongDivision"];
        allSceneRatingObject.PEMDAS = dictionary["PEMDAS"];
        allSceneRatingObject.PemdasHard = dictionary["PemdasHard"];
        allSceneRatingObject.Exponent = dictionary["Exponent"];
        allSceneRatingObject.LineFormulation = dictionary["LineFormulation"];
        allSceneRatingObject.Factoring = dictionary["Factoring"];
        allSceneRatingObject.RollingHardProblems = dictionary["RollingHardProblems"];

        //saveData
        filePath = Path.Combine(Application.persistentDataPath, allSceneRatingsjsonFilePath);
        string allSceneRatingsJsonString = JsonUtility.ToJson(allSceneRatingObject);
        File.WriteAllText(filePath, allSceneRatingsJsonString);
    }
}