using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.IO;
using TMPro;

public class HomescreenSceneManager : MonoBehaviour
{
    public GameObject loadingInterface;
    public Image progressBar;

    List<AsyncOperation> scenesToLoad = new List<AsyncOperation>();

    private string filePath;
    public List<string> completedLevels;
    public List<string> completedLevelTextList;
    public string completedLevelTextFilePath;
    public string currentLevel;
    public TextMeshProUGUI MenuGradeText;

    List<string> levelOrder = new List<string> {"NumberCounting", "NumberCountingScattered", "BasicAdditionV", "BasicSubtractionV", "ShapePatterns", "SmallerOrBigger", "PlaceValues", "Clock", "AdditionV", "AdditionFunctionBox", "SubtractionFunctionBox", "MultiplicationV", "DivisionV", "NormalAddition", "NormalSubtraction", "LongMultiplication", "FractionFromShape", "FractionEqualize", "FractionEqualizeHard", "FractionReduction", "PercentEqualize", "LongDivision", "PEMDAS", "PemdasHard", "Exponent", "LineFormulation", "Factoring", "RollingHardProblems"};
    
    public SceneRatingsDisplay sceneRatingsDisplay;

    public void Start()
    {
        
    }

    List<string> GetStringListFromFile()
    {
        if (File.Exists(filePath))
        {
            return new List<string>(File.ReadAllLines(filePath));
        }
        return new List<string>();
    }

    public void DetermineMenuText()
    {
        filePath = Path.Combine(Application.persistentDataPath, completedLevelTextFilePath);
        List<string> completedLevelTextList = GetStringListFromFile();

        /*
        Debug.LogError("Completed Scenes:");
        foreach (var scene in completedLevelTextList)
        {
            Debug.LogError(scene);
        }
        */


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

    public void SetCurrentLevel(string level)
    {
        currentLevel = level;
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

    public void StartFirstScene()
    {
        //Debug.LogError(currentLevel);
        ShowLoadingBar();
        scenesToLoad.Add(SceneManager.LoadSceneAsync(currentLevel));
        StartCoroutine(LoadingScreen());
    }

    public void LevelsMenu()
    {
        ShowLoadingBar();
        scenesToLoad.Add(SceneManager.LoadSceneAsync("Speed"));
        StartCoroutine(LoadingScreen());
    }

    public void StartAnyScene(string levelName)
    {
        ShowLoadingBar();
        scenesToLoad.Add(SceneManager.LoadSceneAsync(levelName));
        StartCoroutine(LoadingScreen());
    }

    public void ShowLoadingBar()
    {
        loadingInterface.SetActive(true);
    }

    IEnumerator LoadingScreen()
    {
        sceneRatingsDisplay.SetCanvasGroupAlpha1();

        // Load the scenes asynchronously
        for (int i = 0; i < scenesToLoad.Count; ++i)
        {
            scenesToLoad[i].allowSceneActivation = false;
            yield return null;
        }

        // Wait for 4 seconds before activating the scenes
        yield return StartCoroutine(FillProgressBarIn4Seconds());
        //yield return new WaitForSeconds(4f);

        // Activate the scenes
        for (int i = 0; i < scenesToLoad.Count; ++i)
        {
            scenesToLoad[i].allowSceneActivation = true;
        }
    }

    IEnumerator FillProgressBarIn4Seconds()
    {
        float startTime = Time.time;
        float duration = 4f; // 4 seconds
        float progressValue = 0f;

        while (Time.time - startTime < duration)
        {
            progressValue = (Time.time - startTime) / duration;
            progressBar.fillAmount = progressValue;
            yield return null;
        }

        progressBar.fillAmount = 1f; // Ensure the progress bar is fully filled
    }
    /*

        // Wait for the scenes to finish loading
        float progressValue = 0;
        for (int i = 0; i < scenesToLoad.Count; ++i)
        {
            while (!scenesToLoad[i].isDone)
            {
                progressValue += scenesToLoad[i].progress;
                progressBar.fillAmount = progressValue / scenesToLoad.Count;
                yield return null;
            }
        }
    */
    /*IEnumerator LoadingScreen()
    {
        sceneRatingsDisplay.SetCanvasGroupAlpha1();
        float progressValue = 0;
        for(int i=0; i<scenesToLoad.Count; ++i)
        {
            while (!scenesToLoad[i].isDone)
            {
                progressValue += scenesToLoad[i].progress;
                progressBar.fillAmount = progressValue / scenesToLoad.Count;
                yield return null;
            }
        }
        yield return new WaitForSeconds(10f);
    }*/
}