using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerManager415 : AnswerManagerBase
{
    public bool secondInput = false;
    public FractionHandlerHard fractionHandler;
    public int copiedNumerator;
    public int copiedDenominator;

    public TextMeshProUGUI numerator;
    public TextMeshProUGUI denominator;

    public TextMeshProUGUI keyboardNumerator;
    public TextMeshProUGUI keyboardDenominator;

    public FractionMobileKeyboardController03 keyboard;

    // Update is called once per frame
    public override void Update()
    {
        if (isInputActive)
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
    public override void checkStringInput()
    {
        copiedNumerator = fractionHandler.numeratorAns;
        copiedDenominator = fractionHandler.denominatorAns;

        if (mobileVersion)
        {
            if (keyboardNumerator.text == copiedNumerator.ToString() && keyboardDenominator.text == copiedDenominator.ToString()) 
            {
                SceneComplete = true;
                sceneCompleteScript.SceneComplete = true;
                Button.image.color = Color.green;
            }
            else
            {
                //If rating not good enough
                if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
                {
                    if (secondInput == true)
                    {
                        keyboardDenominator.text = "";
                    }
                    else
                    {
                        keyboardNumerator.text = "";
                    }

                
                    Handheld.Vibrate();
                    Color32 shiftColor = new Color32(210, 0, 0, 50);
                    base.DisplayColoredImage(shiftColor, 0.2f);
                }
                // Rating good enough for enhanced speed
                else
                {

                    if (secondInput == true)
                    {
                        if (keyboardDenominator.text.Length >= copiedDenominator.ToString().Length)
                        {
                            keyboardDenominator.text = "";
                            Handheld.Vibrate();
                            Color32 shiftColor = new Color32(210, 0, 0, 50);
                            base.DisplayColoredImage(shiftColor, 0.2f);
                        }
                    }
                    else
                    {
                        if (keyboardNumerator.text.Length >= copiedNumerator.ToString().Length)
                        {
                            keyboardNumerator.text = "";
                            Handheld.Vibrate();
                            Color32 shiftColor = new Color32(210, 0, 0, 50);
                            base.DisplayColoredImage(shiftColor, 0.2f);
                        }
                    }

                }
            }
        }
        else
        {
            if (numerator.text == copiedNumerator.ToString() && denominator.text == copiedDenominator.ToString()) 
            {
                SceneComplete = true;
                sceneCompleteScript.SceneComplete = true;
                Button.image.color = Color.green;
            }
            else
            {
                Color32 shiftColor = new Color32(210, 0, 0, 50);
                base.DisplayColoredImage(shiftColor, 0.2f);
            }
        
            isInputActive = false;
            // Hide the pop-up canvas by setting its alpha to 0 (fully transparent)
            popUpCanvasGroup.alpha = 0f;
            Button.interactable = true;
        }
    }
}

