using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TwoStepMobileKeyboard02 : MonoBehaviour
{
    public string d;
    public string e;
    public TextMeshProUGUI displayed_text1;
    public TextMeshProUGUI displayed_text2;

    public bool symbold_addition = true;
    public bool symbole_addition = true;
    public string symbold;
    public string symbole;

    public bool secondInputBool;
    public AnswerManager695 script;

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

        symbold_addition = true;
        symbole_addition = true;
        displayed_text1.text = "(y+  ";
        displayed_text2.text = ")(y+  ";

        d = "";
        e = "";
    }
    public void Reset_First()
    {
        questionMark_step1.enabled = true;
        blinkingEnabled_step1 = true;

        symbold_addition = true;
        displayed_text1.text = "(y+  ";

        d = "";
    }
    public void Reset_Second()
    {
        questionMark_step2.enabled = true;
        blinkingEnabled_step2 = true;

        symbole_addition = true;
        displayed_text2.text = ")(y+  ";

        e = "";
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
            e += number.ToString();
            displayed_text2.text = $")(y{symbole}{e}";
        }
        else
        {
            blinkingEnabled_step1 = false;
            questionMark_step1.enabled = false;
            blinkingEnabled_step2 = true;
            d += number.ToString();
            displayed_text1.text = $"(y{symbold}{d}";
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

        if (secondInputBool == true)
        {
            blinkingEnabled_step2 = false;
            questionMark_step2.enabled = false;
            symbole_addition = !symbole_addition;
        }
        else
        {
            blinkingEnabled_step1 = false;
            questionMark_step1.enabled = false;
            blinkingEnabled_step2 = true;
            symbold_addition = !symbold_addition;
        }

        if (secondInputBool == true)
        {
            if (symbole_addition)
                symbole = "+";
            else
                symbole = "-";
            displayed_text2.text = $")(y{symbole}{e}";
        }
        else
        {
            if (symbold_addition)
                symbold = "+";
            else
                symbold = "-";
            displayed_text1.text = $"(y{symbold}{d}";
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
            if (e.Length > 0)
            {
                e = e.Substring(0, e.Length - 1);
            }
            displayed_text2.text = $")(y{symbole}{e}";
        }
        else
        {
            if (d.Length > 0)
            {
                d = d.Substring(0, d.Length - 1);
            }
            displayed_text1.text = $"(y{symbold}{d}";
        }
    }
}