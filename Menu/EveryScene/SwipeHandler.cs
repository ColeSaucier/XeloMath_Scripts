using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using UnityEngine.EventSystems;

public class SwipeHandler : MonoBehaviour
{
    public Animator transition;

    // Minimum distance for a swipe to be registered
    public float minSwipeDistance = 75f;

    // Maximum time allowed for a swipe
    public float maxSwipeTime = 1f;

    private Vector2 startPosition;
    private float startTime;
    private bool swipeValid = false;

    int swipeUpScene;
    int swipeDownScene;
    int swipeRightScene;
    int swipeLeftScene;

    public int levelIndex;

    private VariableData variableObject;
    public string variablejsonFilePath;
    private string variableJsonString;
    private string filePath;
    public string playerjsonFilePath;
    private string playerjsonString;
    private PlayerData playerObject;

    public string redoLevelTrigger;
    public string nextLevelTrigger;

    public float animationTime = 0.25f;

    public int triggerTransitionByUpdate = -11;
    public string triggerTransitionByUpdate_SceneName = "";

    private bool swipeDown = true;
    private bool swipeUp = true;
    private bool swipeLeft = true;
    private bool swipeRight = true;

    public bool swipeAllowedTimeBuffer = true;
    private float timeBuffer;
    private float timeBufferEnd;

    public string completedLevelTextFilePath;

    private int GetSceneIndex(string action, string trigger)
    {
        if (actionToSceneIndex.TryGetValue(action, out int index))
        {
            if (action == "Reload Level")
            {
                redoLevelTrigger = trigger;
            }
            else if (action == "Next Level")
            {
                nextLevelTrigger = trigger;
            }
            return index;
        }
        //Debug.LogError("Action not found in swipe association dictionary.");
        return 0; // Default to 0 if action not found
    }
    private readonly Dictionary<string, int> actionToSceneIndex = new Dictionary<string, int>
    {
        {"Reload Level", 0},
        {"Next Level", 1},
        {"Previous Level", -1},
        {"Level Selector", 11}
    };

    void Start()
    {
        CheckIfOnlyScene();
        //Indexs (scene) mean something according to player configured swipe variables
        // 0: Load Current Scene, 1: Load next scene, -1: Load previous scene
        // 11: Load scene selector, 10 Load menu, 12? Load leaderboard/stats

        //retrieve data and create dataObject
        playerObject = new PlayerData();
        LoadPlayerData();
        VariableData variableObject = new VariableData();
        LoadVariableData();

        // Set swipeDown
        swipeDownScene = GetSceneIndex(playerObject.swipeDown, "DownSwipe");
        //  //Debug.LogError("playerObject.swipeDown" + playerObject.swipeDown + "null prolly");

        // Set swipeUp
        swipeUpScene = GetSceneIndex(playerObject.swipeUp, "UpSwipe");

        // Set swipeLeft
        swipeLeftScene = GetSceneIndex(playerObject.swipeLeft, "LeftSwipe");
        // Set swipeRight
        swipeRightScene = GetSceneIndex(playerObject.swipeRight, "RightSwipe");
    }
    void HandleLevelSelectieSPEED()
    {        
        // Get Active Scene
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Speed")
        {
            //Set enable swipe bool to false 
            swipeDown = false;
            swipeUp = false;
            swipeLeft = false;
            swipeRight = false;

            //Find opposite direction scene and set it as 17(which goes to previously current scene), then enable that swipe direction
            if (playerObject.swipeDown == "Level Selector")
            {
                swipeUpScene = 17;
                swipeUp = true;
            }
            if (playerObject.swipeUp == "Level Selector")
            {
                swipeDownScene = 17;
                swipeDown = true;
            }
            if (playerObject.swipeLeft == "Level Selector")
            {
                swipeRightScene = 17;
                swipeRight = true;
            }
            if (playerObject.swipeRight == "Level Selector")
            {
                swipeLeftScene = 17;
                swipeLeft = true;
            }
        }
        else
        {
            //Reset Swipes
            swipeDown = true;
            swipeUp = true;
            swipeLeft = true;
            swipeRight = true;
            //retrieve data and create dataObject
            playerObject = new PlayerData();
            LoadPlayerData();
            swipeDownScene = GetSceneIndex(playerObject.swipeDown, "DownSwipe");
            swipeUpScene = GetSceneIndex(playerObject.swipeUp, "UpSwipe");
            swipeLeftScene = GetSceneIndex(playerObject.swipeLeft, "LeftSwipe");
            swipeRightScene = GetSceneIndex(playerObject.swipeRight, "RightSwipe");
        }
    }

