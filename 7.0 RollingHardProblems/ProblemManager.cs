using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProblemManager : MonoBehaviour
{
    public bool solutionDisplay;
    public int problem;
    public GameObject solutionButton;
    [Header("UI References")]
    public Image displayImageUI;            // Assign your existing Image in scene
    public TextMeshProUGUI displayTextUI;   // Assign your existing TMP text in scene
    public TextMeshProUGUI toggle_solution_button_TEXT;   // Assign your existing TMP text in scene

    public TextMeshProUGUI problemTextinHeader;   // Assign your existing TMP text in scene

    [Header("Scene References")]
    public SceneCompleteMenu sceneCompleteScript;

    [Header("Display Data (Assign 15 total across ratings)")]
    public List<HardProblemClass> problems = new List<HardProblemClass>(); // e.g. 5 options
    //public List<RatingDisplayData> rating2Displays = new List<RatingDisplayData>(); // e.g. 5 options
    //public List<RatingDisplayData> rating3Displays = new List<RatingDisplayData>(); // e.g. 5 options

    private System.Random rnd = new System.Random();

    void Start()
    {
        solutionDisplay = true;
        string problemString = sceneCompleteScript.sceneObject.bestRating;
        int.TryParse(problemString, out problem);
        Debug.LogError($"{problem}");
        Toggle_Solution_Or_Problem();
    }

    // Call this when you have the rating (e.g. after scene complete)
    public void ShowRatingDisplay(int problemNum)
    {
        CleanupPreviousDisplay();

        // Select the correct list based on rating
        List<HardProblemClass> selectedList = problems;

        if (selectedList == null || selectedList.Count == 0)
        {
            Debug.LogWarning($"No display data for rating {problemNum}");
            return;
        }

        // Pick random item from the list
        HardProblemClass chosen = selectedList[problemNum];
        Debug.LogError($"Loaded Problem{problemNum}");

        // Assign content (no position/anchor changes — uses scene placement)
        if (chosen.displayImage != null)
        {
            displayImageUI.sprite = chosen.displayImage;
            displayImageUI.enabled = true;
        }

        if (chosen.displayText != null)
        {
            displayTextUI.text = chosen.displayText;
            displayTextUI.enabled = true;
        }


        if (chosen.solutionText == "")
            solutionButton.SetActive(false);// = false;
        else
            solutionButton.SetActive(true);

        Debug.Log($"Displayed rating {problemNum} content: {chosen.displayText}");
        solutionDisplay = false;
    }

    // Call this when user wants to see the solution (e.g., button press after answering)
    public void GoToAssociatedSolution(int currentProblem)
    {
        CleanupPreviousDisplay();

        // Select the correct list based on rating
        List<HardProblemClass> selectedList = problems;

        if (problems == null || problems.Count == 0)
        {
            Debug.LogWarning($"No display data for rating {currentProblem}");
            return;
        }

        // Pick random item from the list
        HardProblemClass chosen = problems[currentProblem];

        // Replace / reveal solution content
        if (displayImageUI != null && chosen.solutionImage != null)
        {
            displayImageUI.sprite = chosen.solutionImage;
            displayImageUI.enabled = true;
        }

        if (displayTextUI != null && !string.IsNullOrEmpty(chosen.solutionText))
        {
            displayTextUI.text = chosen.solutionText;
            displayTextUI.enabled = true;
        }

        Debug.Log($"Switched to solution for current rating: {chosen.solutionText}");
        solutionDisplay = true;
    }

    public void Toggle_Solution_Or_Problem()
    {
        if (solutionDisplay)
        {
            Debug.LogError($"Trying to Load Problem{problem}");
            problemTextinHeader.text = $"{problem}/15";
            ShowRatingDisplay(problem);
            toggle_solution_button_TEXT.text = "Solution";
        }
        else
        {
            GoToAssociatedSolution(problem);            
            toggle_solution_button_TEXT.text = "Problem";
        }
    }

    // Clean up previous content
    public void CleanupPreviousDisplay()
    {
        displayImageUI.enabled = false;
        displayTextUI.enabled =false;
    }
    public void LoadNewProblem(int next_or_previous_problem)
    {
        if (problems == null || problems.Count == 0)
        {
            Debug.LogWarning("No problems assigned in ProblemManager!");
            return;
        }

        // Later you can re-introduce rating-based selection
        int rating = sceneCompleteScript?.completionRating ?? 1;

        // For now using the single list (you can expand this later)
        var selectedList = problems; // ← replace with switch when you add more rating lists

        if (selectedList.Count == 0)
        {
            Debug.LogWarning($"No problems available for rating {rating}");
            return;
        }
        solutionDisplay = true;
        problem = problem + next_or_previous_problem;
        sceneCompleteScript.sceneObject.bestRating = problem.ToString();;
        sceneCompleteScript.SaveSceneData();
        Debug.LogError($"sceneCompleteScript.sceneObject.bestRating {sceneCompleteScript.sceneObject.bestRating}");
        Toggle_Solution_Or_Problem();
    }
}


