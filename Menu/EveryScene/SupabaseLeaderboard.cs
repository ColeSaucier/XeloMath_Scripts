using Supabase;
using System.Threading.Tasks;
using UnityEngine;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using TMPro;
using Client = Supabase.Client;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using System;
using System.IO;
using Postgrest.Models;
using UnityEngine.UI;   // already added

public class LeaderboardManager : MonoBehaviour
{
    private Client supabase;
    public TextMeshProUGUI[] nameTexts; // Array to hold TextMeshPro objects for display
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI[] durationTexts;
    public TextMeshProUGUI[] rankTexts;
    public TextMeshProUGUI[] ratingTexts;

    private SceneData sceneObject;
    private string scenejsonFilePath;
    private string sceneJsonString;
    private string filePath;
    private VariableData variableObject;
    public string variablejsonFilePath;
    private string variableJsonString;

    private PlayerData playerObject;
    public string playerjsonFilePath;
    private string playerJsonString;

    public SceneCompleteMenu sceneCompleteMenu_script;
    public string currentScene;

    public TextMeshProUGUI medianText;
    public TextMeshProUGUI percentileText;

    public Canvas leaderboard;
    public CanvasGroup leaderboardCanvasGroup;

    private ActivityInsertDelayed activityInsertObject;
    private HeartboolInsertDelayed heartInsertObject;

    private void Start()
    {
        // Initialize the Supabase client
        supabase = new Client("https://crynucdigbnxdsnywawe.supabase.co", "sb_publishable_rXQIggagT9rQEGPMiGnyIg_JXVp2yww");//retrieve data and create dataObjects
        sceneObject = new SceneData();
        scenejsonFilePath = sceneCompleteMenu_script.scenejsonFilePath;
        filePath = Path.Combine(Application.persistentDataPath, scenejsonFilePath);
        sceneJsonString = File.ReadAllText(filePath);
        sceneObject = JsonUtility.FromJson<SceneData>(sceneJsonString);
        variableObject = new VariableData();
        filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        variableJsonString = File.ReadAllText(filePath);
        variableObject = JsonUtility.FromJson<VariableData>(variableJsonString);
    }

    public async Task SceneCompleteInsert()
    {
        filePath = Path.Combine(Application.persistentDataPath, variablejsonFilePath);
        variableJsonString = File.ReadAllText(filePath);
        variableObject = JsonUtility.FromJson<VariableData>(variableJsonString);
        
        // Get the current scene
        currentScene = sceneCompleteMenu_script.currentScene;

        //Debug.LogError("variableObject.user, " + variableObject.user);

        // Insert activity set into the database
        await FormatActivity_SetInsert(variableObject.setId, variableObject.user, currentScene, sceneCompleteMenu_script.rounded_time);
    }
    public async Task SceneCompleteSupabase()
    {
        // Get user rank
        int userRank = await GetUserRank(currentScene, variableObject.user);
        Debug.LogError($"User rank.{userRank}");

        // Get leaderboard data
        await GetLeaderboardData(userRank, currentScene);

        // Update median and percentile
        await UpdateMedianAndPercentile(currentScene, variableObject.user);

        highlightPlayerRowRed(userRank);
        leaderboardCanvasGroup.alpha = 1f;
    }

    public async Task InsertHeartBool(string level, string username, bool heart)
    {

        var parameters = new Dictionary<string, object>
        {
            { "username_param", level},
            { "level_param", username},
            { "like_bool_param", heart}
        };

        try
        {
            var response = await supabase.Rpc("insert_heartbool", parameters);
            //Debug.LogError("User inserted successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to insert heartbool set due to an exception: {ex.Message}");
            //IMPO: Save activity to json object then file.

            heartInsertObject = new HeartboolInsertDelayed();
            // Set ActivityInsert() values from args
            heartInsertObject.level = level;
            heartInsertObject.username = username;
            heartInsertObject.heart = heart;



            int i = 0;
            string currentFilePath = Path.Combine(Application.persistentDataPath, $"HeartboolInsertDelayed{i}.json");//"ActivityInsert0.json"

            //file already exists, find empty value
            while (File.Exists(currentFilePath))
            {
                i++;
                currentFilePath = Path.Combine(Application.persistentDataPath, $"HeartboolInsertDelayed{i}.json");
            }
            //file does not exist
            // Save to JSON file
            string json = JsonUtility.ToJson(heartInsertObject);
            File.WriteAllText(currentFilePath, json);

            // Handle error
            Debug.LogError("Failed to insert activity set. SAVED...");
            //Debug.LogError("currentFilePath: "  + currentFilePath);
        }
    }
    [Serializable]
    public class HeartboolInsertDelayed
    {
        public string level;
        public string username;
        public bool heart;
    }

