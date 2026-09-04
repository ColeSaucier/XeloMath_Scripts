using System;
using System.Collections;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class StringExtensions
{
    public static string AddColor(this string text, Color col) => $"<color={ColorHexFromUnityColor(col)}>{text}</color>";
    public static string ColorHexFromUnityColor(this Color unityColor) => $"#{ColorUtility.ToHtmlStringRGBA(unityColor)}";
}

public class SceneRatingsDisplay : MonoBehaviour
{
    public RectTransform canvasGroup;
    public float verticalSpacing = 50f;
    public float horizontalPosition = 300f;
    public int fontSize = 24;
    public float yPosition = 300f;
    public TMP_FontAsset font;
    public Color32 ratingColor = new Color32(0, 0, 255, 255);
    public string allSceneRatingsjsonFilePath;
    public CanvasGroup displayCanvasGroup; // Reference to the CanvasGroup to modify alpha

    private AllSceneRatingsData allSceneRatingObject;
    private string allSceneRatingsJsonString;

    /*private IEnumerator Start()
    {
        if (displayCanvasGroup != null)
        {
            SetCanvasGroupAlphaZero(); // Set alpha to 0 before displaying ratings
        }

        yield return new WaitForSeconds(0.4f); // Wait a short duration before displaying ratings
        DisplaySceneRatings();
    }*/

    public void SetCanvasGroupAlphaZero()
    {
        displayCanvasGroup.alpha = 0f;
    }

    public void SetCanvasGroupAlpha1()
    {
        displayCanvasGroup.alpha = 1f;
    }

    private void DisplaySceneRatings()
    {
        LoadSceneData();
        float yPos = (canvasGroup.rect.height / 2f) + yPosition;

        string cleanedJsonString = allSceneRatingsJsonString.Replace("\"", "").Replace("{", "").Replace("}", "");
        string[] sceneRatings = cleanedJsonString.Split(new[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < sceneRatings.Length; i += 2)
        {
            string sceneName = sceneRatings[i].Trim();
            string sceneRatingValue = sceneRatings[i + 1].Trim();
            string sceneRating = ConvertSceneRatingValue(sceneRatingValue);

            CreateTextMeshProObject(sceneName, sceneRating, new Vector2(horizontalPosition, yPos), ratingColor);
            yPos -= verticalSpacing;
        }
    }

    private string ConvertSceneRatingValue(string value)
    {
        switch (value)
        {
            case "0":
            {
                ratingColor = new Color32(207, 52, 35, 255);
                return "...";
            }
            case "1":
            {
                ratingColor = new Color32(207, 52, 35, 255);
                //ratingColor = new Color32(210, 0, 0, 255);
                return "Standard";
            }
            case "2":
            {
                ratingColor = new Color32(255, 165, 0, 255);
                return "Epic";
            }
            case "3":
            {
                ratingColor = new Color32(128, 0, 128, 255);
                return "Legendary";
            }
            default:
                return value;
        }
    }

    private void CreateTextMeshProObject(string label, string value, Vector2 position, Color32 color)
    {
        GameObject textObject = new GameObject($"{label} Text", typeof(RectTransform), typeof(TextMeshProUGUI));
        textObject.transform.SetParent(canvasGroup, false);
        textObject.GetComponent<RectTransform>().anchoredPosition = position;

        TextMeshProUGUI textComponent = textObject.GetComponent<TextMeshProUGUI>();
        textComponent.text = $"{label} = {value.AddColor(color)}";
        textComponent.alignment = TextAlignmentOptions.Center;
        textComponent.fontSize = fontSize;
        textComponent.enableWordWrapping = false;
        textComponent.font = font;
    }

    private void LoadSceneData()
    {
        allSceneRatingObject = new AllSceneRatingsData();
        string filePath = Path.Combine(Application.persistentDataPath, allSceneRatingsjsonFilePath);
        allSceneRatingsJsonString = File.ReadAllText(filePath);
        allSceneRatingObject = JsonUtility.FromJson<AllSceneRatingsData>(allSceneRatingsJsonString);
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