    void CheckIfOnlyScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        int sceneCount = SceneManager.sceneCount;

        // Check if there's only one scene loaded
        if (sceneCount == 1)
        {
            if (currentScene == "Menu")
            {
                // Load the scene at build index 0 additively, THIS IS UI
                SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
                StartCoroutine(StartUp_EnsureAdditiveScene(0));
            }
            else
            {
                //Debug.LogError("Current Scene: " + currentScene);
                // Load the scene at build index 1 additively, THIS IS MENU
                SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
                StartCoroutine(StartUp_EnsureAdditiveScene(1));
            };
        }
        else
        {
            //Debug.LogError("More than one scene is already loaded, not loading additional scene.");
        }
        //Debug.LogError($"{SceneManager.GetActiveScene().name} is the current active scene.");

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {

            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        }
        //Debug.LogError($"NOW..........{SceneManager.GetActiveScene().name} is the current active scene.");
    }
    void Update()
    {
        // Handling of scenes where 
        //if (SceneManager.GetActiveScene().buildIndex != 0)
        //{
        if (swipeAllowedTimeBuffer)
        {
            LookForWipe();
        }
        else
        {
            timeBufferEnd = Time.time;
            if ((timeBufferEnd - timeBuffer)> 0.35f)
            {
                swipeAllowedTimeBuffer = true;
            }
        }
    }

    void LookForWipe()
    {
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);
            
            // Detect the start of a touch
            if (touch.phase == TouchPhase.Began)
            {
                //Debug.LogError("Touch Begin");
                startPosition = touch.position;
                startTime = Time.time;

                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = touch.position;
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                // Check if any object is selected by the touch
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (results.Count > 0) {
                    foreach (RaycastResult result in results) {
                        // Check if the hit object has the tag you want to ignore
                        if (result.gameObject.CompareTag("IgnoreSwipe") || result.gameObject.CompareTag("temporary")) {
                            swipeValid = false;
                            //Debug.LogError("Swipe ignored because an object with tag 'YourIgnoreTag' is selected.");
                            return; // Exit the method after setting swipeValid to false
                        }
                    }
                }
                // Check if any object is selected by the touch
                else if (Physics.Raycast(ray, out hit))// && hit.collider.CompareTag("temporary")) //for proper swipe detection
                {
                    //DEBUGGING logs for proper swipe detection
                    //string hitObjectName = hit.collider.gameObject.name;
                    //Debug.LogError("Name of hit object: " + hitObjectName);
                    //Debug.LogError("Name of hit tag: " + hit.collider.gameObject.tag);

                    //swipe should NOT transition
                    swipeValid = false;
                    //Debug.LogError("Swipe ignored because an object is selected.");
                }
                else
                {
                    //swipe SHOULD transition
                    swipeValid = true;
                    //Debug.LogError("Swipe valid");
                }
            }
            // Detect the end of a touch
            else if (touch.phase == TouchPhase.Ended)
            {
                //Debug.LogError("Touch End");
                //check if moveable or temp object selected? the detect swipe
                if (swipeValid)
                {
                    DetectSwipe(touch);
                }
            }
        }
        // Trigger for scene complete (allows for tranition animations when buttons pressed)
        else if (triggerTransitionByUpdate != -11)
        {
            StartCoroutine(LoadLevel_SceneComplete(triggerTransitionByUpdate));
            triggerTransitionByUpdate = -11;
        }
        // Trigger for speed
        else if (triggerTransitionByUpdate_SceneName != "")
        {
            StartCoroutine(StartAnyScene_NoTransition(triggerTransitionByUpdate_SceneName));
            triggerTransitionByUpdate_SceneName = "";
        }
    }

    void DetectSwipe(Touch touch)
    {
        // Calculate the distance of the swipe
        Vector2 endPosition = touch.position;
        Vector2 swipeDelta = endPosition - startPosition;
        // Check if the swipe is long enough
        if (swipeDelta.magnitude > minSwipeDistance)
        {
            // Check if the swipe was quick enough
            if (Time.time - startTime < maxSwipeTime)
            {
                //TransitionToNextLevel();
            
                // Determine the direction of the swipe
                float x = swipeDelta.x;
                float y = swipeDelta.y;

                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    // Horizontal swipe
                    if (x > 0)
                    {
                        if (swipeRight)
                            TransitionToNextLevel(trigger : "RightSwipe", sceneDirection: swipeRightScene);
                    }
                    else
                    {
                        if (swipeLeft)
                            TransitionToNextLevel(trigger : "LeftSwipe", sceneDirection: swipeLeftScene);
                    }
                }
                else
                {
                    // Vertical swipe
                    if (y > 0)
                    {
                        if (swipeUp)
                            TransitionToNextLevel(trigger : "UpSwipe", sceneDirection: swipeUpScene);
                        //SceneCompleteMenu.BeginNextScene();
                    }
                    else
                    {
                        if (swipeDown)
                            TransitionToNextLevel(trigger : "DownSwipe", sceneDirection: swipeDownScene);
                        //SceneCompleteMenu.BeginPreviousScene();
                    }
                }
                swipeAllowedTimeBuffer = false;
                timeBuffer = Time.time;
            }
        }
    }

    public void TransitionToNextLevel(string trigger, int sceneDirection)
    {
        int currentSceneIndex;

        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        //Debug.LogError("5555555555555 sceneDirection: " + sceneDirection);

        //Check if scene
        // Menu or UIanimation Swipes must always go to first level // currentSceneIndex 0 = UIANIMATIONLAYER
        if (currentSceneIndex == 0 || currentSceneIndex == 1)
        {
            LoadVariableData();
            if (variableObject.currentScene != null)
            {
                if (variableObject.currentScene == "Menu") //|| variableObject.currentScene == "Speed")
                {
                    string lastLevel = ReturnLastCompletedLevel(completedLevelTextFilePath);
                    if (lastLevel == "")
                    {
                        //Debug.LogError("1 This probably wont be triggered");
                        levelIndex = SceneManager.GetActiveScene().buildIndex + 1;
                    }
                    else
                    {
                        levelIndex = SceneUtility.GetBuildIndexByScenePath(lastLevel);
                        //Debug.LogError("2 lastLevel/levelIndex: " + lastLevel+levelIndex);
                    }
                }
                else
                {
                    levelIndex = SceneUtility.GetBuildIndexByScenePath(variableObject.currentScene);
                    //Debug.LogError("3 SwipeHandler variableObject.currentScene: " + variableObject.currentScene);
                }
            }
            else
            {
                //Debug.LogError("4 No variableObject.currentScene, No lastLevel from list either");
                levelIndex = SceneManager.GetActiveScene().buildIndex + 1;
            }
        }
        else
        {
            //Debug.LogError("5 No variableObject.currentScene, No lastLevel from list either");
            // 0: Load Current Scene, 1: Load next scene, -1: Load previous scene
            // 11: Load scene selector, 10 Load menu, 12? Load leaderboard/stats
            if (sceneDirection == 1)
            {
                levelIndex = SceneManager.GetActiveScene().buildIndex + 1;
            }
            else if (sceneDirection == -1)
            {
                levelIndex = SceneManager.GetActiveScene().buildIndex - 1;
            }
            else if (sceneDirection == 0)
            {
                //Debug.LogError("0000000 No variableObject.currentScene, No lastLevel from list either");
                levelIndex = SceneManager.GetActiveScene().buildIndex;
            }
            else if (sceneDirection == 10)
            {
                levelIndex = 1;
            }
            else if (sceneDirection == 11)
            {
                //Level select will be final scene for forseable future
                levelIndex = SceneManager.sceneCountInBuildSettings - 1;
            }
            else if (sceneDirection == 17)
            {
                // Previous scene, used in Speed Level select
                //Debug.LogError($"SCENEEEEEEE {variableObject.currentScene}");
                levelIndex = SceneUtility.GetBuildIndexByScenePath(variableObject.currentScene);
            }
        }
        //Reset variable data is only used in SwipeHandlers version of TransitiontoNextLevel 
        ResetVariableData();
        StartCoroutine(LoadLevel(levelIndex, trigger));
    }
    IEnumerator LoadLevel(int levelIndex, string trigger)
    {
        // Store the current scene before transition
        Scene currentScene = SceneManager.GetActiveScene();

        if (transition == null)
        {
            //Debug.LogError("Animator component not found or assigned!");
        }

        //Delete temporary objects
        GameObject[] del_objects = GameObject.FindGameObjectsWithTag("temporary");
        foreach (GameObject del_object in del_objects) {
            Destroy(del_object);    
        }
        // Trigger the scene transition animation
        transition.SetTrigger(trigger);
        
        // Wait for the transition animation to finish or for a short delay
        yield return new WaitForSeconds(animationTime); // Adjust based on your animation duration

        if (currentScene.buildIndex != 0)
        {
            // Ensure there is a menu or other level to unload, NOT animation layer
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);

            // Wait until the current scene is fully unloaded
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        // Load the new scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);

        // Wait until the new scene is loaded
        while (!loadOp.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelIndex));

        HandleLevelSelectieSPEED();
    }
    public IEnumerator LoadLevel_SceneComplete(int sceneDirection)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        // 0: Load Current Scene, 1: Load next scene, -1: Load previous scene
        // 11: Load scene selector, 10 Load menu, 12? Load leaderboard/stats
        if (sceneDirection == 1)
        {
            levelIndex = currentScene.buildIndex + 1;
            if (nextLevelTrigger != null)
            {
                transition.SetTrigger(nextLevelTrigger);
            }
            else
            {
                transition.SetTrigger("DownSwipe");
            }
        }
        else if (sceneDirection == -1)
        {
            levelIndex = currentScene.buildIndex - 1;
        }
        else if (sceneDirection == 0)
        {
            if (redoLevelTrigger != null)
            {
                //play animation - redo level associated
                transition.SetTrigger(redoLevelTrigger);               
            }
            else
            {
                transition.SetTrigger("DownSwipe");
            }
            levelIndex = currentScene.buildIndex;
        }
        else if (sceneDirection == 10)
        {
            levelIndex = 1;
        }
        else if (sceneDirection == 11)
        {
            //Level selection scene is the final scene
            levelIndex = SceneManager.sceneCountInBuildSettings - 1;
        }

        //Delete temporary objects
        GameObject[] del_objects = GameObject.FindGameObjectsWithTag("temporary");
        foreach (GameObject del_object in del_objects) {
            Destroy(del_object);    
        }

        //wait
        yield return new WaitForSeconds(animationTime);

        // Unload Scene (scenecomplete menu assumed)
        AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);
        // Wait until the current scene is fully unloaded
        while (!unloadOp.isDone)
        {
            yield return null;
        }

        // Load the new scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(levelIndex, LoadSceneMode.Additive);
        // Wait until the new scene is loaded
        while (!loadOp.isDone)
        {
            yield return null;
        }
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelIndex));
    }

    IEnumerator StartUp_EnsureAdditiveScene(int levelIndex)
    {
        Scene newScene = SceneManager.GetSceneByBuildIndex(levelIndex);
        while (!newScene.isLoaded)
        {
            // If the scene is not loaded, wait for it or handle this situation appropriately
            yield return null; // Wait for the load operation if you're not already doing so
        }

        // Now try to set the scene active
        if (newScene.isLoaded)
        {
            SceneManager.SetActiveScene(newScene);
        }
        else
        {
            //Debug.LogError("Failed to set active scene: Scene not valid or not loaded.");
            yield break; // Exit coroutine since we can't proceed
        }
        SceneManager.SetActiveScene(newScene);
        //Debug.LogError($"{newScene.name} is now the active scene.");
    }

    public IEnumerator StartAnyScene_NoTransition(string sceneName)
    {
        Scene currentScene = SceneManager.GetActiveScene();
        
        if (currentScene.buildIndex != 0)
        {
            // Ensure scene is menu or other level to unload, NOT animation layer
            AsyncOperation unloadOp = SceneManager.UnloadSceneAsync(currentScene);

            // Wait until the current scene is fully unloaded
            while (!unloadOp.isDone)
            {
                yield return null;
            }
        }

        //Delete temporary objects
        GameObject[] del_objects = GameObject.FindGameObjectsWithTag("temporary");
        foreach (GameObject del_object in del_objects) {
            Destroy(del_object);    
        }
        
        // Load the new scene additively
        AsyncOperation loadOp = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

        // Wait for the new scene to finish loading
        while (!loadOp.isDone)
        {
            yield return null;
        }

        //God knows this must be the perfect order
        int buildIndex = SceneManager.GetSceneByName(sceneName).buildIndex;

        // Start the coroutine from sceneHandlerScript to handle post-load actions
        yield return StartCoroutine(StartUp_EnsureAdditiveScene(buildIndex));
        HandleLevelSelectieSPEED();
    }



    private void LoadPlayerData()
    {
        //Load the data
        string filePath = Path.Combine(Application.persistentDataPath, playerjsonFilePath);
        playerjsonString = File.ReadAllText(filePath);
        playerObject = JsonUtility.FromJson<PlayerData>(playerjsonString);
        //Debug.LogError("playerjsonString - Load: " + playerjsonString);
    }

    private void SavePlayerData()
    {
        //save the data
        string filePath = Path.Combine(Application.persistentDataPath, playerjsonFilePath);
        playerjsonString = JsonUtility.ToJson(playerObject);
        File.WriteAllText(filePath, playerjsonString);
    }
    private void LoadVariableData()
    {
        //Load the data
        string filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        variableJsonString = File.ReadAllText(filePath);
        variableObject = JsonUtility.FromJson<VariableData>(variableJsonString);
        //Debug.LogError("SwipeHandler variableJsonString - Load: " + variableJsonString);
    }

    private void SaveVariableData()
    {
        //save the data
        string filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        variableJsonString = JsonUtility.ToJson(variableObject);
        File.WriteAllText(filePath, variableJsonString);
        //Debug.LogError("variableJsonString - Save: " + variableJsonString);
    }
    void ResetVariableData()
    {
        variableObject.counterScene = 0;
        variableObject.timeElapsed = 0;
        variableObject.currentScene = SceneManager.GetActiveScene().name;
        SaveVariableData();
    }

    // Helper function to get the list of strings from the file
    private List<string> GetStringListFromFile()
    {
        if (File.Exists(filePath))
        {
            return new List<string>(File.ReadAllLines(filePath));
        }
        return new List<string>();
    }

    // Function to return the last completed level
    public string ReturnLastCompletedLevel(string completedLevelTextFilePath)// ReturnLastCompletedLevel(completedLevelTextFilePath)
    {
        filePath = Path.Combine(Application.persistentDataPath, completedLevelTextFilePath);
        
        List<string> completedLevelTextList = GetStringListFromFile();
        
        // Check if there are any completed levels
        if (completedLevelTextList.Count > 0)
        {
            // Return the last item in the list, which represents the newest completed level
            return completedLevelTextList[completedLevelTextList.Count - 1];
        }
        else
        {
            return "";
        }
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
