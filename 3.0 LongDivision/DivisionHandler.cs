using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections;

public class DivisionHandler : MonoBehaviour
{
    // References needed for answer button
    public Button Button;
    public string userInput = "";
    private bool isInputActive = false;
    public TextMeshProUGUI inputText;
    public CanvasGroup popUpCanvasGroup;
    public SceneCompleteMenu sceneCompleteScript;

    private Transform uselessGameObject1;
    private Transform uselessGameObject2;
    private GameObject uselessGameObjectHolder;

    public TextMeshProUGUI numeratorText;
    public TextMeshProUGUI divisorText;
    public TextMeshProUGUI answerAdditionText;
    public int divisor;
    public int answer;
    public int currentNumerator;
    public string answerString;
    public int subtractInput;

    public int correlatedAdditionInt;
    public bool additionNeeded;
    public bool finalAnswerNeeded;

    public TextMeshProUGUI subtractNum1;
    public TextMeshProUGUI resultNum1;
    public TextMeshProUGUI subtractNum2;
    public TextMeshProUGUI resultNum2;
    public TextMeshProUGUI subtractNum3;
    public TextMeshProUGUI resultNum3;
    public TextMeshProUGUI subtractNumFinal;
    public TextMeshProUGUI resultNumFinal;

    public CanvasGroup subtractCanvas1;
    public CanvasGroup subtractCanvas2;
    public CanvasGroup subtractCanvas3;
    public CanvasGroup subtractCanvasFinal;

    public GameObject AdditionArrow;
    public GameObject Arrow1;
    public GameObject Arrow2;
    public GameObject Arrow3;
    public GameObject ArrowFinal;

    public Canvas mobileKeyboard;
    private bool mobileVersion = true;
    public TextMeshProUGUI KeyboardInputText;
    
    public Image PlusSignImage;
    public Image MoveUpIndicator;
    void Start()
    {
        divisor = UnityEngine.Random.Range(2, 15);
        answer = UnityEngine.Random.Range(2, 51);
        currentNumerator = answer * divisor;

        numeratorText.text = currentNumerator.ToString();
        divisorText.text = divisor.ToString();
        answerString = answer.ToString();
    }