    public async Task<int> GetUserRank(string level, string username)
    {
        // Call the get_user_rank function using Supabase's RPC
        var rankResponse = await supabase.Rpc("get_user_rank", new Dictionary<string, object>
        {
            { "level_param", level },
            { "user_param", username }
        });

        if (rankResponse.Content.ToString() != null)
        {
            // Parse the rank from the response
            var rank = JsonConvert.DeserializeObject<int>(rankResponse.Content.ToString());
            return rank;
        }
        else
        {
            // Handle error
            //Debug.LogError("Failed to retrieve user rank.");
            return -1; // Return a default value or handle the error case accordingly
        }
    }

    private async Task GetLeaderboardData(int number, string level)
    {
        if (number <= 6)
            number = 6;
        // Call the get_leaderboard_data function
        var baseResponse = await supabase.Rpc("get_leaderboard_data", new Dictionary<string, object>
        {
            { "level_param", level },
            { "rank_param", number }
        });

        levelText.text = currentScene;
        //Debug.LogError("Levelt tect: "+currentScene);
        // Check if response is successful and contains data
        if (baseResponse != null)
        {
            // Parse the JSON string into a list of leaderboard entries
            var leaderboardEntries = JsonConvert.DeserializeObject<List<LeaderboardEntry>>(baseResponse.Content.ToString());

            // Loop through each leaderboard entry and update TextMeshPro objects
            for (int i = 0; i < leaderboardEntries.Count && i < nameTexts.Length; i++)
            {
                // Ensure the corresponding TextMeshPro object exists
                if (nameTexts[i] != null)
                {
                    // Update TextMeshPro text with leaderboard entry details
                    nameTexts[i].text = $"{leaderboardEntries[i].firstname1}";
                }
                // Update duration text
                if (durationTexts[i] != null)
                {
                    CalculateRatingsForLeaderboard(leaderboardEntries[i].duration1, ratingTexts[i]);
                    durationTexts[i].text = $"{leaderboardEntries[i].duration1}";
                }
                // Update rank text
                if (rankTexts[i] != null)
                {
                    rankTexts[i].text = $"{leaderboardEntries[i].rank1}";
                }
            }
        }
        else
        {
            // Handle error
            Debug.LogError("Failed to fetch leaderboard data.");
        }
    }

    public void CalculateRatingsForLeaderboard(decimal duration, TextMeshProUGUI TextMeshObject)
    {
        // Determine Rating
        int numberOfStars = 1;
        if (duration <= sceneObject.goldTime)
        {
            numberOfStars = 2;
            //ompletionRating = "Epic";
            //TextMeshObject.color = new Color32(255, 165, 0, 255); // Orange
        }
        if (duration <= sceneObject.perfTime)
        {
            numberOfStars = 3;
            //completionRating = "Legendary";
            //
        }
        UpdateStarRatings(TextMeshObject, numberOfStars);
        //TextMeshObject.text = completionRating;
    }

