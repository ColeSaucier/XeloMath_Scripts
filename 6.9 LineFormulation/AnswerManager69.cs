using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using Random = UnityEngine.Random;
using UObject = UnityEngine.Object;

public class AnswerManager69 : AnswerManagerBase
{
    public bool secondInput = false;

    public int slope;
    public int constant;

    //public TextMeshProUGUI numerator; //not used except by non movile
    //public TextMeshProUGUI denominator;
    
    public TextMeshProUGUI keyboardSlope;
    public TextMeshProUGUI keyboardConstant;

    public TwoStepMobileKeyboard01 keyboard;
    // ── Inspector / Public fields ───────────────────────────────────────────
    [Header("Graph References & Style")]
    [SerializeField] private RectTransform graphContainer;          // Must assign in Inspector
    [SerializeField] private Sprite dotSprite;                      // Optional - small circle for points
    [SerializeField] private Color lineColor       = Color.cyan;
    [SerializeField] private Color gridColor       = new Color(0.3f, 0.3f, 0.3f);
    [SerializeField] private Color zeroAxisColor   = Color.yellow;
    [SerializeField] private float lineThickness   = 4f;
    [SerializeField] private float gridThickness   = 1f;
    [SerializeField] private float zeroAxisThickness = 2.5f;
    [SerializeField] private bool  showDots        = true;
    [SerializeField] private float  fontSize        = 80;

    [Header("Dot Appearance")]
    [SerializeField] private float magnitudeDotMultiplier = 1.0f;

    // ── Private cached values ────────────────────────────────────────────────
    private float graphWidth;
    private float graphHeight;
    private const float minX = -1f;
    private const float maxX = 9f;
    private float xRange;

