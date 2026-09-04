using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class AnswerManager41 : AnswerManagerBase
{
    public bool secondInput = false;
    public FractionHandler fractionHandler;
    public int copiedNumerator;
    public int copiedDenominator;
    public TextMeshProUGUI numerator;
    public TextMeshProUGUI denominator;
   
    public TextMeshProUGUI keyboardNumerator;
    public TextMeshProUGUI keyboardDenominator;
    public FractionMobileKeyboardController01 keyboard;
    public override void checkStringInput()
    {
        copiedNumerator = fractionHandler.numeratorAns;
        copiedDenominator = fractionHandler.denominatorAns;
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
                        if (keyboardDenominator.text.Length >= copiedDenominator.ToString().Length)
                        {
                            keyboardDenominator.text = "";
                        }
                    }
                }
                else
                {
                    if (keyboardNumerator.text == copiedNumerator.ToString())
                    {
                        secondInput = true;
                    }
                    else
                    {
                        // No punish case BUT RESET WRONG
                        if (keyboardNumerator.text.Length >= copiedNumerator.ToString().Length)
                        {
                            keyboardNumerator.text = "";
                        }
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