using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using Client = Supabase.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

public class InputUserHandling : MonoBehaviour
{
    public TMP_InputField firstNameInputField;
    public TMP_InputField ageInputField;
    public TMP_Dropdown dropdown;
    public TMP_Dropdown dropdownGender;
    public TMP_InputField usernameInputField;
    public TMP_InputField haveUsernameInputField;

    public Image firstNameImageC;
    public Image ageImageC;
    public Image usernameImageC;
    public Image dropdownImageC;
    public Image alreadyusernameImageC;

    public Image firstNameImageX;
    public Image ageImageX;
    public Image usernameImageX;
    public Image alreadyusernameImageX;
    private Client supabase;

    public CanvasGroup canvas;

    private VariableData variableObject;
    public string variablejsonFilePath;
    private string variableJsonString;
    private string filePath;
    public string playerjsonFilePath;
    private string playerjsonString;
    private PlayerData playerObject;
    public SceneData sceneObject;
    public string scenejsonFilePath;
    private string sceneJsonString;
    private AllSceneRatingsData allSceneRatingObject;
    public string allSceneRatingsjsonFilePath;
    private string allSceneRatingsJsonString;

    public TextMeshProUGUI MenuGradeText;
    public TextMeshProUGUI MenuUsername;

    public List<string> completedLevelTextList;
    public string completedLevelTextFilePath;

    private void Start()
    {
        playerObject = new PlayerData();
        LoadPlayerData();

        supabase = new Client("https://crynucdigbnxdsnywawe.supabase.co", "sb_publishable_rXQIggagT9rQEGPMiGnyIg_JXVp2yww");
        if (playerObject.user == "null")
        {
            canvas.alpha = 1f;
            canvas.interactable = true;
            canvas.blocksRaycasts = true;
        }
        else
        {
            MenuGradeText.text = playerObject.menuText;
            playerObject.menuText = MenuGradeText.text;
            SavePlayerData();
            MenuUsername.text = $"Player/Username: {playerObject.user}";
            //Debug.LogError($"Player/Username: {playerObject.user}");
            //THIS SETS GO BUTTON'S "CURRENT LEVEL"
            variableObject = new VariableData();
            LoadVariableData();
            variableObject.user = playerObject.user;
            string currentScene = variableObject.currentScene;
            SaveVariableData();
            //Debug.LogError(variableObject.currentScene);
            //Debug.LogError(currentScene);
            
            filePath = Path.Combine(Application.persistentDataPath, completedLevelTextFilePath);
            List<string> completedLevelTextList = GetStringListFromFile();
            string gradeText = MenuGradeText.text;
            // Check if the grade level text needs an update based on the length of the current text
            if (gradeText.Length < 5)
            {
                // Determine grade level based on completed levels
                if (completedLevelTextList.Contains("LongDivision"))
                {
                    MenuGradeText.text = "Math Goat";
                }
                else if (completedLevelTextList.Contains("LongMultiplication"))
                {
                    MenuGradeText.text = "4th";
                }
                else if (completedLevelTextList.Contains("MultiplicationV"))
                {
                    MenuGradeText.text = "3rd";
                }
                else if (completedLevelTextList.Contains("SmallerOrBigger"))
                {
                    MenuGradeText.text = "2nd";
                }
                else if (completedLevelTextList.Contains("BasicSubtractionV"))  // Fixed incorrect string check
                {
                    MenuGradeText.text = "1st";
                }
            }
        }
    }
    List<string> GetStringListFromFile()
    {
        if (File.Exists(filePath))
        {
            return new List<string>(File.ReadAllLines(filePath));
        }
        return new List<string>();
    }

    public void ValidateFirstName()
    {
        // Check if first name is not empty
        if (!string.IsNullOrEmpty(firstNameInputField.text))
        {
            // Capitalize the first letter
            string capitalizedFirstName = char.ToUpper(firstNameInputField.text[0]) + firstNameInputField.text.Substring(1).ToLower();
            
            // Set the input field text to the capitalized version
            firstNameInputField.text = capitalizedFirstName;

            // Activate the checkmark image
            firstNameImageC.gameObject.SetActive(true);
        }
    }

    public void ValidateAge()
    {
        // Check if age is a valid integer
        if (ageInputField.text != "")
        {
            if (int.TryParse(ageInputField.text, out int parsedAge))
            {
                // Check if the parsed age is within a reasonable range, e.g., 0 to 120 years
                if (parsedAge >= 0 && parsedAge <= 120) 
                {
                    ageImageC.gameObject.SetActive(true);
                    ageImageX.gameObject.SetActive(false);
                }
                else
                {
                    ageImageX.gameObject.SetActive(true);
                    ageImageC.gameObject.SetActive(false);
                }
            }
        }
        else
        {
            ageImageX.gameObject.SetActive(true);
            ageImageC.gameObject.SetActive(false);
        }
    }

