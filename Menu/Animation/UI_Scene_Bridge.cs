using UnityEngine;

public class UI_Scene_Bridge : MonoBehaviour
{
	public AnimationSwipeHint SwipeHintControl;

	public void PassOnSwipeHint_ToAnimationSwipeHint(bool hintYes)
	{
		SwipeHintControl.ToggleHint(hintYes);
	}
}
/* Code within any unique level scene script to get this bridge


GameObject UI_Bridge_Object = GameObject.Find("UI_Bridge");
if (UI_Bridge_Object != null)
{
    UI_Scene_Bridge__SCRIPT = UI_Bridge_Object.GetComponent<UI_Scene_Bridge>();
    if (UI_Scene_Bridge__SCRIPT == null)
    {
        Debug.LogError("Bridge Failed");
    }
    else
    {
        //Debug.LogError("Bridge working");
    }
}
else
{
    Debug.LogError("UI_Bridge object not found.");
}


*/