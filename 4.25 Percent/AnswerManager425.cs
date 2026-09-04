using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerManager425 : AnswerManagerBase
{
    public TextMeshProUGUI problem; // Displays unreduced numerator
    public TextMeshProUGUI numeratorText; // Displays unreduced denominator
    public TextMeshProUGUI denominatorText; // Displays unreduced denominator

    private readonly int[] factorsOf100 = {2, 4, 5, 10, 20, 25, 50}; // List of all positive factors of 100

    public void Start()
    {
        // Pick random denominator (factor of 100)
        int randomIndex = Random.Range(0, factorsOf100.Length);
        int denominator = factorsOf100[randomIndex];
        denominatorText.text = denominator.ToString();

        // Pick random numerator from 1 to denominator inclusive (allows 100% cases like 1/1, 2/2, etc.)
        int numerator = Random.Range(1, denominator + 1);
        numeratorText.text = numerator.ToString();

        // Correct floating-point calculation
        float answer = (float)numerator / denominator * 100f;
        answerString = answer.ToString(); // e.g., "75.0" instead of "75" — adjust formatting as needed
    }
}