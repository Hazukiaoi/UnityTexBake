using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class CheckFlo : EditorWindow {
    [MenuItem("TexTools/CheckDir")]
    static void window()
    {
        CheckFlo window = (CheckFlo)GetWindowWithRect(typeof(CheckFlo), new Rect(0, 0, 400, 100), true, "getDir");
        window.Show();
    }
    public void OnGUI()
    {
        GUILayout.BeginHorizontal();
        if (GUI.Button(new Rect(0,0,position.width,position.height), "不要按"))
        {
            getPath(GameObject.Find("4"));
        }
        /*GUILayout.Box((Texture2D)Resources.LoadAssetAtPath("Assets/No_Name.png", typeof(Texture2D)));
        string str = Application.dataPath + "/Resources";
        string stri = "Assets" + str.Substring(Application.dataPath.Length - 1, str.Length - Application.dataPath.Length);
        GUILayout.Label(stri);*/
        GUILayout.EndHorizontal();
    }
    public void getDir()
    {
        string path = EditorUtility.SaveFolderPanel("Save Tex","","");
        Debug.Log(path);
    }
    public void showWindow()
    {
        EditorUtility.DisplayDialog("警告", "你必须设置一个保存位置", "确定");
    }
    public void buildPlane()
    {   
        Vector2[] uvs = new Vector2[4];


        GameObject go = new GameObject();
        go.name = "face";
        go.AddComponent("MeshRenderer");
        go.AddComponent("MeshFilter");
        MeshFilter meshf = (MeshFilter)GameObject.Find("face").GetComponent(typeof(MeshFilter));
        Mesh mesh = go.GetComponent<MeshFilter>().sharedMesh;
        mesh = meshf.sharedMesh;
        //go.GetComponent<MeshFilter>().mesh = (Mesh)GameObject.Find("face").GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = new Vector3[4];
        //三角形顶点坐标数组
        int[] triangles = new int[6];
        //三角形顶点ID数组


        vertices[0] = new Vector3(-5, 0, -5);
        vertices[1] = new Vector3(-5, 0, 5);
        vertices[2] = new Vector3(5, 0, 5);
        vertices[3] = new Vector3(5, 0, -5);
        //创建顶点数据

        triangles[0] = 0;
        triangles[1] = 1;
        triangles[2] = 2;
        triangles[3] = 2;
        triangles[4] = 3;
        triangles[5] = 0;

        //go.GetComponent<MeshFilter>().sharedMesh.vertices = vertices;
        //go.GetComponent<MeshFilter>().sharedMesh.triangles = triangles;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        mesh.normals[0] = new Vector3(0, 1, 0);
        mesh.normals[1] = new Vector3(0, 1, 0);
        mesh.normals[2] = new Vector3(0, 1, 0);
        mesh.normals[3] = new Vector3(0, 1, 0);

        Debug.Log(mesh.tangents.Length);

        /*mesh.tangents[0] = new Vector4(-1, 0, 0, -1);
        mesh.tangents[1] = new Vector4(-1, 0, 0, -1);
        mesh.tangents[2] = new Vector4(-1, 0, 0, -1);
        mesh.tangents[3] = new Vector4(-1, 0, 0, -1);*/

        //Debug.Log(GameObject.Find("4").GetComponent<MeshFilter>().sharedMesh.tangents[0]);
        
        uvs[0] = new Vector2(0,0);
        uvs[1] = new Vector2(0,1);
        uvs[2] = new Vector2(1,1);
        uvs[3] = new Vector2(1,0);
        mesh.uv = uvs ; 
        

        
    }
    public void creatPlane()
    {
        GameObject go = new GameObject();
        go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.transform.position = new Vector3(0, 0, 0);
        Debug.Log(go.GetComponent<MeshFilter>().sharedMesh.uv.Length);
    }

    public void getPath(GameObject go)
    {
        Debug.Log(AssetDatabase.GetAssetPath(go.transform.renderer.sharedMaterials[0]));
        Debug.Log(go.transform.renderer.sharedMaterials[0].name);
    }
}