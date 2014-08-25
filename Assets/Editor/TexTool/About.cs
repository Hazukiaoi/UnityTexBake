using UnityEngine;
using System.Collections;
using UnityEditor;

public class About : EditorWindow {


    [MenuItem("TexTools/About")]
    static void AboutThis()
    {
        About window = (About)GetWindowWithRect(typeof(About), new Rect(0, 0, 200, 100), true, "TexTool 1.2");
        window.ShowPopup();
    }
    public void OnGUI()
    {
        if (GUI.Button(new Rect(60, 58, position.width-120, 20), "锟斤拷"))
        {
            this.Close();
        }

        GUILayout.BeginHorizontal();
        GUILayout.Label("屯屯屯屯屯屯屯屯屯屯屯屯屯屯屯屯屯\n屯                                         屯\n屯                                         屯\n屯            by Hazukiaoi           屯\n屯                                         屯\n屯                                         屯\n屯屯屯屯屯屯屯屯屯屯屯屯屯屯屯屯屯");
        GUILayout.EndHorizontal();

    }
}
