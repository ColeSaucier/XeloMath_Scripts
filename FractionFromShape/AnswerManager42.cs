using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerManager42 : AnswerManagerBase
{
    public bool secondInput = false;

    public TwoSeparateObjects objectGenerator;
    public int copiedNumerator;
    public int copiedDenominator;

    public TextMeshProUGUI numerator;
    public TextMeshProUGUI denominator;
    
    public TextMeshProUGUI keyboardNumerator;
    public TextMeshProUGUI keyboardDenominator;

    public FractionMobileKeyboardController02 keyboard;


    // Update is called once per frame
    public override void Update()
    {
        if (isInputActive)
        {
            if (mobileVersion != true)
            {
                // Check for input and handle it
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    checkStringInput();
                }
                else if (Input.GetKeyDown(KeyCode.Backspace) && userInput.Length > 0)
                {
                    userInput = userInput.Substring(0, userInput.Length - 1);
                }
                else
                {
                    userInput += Input.inputString;
                }

                if (secondInput == true)
                    denominator.text = userInput;
                else
                    numerator.text = userInput;
            }
        }
    }
    public override void checkStringInput()
    {
        copiedNumerator = objectGenerator.numerator;
        copiedDenominator = objectGenerator.numberOfSides;

        if (mobileVersion)
        {
            //If rating not good enough
            if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
            {
                if (secondInput == true)
                {
                    if (keyboardNumerator.text == copiedNumerator.ToString() && keyboardDenominator.text == copiedDenominator.ToString()) 
                    {
                        SceneComplete = true;
                        sceneCompleteScript.SceneComplete = true;
                        Button.image.color = Color.green;
                    }
                    else
                    {
                        Handheld.Vibrate();
                        secondInput = false;
                        keyboardNumerator.text = "";
                        keyboardDenominator.text = "";
                        keyboard.Reset_QuestionMark_Enable();
                        Color32 shiftColor = new Color32(210, 0, 0, 50);
                        base.DisplayColoredImage(shiftColor, 0.2f);
                    }
                }
                else
                {
                    secondInput = true;
                }
            }
            else
            {
                //Rating is high enough for skip
                if (secondInput == true)
                {
                    if (keyboardNumerator.text == copiedNumerator.ToString() && keyboardDenominator.text == copiedDenominator.ToString()) 
                    {
                        SceneComplete = true;
                        sceneCompleteScript.SceneComplete = true;
                        Button.image.color = Color.green;
                    }
                    else
                    {
                        // No punish case BUT RESET WRONG
                        keyboardDenominator.text = "";
                    }
                }
                else
                {
                    if (keyboardNumerator.text == copiedNumerator.ToString())
                    {
                        secondInput = true;
                    }
                }
            }
        }
        else
        {
            if (secondInput == true)
            {
                if (numerator.text == copiedNumerator.ToString() && denominator.text == copiedDenominator.ToString()) 
                {
                    SceneComplete = true;
                    sceneCompleteScript.SceneComplete = true;
                    Button.image.color = Color.green;
                    activateInput();
                }
                else
                {
                    activateInput();
                    Color32 shiftColor = new Color32(210, 0, 0, 50);
                    base.DisplayColoredImage(shiftColor, 0.2f);
                }
            }
            else
            {
                secondInput = true;
                userInput = "";
            }
        }
    }
}