using UnityEngine;

/// <summary>
/// Stage 2 placeholder — shows a visible message on device.
/// </summary>
public class Starter : MonoBehaviour
{
    void Start()
    {
        Debug.Log("Unity App Template loaded on Android.");
    }

    void OnGUI()
    {
        var style = new GUIStyle(GUI.skin.label)
        {
            fontSize = 48,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = Color.white },
        };

        GUI.Label(new Rect(0, Screen.height * 0.35f, Screen.width, 120f), "Unity App Template", style);

        var subStyle = new GUIStyle(GUI.skin.label)
        {
            fontSize = 28,
            alignment = TextAnchor.MiddleCenter,
            normal = { textColor = new Color(0.85f, 0.85f, 0.85f) },
        };

        GUI.Label(new Rect(0, Screen.height * 0.35f + 100f, Screen.width, 80f), "Running on your phone", subStyle);
    }
}
