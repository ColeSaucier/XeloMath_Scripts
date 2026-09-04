using System;
using System.Reflection; // Add this using directive to access PropertyInfo
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class clickOnAnswerBox05 : MonoBehaviour
{
    public TextMeshProUGUI inputText;
    public CanvasGroup popUpCanvasGroup; // Reference to the pop-up canvas's CanvasGroup component
    Renderer objectRenderer; // Reference to the object's Renderer component

    private bool isInputActive = false;
    private string userInput = "";

    void Start()
    {
        objectRenderer = GetComponent<Renderer>(); // Get the Renderer component
        popUpCanvasGroup.alpha = 0f; // Set the pop-up canvas's alpha to 0 (fully transparent) initially
        popUpCanvasGroup.interactable = false; // Disable interactions with the pop-up canvas
    }

    void OnMouseDown()
    {
        isInputActive = true;
        userInput = "";

        // Show the pop-up canvas by setting its alpha to 1 (fully opaque)
        popUpCanvasGroup.alpha = 1f;
        popUpCanvasGroup.interactable = true; // Enable interactions with the pop-up canvas
    }

    void Update()
    {
        if (isInputActive)
        {
            // Check for input and handle it
            if (Input.GetKeyDown(KeyCode.Return))
            {
                int numberInput;
                if (int.TryParse(userInput, out numberInput))
                {
                    if (numberInput == answerMatcher05.correctAnswer)
                    {
                        objectRenderer.material.color = Color.green;
                    }
                    else
                    {
                        objectRenderer.material.color = new Color32(207, 52, 35, 50);
                    }
                }

                // Hide the pop-up canvas by setting its alpha to 0 (fully transparent)
                popUpCanvasGroup.alpha = 0f;
                popUpCanvasGroup.interactable = false; // Disable interactions with the pop-up canvas
                isInputActive = false;
            }
            else if (Input.GetKeyDown(KeyCode.Backspace) && userInput.Length > 0)
            {
                userInput = userInput.Substring(0, userInput.Length - 1);
            }
            else
            {
                userInput += Input.inputString;
            }

            inputText.text = userInput;
        }
    }
}