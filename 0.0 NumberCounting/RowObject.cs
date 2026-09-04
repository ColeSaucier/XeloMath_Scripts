using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowObject : MonoBehaviour
{
    public GameObject boxPrefab;

    public int numInteractableLevel = 0;
    public int minRandomNumberOfBoxes = 1;
    public int maxRandomNumberOfBoxes = 9;
    public int answer; // Declare the answer variable

    public float xSpacingBetweenObjects = 1.25f;
    public float ySpacingBetweenRows = 1.25f;

    public Transform xCenterObject;
    public Transform yCenterObject; // Reference to the center object (e.g., Main Camera)

    public float xOffset = 0f; // Offset for the x position of the generated boxes
    public float yOffset = 0f; // Offset for the y position of the generated boxes
    public SceneCompleteMenu backend;

    void Start()
    {
        int numberOfBoxes = Random.Range(minRandomNumberOfBoxes, maxRandomNumberOfBoxes + 1);
        answer = numInteractableLevel + numberOfBoxes; // Assign value to answer variable

        int boxesPerRow = Mathf.CeilToInt(numberOfBoxes / 2f);
        int row1 = boxesPerRow;
        int row2 = numberOfBoxes - row1;

        GameObject parentObject = new GameObject("CirclesGroup"); // Create the parent object for circles

        // Calculate the initial x position for each row based on the number of objects and spacing
        float initialXRow1 = -(row1 - 1) * xSpacingBetweenObjects / 2f;
        float initialXRow2 = -(row2 - 1) * xSpacingBetweenObjects / 2f;

        // Generate the first row of boxes
        for (int i = 0; i < row1; i++)
        {
            float xPosition = initialXRow1 + i * xSpacingBetweenObjects + xOffset;
            Instantiate(boxPrefab, new Vector3(xPosition, yOffset, 0f), Quaternion.identity, parentObject.transform);
        }

        // Generate the second row of boxes
        for (int i = 0; i < row2; i++)
        {
            float xPosition = initialXRow2 + i * xSpacingBetweenObjects + xOffset;
            float yPosition = -ySpacingBetweenRows + yOffset;
            Instantiate(boxPrefab, new Vector3(xPosition, yPosition, 0f), Quaternion.identity, parentObject.transform);
        }
    }
}