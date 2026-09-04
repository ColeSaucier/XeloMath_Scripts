using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject squarePrefab;
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public float intitialXOffset = 1;
    public float intitialYOffset = 1;
    public float spacing = 1.0f;
    public bool working = false;
    public GameObject parentObject;

    private GameObject clickedSquare = null;

    private void Start()
    {
        CreateGrid();
    }

    void CreateGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 spawnPosition = new Vector3(x * spacing, y * spacing, 0);
                Quaternion spawnRotation = Quaternion.identity;

                GameObject square = Instantiate(squarePrefab, spawnPosition, spawnRotation);
                square.name = $"{x}, {y}"; // Assign a unique name based on (x, y) coordinates


                square.transform.SetParent(parentObject.transform);

                // Attach this script to the square and set its coordinates
                SquareScript squareScript = square.AddComponent<SquareScript>();
                squareScript.ToggleHighlight();
            }
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            //if (Physics.Raycast(ray, out hit))
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                working = true;
                GameObject clickedObject = hit.collider.gameObject;
                Renderer squareRenderer = clickedObject.GetComponent<Renderer>();
                squareRenderer.material.color = new Color32(207, 52, 35, 50);

                //SquareScript squareScript = clickedObject.GetComponent<SquareScript>();

                // Handle highlighting logic here
                //squareScript.ToggleHighlight();
            }
        }
    }
}