    public void CreatePossibleUsername()
    {
        // Autofill username if first name and age are complete
        if (firstNameInputField.text != "" && ageInputField.text != "") 
        {
            usernameInputField.text = firstNameInputField.text + ageInputField.text;
        }
        ValidateUsername();
    }

    public async void ValidateUsername()
    {
        if (firstNameInputField.text != "" && ageInputField.text != "") 
        {
            if (!(await ValidateUsernameSupabase(usernameInputField)))
            {
                //Debug.LogError("User not exists");
                usernameImageC.gameObject.SetActive(true);
                usernameImageX.gameObject.SetActive(false);
            }
            else
            {
                //Debug.LogError("User exists");    
                usernameImageX.gameObject.SetActive(true);
                usernameImageC.gameObject.SetActive(false);
            }
        }
    }

    public async void ValidateAlreadyUsername()
    {
        bool exists = await ValidateUsernameSupabase(haveUsernameInputField);
        if (exists)
        {
            //Debug.LogError("User exists"); 
            alreadyusernameImageC.gameObject.SetActive(true);
            alreadyusernameImageX.gameObject.SetActive(false);
        }
        else
        {
            //Debug.LogError("User not exists");
            alreadyusernameImageX.gameObject.SetActive(true);
            alreadyusernameImageC.gameObject.SetActive(false);
        }
    }

    // Helper method to validate inputs for EnterGame()
    private bool IsInputValid()
    {
        return !string.IsNullOrEmpty(firstNameInputField.text) &&
               !string.IsNullOrEmpty(ageInputField.text) &&
               !string.IsNullOrEmpty(usernameInputField.text) &&
               int.TryParse(ageInputField.text, out _); // Check if age is a valid integer
    }