    public Vector3 centerPointOffset = Vector3.zero; // Public variable to adjust the center point
    public Sprite starSprite; // Public reference to set the star image
    public float starScale = 1.0f; // Public variable to adjust star size
    public float localOffset; // Example value, adjust as needed
    private Color32 starcolor;
    public void UpdateStarRatings(TextMeshProUGUI textUGUI, int numberOfStars)
    {
        string ratingString = textUGUI.gameObject.name;
        
        // Get the position of the text and adjust with the public offset
        Vector3 centerPoint = centerPointOffset;
        switch (numberOfStars)
        {
            case 0:
                // Do nothing
                break;
            case 1: 
                starcolor = new Color32(207, 52, 35, 255); // Red
                GenerateStar(centerPoint, $"{ratingString}_Star1", textUGUI.gameObject, starcolor);
                break;
            case 2:
                starcolor = new Color32(255, 165, 0, 255); // Orange
                GenerateStar(centerPoint + Vector3.left * localOffset, $"{ratingString}_Star1", textUGUI.gameObject, starcolor);
                GenerateStar(centerPoint + Vector3.right * localOffset, $"{ratingString}_Star2", textUGUI.gameObject, starcolor);
                break;
            case 3:
                starcolor = new Color32(128, 0, 128, 255); // Purple
                // Positions of the stars
                float usedOffset = localOffset * (float)2.0;
                Vector3 star1Position = centerPoint + Vector3.left * usedOffset;
                Vector3 star2Position = centerPoint; // Right
                Vector3 star3Position = centerPoint + Vector3.right * usedOffset; // Left

                // Generate the stars
                GenerateStar(star1Position, $"{ratingString}_Star1", textUGUI.gameObject, starcolor); // Star 1
                GenerateStar(star2Position, $"{ratingString}_Star2", textUGUI.gameObject, starcolor); // Star 2
                GenerateStar(star3Position, $"{ratingString}_Star3", textUGUI.gameObject, starcolor); // Star 3

                // Calculate distances between the stars
                //loat distance1_2 = Vector3.Distance(star1Position, star2Position); // Distance between Star 1 and Star 2
                //float distance1_3 = Vector3.Distance(star1Position, star3Position); // Distance between Star 1 and Star 3
                //float distance2_3 = Vector3.Distance(star2Position, star3Position); // Distance between Star 2 and Star 3

                // Log the distances for debugging
                //Debug.LogError($"Distance between Star 1 and Star 2: {distance1_2}");
                //Debug.LogError($"Distance between Star 1 and Star 3: {distance1_3}");
                //Debug.LogError($"Distance between Star 2 and Star 3: {distance2_3}");
                break;
            default:
                Debug.LogWarning("too many stars");
                break;
        }
    }

    private void GenerateStar(Vector3 position, string starName, GameObject parentObject, Color32 color)
    {
        // Create the star GameObject and set its parent
        GameObject star = new GameObject(starName);
        star.transform.SetParent(parentObject.transform, false); // Keep local position, don't use world position

        // Get the RectTransform component to manipulate the UI positioning
        RectTransform rectTransform = star.AddComponent<RectTransform>();

        // Set the position in local space relative to the parent
        rectTransform.localPosition = position; // Adjust the position

        // Add Image component to display the star sprite
        Image image = star.AddComponent<Image>();
        image.sprite = starSprite; // Set the sprite for the star
        image.color = color;
        image.preserveAspect = true; // Optionally preserve the aspect ratio of the sprite

        // Scale the star based on the starScale
        rectTransform.sizeDelta = new Vector2(starScale, starScale); // Set the size of the image (this is the UI equivalent of scaling the sprite)
        //Debug.LogError($"Position: {position}");
    }

    private async Task UpdateMedianAndPercentile(string level, string username)
    {
        // Call the calculate_median and get_percentile functions using Supabase's RPC
        var medianResponse = await supabase.Rpc("calculate_median", new Dictionary<string, object>
        {
            { "level_param", level }
        });

        var percentileResponse = await supabase.Rpc("get_percentile", new Dictionary<string, object>
        {
            { "level_param", level },
            { "user_param", username }
        });

        // Check if responses are successful and contain data
        if (medianResponse != null && percentileResponse != null)
        {
            // Parse the JSON strings into numeric values
            var medianValue = JsonConvert.DeserializeObject<decimal>(medianResponse.Content.ToString());
            var percentileValue = JsonConvert.DeserializeObject<decimal>(percentileResponse.Content.ToString());

            // Update TextMeshPro objects with median and percentile values
            medianText.text = $"Median Speed: {medianValue}";
            percentileText.text = $"You're Top: {(float)Math.Round(percentileValue, 2)}%";
        }
        else
        {
            // Handle error
            Debug.LogError("Failed to update median and percentile values.");
        }
    }

