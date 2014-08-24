using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

public class MatAndTexFunction
{

    public void getAllChild(Transform tran, ref string matName, ref Dictionary<string, List<Transform>> nameBox, ref List<Material> matList)
    {
        for (int i = 0; i < tran.renderer.sharedMaterials.Length; i++)
        {
            matName = tran.renderer.sharedMaterials[i].name;
            if (!nameBox.ContainsKey(matName))
            {
                matList.Add(tran.renderer.sharedMaterials[i]);
                nameBox.Add(matName, new List<Transform>());//如果此名字的材质未被使用，则创建一个以此名称命名的字典，并创建一个新的对象列表用于记录使用此材质的对象。
                nameBox[matName].Add(tran);
            }
            else
            {
                if (!nameBox[matName].Contains(tran))
                {
                    nameBox[matName].Add(tran);//如果此材质名已存在,并且不存在于列表内，则把此对象添加到字典内对应的List内
                }
            }
        }//不管有没子对象的时候
        for (int j = 0; j < tran.childCount; j++)
        {
            getAllChild(tran.GetChild(j), ref matName, ref nameBox, ref matList);
        }//有子对象的时候
    }//递归的第一次成功
    public void saveTex(Texture2D t, string savePath, string texName, int tf)
    {
        byte[] tex = null;
        switch (tf + "")
        {
            case "0":
                tex = t.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + "/Resources/" + savePath + "/" + texName + ".png", tex);
                break;
            case "1":
                tex = t.EncodeToJPG();
                File.WriteAllBytes(Application.dataPath + "/Resources/" + savePath + "/" + texName + ".jpg", tex);
                break;
        }
    }
    /* public Texture2D getTexture(Material m)
     {
         Texture2D tex = null;
         Texture2D GetTex = null;
         Color o = Color.white;
         if (m.GetTexture("_MainTex") != null)
         {
             tex = new Texture2D((int)m.GetTexture("_MainTex").width, (int)m.GetTexture("_MainTex").height, TextureFormat.ARGB32, false);
             GetTex = (Texture2D)m.GetTexture("_MainTex");
             for (int i = 0; i < (int)m.GetTexture("_MainTex").width; i++)
             {
                 for (int j = 0; j < (int)m.GetTexture("_MainTex").height; j++)
                 {
                     o = GetTex.GetPixel(i, j);
                     tex.SetPixel(i, j, o);
                 }
             }
             return tex;
         }//有MainTex的时候优先处理MainTex
         else if (m.GetTexture("_MainTex") == null && m.GetTexture("_BumpMap") != null)
         {
             tex = new Texture2D((int)m.GetTexture("_BumpMap").width, (int)m.GetTexture("_BumpMap").height, TextureFormat.ARGB32, false);
             GetTex = (Texture2D)m.GetTexture("_BumpMap");
             for (int i = 0; i < (int)m.GetTexture("_BumpMap").width; i++)
             {
                 for (int j = 0; j < (int)m.GetTexture("_BumpMap").height; j++)
                 {
                     o = m.color;
                     tex.SetPixel(i, j, o);
                 }
             }
             return tex;
         }//没MainTex的时候处理凹凸贴图
         else
         {
             tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
             tex.SetPixel(0, 0, m.color);
             return tex;
         }//MainTex和BumpTex都没的时候返回一个mainColor，1像素贴图
        
     }*/

    public void setScene()
    {
        GameObject cam = new GameObject();
        GameObject plane;
        List<GameObject> light = new List<GameObject>();

        plane = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/Plane"));
        plane.transform.position = new Vector3(0, 0, 0);
        plane.layer = 30;
        plane.name = "_renderPlane";

        cam.AddComponent<Camera>();
        cam.transform.eulerAngles = new Vector3(90, 0, 0);
        cam.transform.position = new Vector3(0, 30, 0);
        cam.name = "_renderCamera";
        cam.camera.orthographic = true;
        cam.camera.orthographicSize = 5.0f;
        cam.camera.cullingMask = 1 << 30;
        cam.camera.backgroundColor = Color.black;
        cam.camera.clearFlags = CameraClearFlags.SolidColor;

        for (int i = 0; i < 8; i++)
        {
            light.Add(new GameObject());
            light[i].AddComponent<Light>();
            light[i].transform.position = new Vector3(0, 0, 0);
            light[i].transform.eulerAngles = new Vector3(45, i * 45, 0);
            light[i].light.intensity = 0.0625f;
            light[i].light.type = LightType.Directional;
            light[i].light.cullingMask = 1 << 30;
            light[i].name = "_renderLight_" + i;
        }

    }//创建一个场景用于烘焙贴图，内含摄像机，片面和灯光
    public RenderTexture getRenderTexture(int width, int hight)
    {
        RenderTexture rt = new RenderTexture(width, hight, 24);
        GameObject cam = GameObject.Find("_renderCamera");
        cam.camera.targetTexture = rt;
        cam.camera.Render();
        return rt;

    }//通过摄像机取得renderTexture
    public Texture2D renTexToTex(RenderTexture m)
    {
        RenderTexture.active = m;
        Texture2D tex = new Texture2D(m.width, m.height, TextureFormat.ARGB32, false);
        tex.ReadPixels(new Rect(0, 0, m.width, m.height), 0, 0);
        RenderTexture.active = null;
        return tex;

    }//取得texture
    public void breakScene()
    {
        GameObject.DestroyImmediate(GameObject.Find("_renderCamera").camera.targetTexture);
        GameObject.DestroyImmediate(GameObject.Find("_renderPlane"));
        GameObject.DestroyImmediate(GameObject.Find("_renderCamera"));
        for (int i = 0; i < 8; i++)
        {
            GameObject.DestroyImmediate(GameObject.Find("_renderLight_" + i));
        }

    }
    public void setMat(Material m)
    {
        GameObject.Find("_renderPlane").transform.renderer.material = m;
    }

    public void SaveMatAssest(string matName, Texture2D tex, string savePath)
    {
        Material mat = new Material(Shader.Find("Diffuse"));
        mat.name = matName;
        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_Color", Color.white);
        //将材质放入创建的文件夹中  
        AssetDatabase.CreateAsset(mat, "Assets/Resources/" + savePath + "/" + matName + ".mat");
        AssetDatabase.Refresh();
    }//保存材质
    public Texture2D checkCameraCa(Camera cam)
    {
        Rect r = cam.camera.pixelRect;
        cam.Render();
        Texture2D t = new Texture2D(Mathf.FloorToInt(r.xMax), Mathf.FloorToInt(r.yMax));
        t.ReadPixels(r, 0, 0);
        return t;
    }
    public void pathSet()
    {
        /*string path = Application.dataPath;
        if (!Directory.Exists(path + "/Resources"))
        {
            Directory.CreateDirectory(path + "/Resources");
            Directory.CreateDirectory(path + "/Resources"+"/Texture");
            Directory.CreateDirectory(path + "/Resources" + "/Materials");
            
        }//如果不存在Resources文件夹则创建，同时创建Resources/Texture，Resources/Materials文件夹
        else 
        {
            if (!Directory.Exists(Application.dataPath + "/Resources/Texture"))
            {
                Directory.CreateDirectory(path + "/Resources" + "/Texture");
            }
            if (!Directory.Exists(Application.dataPath + "/Resources/Materials"))
            {
                Directory.CreateDirectory(path + "/Resources" + "/Materials");
            }
        }//如果有则检查是否有Texture和Materials文件夹，没则创建*/
        Directory.CreateDirectory(Application.dataPath + "/Resources" + "/Texture" + "/_tex");
        Directory.CreateDirectory(Application.dataPath + "/Resources" + "/Materials" + "/_mat");
        AssetDatabase.Refresh();
    }//判断是否存在文件夹存在，然后创建
}