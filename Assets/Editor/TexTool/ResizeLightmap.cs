using UnityEngine;
using System.Collections;
using UnityEditor;

public class ResizeLightmap : EditorWindow {
    int lmSizeNum = 1;
    string[] lmSize = new string[4] { "256", "512", "1024", "2048" };

    [MenuItem("TexTools/Lightmap Resize")]
    static void TexWindow()
    {
        Rect re = new Rect(0, 0, 200, 150);
        ResizeLightmap window = (ResizeLightmap)EditorWindow.GetWindowWithRect(typeof(ResizeLightmap), re, true, "Lightmap Resize");
        window.Show();
    }
    void OnGUI()
    {

        GUILayout.BeginHorizontal();


        if (GUI.Button(new Rect(3, position.height - 28, position.width - 6, 25), "Close"))
        {
            this.Close();
        }
        if (GUI.Button(new Rect(3, position.height - 53, position.width - 6, 25), "Bake"))
        {
            lightmapResize(lmSizeNum);

        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("设置烘焙贴图尺寸");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        lmSizeNum = GUILayout.Toolbar(lmSizeNum, lmSize);
        GUILayout.EndHorizontal();
    }
    public void lightmapResize(int lmSize)
    {
        int maxAtlas = 512;
        switch (lmSize+""){
            case "0":
                maxAtlas = 256;
                break;
            case "1":
                maxAtlas = 512;
                break;
            case "2":
                maxAtlas = 1024;
                break;
            case "3":
                maxAtlas = 2048;
                break;
        }
        LightmapEditorSettings.maxAtlasHeight = maxAtlas;
        LightmapEditorSettings.maxAtlasWidth = maxAtlas;
        Lightmapping.Clear();
        Lightmapping.Bake();
    }
}
