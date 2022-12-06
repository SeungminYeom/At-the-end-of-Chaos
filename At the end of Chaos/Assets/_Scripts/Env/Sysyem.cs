using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using UnityEngine.UIElements;

public class Sysyem : MonoBehaviour
{
    public int fontSize = 30;
    public Color color = new Color(.0f, .0f, .0f, 1.0f);
    public float width, height;

    float fps;
    float ms;
    string fpstext;
    string resolution;

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    private void OnGUI()
    {
        fps = 1.0f / Time.deltaTime;
        ms = Time.deltaTime * 1000.0f;
        fpstext = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);
        resolution = string.Format("{0:N0} x {1:N0}", Screen.width, Screen.height);
        Rect position = new Rect(width+100, height, Screen.width, Screen.height);
        Rect position1 = new Rect(width+100, height + 40f, Screen.width, Screen.height);
        GUIStyle style = new GUIStyle();
        style.fontSize = fontSize;
        style.normal.textColor = color;

        GUI.Label(position, fpstext, style);
        GUI.Label(position1, resolution, style);
    }

}