    public void Update()
    {
        if (isInputActive)
        {   
            // Real Keyboard Usage
            if (mobileVersion != true)
            {
                // Check for input and handle it
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    // Hide the pop-up canvas by setting its alpha to 0 (fully transparent)
                    popUpCanvasGroup.alpha = 0f;
                    popUpCanvasGroup.interactable = false; // Disable interactions with the pop-up canvas
                    isInputActive = false;
                    Button.image.color = Color.white;
                    checkstringinput();
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
        }
    }
    public void checkstringinput()
    {
        if (mobileVersion)
        {
            if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
            {
                inputText.text = KeyboardInputText.text;//.ToString();
                KeyboardInputText.text = "";
            }
        }

        // Scene ALMOST complete circumstance
        if (finalAnswerNeeded == true)
        {
            if (inputText.text == answer.ToString())
            {
                sceneCompleteScript.SceneComplete = true;
                Button.image.color = Color.green;
            }

            else
            {
                if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
                {
                    //Number not valid, restart
                    Handheld.Vibrate();
                    ResetScene();
                    Color32 shiftColor = new Color32(210, 0, 0, 50);
                    StartCoroutine(ShowColoredImage(shiftColor, 0.2f));
                }
            }
        }

        // Subtraction Here
        else if (!additionNeeded)
        {
            string subtractionString = inputText.text;

            if (subtractCanvas1.alpha == 0f)
            {
                validateInput(subtractionString);
                if ((subtractInput % divisor) == 0)
                    AddSubtractNumber(subtractInput, subtractNum1, resultNum1, subtractCanvas1);
            }
            else if (subtractCanvas2.alpha == 0f)
            {
                validateInput(subtractionString);
                if ((subtractInput % divisor) == 0)
                    AddSubtractNumber(subtractInput, subtractNum2, resultNum2, subtractCanvas2);
            }
            else if (subtractCanvas3.alpha == 0f)
            {
                validateInput(subtractionString);
                if ((subtractInput % divisor) == 0)
                    AddSubtractNumber(subtractInput, subtractNum3, resultNum3, subtractCanvas3);
                MoveNumbersUp();
            }
            else if (subtractCanvasFinal.alpha == 0f)
            {
                validateInput(subtractionString);
                if ((subtractInput % divisor) == 0)
                    AddSubtractNumber(subtractInput, subtractNumFinal, resultNumFinal, subtractCanvasFinal);
            } 
            else
            {
                validateInput(subtractionString);
                if ((subtractInput % divisor) == 0)
                    AddSubtractNumber(subtractInput, subtractNumFinal, resultNumFinal, subtractCanvasFinal);
                // Code to execute if subtractCanvas1 alpha is 0
            }
        }
        else if (additionNeeded)
        {
            string additionString = inputText.text;

            //Check For immediate answer case
            if (additionString == answer.ToString())
            {
                sceneCompleteScript.SceneComplete = true;
                Button.image.color = Color.green;
            }

            if (additionString == correlatedAdditionInt.ToString())
            {
                string secondString;
                if (answerAdditionText.text != "")
                    secondString = $"+{additionString}";
                else
                    secondString = $"{additionString}";
                string firstString = answerAdditionText.text;

                answerAdditionText.text = firstString + secondString;
                additionNeeded = false;

                if (currentNumerator == 0)
                {
                    finalAnswerNeeded = true;
                    answerAdditionText.text = answerAdditionText.text + "=";
                }
                else
                {
                    AdditionArrow.SetActive(false);
                    // Set active appropriate arrow
                    if (subtractCanvasFinal.alpha == 1f)
                        ArrowFinal.SetActive(true);
                    else if (subtractCanvas3.alpha == 1f)
                        ArrowFinal.SetActive(true);
                    else if (subtractCanvas2.alpha == 1f)
                        Arrow3.SetActive(true);
                    else if (subtractCanvas1.alpha == 1f)
                        Arrow2.SetActive(true);
                }
            }
            else
            {
                if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
                {
                    //Number not valid, restart
                    ResetScene();
                    Color32 shiftColor = new Color32(210, 0, 0, 50);
                    StartCoroutine(ShowColoredImage(shiftColor, 0.2f));
                }
            }
        }
    }
    /*
    public void activateInput()
    {
        isInputActive = true;
        userInput = "";

        // Show the pop-up canvas by setting its alpha to 1 (fully opaque)
        popUpCanvasGroup.alpha = 1f;
        popUpCanvasGroup.interactable = true; // Enable interactions with the pop-up canvas
        Button.image.color = Color.grey;
    }*/

    void validateInput(string input)
    {
        subtractInput = Int32.Parse(input);
        int input10x = 10 * subtractInput;
        int input100x = 100 * subtractInput;

        if (input100x <= currentNumerator && (input100x % divisor) == 0)
        {
            subtractInput = input100x;
            return;
        }
        else if (input10x <= currentNumerator && (input10x % divisor) == 0)
        {
            subtractInput = input10x;
            return;
        }
        else if (subtractInput <= currentNumerator && (subtractInput % divisor) == 0)
        {
            return;
        }
        else
        {
            if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
            {
                subtractInput = 1009;
                //Number not valid, restart
                ResetScene();
                Color32 shiftColor = new Color32(210, 0, 0, 50);
                StartCoroutine(ShowColoredImage(shiftColor, 0.2f));
            }
        }
    }

    void AddSubtractNumber(int subtractInput, TextMeshProUGUI subtract, TextMeshProUGUI result, CanvasGroup subtractCanvas)
    {
        subtractCanvas.alpha = 1f;
        subtract.text = subtractInput.ToString();

        int resultInt = currentNumerator - subtractInput;
        result.text = resultInt.ToString();
        currentNumerator = resultInt;

        AdditionArrow.SetActive(true);
        additionNeeded = true;
        ArrowFinal.SetActive(false);
        Arrow3.SetActive(false);
        Arrow2.SetActive(false);
        Arrow1.SetActive(false);
        correlatedAdditionInt = subtractInput/divisor;

        if (currentNumerator == 0)
        {
            GameObject uselessGameObjectHolder = subtractCanvas.gameObject;
            Transform uselessGameObject1 = uselessGameObjectHolder.transform.Find("LineImage");
            Transform uselessGameObject2 = uselessGameObjectHolder.transform.Find("PlusSignImage");
            uselessGameObject1.gameObject.SetActive(false);
            uselessGameObject2.gameObject.SetActive(false);
        }
    }

    void ResetScene()
    {
        currentNumerator = divisor * answer;
        if (uselessGameObject1 != null)
        {
            uselessGameObject1.gameObject.SetActive(true);
            uselessGameObject2.gameObject.SetActive(true);
        }
        resultNum3.text = "";
        resultNum2.text = "";
        resultNum1.text = "";
        resultNumFinal.text = "";
        subtractNum3.text = "";
        subtractNum2.text = "";
        subtractNum1.text = "";
        subtractNumFinal.text = "";
        answerAdditionText.text = "";

        subtractCanvasFinal.alpha = 0f;
        subtractCanvas3.alpha = 0f;
        subtractCanvas2.alpha = 0f;
        subtractCanvas1.alpha = 0f;
        additionNeeded = false;
        finalAnswerNeeded = false;
        AdditionArrow.SetActive(false);
        ArrowFinal.SetActive(false);
        Arrow3.SetActive(false);
        Arrow2.SetActive(false);
        Arrow1.SetActive(true);
        PlusSignImage.gameObject.SetActive(true);
        MoveUpIndicator.gameObject.SetActive(false);
    }

    void MoveNumbersUp()
    {
        subtractCanvas3.alpha = 0f;
        MoveUpIndicator.gameObject.SetActive(true);
        PlusSignImage.gameObject.SetActive(false);
        resultNum1.text = resultNum2.text;
        resultNum2.text = resultNum3.text;
        resultNum3.text = resultNumFinal.text;
        resultNumFinal.text = "";
        subtractNum1.text = "";
        subtractNum2.text = subtractNum3.text;
        subtractNum3.text = subtractNumFinal.text;
        subtractNumFinal.text = "";
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
}