    public async Task FormatActivity_SetInsert(int setId1, string user_name1, string level1, float duration1)
    {
        if (supabase == null) {
            Debug.LogError("Supabase client has not been initialized.");
            return;
        }

        try {
            var result = await supabase.Rpc("insert_activity_set", new Dictionary<string, object> {
                { "set_id_param", setId1 },
                { "user_name_param", user_name1 },
                { "level_param", level1 },
                { "duration_param", duration1 }
            });

            if (result != null) {
                Debug.LogError("Activity set inserted successfully");
            } else {
                Debug.LogError("Received null result without throwing an exception.");
                throw new InvalidOperationException("Supabase returned a null result unexpectedly.");
            }
        } catch (Exception ex) {
            Debug.LogError($"Failed to insert activity set due to an exception: {ex.Message}");
            //IMPO: Save activity to json object then file.

            activityInsertObject = new ActivityInsertDelayed();
            // Set ActivityInsert() values from args
            activityInsertObject.setId1 = setId1;
            activityInsertObject.user_name1 = user_name1;
            activityInsertObject.level1 = level1;
            activityInsertObject.duration1 = duration1;



            int i = 0;
            string currentFilePath = Path.Combine(Application.persistentDataPath, $"ActivityInsertDelayed{i}.json");//"ActivityInsert0.json"

            //file already exists, find empty value
            while (File.Exists(currentFilePath))
            {
                i++;
                currentFilePath = Path.Combine(Application.persistentDataPath, $"ActivityInsertDelayed{i}.json");
            }
            //file does not exist
            // Save to JSON file
            string json = JsonUtility.ToJson(activityInsertObject);
            File.WriteAllText(currentFilePath, json);

            // Handle error
            Debug.LogError("Failed to insert activity set. SAVED...");
            //Debug.LogError("currentFilePath: "  + currentFilePath);
        }
    }
    [Serializable]
    public class ActivityInsertDelayed
    {
        public int setId1;
        public string user_name1;
        public string level1;
        public float duration1;
    }

    public void highlightPlayerRowRed(int i)
    {
        i -= 1;
        if (i > 5)
            i = 5;

        if (nameTexts[i] != null)
        {
            nameTexts[i].color = new Color32(207, 52, 35, 255); // Change color to red
        }
        // Update duration text
        if (durationTexts[i] != null)
        {
            durationTexts[i].color = new Color32(207, 52, 35, 255); // Change color to red
        }
        // Update rank text
        if (rankTexts[i] != null)
        {
            rankTexts[i].color = new Color32(207, 52, 35, 255); // Change color to red
        }
    }



    // Class to represent a single leaderboard entry
    private class LeaderboardEntry
    {
        public string firstname1 { get; set; }
        public decimal duration1 { get; set; }
        public long rank1 { get; set; }
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


/*
using Supabase;
using System.Threading.Tasks;
using UnityEngine;
using Supabase.Gotrue;
using Supabase.Gotrue.Exceptions;
using TMPro;
using Client = Supabase.Client;
using System.Collections.Generic;
using Newtonsoft.Json;

public class FunctionResult
{
    public string Username { get; set; }
    public string Firstname1 { get; set; }
    public string Level1 { get; set; }
    public decimal Duration1 { get; set; }
    public long Rank1 { get; set; }
}

public class LeaderboardManager : MonoBehaviour
{
    private Client supabase;
    public string leaderboardDataString;

    private async void Start()
    {
        // Initialize the Supabase client
        supabase = new Client("https://acpornqddkzqsdppbabw.supabase.co", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6ImFjcG9ybnFkZGt6cXNkcHBiYWJ3Iiwicm9sZSI6ImFub24iLCJpYXQiOjE3MTM2NTU5MzcsImV4cCI6MjAyOTIzMTkzN30.UQ73w2nx-UxmXhxBF2_jSTl19aZ1bjb9LjYY4eraMtY");

        await GetLeaderboardData(6, "NumberCounting");
    }

    private async Task GetLeaderboardData(int number, string level)
    {
        // Call the get_leaderboard_data function
        var baseResponse = await supabase.Rpc("get_leaderboard_data", new Dictionary<string, object>
        {
            { "level_param", "NumberCounting" },
            { "rank_param", 6 }
        });
        
        var modeledResponse = new ModeledResponse<FunctionResult>(baseResponse, new JsonSerializerSettings());

        // Access the data
        foreach (var result in modeledResponse.Models)
        {
            Console.WriteLine($"Username: {result.Username}, Firstname1: {result.Firstname1}, Level1: {result.Level1}, Duration1: {result.Duration1}, Rank1: {result.Rank1}");
        }
        

        // Check if response is successful and contains data
        if (baseResponse != null)
        {
            // Convert response data to string (you may need to adjust this based on the actual response structure)
            leaderboardDataString = baseResponse.Content.ToString();

            // Log the data
            Debug.Log(leaderboardDataString);
        }
        else
        {
            // Handle error
            Debug.LogError("Failed to fetch leaderboard data.");
        }
    }
}
*/