using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FractionMobileKeyboardController03 : MonoBehaviour
{
    public TextMeshProUGUI numerator;
    public TextMeshProUGUI denominator;
    public bool secondInputBool;
    public AnswerManager415 script;

    // Variables for blinking effect
    private float onDuration = 1f; // Duration for image to be visible
    private float offDuration = 0.5f; // Duration for image to be invisible
    private bool isImageVisible = false;
    private float nextActionTime = 0.0f;

    private bool blinkingEnabled_step1 = true;
    private bool blinkingEnabled_step2 = true;

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

            if (blinkingEnabled_step1 || blinkingEnabled_step2)
            {
                //secondInputBool = script.secondInput;
                if (secondInputBool)
                {
                    questionMark_step2.enabled = isImageVisible;
                }
                else 
                {
                    questionMark_step1.enabled = isImageVisible;             
                }
            }
            // Set next action time
            nextActionTime = Time.time + (isImageVisible ? onDuration : offDuration);
        }
    }

    // Function to add a number to the text
    public void NumberInput(int number)
    {
        secondInputBool = script.secondInput;
        Vibrator.Vibrate(50);

        if (secondInputBool == true)
        {
            denominator.text += number.ToString();
        }
        else
        {
            numerator.text += number.ToString();
        }
        blinkingEnabled_step1 = false;
        blinkingEnabled_step2 = false;
        questionMark_step1.enabled = false;
        questionMark_step2.enabled = false;
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
        Vibrator.Vibrate(50);

        if (secondInputBool == true)
        {
            if (denominator.text.Length > 0)
            {
                denominator.text = denominator.text.Substring(0, denominator.text.Length - 1);
            }
        }
        else
        {
            if (numerator.text.Length > 0)
            {
                numerator.text = numerator.text.Substring(0, numerator.text.Length - 1);
            }
        }
    }
    public void setCorrect_questionMark()
    {
        secondInputBool = script.secondInput;
        if (secondInputBool)
        {
            Debug.LogError("Step1 should be false");
            blinkingEnabled_step1 = false;
            questionMark_step1.enabled = false;
        }
        else
        {
            Debug.LogError("Step2 should be false");
            blinkingEnabled_step2 = false;
            questionMark_step2.enabled = false;
        }

    }
}