using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlaceCircleGenerator : MonoBehaviour
{
    public GameObject prefab;
    public float spacing = 1f;
    public float spacing1s = 0.75f;
    public float xOffset100;
    public float yOffset100;
    public float xOffset10;
    public float yOffset10;
    public float xOffset1;
    public float yOffset1;
    public float zRotationOffset = 0f;
    public GameObject Holder;


    public float counter100 = 0;
    public float counter10 = 0;
    public float counter1 = 0;

    public static int hundred = 5;
    public static int ten = 6;
    public static int ones = 7;

    public TextMeshProUGUI textMesh1;
    public Button Button;

    public SceneCompleteMenu sceneCompleteScript;


    public void Start()
    {
        hundred = Random.Range(0, 6);
        ten = Random.Range(0, 10);
        ones = Random.Range(0, 10);
        textMesh1.text = $"{hundred}{ten}{ones} =";
    }
    void ApplyTransformToParent(GameObject parent, float counter, float xOffset, float yOffset)
    {
        parent.transform.position += new Vector3(xOffset + counter/6, yOffset - counter/6, 0);
        parent.transform.Rotate(0, 0, zRotationOffset);
    }
    public void Generate100s()
    {
        GameObject newParent = new GameObject();
        for (int i = 0; i < 10; i++)
        {
            //if spriteRenderer.color = newColor;
            for (int j = 0; j < 10; j++)
            {
                GameObject newObject = Instantiate(prefab);
                newObject.transform.SetParent(newParent.transform);
                newObject.transform.localPosition = new Vector3(i * spacing, j * spacing, 0);

            //Sprite rederer color change could go up aroung here
            }
        }

        ApplyTransformToParent(newParent, counter100*2, xOffset100, yOffset100);
        newParent.transform.SetParent(Holder.transform);
        newParent.name = $"100Object{counter100}";
        counter100++;
    }

    public void Generate10s()
    {
        GameObject newParent10 = new GameObject();
        for (int i = 0; i < 10; i++)
        {
            GameObject newObject = Instantiate(prefab);
            newObject.transform.SetParent(newParent10.transform);
            newObject.transform.localPosition = new Vector3(0, i * spacing, 0);
        }

        ApplyTransformToParent(newParent10, counter10, xOffset10, yOffset10);
        newParent10.transform.SetParent(Holder.transform);
        newParent10.name = $"10Object{counter10}";
        counter10++;
    }

    public void Generate1s()
    {   
        GameObject newParent = new GameObject();
        GameObject newObject = Instantiate(prefab);
        newObject.transform.SetParent(newParent.transform);
        newObject.transform.localPosition = new Vector3(0, 0, 0);

        ApplyTransformToParent(newParent, 0, xOffset1, yOffset1 - counter1 / 4);
        newParent.transform.SetParent(Holder.transform);
        newParent.name = $"oneObject{counter1}";
        counter1++;
        
        /*GameObject newObject = Instantiate(prefab);
        newObject.transform.SetParent(Holder.transform);
        newObject.transform.localPosition = new Vector3(xOffset1 + counter1 * spacing, yOffset1 - counter1 * spacing, 0);
        newObject.name = $"oneObject{counter1}";
        counter1++; */
    }
    public void CheckAnswer()
    {   
        if (counter100 == hundred && counter10 == ten && counter1 == ones)
        {
            sceneCompleteScript.SceneComplete = true;
            Button.image.color = Color.green;
        }
        else
        {
            foreach (Transform child in Holder.transform)
            {
                Destroy(child.gameObject);
            }
            counter100 = 0;
            counter10 = 0;
            counter1 = 0;
            Handheld.Vibrate();
            Color32 shiftColor = new Color32(210, 0, 0, 50);
            StartCoroutine(ShowColoredImage(shiftColor, 0.2f));
        }
    }
    // Method to start the coroutine that creates a colored image
    public virtual void DisplayColoredImage(Color32 color, float duration)
    {
        StartCoroutine(ShowColoredImage(color, duration));
    }

    public Sprite background;
    IEnumerator ShowColoredImage(Color32 color, float duration)
    {
        // Create Canvas
        GameObject canvasObject = new GameObject("TemporaryCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 15;

        // Add a Panel (which is just an Image component with a default rect)
        GameObject panel = new GameObject("ColoredPanel");
        panel.transform.SetParent(canvas.transform, false);
        Image panelImage = panel.AddComponent<Image>();
        
        // Load the "Background" sprite
        Sprite backgroundSprite = background; // Assumes the sprite is in a Resources folder

        // Set the sprite to the panel's Image component
        panelImage.sprite = backgroundSprite;
        panelImage.color = color;

        // Position and size the panel (you might adjust these values)
        RectTransform panelRectTransform = panel.GetComponent<RectTransform>();
        panelRectTransform.sizeDelta = new Vector2(Screen.width, Screen.height); // Full screen size, adjust as needed

        // Wait for the duration
        yield return new WaitForSeconds(duration);

        // Clean up
        Destroy(canvasObject);
    }
}