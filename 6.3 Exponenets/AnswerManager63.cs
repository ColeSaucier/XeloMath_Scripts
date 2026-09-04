using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnswerManager63 : AnswerManagerBase
{
    public TextMeshProUGUI baseText;
    public TextMeshProUGUI exponentText;

    // References for grid generation (add these in Inspector or via code)
    [Header("Grid Generation")]
    public GameObject prefab;           // The object to instantiate (e.g. a unit cube/sphere)
    public Transform Holder;            // Parent transform for all generated objects
    public float xOffset100;
    public float yOffset100;
    private float spacing;        // Spacing between objects
    private float spacing_3d; //spacing for 3d
    public float default_spacing = 1.1f;
    public float max_grid_width = 4.4f - 0.7f;

    // Optional: custom method to apply transform/scale/position to parent group
    private void ApplyTransformToParent(GameObject parent, float scale, float xOffset, float yOffset)
    {
        parent.transform.localScale = Vector3.one * scale;

        // Store current position, then add the offsets (simple & safe)
        Vector3 currentPos = parent.transform.localPosition;
        currentPos.x += xOffset;
        currentPos.y += yOffset;
        // Z remains unchanged
        parent.transform.localPosition = currentPos;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Generate random base and exponent
        int baseInt = Random.Range(1,12);

        int exponent = (baseInt < 5) ? Random.Range(2, 4) : 2;

        // Calculate the answer (base^exponent)
        long answerInIntForm = 1;
        for (int i = 0; i < exponent; i++)
        {
            answerInIntForm *= baseInt;
        }

        answerString = answerInIntForm.ToString();

        // Update UI text
        baseText.text = baseInt.ToString();
        exponentText.text = exponent.ToString();

        // Generate the visual representation
        GenerateExponentVisualization(baseInt, exponent);
    }

    // Combined generation method: creates grid for square, optional 3D cube for exponent=3
    private void GenerateExponentVisualization(int baseInt, int exponent)
    {
        if (baseInt <= 0 || exponent < 2) return;

        spacing = default_spacing;
        spacing_3d = (float) ((0.67 * spacing) / baseInt);
        if ((default_spacing * baseInt) > max_grid_width)
        {
            spacing = max_grid_width / baseInt;
            spacing_3d = (float) ((0.8 * spacing) / baseInt);
        }

        GameObject rootParent = new GameObject("ExponentVisualization");
        rootParent.transform.SetParent(Holder);
        rootParent.transform.localPosition = Vector3.zero;

        // Always generate the base^2 grid (square layer)
        GameObject squareParent = GenerateGridLayer(baseInt, baseInt, spacing, 0); // Z = 0
        squareParent.transform.SetParent(rootParent.transform);
        squareParent.name = $"Square_{baseInt}x{baseInt}";

        float additional3dparent_XOffset = 0f;   // e.g. half the grid width
        float additional3dparent_YOffset = 0f;
        if (exponent == 3)
        {
            additional3dparent_XOffset = (float) 0.5 *spacing_3d * (baseInt-1);
            additional3dparent_YOffset = (float) 0.5 *spacing_3d * (baseInt-1);
            // Generate two additional layers for the cube (total baseInt layers in Z)
            for (int z = 1; z < baseInt; z++)
            {
                GameObject cubeLayer = GenerateGridLayer(baseInt, baseInt, spacing, 0);
                cubeLayer.transform.SetParent(rootParent.transform);
                cubeLayer.name = $"CubeLayer_Z{z}";
                // Apply 3d adjustment
                ApplyTransformToParent(cubeLayer, 1f, spacing_3d*z, spacing_3d*z);
            }
            
            // Optional: slightly elevate/offset the whole cube for visibility
            //rootParent.transform.localPosition = new Vector3(0, 2f, -5f); // adjust as needed
        }
        else
        {
            // Just the square — center it nicely
            //rootParent.transform.localPosition = new Vector3(0, 0, 0);
        }
        // Apply any global scaling/offset if desired
        ApplyTransformToParent(rootParent, 1f, xOffset100-additional3dparent_XOffset, yOffset100-additional3dparent_YOffset);
    }

    // Helper: generates a single 2D grid layer of size width x height at given Z depth
    private GameObject GenerateGridLayer(int width, int height, float spacing, int zDepth)
    {
        GameObject layerParent = new GameObject();
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject newObject = Instantiate(prefab);
                newObject.transform.SetParent(layerParent.transform);
                newObject.transform.localPosition = new Vector3(x * spacing, y * spacing, 0);

                // Optional: color variation per layer or position
                // SpriteRenderer sr = newObject.GetComponent<SpriteRenderer>();
                // if (sr) sr.color = new Color(1f, 1f - (zDepth / (baseInt * spacing)), 1f);
            }
        }

        // ── Center the entire layer after all objects are created ─────────────────
        if (layerParent.transform.childCount > 0)
        {
            // First generated object (bottom-left corner)
            Vector3 firstPos = layerParent.transform.GetChild(0).localPosition;
            
            // Last generated object (top-right corner)
            int lastIndex = layerParent.transform.childCount - 1;
            Vector3 lastPos = layerParent.transform.GetChild(lastIndex).localPosition;
            
            // Average (midpoint)
            Vector3 midpoint = (firstPos + lastPos) / 2f;
            
            // Apply negative offset to center the grid at (0,0,zDepth)
            ApplyTransformToParent(layerParent, 1f, -midpoint.x, -midpoint.y);
        }

        layerParent.name = $"Grid_{width}x{height}_Z{zDepth}";
        return layerParent;
    }
}