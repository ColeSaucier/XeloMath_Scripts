using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;        
using UnityEngine.SceneManagement;
using System.IO;
using System.Reflection;

public class DetermineCompletedLevels : MonoBehaviour
{
    public List<string> completedLevels;// = new List<string> { "0.0 NumCountingOrdered", "0.5 NumCountingRandom", "0.75 ShapePattern", "1.0 AdditionV", "1.02 MultiplicationV"};
    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;
    public Button Button5;
    public Button Button6;
    public Button Button7;
    public Button Button8;
    public Button Button9;
    public Button Button10;
    public Button Button11;
    public Button Button12;
    public Button Button13;
    public Button Button14;
    public Button Button15;
    public Button Button155;
    public Button Button16;
    public Button Button17;
    public Button Button18;
    public Button Button19;
    public Button Button20;
    public Button Button21;
    public Button Button22;
    public Button Button23;
    public Button Button24;
    public Button Button25;
    public Button Button26;
    public Button Button27;
    public Button Button28;

    public TextMeshProUGUI text1;
    public TextMeshProUGUI text2;
    public TextMeshProUGUI text3;
    public TextMeshProUGUI text4;
    public TextMeshProUGUI text5;
    public TextMeshProUGUI text6;
    public TextMeshProUGUI text7;
    public TextMeshProUGUI text8;
    public TextMeshProUGUI text9;
    public TextMeshProUGUI text10;
    public TextMeshProUGUI text11;
    public TextMeshProUGUI text12;
    public TextMeshProUGUI text13;
    public TextMeshProUGUI text14;
    public TextMeshProUGUI text15;
    public TextMeshProUGUI text155;
    public TextMeshProUGUI text16;
    public TextMeshProUGUI text17;
    public TextMeshProUGUI text18;
    public TextMeshProUGUI text19;
    public TextMeshProUGUI text20;
    public TextMeshProUGUI text21;
    public TextMeshProUGUI text22;
    public TextMeshProUGUI text23;
    public TextMeshProUGUI text24;
    public TextMeshProUGUI text25;
    public TextMeshProUGUI text26;
    public TextMeshProUGUI text27;
    public TextMeshProUGUI text28;

    public GameObject menu;
    public GameObject loadingInterface;
    public Image progressBar;

    public Dictionary<string, string> allSceneRatingDictionary;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private string filePath;
    public string completedLevelTextFilePath;
    private string ratingString;

