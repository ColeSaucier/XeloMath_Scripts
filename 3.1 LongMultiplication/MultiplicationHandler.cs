using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

public class MultiplicationHandler : MonoBehaviour
{
	// References needed for answer button
    public Button Button;
    public string userInput = "";
    private bool isInputActive = false;
    public TextMeshProUGUI inputText;
    public CanvasGroup popUpCanvasGroup;
    public SceneCompleteMenu sceneCompleteScript;

    public TextMeshProUGUI Num1text;
    public TextMeshProUGUI Num2text;
    public TextMeshProUGUI Num3text;
    public TextMeshProUGUI Num4text;

    public TextMeshProUGUI FinalText;
    public GameObject FinalLineAndNum;
    public GameObject PlusSymbol;
    public GameObject ArithmeticFrivilous;

    public GameObject Equation1;
    public GameObject Equation2;
    public GameObject Equation3;
    public GameObject Equation4;
    private int equationsTotal;
    private int firstNumDigits;
    private int secondNumDigits;

    //Alway active (original Nums)
    public TextMeshProUGUI FirstNumText;
    public TextMeshProUGUI SecondNumText;
    public int FirstNum;
    public int SecondNum;

    //Answers
    public int equation1Answer;
    public int equation2Answer;
    public int equation3Answer;
    public int equation4Answer;
    public int finalAnswer;
    public int currentAnswer;

    public bool finalAnswerNeeded;

    public Canvas mobileKeyboard;
    private bool mobileVersion = true;
    public TextMeshProUGUI KeyboardInputText;
    void Start()
    {
    	FirstNum = UnityEngine.Random.Range(2, 100);
        SecondNum = UnityEngine.Random.Range(2, 100);
        FirstNumText.text = FirstNum.ToString();
        SecondNumText.text = SecondNum.ToString();

        // Calculate the number of digits
        firstNumDigits = CountDigits(FirstNum);
        secondNumDigits = CountDigits(SecondNum);
        equationsTotal = firstNumDigits * secondNumDigits;

        // Calculate answers
        equation1Answer = (FirstNum%10) * (SecondNum%10); //Always going to be valid
	    equation2Answer = (FirstNum / 10) * (SecondNum % 10) * 10; //
        equation3Answer = (FirstNum % 10) * (SecondNum / 10) * 10;
        equation4Answer = (FirstNum/10) * (SecondNum/10) * 100;
        finalAnswer = FirstNum * SecondNum;
        currentAnswer = equation1Answer;

        if (secondNumDigits == 1)
        {
            equation3Answer = 17;
            equation4Answer = 17;
        }
        if (firstNumDigits == 1)
        {
            equation2Answer = 17;
            equation4Answer = 17;
        }

        //Move the Addition symbol and Line appropriately FinalLineAndNum and PlusSymbol
        int yMultiplier =  4 - equationsTotal;
        Vector3 currentPosition = FinalLineAndNum.transform.position;
        currentPosition.y = currentPosition.y + 160 * yMultiplier;
        FinalLineAndNum.transform.position = currentPosition;
        Equation1.SetActive(true);

        if (equation4Answer > 0)
        {
        	currentPosition = PlusSymbol.transform.position;
        	currentPosition.x = currentPosition.x - 60;
        	PlusSymbol.transform.position = currentPosition;
        }
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
                    checkStringInput();
                    //isInputActive = false;
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
    public void checkStringInput()
    {
        if (mobileVersion)
        {
            inputText.text = KeyboardInputText.text;
        }

        if (inputText.text == currentAnswer.ToString())
        {
            if (finalAnswerNeeded == true)
            {
                //Scene Complete
                sceneCompleteScript.SceneComplete = true;
                Button.image.color = Color.green;
            }
            else if (Num1text.text == "")
            {
                Num1text.text = inputText.text;
                confirmNextEqAndAnswer();
            }
            else if (Num2text.text == "")
            {
                Num2text.text = inputText.text;
                confirmNextEqAndAnswer();
            }
            else if (Num3text.text == "")
            {
                Num3text.text = inputText.text;
                confirmNextEqAndAnswer();
            }
            else if (Num4text.text == "")
            {
                Num4text.text = inputText.text;
                confirmNextEqAndAnswer();
            }

            if (mobileVersion)
            {
                KeyboardInputText.text = "";
            }
            else
            {
                inputText.text = "";
            }
        }
        else
        {
            if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
            {
                // Best rating is bad, so punish
                Handheld.Vibrate();
                Color32 shiftColor = new Color32(210, 0, 0, 50);
                StartCoroutine(ShowColoredImage(shiftColor, 0.2f));
                ResetScene();
                if (mobileVersion)
                {
                    KeyboardInputText.text = "";
                }
                else
                {
                    inputText.text = "";
                }
            }
            else
            {
            }
        }
    }
    public void confirmNextEqAndAnswer()
    {
    	if (Num2text.text == "")
    	{
            if (equation3Answer == 17 & equation2Answer == 17)
            {
                Equation1.SetActive(false);
                currentAnswer = finalAnswer;
                finalAnswerNeeded = true;
                ArithmeticFrivilous.SetActive(true);
            }
            else if (equation2Answer == 17)
            {
                currentAnswer = equation3Answer;
                Equation3.SetActive(true);
                Equation1.SetActive(false);
            }
            else
            {
                currentAnswer = equation2Answer;
                Equation2.SetActive(true);
                Equation1.SetActive(false);
            }
    	}
    	else if (Num3text.text == "")
    	{
            if (equation3Answer == 17)
            {
                Equation1.SetActive(false);
                Equation2.SetActive(false);
                currentAnswer = finalAnswer;
                finalAnswerNeeded = true;
                ArithmeticFrivilous.SetActive(true);
            }
            else if (equation2Answer == 17)
            {
                Equation1.SetActive(false);
                Equation2.SetActive(false);
                Equation3.SetActive(false);
                currentAnswer = finalAnswer;
                finalAnswerNeeded = true;
                ArithmeticFrivilous.SetActive(true);
            }
            else
            {
                currentAnswer = equation3Answer;
                Equation3.SetActive(true);
                Equation2.SetActive(false);
            }
    	}
    	else if (Num4text.text == "")
    	{
            //if (equation4Answer != 0)
            //{
            Equation3.SetActive(false);
            currentAnswer = equation4Answer;
    		Equation4.SetActive(true);
    		//}
    		//else
    		//{
    		//	currentAnswer = finalAnswer;
    		//	finalAnswerNeeded = true;
    		//}
    	}
    	else
    	{
            Equation4.SetActive(false);
            currentAnswer = finalAnswer;
    		finalAnswerNeeded = true;
            ArithmeticFrivilous.SetActive(true);
    	}
    	Equation1.SetActive(false);
    }

    public void activateInput()
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
    public void ResetScene()
    {
    	Num1text.text = "";
    	Num2text.text = "";
    	Num3text.text = "";
    	Num4text.text = "";

    	Equation1.SetActive(true);
    	Equation2.SetActive(false);
    	Equation3.SetActive(false);
    	Equation4.SetActive(false);
        ArithmeticFrivilous.SetActive(false);
        currentAnswer = equation1Answer;
        finalAnswerNeeded = false;
    }
    // Function to count the number of digits in a number
    int CountDigits(int number)
    {
        int count = 0;
        while (number != 0)
        {
            number /= 10;
            count++;
        }
        return count;
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