using System;
using UnityEngine;
using UnityEngine.UI;

public class VRDebug
{
    public static bool IsDebug = true;
    static Canvas s_Canvas;
    static Canvas _Canvas
    {
        get
        {
            if (s_Canvas == null)
            {
                if (Camera.main.transform.Find("VRCanvas") != null)
                    s_Canvas = Camera.main.transform.Find("VRCanvas").GetComponent<Canvas>();
                if (s_Canvas == null)
                {
                    var canvasObj = new GameObject("VRCanvas");
                    canvasObj.transform.SetParent(Camera.main.transform, false);
                    s_Canvas = canvasObj.AddComponent<Canvas>();
                    s_Canvas.GetComponent<RectTransform>().localPosition = new Vector3(0.03f, -0.05f, 0.3f);
                    s_Canvas.transform.localRotation = Quaternion.identity;
                    s_Canvas.renderMode = RenderMode.WorldSpace;
                    s_Canvas.sortingOrder = 100;
                }
            }
            return s_Canvas;
        }
    }
    static Text s_Text;
    static Text _Text
    {
        get
        {
            if (s_Text == null)
            {
                if (_Canvas.transform.Find("DebugText") != null)
                    s_Text = _Canvas.transform.Find("DebugText").GetComponent<Text>();
                if (s_Text == null)
                {
                    var debugTextObj = new GameObject("DebugText");
                    debugTextObj.transform.SetParent(_Canvas.transform, false);
                    s_Text = debugTextObj.AddComponent<Text>();
                    s_Text.GetComponent<RectTransform>().sizeDelta = new Vector2(800, 600);
                    s_Text.transform.localRotation = Quaternion.identity;
                    s_Text.transform.localScale = Vector3.one * 0.0005f;
                    s_Text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
                    s_Text.fontSize = 15;
                    s_Text.alignment = TextAnchor.UpperLeft;
                    s_Text.color = Color.yellow;
                }
            }
            return s_Text;
        }
    }
    const int LOG_SUM = 25;
    static string[] s_Logs = new string[LOG_SUM];
    static int s_Index = 0;
    public static void Log(string text)
    {
        if (IsDebug)
        {
            s_Logs[s_Index] = $"{DateTime.Now}: {text}";
            var str = "";
            for (int i = 1; i <= LOG_SUM; i++)
            {
                if (!string.IsNullOrEmpty(s_Logs[(s_Index + i) % LOG_SUM]))
                {
                    str += $"{s_Logs[(s_Index + i) % LOG_SUM]} \n";
                }
            }
            s_Index = ++s_Index % LOG_SUM;
            _Text.text = str;
        }
    }
}