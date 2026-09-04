using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerManager695 : AnswerManagerBase
{
    public TextMeshProUGUI problemText;
    public int d_nosign;
    public int e_nosign;
    public bool d_sign_addition;
    public bool e_sign_addition;

    public bool secondInput;
    public TwoStepMobileKeyboard02 keyboard;
    private System.Random rnd = new System.Random();

    public void Start()
    {
        // Pick random numerator from 1 to denominator inclusive (allows 100% cases like 1/1, 2/2, etc.)
        int range = 9;
        int d = Random.Range(1, range);
        int e = Random.Range(1, range);
        d_nosign = d;
        e_nosign = e;

        // Randomly make each sign positive (true) or negative (false) — 50/50 chance
        d_sign_addition = rnd.NextDouble() < 0.5f;
        e_sign_addition = rnd.NextDouble() < 0.5f;

        d = d_sign_addition ? d_nosign : -d_nosign;
        e = e_sign_addition ? e_nosign : -e_nosign;

        problemText.text = $"1y<sup>2</sup>+<color=#CF3422>{(d+e).ToString()}</color>y+<color=#CF3422>{(d*e).ToString()}</color>";
    }

    public bool oneAnswerCorrect_Check()
    {
        bool oneCorrectInput = false;
        if (keyboard.d == d_nosign.ToString() && keyboard.symbold_addition == d_sign_addition)
            oneCorrectInput = true;
        if (keyboard.d == e_nosign.ToString() && keyboard.symbole_addition == e_sign_addition)
            oneCorrectInput = true;

        // Return the result
        return oneCorrectInput;
    }

    public bool bothAnswersCorrect_Check()
    {
        bool bothCorrect = false;
        if ((keyboard.d == d_nosign.ToString()) && (keyboard.symbold_addition == d_sign_addition))
        {
            if ((keyboard.e == e_nosign.ToString()) && (keyboard.symbole_addition == e_sign_addition))
            {
                bothCorrect = true;
            }
        }
        else if ((keyboard.d == e_nosign.ToString()) && (keyboard.symbold_addition == e_sign_addition)) //Inverted Answer Case but still correct
        {
            if ((keyboard.e == d_nosign.ToString()) && (keyboard.symbole_addition == d_sign_addition))
            {
                bothCorrect = true;
            }
        }

        // Return the result
        return bothCorrect;
    }

    public override void checkStringInput()
    {
        if (mobileVersion)
        {
            // If rating not good enough
            if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
            {
                if (secondInput == true)
                {
                    if (bothAnswersCorrect_Check())
                    {
                        SceneComplete = true;
                        sceneCompleteScript.SceneComplete = true;
                        Button.image.color = Color.green;
                    }
                    else
                    {
                        Handheld.Vibrate();
                        secondInput = false;
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
                // Rating is high enough for skip
                if (secondInput == true)
                {
                    if (bothAnswersCorrect_Check())
                    {
                        SceneComplete = true;
                        sceneCompleteScript.SceneComplete = true;
                        Button.image.color = Color.green;
                    }
                    else
                    {
                        // No punish case BUT RESET WRONG
                        // THIS WONT WORK IF e/d not same length currently they can only be 1
                        if (keyboard.e.Length >= e_nosign.ToString().Length)
                        {
                            keyboard.Reset_Second();
                        }
                    }
                }
                else
                {
                    // Answer inputed is either E or D and correct
                    if (oneAnswerCorrect_Check())
                    {
                        secondInput = true;
                    }
                    else
                    {
                        // No punish case BUT RESET WRONG
                        if (keyboard.d.Length >= d_nosign.ToString().Length)
                        {
                            keyboard.Reset_First();
                        }
                    }
                }
            }
        }
        else
        {
        }
    }
}