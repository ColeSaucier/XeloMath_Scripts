using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TwoStepMobileKeyboard01 : MonoBehaviour
{
    public TextMeshProUGUI slope;
    public TextMeshProUGUI constant;
    public TextMeshProUGUI PlusNegativeKeybaordSymbolFrivilious;
    public bool secondInputBool;
    public AnswerManager69 script;

    // Variables for blinking effect
    private float onDuration = 1f; // Duration for image to be visible
    private float offDuration = 0.5f; // Duration for image to be invisible
    private bool isImageVisible = false;
    private float nextActionTime = 0.0f;

    private bool blinkingEnabled_step1 = true;
    private bool blinkingEnabled_step2 = false;

    public Image questionMark_step1;
    public Image questionMark_step2;

    public Button answerbutton;
    public SceneCompleteMenu sceneCompleteScript;

    // Call this method in Update to handle blinking
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            // Toggle visibility bool
            isImageVisible = !isImageVisible;

            secondInputBool = script.secondInput;
            if (secondInputBool)
            {
                if (blinkingEnabled_step2)
                    questionMark_step2.enabled = isImageVisible;
                Debug.LogError($"questionMark_step2.enabled{questionMark_step2.enabled}");
            }
            else 
            {
                if(blinkingEnabled_step1)
                    questionMark_step1.enabled = isImageVisible;  
                Debug.LogError($"questionMark_step1.enabled{questionMark_step1.enabled}");           
            }
            // Set next action time
            nextActionTime = Time.time + (isImageVisible ? onDuration : offDuration);
        }
    }
    public void Reset_QuestionMark_Enable()
    {
        questionMark_step1.enabled = true;
        questionMark_step2.enabled = true;
        blinkingEnabled_step1 = true;
        blinkingEnabled_step2 = false;
    }
    // Function to add a number to the text
    public void NumberInput(int number)
    {
        secondInputBool = script.secondInput;
        Vibrator.Vibrate(50);

        if (secondInputBool == true)
        {
            blinkingEnabled_step2 = false;
            questionMark_step2.enabled = false;
            constant.text += number.ToString();
        }
        else
        {
            blinkingEnabled_step1 = false;
            questionMark_step1.enabled = false;
            blinkingEnabled_step2 = true;
            slope.text += number.ToString();
        }
        if (int.TryParse(sceneCompleteScript.sceneObject.bestRating, out int value))
        {
            int bestRating = value;
            //Debug.LogError($"112sceneObject.bestRating {bestRating}");
            if (bestRating >= 2)
            {
                //Debug.LogError($"sceneCompleteScript.sceneObject.bestRating {sceneCompleteScript.sceneObject.bestRating}");
                answerbutton.onClick.Invoke();
            }
        }
    }
    // Function to add a negative, OR reverse back to nothing (additon)
    public void SymbolInput()
    {
        secondInputBool = script.secondInput;
        Vibrator.Vibrate(50);

        // Decide which field we're modifying
        TextMeshProUGUI targetText = secondInputBool ? constant : slope;

        if (secondInputBool == true)
        {
            blinkingEnabled_step2 = false;
            questionMark_step2.enabled = false;
        }
        else
        {
            blinkingEnabled_step1 = false;
            questionMark_step1.enabled = false;
            blinkingEnabled_step2 = true;
        }
        if (targetText == null)
        {
            Debug.LogWarning("Target text field is null!");
            return;
        }

        string current = targetText.text.Trim();

        if (string.IsNullOrEmpty(current))
        {
            // Empty → set to negative sign
            targetText.text = "-";
        }
        else if (current == "-")
        {
            // Just "-" → clear it (or set to "0" — your choice)
            targetText.text = "0";   // or "" if you prefer empty
        }
        else
        {
            // Has content → toggle sign
            if (current.StartsWith("-"))
            {
                // Remove negative
                targetText.text = current.Substring(1);
                PlusNegativeKeybaordSymbolFrivilious.text = "+";
            }
            else
            {
                // Add negative
                targetText.text = "-" + current;
                PlusNegativeKeybaordSymbolFrivilious.text = "";
            }
        }

        if (int.TryParse(sceneCompleteScript.sceneObject.bestRating, out int value))
        {
            int bestRating = value;
            //Debug.LogError($"112sceneObject.bestRating {bestRating}");
            if (bestRating >= 2)
            {
                //Debug.LogError($"sceneCompleteScript.sceneObject.bestRating {sceneCompleteScript.sceneObject.bestRating}");
                answerbutton.onClick.Invoke();
            }
        }
    }

    // Function to delete the last character in the text
    public void DeleteInput()
    {
        secondInputBool = script.secondInput;
        Vibrator.Vibrate(100);

        if (secondInputBool == true)
        {
            if (constant.text.Length > 0)
            {
                constant.text = constant.text.Substring(0, constant.text.Length - 1);
            }
        }
        else
        {
            if (slope.text.Length > 0)
            {
                slope.text = slope.text.Substring(0, slope.text.Length - 1);
            }
        }
    }
}