    void Start()
    {
        if (graphContainer == null)
        {
            Debug.LogError("GraphContainer is not assigned in AnswerManager69!", this);
            return;
        }

        // Cache dimensions once
        graphWidth = graphContainer.sizeDelta.x;
        graphHeight = graphContainer.sizeDelta.y;
        xRange = maxX - minX;

        GenerateAndShowGraph();
    }
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
                /*
                if (secondInput == true)
                    denominator.text = userInput;
                else
                    numerator.text = userInput;*/
            }
        }
    }
    public override void checkStringInput()
    {
        if (mobileVersion)
        {
            //If rating not good enough
            if (int.Parse(sceneCompleteScript.sceneObject.bestRating) < 2)
            {
                if (secondInput == true)
                {
                    if (keyboardSlope.text == slope.ToString() && keyboardConstant.text == constant.ToString()) 
                    {
                        SceneComplete = true;
                        sceneCompleteScript.SceneComplete = true;
                        Button.image.color = Color.green;
                    }
                    else
                    {
                        Handheld.Vibrate();
                        secondInput = false;
                        keyboardSlope.text = "";
                        keyboardConstant.text = "";
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
                    if (keyboardSlope.text == slope.ToString() && keyboardConstant.text == constant.ToString()) 
                    {
                        SceneComplete = true;
                        sceneCompleteScript.SceneComplete = true;
                        Button.image.color = Color.green;
                    }
                    else
                    {
                        // No punish case BUT RESET WRONG
                        keyboardConstant.text = "";
                    }
                }
                else
                {
                    if (keyboardSlope.text == constant.ToString())
                    {
                        secondInput = true;
                    }
                }
            }
        }
        else
        {
            /*
            if (secondInput == true)
            {
                if (numerator.text == slope.ToString() && denominator.text == constant.ToString()) 
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
            */
        }
    }

    private void GenerateAndShowGraph()
    {
        // ── Generate random linear data (two points) ─────────────────────────
        slope = Random.Range(0, 13);
        constant = Random.Range(-5, 5);
        int x_change = Random.Range(1, 5);

        // Determine sign and symbol
        int slope_neg = Random.Range(0, 2);
        if (slope_neg == 1)
        {
            slope = slope * -1;
        }

        answerString = $"{slope}x{constant}";
        // Create point x, y based on formula
        float x1 = Random.Range(-1, 6);
        float x2 = x1 + x_change;
        float y1 = x1*slope + constant;
        float y2 = x2*slope + constant;

        //Check if scale can be reduced
        int scale = 2;
        int y_diff_max = 24;
        if (Math.Abs(y2-y1) < 12)
        {
            scale = 1;
            y_diff_max = 12;
        }
        // Scale isnt reduced, but we are out of bounds. Reduce X2 and recalculate Y.
        while (Math.Abs(y2-y1) > y_diff_max)
        {
            x2 -= 1;
            y2 = x2*slope + constant;
        }

        // Create points and calculate necesities for graph generation
        List<float> xValues = new List<float> { x1, x2 };
        List<float> yValues = new List<float> { y1, y2 };

        // ── Compute center and round it to nearest integer ───────────────────
        float rawCenter = (y1 + y2) / 2f;
        float centerY = Mathf.Round(rawCenter);     // ← key change: round to nearest whole number

        // ── Determine visible range and grid step (scale) ─────────────────────
        float yHalfRange = Mathf.Max(Mathf.Abs(y1 - rawCenter), Mathf.Abs(y2 - rawCenter)) * 1.3f;
        float graphScale = yHalfRange / 5f;         // aim ~5 steps to edge

        // Force graphScale to be integer or half-integer so labels stay clean
        graphScale = Mathf.Round(graphScale * 2f) / 2f;  // e.g. 1.0, 1.5, 2.0, 2.5, ...
        graphScale = Mathf.Max(graphScale, 1f);     // prevent too tiny grids

        // ── Draw ──────────────────────────────────────────────────────────────
        GraphManager.DrawLineGraph(
            graphContainer,
            constant, slope,
            xValues, yValues,
            centerY, scale,
            graphWidth, graphHeight,
            lineColor, gridColor, zeroAxisColor,
            lineThickness, gridThickness, zeroAxisThickness,
            showDots, fontSize, magnitudeDotMultiplier, dotSprite
        );
    }

    // Optional: public method so you can regenerate on button / event
    public void RegenerateGraph()
    {
        GenerateAndShowGraph();
    }
}

// ─────────────────────────────────────────────────────────────────────────────
// Static helper – no MonoBehaviour, no serialized fields
// ─────────────────────────────────────────────────────────────────────────────
public static class GraphManager
{
    public static void DrawLineGraph(
        RectTransform container,
        int constant,
        int slope,
        List<float> xValues,
        List<float> yValues,
        float centerY,
        float graphScale,
        float graphWidth,
        float graphHeight,
        Color lineColor,
        Color gridColor,
        Color zeroAxisColor,
        float lineThickness,
        float gridThickness,
        float zeroAxisThickness,
        bool showDots,
        float fontSize,
        float magnitudeDotMultiplier,
        Sprite dotSprite = null)
    {
        ClearGraph(container);

        if (xValues.Count < 2 || yValues.Count < 2) return;

        float yMin   = centerY - (graphScale * 6f);
        float yMax   = centerY + (graphScale * 6f);
        Debug.LogError($"{yMin} yMin, {yMax} yMax");
        float yRange = yMax - yMin;

        // Grid + labels
        DrawAxisAndGrid(container, constant, slope, centerY, graphScale, yMin, yMax, yRange,
                        graphWidth, graphHeight, gridColor, zeroAxisColor, lineColor,
                        gridThickness, zeroAxisThickness, fontSize);

        // Line + optional dots
        float xNorm0 = NormalizeX(xValues[0], graphWidth);
        float yNorm0 = NormalizeY(yValues[0], yMin, yRange, graphHeight);
        Vector2 prevPos = new Vector2(xNorm0, yNorm0);
        float xNorm1 = NormalizeX(xValues[1], graphWidth);
        float yNorm1 = NormalizeY(yValues[1], yMin, yRange, graphHeight);
        Vector2 pos = new Vector2(xNorm1, yNorm1);

        if (showDots)
        {
            CreateDot(container, prevPos, dotSprite, magnitudeDotMultiplier);
            CreateDot(container, pos, dotSprite, magnitudeDotMultiplier);
        }
        Debug.LogError($"yMin: {yMin:F2}, yMax: {yMax:F2}, yRange: {yRange:F2}");
        Debug.LogError($"y1={yValues[0]:F2} → normY={NormalizeY(yValues[0], yMin, yRange, graphHeight):F2}");
        Debug.LogError($"y2={yValues[1]:F2} → normY={NormalizeY(yValues[1], yMin, yRange, graphHeight):F2}");
    }

