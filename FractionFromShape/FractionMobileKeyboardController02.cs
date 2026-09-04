using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FractionMobileKeyboardController02 : MonoBehaviour
{
    public TextMeshProUGUI numerator;
    public TextMeshProUGUI denominator;
    public bool secondInputBool;
    public AnswerManager42 script;

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
            denominator.text += number.ToString();
        }
        else
        {
            blinkingEnabled_step1 = false;
            questionMark_step1.enabled = false;
            blinkingEnabled_step2 = true;
            numerator.text += number.ToString();
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
}