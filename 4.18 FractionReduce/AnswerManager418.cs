using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class AnswerManager418 : AnswerManagerBase
{
    public bool secondInput = false;
    public int answNumerator;
    public int answDenominator;
    public TextMeshProUGUI numeratorText;      // Displays unreduced numerator
    public TextMeshProUGUI denominatorText;    // Displays unreduced denominator
   
    public TextMeshProUGUI keyboardNumerator;
    public TextMeshProUGUI keyboardDenominator;
    public FractionMobileKeyboardController04 keyboard;

    private System.Random rnd = new System.Random();

    public void Start()
    {
        GenerateFraction();
    }
    // Helper: Greatest Common Divisor (Euclidean algorithm)
    private int GCD(int a, int b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        while (b != 0)
        {
            int temp = b;
            b = a % b;
            a = temp;
        }
        return a;
    }
    private void GenerateFraction()
    {
        // Generate denominator 2–12 (avoid 1 to prevent whole numbers)
        int denominator = rnd.Next(3, 13); // 2 to 12

        // Numerator up to 2x denominator (proper or improper fraction)
        int baseNumerator;
        if (denominator < 9)
            baseNumerator = rnd.Next(1, denominator * 2 + 1);
        else
            baseNumerator = rnd.Next(1, denominator + 2);

        if (baseNumerator == denominator)
            baseNumerator += 1;

        // Determine GCF range
        int maxGCF = (denominator > 7) ? 8 : 12;

        // GCF from 2 to maxGCF (ensure fraction is reducible)
        int gcf = rnd.Next(2, maxGCF + 1);

        // FINAL SAFETY: Reduce again if they still share factors (rare but possible edge case)
        int finalGCD = GCD(baseNumerator, denominator);
        if (finalGCD > 1)
        {
            baseNumerator /= finalGCD;
            denominator /= finalGCD;
            Debug.LogWarning($"Extra reduction applied: GCD={finalGCD}");
        }

        // Unreduced fraction for display
        string unreducedNumerator = (baseNumerator * gcf).ToString();
        string unreducedDenominator = (denominator * gcf).ToString();

        // Reduced fraction is the answer
        int reducedNumerator = baseNumerator;
        int reducedDenominator = denominator;

        // Set display (unreduced)
        numeratorText.text = unreducedNumerator;
        denominatorText.text = unreducedDenominator; 

        // For checking
        answNumerator = reducedNumerator;
        answDenominator = reducedDenominator;

        Debug.LogError($"Generated unreduced: {unreducedNumerator}/{unreducedDenominator} → reduced: {reducedNumerator}/{reducedDenominator} (GCF={gcf})");
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
                    if (keyboardNumerator.text == answNumerator.ToString() && keyboardDenominator.text == answDenominator.ToString())
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
                // Rating is high enough for skip
                if (secondInput == true)
                {
                    if (keyboardNumerator.text == answNumerator.ToString() && keyboardDenominator.text == answDenominator.ToString())
                    {
                        SceneComplete = true;
                        sceneCompleteScript.SceneComplete = true;
                        Button.image.color = Color.green;
                    }
                    else
                    {
                        // No punish case BUT RESET WRONG
                        if (keyboardDenominator.text.Length >= answDenominator.ToString().Length)
                        {
                            keyboardDenominator.text = "";
                        }
                    }
                }
                else
                {
                    if (keyboardNumerator.text == answNumerator.ToString())
                    {
                        secondInput = true;
                    }
                    else
                    {
                        // No punish case BUT RESET WRONG
                        if (keyboardNumerator.text.Length >= answNumerator.ToString().Length)
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
                if (keyboardNumerator.text == answNumerator.ToString() && keyboardDenominator.text == answDenominator.ToString())
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

    // Optional: call this to regenerate for testing/next question
    public void RegenerateFraction()
    {
        GenerateFraction();
    }
}