    public async void EnterGame()
    {
        if (usernameImageC.gameObject.activeSelf || alreadyusernameImageC.gameObject.activeSelf)
        {
            //Handle updates to persistance files
            variableObject = new VariableData();
            LoadVariableData();
            //Player data should be loaded from start()

            //case when player already has a username (and fills it correctly)
            if (alreadyusernameImageC.gameObject.activeSelf)
            {
                variableObject.user = haveUsernameInputField.text;
                playerObject.user = haveUsernameInputField.text;
                MenuGradeText.text = playerObject.menuText; //Default "math goat"
                MenuUsername.text = $"Player/Username: {playerObject.user}";
                GetBestRatingForALLLevels_ByUser(haveUsernameInputField.text);

                SaveVariableData();
            }
            //case when player fills out new user form and already user not filled
            else if (IsInputValid() & usernameImageC.gameObject.activeSelf)
            {
                // Assuming InsertUserAsync takes a username, firstname, and age
                await InsertUserAsync();
                variableObject.user = usernameInputField.text;
                playerObject.user = usernameInputField.text;
                MenuUsername.text = $"Player/Username: {playerObject.user}";

                int value = dropdown.value;
                // Get the text of the current selected item, based on the index
                string GradeText = dropdown.options[value].text;

                if (GradeText != "n/a")
                {
                    MenuGradeText.text = GradeText;
                    playerObject.menuText = GradeText;
                    if (GradeText == "1st")
                    {
                        variableObject.currentScene = "BasicAdditionV";
                    }
                    else if (GradeText == "2nd")
                    {
                        variableObject.currentScene = "SmallerOrBigger";
                    }
                    else if (GradeText == "3rd")
                    {
                        variableObject.currentScene = "NormalAddition";
                    }
                    else if (GradeText == "4th")
                    {
                        variableObject.currentScene = "LongMultiplication";
                    }
                }
                else
                {
                    MenuGradeText.text = "Math Goat";
                    playerObject.menuText = "Math Goat";
                    variableObject.currentScene = "NormalAddition";
                }
            }

            //saveData ####
            SaveVariableData();
            SavePlayerData();

            //Both
            canvas.alpha = 0f;
            canvas.interactable = false;
            canvas.blocksRaycasts = false;
        }
    }
    // Class to represent a all players best level entry
    private class PlayerSceneDatas
    {
        public string level { get; set; }
        public decimal duration { get; set; }
    }
    private async Task GetBestRatingForALLLevels_ByUser(string user)
    {
        // Call the get_leaderboard_data function
        var baseResponse = await supabase.Rpc("get_update_user_data", new Dictionary<string, object>
        {
            { "p_user_name", user }
        });
        Debug.LogError("RPC call for DATA!!!!!!!!!!!!!!!!!!!");


        // Check if response is successful and contains data
        if (baseResponse != null)
        {
            Debug.LogError("FOLLOWED THROUGH RPC call for DATA!!!!!!!!!!!!!!!!!!!");
            // Parse the JSON string into a list of leaderboard entries
            var playerSceneDatas = JsonConvert.DeserializeObject<List<PlayerSceneDatas>>(baseResponse.Content.ToString());

            // Loop through each leaderboard entry (best performance)
            Dictionary<string, string> allSceneDataDictionary = new Dictionary<string, string>();  //Dictionary for All Scene data 
            for (int i = 0; i < playerSceneDatas.Count; i++)
            {
                try
                {
                    //Update scene object
                    scenejsonFilePath = playerSceneDatas[i].level + ".json";
                    float durationOfParticularLevel = (float)playerSceneDatas[i].duration;
                    LoadSceneData();


                    //Determine Rating
                    int completionRating = 1;
                    if (durationOfParticularLevel <= sceneObject.goldTime)
                    {
                        completionRating = 2;
                    }
                    else if (durationOfParticularLevel <= sceneObject.perfTime)
                    {
                        completionRating = 3;
                    }

                    //Change Rating
                    sceneObject.bestRating = completionRating.ToString();
                    sceneObject.bestTime = durationOfParticularLevel;
                    Debug.LogError($"playerSceneDatas[i].level: {playerSceneDatas[i].level}, completionRating: {completionRating.ToString()}");
                    allSceneDataDictionary.Add(playerSceneDatas[i].level, completionRating.ToString());
                    //Save
                    SaveSceneData();

                    // Add level to completed levels
                    Debug.LogError("GETTING TO THE IMPORTANT SHIT");
                    completedLevelTextList.Add(playerSceneDatas[i].level.ToString());
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Crash in loop index {i}: {ex.Message}\nStack: {ex.StackTrace}");
                }
            }

            // Save (overwrites file, creates if missing)
            //string filePath = Application.persistentDataPath + "/CompletedLevel.txt";
            //File.WriteAllLines(filePath, completedLevelTextList);

            // Convert list to a single string (one level per line)
            string levelsString = string.Join(Environment.NewLine, completedLevelTextList);
            Debug.LogError($"Saving? {levelsString}");

            // Full path
            string filePath = Path.Combine(Application.persistentDataPath, "CompletedLevel.txt");
            Debug.LogError($"Saving? {levelsString}");

            // Save (overwrites file, creates if missing)
            File.WriteAllText(filePath, levelsString);

            Debug.LogError("FOLLOWED THROUGH2 RPC call for DATA!!!!!!!!!!!!!!!!!!!");
            ConvertDictionaryToJson__SaveIt_RegardlessOfNull(allSceneDataDictionary);
        }
        else
        {
            //Debug.LogError("Failed to fetch user data: " + baseResponse.ResponseMessage.StatusCode);
        }
    }
    private void LoadAllSceneRatingsData()
    {
        //retrieve data and create dataObject
        filePath = Path.Combine(Application.persistentDataPath, allSceneRatingsjsonFilePath); // Combine with the Assets folder
        allSceneRatingsJsonString = File.ReadAllText(filePath);
        allSceneRatingObject = JsonUtility.FromJson<AllSceneRatingsData>(allSceneRatingsJsonString);
    }
    public void ConvertDictionaryToJson__SaveIt_RegardlessOfNull(Dictionary<string, string> dictionary)
    {
        allSceneRatingObject = new AllSceneRatingsData();

        // Helper to get value or default "0"
        string GetOrZero(string key)
        {
            return dictionary.TryGetValue(key, out var value) ? value : "0";
        }

        // Assign every field — missing ones become "0"
        allSceneRatingObject.NumberCounting          = GetOrZero("NumberCounting");
        allSceneRatingObject.NumberCountingScattered = GetOrZero("NumberCountingScattered");
        allSceneRatingObject.BasicAddition           = GetOrZero("BasicAdditionV");          // note: key has V
        allSceneRatingObject.BasicSubtraction        = GetOrZero("BasicSubtractionV");
        allSceneRatingObject.ShapePatterns           = GetOrZero("ShapePatterns");
        allSceneRatingObject.SmallerOrBigger         = GetOrZero("SmallerOrBigger");
        allSceneRatingObject.Clock                   = GetOrZero("Clock");
        allSceneRatingObject.PlaceValues             = GetOrZero("PlaceValues");
        allSceneRatingObject.AdditionV               = GetOrZero("AdditionV");
        allSceneRatingObject.AdditionFunctionBox     = GetOrZero("AdditionFunctionBox");
        allSceneRatingObject.SubtractionFunctionBox  = GetOrZero("SubtractionFunctionBox");
        allSceneRatingObject.NormalAddition          = GetOrZero("NormalAddition");
        allSceneRatingObject.NormalSubtraction       = GetOrZero("NormalSubtraction");
        allSceneRatingObject.MultiplicationV         = GetOrZero("MultiplicationV");
        allSceneRatingObject.DivisionV               = GetOrZero("DivisionV");
        allSceneRatingObject.LongMultiplication      = GetOrZero("LongMultiplication");
        allSceneRatingObject.FractionFromShape       = GetOrZero("FractionFromShape");
        allSceneRatingObject.FractionEqualize        = GetOrZero("FractionEqualize");
        allSceneRatingObject.FractionEqualizeHard    = GetOrZero("FractionEqualizeHard");
        allSceneRatingObject.PercentEqualize         = GetOrZero("PercentEqualize");
        allSceneRatingObject.FractionReduction       = GetOrZero("FractionReduction");
        allSceneRatingObject.LongDivision            = GetOrZero("LongDivision");
        allSceneRatingObject.PEMDAS                  = GetOrZero("PEMDAS");
        allSceneRatingObject.PemdasHard              = GetOrZero("PemdasHard");
        allSceneRatingObject.Exponent                = GetOrZero("Exponent");
        allSceneRatingObject.LineFormulation         = GetOrZero("LineFormulation");
        allSceneRatingObject.Factoring               = GetOrZero("Factoring");
        allSceneRatingObject.RollingHardProblems     = GetOrZero("RollingHardProblems");

        // Save as before
        filePath = Path.Combine(Application.persistentDataPath, allSceneRatingsjsonFilePath);
        string allSceneRatingsJsonString = JsonUtility.ToJson(allSceneRatingObject, true); // true = pretty print
        File.WriteAllText(filePath, allSceneRatingsJsonString);
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
    private void LoadVariableData()
    {
        //Load the data
        string filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        variableJsonString = File.ReadAllText(filePath);
        variableObject = JsonUtility.FromJson<VariableData>(variableJsonString);
        //Debug.LogError("variableJsonString - Load: " + variableJsonString);
    }

    private void SaveVariableData()
    {
        //save the data
        string filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        variableJsonString = JsonUtility.ToJson(variableObject);
        File.WriteAllText(filePath, variableJsonString);
        //Debug.LogError("variableJsonString - Save: " + variableJsonString);
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
        Debug.LogError($"LOADED scenedata: {sceneJsonString}");
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
    public async Task InsertUserAsync()
    {
        //Debug.LogError($"line inserted!!");
        int index = dropdown.value;
        // Get the text of the current selected item, based on the index
        string GradeselectedText = dropdown.options[index].text;

        index = dropdownGender.value;
        // Get the text of the current selected item, based on the index
        string GenderselectedText = dropdownGender.options[index].text;

        var parameters = new Dictionary<string, object>
        {
            { "p_user_name", usernameInputField.text },
            { "p_firstname", firstNameInputField.text },
            { "p_age", ageInputField.text },
            { "p_grade", GradeselectedText },
            { "p_gender", GenderselectedText }
        };

        try
        {
            var response = await supabase.Rpc("insert_player", parameters);
            //Debug.LogError("User inserted successfully.");
        }
        catch (Exception ex)
        {
            //Debug.LogError($"Error inserting user: {ex.Message}");
        }
    }

    public void UpdateDropdownOptions()
    {
        dropdownImageC.gameObject.SetActive(true);
    }

    public async Task<bool> ValidateUsernameSupabase(TMP_InputField input)
    {
        string username = input.text.ToString();
        // Call the CheckUserNameValid function using Supabase's RPC
        Debug.LogError($"username {username}");
        var response = await supabase.Rpc("checkusernamevalidtest", new Dictionary<string, object>
        {
            { "username_param", username} 
        });
        if (response != null)
        {
            //Debug.LogError($"Making RPC Call with: {JsonConvert.SerializeObject(new Dictionary<string, object> { { "username_param", username } })}");
            // Parse the result from the response
            bool isValid = JsonConvert.DeserializeObject<bool>(response.Content.ToString());
            //Debug.LogError($"Output: {isValid}");
            return isValid;
        }
        else
        {
            // Handle error - response is null
            //Debug.LogError("Failed to call CheckUserNameValid function.");
            return false;
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
}