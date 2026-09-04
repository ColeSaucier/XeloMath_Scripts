using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CircleColumnManipulation : MonoBehaviour
{
    public Slider uiSlider;
    public GameObject circlePrefab; // Reference to the prefab asse
    public GameObject remainderPrefab; // Reference to the prefab asse
    public TextMeshProUGUI num1Text; // Reference to TextMeshPro for randomNum1
    public TextMeshProUGUI num2Text; // Reference to TextMeshPro for randomNum2
    public TextMeshProUGUI ColumnsNum;
    public TextMeshProUGUI ArrayRowNum;
    public TextMeshProUGUI TopArrayColumnNum;

    public int internalColumn;
    public int numerator;
    public int numberOfRows;
    public int remainder;
    public int answer;

    public float distanceModifier = 0.5f; // Modifier for decreasing vector distance
    public float xOffsetModifier = 1.5f; // X offset modifier
    public float yOffsetModifier = 1.0f; // Y offset modifier

    void Start()
    {
        // Randomly generate two integers between 1 and 10
        int randomNum1 = Random.Range(3, 11);
        int randomNum2 = Random.Range(3, 11);

        // Multiply the two integers to generate the numerator
        numerator = randomNum1 * randomNum2;
        answer = randomNum1;

        // Set TextMeshPro text values
        num1Text.text = numerator.ToString();
        num2Text.text = randomNum2.ToString();

        // Update internalValue
        internalColumn = Mathf.RoundToInt(uiSlider.value);

        // Call the method to update numberOfRows and remainder
        UpdateCalculation();

        // Generate objects in a grid
        GenerateObjectsGrid();
    }

    void Update()
    {
        // Update internalValue based on the UI Slider value
        internalColumn = Mathf.RoundToInt(uiSlider.value);
        ColumnsNum.text = $"Rows = {internalColumn.ToString()}";
        ArrayRowNum.text = internalColumn.ToString();

        // Call the method to update numberOfRows and remainder
        UpdateCalculation();

        // Generate objects in a grid
        GenerateObjectsGrid();
    }

    void UpdateCalculation()
    {
        // Calculate numberOfRows and remainderPrefab
        try
        {
            numberOfRows = numerator / internalColumn;
            remainder = numerator % internalColumn;

            TopArrayColumnNum.text = numberOfRows.ToString();
            if (remainder == 0)
                TopArrayColumnNum.text = numberOfRows.ToString();
            else
                TopArrayColumnNum.text = $"{numberOfRows.ToString()}<color=#CF3423>r{remainder.ToString()}</color>";
        }
        catch
        {
            TopArrayColumnNum.text = "Error";
        }
    }

    void GenerateObjectsGrid()
    {
        // Clear existing objects
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Generate objects in a grid
        for (int col = 0; col < internalColumn; col++)
        {
            for (int row = 0; row < numberOfRows; row++)
            {
                float x = row + xOffsetModifier;
                float y = -col + yOffsetModifier;

                Instantiate(circlePrefab, new Vector3(x, y, 0) * distanceModifier, Quaternion.identity, transform);
            }
        }

        // Incomplete column with remainder
        for (int row = 0; row < remainder; row++)
        {
            float x = row + xOffsetModifier;
            float y = -internalColumn + yOffsetModifier;

            Instantiate(remainderPrefab, new Vector3(x, y, 0) * distanceModifier, Quaternion.identity, transform);
        }
    }
}