    private static float NormalizeX(float x, float graphWidth)
    {
        const float minX = -1f;
        const float maxX = 9f;
        float xRange = maxX - minX;
        return ((x - minX) / xRange) * graphWidth;
    }

    private static float NormalizeY(float y, float yMin, float yRange, float graphHeight)
    {
        return ((y - yMin) / yRange) * graphHeight;
    }

    private static void CreateLineSegment(RectTransform parent, Vector2 a, Vector2 b, Color color, float thickness)
    {
        GameObject go = new GameObject("Line", typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = Vector2.zero;

        Vector2 dir = (b - a).normalized;
        float distance = Vector2.Distance(a, b);
        rt.sizeDelta = new Vector2(distance, thickness);
        rt.anchoredPosition = a;
        rt.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        go.GetComponent<Image>().color = color;
    }

    private static void CreateDot(RectTransform parent, Vector2 pos, Sprite sprite, float magnitudeDotMultiplier)
    {
        GameObject dot = new GameObject("Dot", typeof(Image));
        dot.transform.SetParent(parent, false);
        RectTransform rt = dot.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = new Vector2(11f*magnitudeDotMultiplier, 11f*magnitudeDotMultiplier);
        rt.anchoredPosition = pos;

        Image img = dot.GetComponent<Image>();
        img.sprite = sprite;
        img.color = Color.white; // or match line color, etc.
    }

    private static void DrawAxisAndGrid(RectTransform parent, int constant, int slope,
        float centerY, float graphScale, float yMin, float yMax, float yRange,
        float graphWidth, float graphHeight,
        Color gridColor, Color zeroColor, Color lineColor,
        float gridThick, float zeroThick, float fontSize)
    {
        // Draw vertical Y-axis at x = 0
        float originXPixel = NormalizeX(0f, graphWidth);
        CreateVerticalYAxis(parent, originXPixel, graphHeight, zeroColor, zeroThick);

        // Horizontal grids + Y labels (still -6 to +6 steps from center)\
        int lastK = 0;
        int lastX = 0;
        for (int k = -6; k <= 6; k++)
        {
            // Calculate the exact grid value using the rounded center + integer steps
            float gridY = centerY + k * graphScale;

            // Skip if completely outside visible range
            if (gridY < yMin || gridY > yMax) continue;

            float normY = NormalizeY(gridY, yMin, yRange, graphHeight);

            // Determine if this is close to y=0
            bool isZero = Mathf.Abs(gridY) < 0.01f;  // tolerance for floating-point

            if (isZero)
            {
                CreateHorizontalGrid(parent, normY, graphWidth,
                    isZero ? zeroThick : gridThick,
                    isZero ? zeroColor : gridColor);
            }
            // Force label to integer (round to nearest whole number)
            float displayValue = Mathf.Round(gridY);
            Debug.LogError($"{displayValue}");

            CreateYLabel(parent, normY, displayValue, isZero ? zeroColor : Color.white, fontSize);
            lastK = k;   // will end up as 6
        }
        // X labels unchanged (every 2 units)
        for (int x = -1; x <= 9; x += 2)
        {
            float normX = NormalizeX(x, graphWidth);
            CreateXLabel(parent, normX, x, fontSize, Color.white);
            lastX = x;
        }
        float norm0 = NormalizeX(0, graphWidth);
        CreateXLabel(parent, norm0, 0, fontSize, zeroColor);

        // Left graph edge
        float yAtLeft  = constant + slope * -1;   // at x = -1 minX !!!!
        float yAtRight = constant + slope * 9;   // at x = 9 minX

        // Clamp to visible y range
        float startY = Mathf.Clamp(yAtLeft,  yMin, yMax);
        float endY   = Mathf.Clamp(yAtRight, yMin, yMax);

        // Back-calculate the x where the clamped y occurs
        float startX = (startY - constant) / (float)slope;
        float endX   = (endY   - constant) / (float)slope;

        // Pixel positions
        Vector2 startPos = new Vector2(NormalizeX(startX, graphWidth), NormalizeY(startY, yMin, yRange, graphHeight));
        Vector2 endPos   = new Vector2(NormalizeX(endX,   graphWidth), NormalizeY(endY,   yMin, yRange, graphHeight));

        // Draw
        CreateLineSegment(parent, startPos, endPos, lineColor, zeroThick);
    }

    private static void CreateHorizontalGrid(RectTransform parent, float yPos, float width, float thickness, Color col)
    {
        GameObject go = new GameObject("GridH", typeof(Image));
        go.transform.SetParent(parent, false);
        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;
        rt.sizeDelta = new Vector2(width, thickness);
        rt.anchoredPosition = new Vector2(0, yPos);
        go.GetComponent<Image>().color = col;
    }

    private static void CreateYLabel(RectTransform parent, float yPos, float value, Color textColor, float fontSize)
    {
        GameObject lbl = new GameObject("YLabel", typeof(TextMeshProUGUI));
        lbl.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = lbl.GetComponent<TextMeshProUGUI>();
        
        // Show as integer (no decimals)
        tmp.text = value.ToString("F0");   // ← F0 = no decimal places
        tmp.color = textColor;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Right;
        tmp.enableWordWrapping = false;

        RectTransform rt = lbl.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(1f, 0.5f);
        rt.anchoredPosition = new Vector2(-30f, yPos);
        rt.sizeDelta = new Vector2(60f, 30f);
    }

    private static void CreateXLabel(RectTransform parent, float xPos, float value, float fontSize, Color textColor)
    {
        GameObject lbl = new GameObject("XLabel", typeof(TextMeshProUGUI));
        lbl.transform.SetParent(parent, false);
        TextMeshProUGUI tmp = lbl.GetComponent<TextMeshProUGUI>();
        tmp.text = value.ToString("F0");
        tmp.color = textColor;
        tmp.fontSize = fontSize;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = false;

        RectTransform rt = lbl.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = Vector2.zero;
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(xPos, -40f);   // below axis
        rt.sizeDelta = new Vector2(40f, 30f);
    }

    private static void ClearGraph(RectTransform container)
    {
        for (int i = container.childCount - 1; i >= 0; i--)
        {
            UObject.Destroy(container.GetChild(i).gameObject);
        }
    }
    private static void CreateVerticalYAxis(RectTransform parent, 
                                            float xPixelPosition, 
                                            float graphHeight, 
                                            Color color, 
                                            float thickness)
    {
        GameObject axis = new GameObject("YAxis", typeof(Image));
        axis.transform.SetParent(parent, false);
        RectTransform rt = axis.GetComponent<RectTransform>();

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.zero;
        rt.pivot = Vector2.zero;

        rt.anchoredPosition = new Vector2(xPixelPosition, 0f);

        rt.sizeDelta = new Vector2(thickness, graphHeight);

        Image img = axis.GetComponent<Image>();
        img.color = color;
    }
}