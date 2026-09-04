using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationSwipeHint : MonoBehaviour
{
    public Animator animator; //SwipeHint Animator
    private float idleTimer = 0f;
    private float idleThreshold = 8f; // 8 seconds
    private Vector3 lastMousePosition;
    public bool hintEnabled = true;
    private bool validSceneForSwipe = true;
    private float baselineTimeWait = 0f;
    private bool let_idle_check_begin = false;

    void Start()
    {
        // Get the Animator component attached to this GameObject
        animator = GetComponent<Animator>();
        
        // Store initial mouse position
        lastMousePosition = Input.mousePosition;
        
        // Warn if no animator is found
        if (animator == null)
        {
            //Debug.LogWarning("No Animator component found on " + gameObject.name);
        }
    }

    void Update()
    {
        if (hintEnabled)
        {
            //Check if scene valid 
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene == "Menu" || currentScene == "Speed")
                validSceneForSwipe = false;
            else
               validSceneForSwipe = true; 

            if (validSceneForSwipe)
            {
                if (baselineTimeWait > 12)
                {
                    let_idle_check_begin = true;
                }
                if (let_idle_check_begin)
                {
                    // Check for any input
                    if (IsAnyInputDetected())
                    {
                        // Reset timer if there's input
                        idleTimer = 0f;
                    }
                    else
                    {
                        // Increment timer when no input
                        idleTimer += Time.deltaTime;

                        // If we've exceeded the threshold, trigger the animation
                        if (idleTimer >= idleThreshold)
                        {
                            TriggerIdleAnimation();
                        }
                    }

                    // Update last mouse position
                    lastMousePosition = Input.mousePosition;
                }
                else
                    baselineTimeWait += Time.deltaTime;
            }
        }
    }

    bool IsAnyInputDetected()
    {
        // Check keyboard input
        if (Input.anyKeyDown)
            return true;

        // Check mouse movement
        if (Input.touchCount == 1)
            return true;

        // Check mouse clicks
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2))
            return true;

        return false;
    }

    void TriggerIdleAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("SwipeHint");
            // Reset timer after triggering to prevent continuous triggering
            idleTimer = 0f;
        }
    }
    public void ToggleHint(bool hintYes)
    {
        hintEnabled = hintYes;
    }
}