    public string allSceneRatingsjsonFilePath;
    private string allSceneRatingsJsonString;
    private AllSceneRatingsData allSceneRatingObject;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, completedLevelTextFilePath);
        Debug.LogError($"Completed levels file path: {filePath}");
        completedLevels = GetUniqueValuesFromFile();
        Debug.LogError($"completedLevels: {string.Join(", ", completedLevels)}");

        //retrieve data and create dataObject + UpdateValue_AllSceneRatings()
        allSceneRatingObject = new AllSceneRatingsData();
        filePath = Path.Combine(Application.persistentDataPath, allSceneRatingsjsonFilePath); // Combine with the Assets folder
        Debug.LogError($"All scene ratings file path: {filePath}");
        allSceneRatingsJsonString = File.ReadAllText(filePath);
        Debug.LogError($"All scene STRINGGGG : {allSceneRatingsJsonString}");
        allSceneRatingObject = JsonUtility.FromJson<AllSceneRatingsData>(allSceneRatingsJsonString);
        Debug.LogError("We appear to have gathered the allSceneRatingObject");
        
        allSceneRatingDictionary = new Dictionary<string, string>
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

        DeactivateButtons();
    }

    void DeactivateButtons()
    {
        //DeactivateButtonIfNotInList(Button1);
        DeactivateButtonIfNotInList(Button2);
        DeactivateButtonIfNotInList(Button3);
        DeactivateButtonIfNotInList(Button4);
        DeactivateButtonIfNotInList(Button5);
        //DeactivateButtonIfNotInList(Button6);
        DeactivateButtonIfNotInList(Button7);
        DeactivateButtonIfNotInList(Button8);
        DeactivateButtonIfNotInList(Button9);
        DeactivateButtonIfNotInList(Button10);
        DeactivateButtonIfNotInList(Button11);
        //DeactivateButtonIfNotInList(Button12);
        DeactivateButtonIfNotInList(Button13);
        DeactivateButtonIfNotInList(Button14);
        DeactivateButtonIfNotInList(Button15);
        //DeactivateButtonIfNotInList(Button16);
        DeactivateButtonIfNotInList(Button17);
        DeactivateButtonIfNotInList(Button18);
        DeactivateButtonIfNotInList(Button19);
        DeactivateButtonIfNotInList(Button20);
        DeactivateButtonIfNotInList(Button21);
        //DeactivateButtonIfNotInList(Button22);
        DeactivateButtonIfNotInList(Button23);
        DeactivateButtonIfNotInList(Button24);
        DeactivateButtonIfNotInList(Button25);
        DeactivateButtonIfNotInList(Button26);
        DeactivateButtonIfNotInList(Button27);
        //DeactivateButtonIfNotInList(Button28);

        UpdateStarRatings(text1);
        UpdateStarRatings(text2);
        UpdateStarRatings(text3);
        UpdateStarRatings(text4);
        UpdateStarRatings(text5);
        UpdateStarRatings(text6);
        UpdateStarRatings(text7);
        UpdateStarRatings(text8);
        UpdateStarRatings(text9);
        UpdateStarRatings(text10);
        UpdateStarRatings(text11);
        UpdateStarRatings(text12);
        UpdateStarRatings(text13);
        UpdateStarRatings(text14);
        UpdateStarRatings(text15);
        UpdateStarRatings(text16);
        UpdateStarRatings(text17);
        UpdateStarRatings(text18);
        UpdateStarRatings(text19);
        UpdateStarRatings(text20);
        UpdateStarRatings(text21);
        UpdateStarRatings(text20);
        UpdateStarRatings(text21);
        UpdateStarRatings(text22);
        UpdateStarRatings(text23);
        UpdateStarRatings(text24);
        UpdateStarRatings(text25);
        UpdateStarRatings(text26);
        UpdateStarRatings(text27);
        UpdateStarRatings(text28);
    }

    public void DeactivateButtonIfNotInList(Button button)
    {
        Debug.Log("The name of this GameObject is: " + button.gameObject.name);
        Debug.Log("completedLevels " + string.Join(", ", completedLevels.ToArray()));
        // Check if the button's name is not in the completedLevels list
        if (!completedLevels.Contains(button.gameObject.name))
        {
            // If not in the list, deactivate the button
            //button.interactable = false;
            button.gameObject.SetActive(false);
        }
    }
    
    public void UpdateTextRatings(TextMeshProUGUI textUGUI)
    {
        ratingString = textUGUI.gameObject.name;
        // text.text = GetPropertyValue(allSceneRatingObject, ratingString);
        string rating = allSceneRatingDictionary[ratingString];
    }

    public Vector3 centerPointOffset = Vector3.zero; // Public variable to adjust the center point
    public Sprite starSprite; // Public reference to set the star image
    public float starScale = 1.0f; // Public variable to adjust star size
    public float localOffset; // Example value, adjust as needed

    public TextMeshProUGUI ratingDisplayHardProblems; 
    public void UpdateStarRatings(TextMeshProUGUI textUGUI)
    {
        ratingString = textUGUI.gameObject.name;
        string rating = allSceneRatingDictionary[ratingString];
        int numberOfStars = int.Parse(rating);
        
        // Get the position of the text and adjust with the public offset
        Vector3 centerPoint = textUGUI.rectTransform.localPosition + centerPointOffset;
        switch (numberOfStars)
        {
            case 0:
                // Do nothing
                break;
            case 1: 
                GenerateStar(centerPoint, $"{ratingString}_Star1", textUGUI.gameObject);
                break;
            case 2:

                GenerateStar(centerPoint + Vector3.left * localOffset, $"{ratingString}_Star1", textUGUI.gameObject);
                GenerateStar(centerPoint + Vector3.right * localOffset, $"{ratingString}_Star2", textUGUI.gameObject);
                break;
            case 3:
                float angle = 30f * Mathf.Deg2Rad; // Convert angle to radians
                float xOffset = localOffset * Mathf.Cos(angle);
                float yOffset = localOffset * Mathf.Sin(angle);
                centerPoint = centerPoint + new Vector3(0, -10f, 0);

                // Positions of the stars
                Vector3 star1Position = centerPoint + Vector3.up * localOffset; // Directly above
                Vector3 star2Position = centerPoint + new Vector3(xOffset, -yOffset, 0); // Right
                Vector3 star3Position = centerPoint + new Vector3(-xOffset, -yOffset, 0); // Left

                // Generate the stars
                GenerateStar(star1Position, $"{ratingString}_Star1", textUGUI.gameObject); // Star 1
                GenerateStar(star2Position, $"{ratingString}_Star2", textUGUI.gameObject); // Star 2
                GenerateStar(star3Position, $"{ratingString}_Star3", textUGUI.gameObject); // Star 3

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
                ratingDisplayHardProblems.text = numberOfStars.ToString();
                Debug.LogWarning("Rating out of expected range: " + rating);
                break;
        }
    }

    private void GenerateStar(Vector3 position, string starName, GameObject parentObject)
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
        image.preserveAspect = true; // Optionally preserve the aspect ratio of the sprite

        // Scale the star based on the starScale
        rectTransform.sizeDelta = new Vector2(starScale, starScale); // Set the size of the image (this is the UI equivalent of scaling the sprite)
        //Debug.LogError($"Position: {position}");
    }

    private SwipeHandler swipeHandler;
    public void StartAnyScene(Button button)
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
            //Debug.LogError("Object not found. //");
        }

        //Start scene via button name
        string sceneString = button.gameObject.name;
        Debug.LogError($"sceneString {sceneString}");

        //Debug.LogError("Starting Trigger with string: "+sceneString);
        //StartCoroutine(sceneHandlerScript.StartAnyScene_NoTransition(sceneString));
        swipeHandler.triggerTransitionByUpdate_SceneName = sceneString;
    }


    IEnumerator LoadingScreen()
    {
        float progressValue = 0;
        for(int i=0; i<scenesToLoad.Count; ++i)
        {
            while (!scenesToLoad[i].isDone)
            {
                progressValue += scenesToLoad[i].progress;
                // Error below, because no progressbar set in the level Menu
                //progressBar.fillAmount = progressValue / scenesToLoad.Count;
                yield return null;
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

    List<string> GetUniqueValuesFromFile()
    {
        List<string> allValues = GetStringListFromFile();

        // Use a HashSet to efficiently remove duplicates
        HashSet<string> uniqueSet = new HashSet<string>(allValues);

        // Convert back to a List if needed
        List<string> completedLevels = new List<string>(uniqueSet);

        return completedLevels;
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