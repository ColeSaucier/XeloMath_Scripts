using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class AnswerManagerBase : MonoBehaviour
{
    // References needed for answer button
    public Button Button;
    public string userInput = "";
    public bool isInputActive = false;
    public TextMeshProUGUI inputText;
    public CanvasGroup popUpCanvasGroup;

    // Scene Variables
    public string answerString;
    public bool SceneComplete;
    public SceneCompleteMenu sceneCompleteScript;
    
    //Mobile Keyboard Enabling
    public Canvas mobileKeyboard;
    public bool mobileVersion = true;
    public TextMeshProUGUI KeyboardInputText;
    //private Action functionReference;

    public virtual void Update()
    {
        if (isInputActive)
        {
            if (!mobileVersion)
            {
                HandleDesktopInput();
            }
        }
    }

    public virtual void HandleDesktopInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            checkStringInput();
            isInputActive = false;
        }
        else if (Input.GetKeyDown(KeyCode.Backspace) && userInput.Length > 0)
        {
            userInput = userInput.Substring(0, userInput.Length - 1);
        }
        else
        {
            userInput += Input.inputString;
        }
        inputText.text = userInput;
    }

    public virtual void checkStringInput()
    {
        //Answer (answerString) must be set within individulaistic scripts
        if (mobileVersion)
        {
            if (KeyboardInputText.text == answerString)
            {
                SceneComplete = true;
                sceneCompleteScript.SceneComplete = true;
                if (Button != null)
                {
                    Button.image.color = Color.green;
                }
                // Reset input
                KeyboardInputText.text = "";
            }
            else
            {
                if (KeyboardInputText.text.Length >= answerString.Length)
                {
                    if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
                    {
                        print($"answer manager sceneCompleteScript.sceneObject.bestRating {sceneCompleteScript.sceneObject.bestRating}");
                        Handheld.Vibrate();
                        //Green shift for correct input
                        Color32 shiftColor = new Color32(210, 0, 0, 50);
                        StartCoroutine(ShowColoredImage(shiftColor, 0.2f));
                        // Reset input
                        KeyboardInputText.text = "";
                    }
                }
            }
        }
        else
        {
            if (inputText.text == answerString)
            {
                SceneComplete = true;
                sceneCompleteScript.SceneComplete = true;
                Button.image.color = Color.green;
            }
            // Close answerbox
            popUpCanvasGroup.alpha = 0f;
        }
    }

    // Method to start the coroutine that creates a colored image
    public virtual void DisplayColoredImage(Color32 color, float duration)
    {
        StartCoroutine(ShowColoredImage(color, duration));
    }

    public Sprite background;
    IEnumerator ShowColoredImage(Color32 color, float duration)
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("TemporaryCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 15;

        // Add a Panel (which is just an Image component with a default rect)
        GameObject panel = new GameObject("ColoredPanel");
        panel.transform.SetParent(canvas.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        
        // Load the "Background" sprite
        Sprite backgroundSprite = background; // Assumes the sprite is in a Resources folder

        // Set the sprite to the panel's Image component
        panelImage.sprite = backgroundSprite;
        panelImage.color = color;

        // Position and size the panel (you might adjust these values)
        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
        panelRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height); // Full screen size, adjust as needed

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Clean up
        Destroy(canvasObject);
    }

    public virtual void activateInput()
    {
        isInputActive = !isInputActive;
        if (isInputActive == true)
        { 
            // Reset answerbox input
            userInput = "";
            inputText.text = "";
            // Show answerbox
            popUpCanvasGroup.alpha = 1f;
        }
        else
        {
            // Close answerbox
            popUpCanvasGroup.alpha = 0f;
            checkStringInput();
        }
    }
}