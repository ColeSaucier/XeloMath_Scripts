using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[CreateAssetMenu(fileName = "HardProblem", menuName = "HardProblem")]
public class HardProblemClass : ScriptableObject
{
    public Sprite displayImage;       // Initial image (e.g., feedback/reward)
    public string displayText;        // Initial text (e.g., "Great job!")

    [Header("Solution Reveal")]
    public Sprite solutionImage;      // Image showing the solution (e.g., correct fraction, graph, etc.)
    public string solutionText;       // Text explaining the solution (e.g., "21/18 reduces to 7/6")
}