using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MobileKeyboardController : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    public Image questionMark;

    // Variables for blinking effect
    private float onDuration = 1f; // Duration for image to be visible
    private float offDuration = 0.5f; // Duration for image to be invisible
    private bool isImageVisible = false;
    private float nextActionTime = 0.0f;

    private bool blinkingEnabled = true;
    public Button answerbutton;
    public SceneCompleteMenu sceneCompleteScript;


    // Call this method in Update to handle blinking
    void Update()
    {
        if (Time.time > nextActionTime)
        {
            // Toggle visibility
            isImageVisible = !isImageVisible;

            if (blinkingEnabled)
            {
                questionMark.enabled = isImageVisible;

                // Set next action time
                nextActionTime = Time.time + (isImageVisible ? onDuration : offDuration);
            }
        }
    }

    // Function to add a number to the text
    public void NumberInput(int number)
    {
        inputText.text += number.ToString();
        Vibrator.Vibrate(50);

        questionMark.enabled = false;
        blinkingEnabled = false;
        //Debug.LogError($"111sceneObject.bestRating {sceneCompleteScript.sceneObject.bestRating}");

        if (int.TryParse(sceneCompleteScript.sceneObject.bestRating, out int value))
        {
            int bestRating = value;
            //Debug.LogError($"112sceneObject.bestRating {bestRating}");
            if (bestRating >= 2)
            {
                Debug.LogError($"sceneCompleteScript.sceneObject.bestRating {sceneCompleteScript.sceneObject.bestRating}");
                answerbutton.onClick.Invoke();
            }
        }
    }

    // Function to delete the last character in the text
    public void DeleteInput()
    {
        if (inputText.text.Length > 0)
        {
            inputText.text = inputText.text.Substring(0, inputText.text.Length - 1);
            Vibrator.Vibrate(50);
        }
    }
    private void CheckInputForRightMiddleAlignment()
    {
        if (inputText.text.Length > 2)
        {
            inputText.alignment = TextAlignmentOptions.Center;
        }
    }
}
