using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Collections.Generic;

public class BakeTex : EditorWindow {
    string selNumText = null;

    string[] texSizeSelText = new string[4] { "256", "512", "1024", "2048" };
    int texSizeSelNum = 2;//设置贴图尺寸

    string[] texFormat = new string[2] { "PNG", "JPG" };
    int texFormatNum = 0;

    string[] useMat = new string[2] { "Yes", "No" };
    int useMatNum = 0;



    [MenuItem("TexTools/BakeTex")]
    static void TexWindow()
    {
        Rect re = new Rect(0, 0, 300, 260);
        BakeTex window = (BakeTex)EditorWindow.GetWindowWithRect(typeof(BakeTex), re, true, "BakeTex");
        window.Show();
    }
    void OnGUI(){

        GUILayout.BeginHorizontal();

        
        if (GUI.Button(new Rect(3,position.height-28,position.width-6,25),"Close"))
        {
            this.Close();
        }
        if (GUI.Button(new Rect(3, position.height - 53, position.width - 6, 25), "Bake"))
        {
            create(texSizeSelNum,texFormatNum,useMatNum);

        }
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        selNumText = "已选中对象数量（含子物体）：" + getAllChileInfo()[0] +"\n" + "对象使用材质总数：" + getAllChileInfo()[1]+"\n";
        GUILayout.Label(selNumText);
        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.Label("贴图大小：");
        texSizeSelNum = GUILayout.Toolbar(texSizeSelNum,texSizeSelText);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("贴图类型：");
        texFormatNum = GUILayout.Toolbar(texFormatNum, texFormat);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("是否用新材质替换旧材质？");
        useMatNum = GUILayout.Toolbar(useMatNum, useMat);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("说明：\n材质默认保存于\n Resources\\Materials\\_mat \n贴图保存于\n Resources\\Texture\\_tex");
        GUILayout.EndHorizontal();
        Repaint();  //重绘窗口
    }

    public int[] getAllChileInfo()
    {
        MatAndTexFunction gac = new MatAndTexFunction();
        string matName = null;
        Dictionary<string, List<Transform>> nameBox = new Dictionary<string, List<Transform>>();
        List<Material> matList = new List<Material>();
        GameObject[] tran = Selection.gameObjects;
        int num = 0;
        int[] res = new int[2] {0 ,0};


        for (int i = 0; i < tran.Length; i++)
        {
            gac.getAllChild(tran[i].transform, ref matName, ref nameBox, ref matList);
        }
        for (int j = 0; j < matList.Count; j++)
        {
            num += nameBox[matList[j].name].Count;
        }
        res[0] = num;
        res[1] = matList.Count;
        return res;
    }//取得对象总数和材质总数

    public void create(int texSiez,int texFormat,int useMat)
    {
        GameObject[] obj = Selection.gameObjects;
        //将选择的对象读入
        List<Material> matList = new List<Material>();
        //创建材质名列表
        string matName = null;
        //创建临时材质名变量
        Dictionary<string, List<Transform>> nameBox = new Dictionary<string, List<Transform>>();//创建一个字典用以关联材质名与使用此材质的对象与列表
        //创建材质名与对象关联字典
        Texture2D t = null;
        //临时贴图
        MatAndTexFunction cre = new MatAndTexFunction();
        //调用
        string[] texNames = new string[5]{"_MainTex", "_BumpMap", "_Detail", "_ParallaxMap", "_Parallax"};

        cre.pathSet();
        //检查和创建路径与保存文件夹
        for (int i = 0; i < obj.Length; i++)
        {
            cre.getAllChild(obj[i].transform, ref matName, ref nameBox, ref matList);
            //创建整个场景对象与材质关系列表
        }

        /*foreach (Material m in matList)
        {
            Debug.Log(m.name+":"+nameBox[m.name].Count);
        }//输出每个材质被使用的数量*/

        cre.setScene();
        for (int j = 0; j < matList.Count; j++)
        {
            cre.setMat(matList[j]);
            //将材质赋予烘焙用Plane;
            foreach (string s in texNames)
            {
                if (matList[j].HasProperty(s))
                {
                    if (matList[j].GetTexture(s) != null){
                        RenderTexture rt = new RenderTexture(1024,1024,1);
                        switch (texSiez + "")
                        {
                            case "0":
                                rt = cre.getRenderTexture(256, 256);
                                break;
                            case "1":
                                rt = cre.getRenderTexture(512, 512);
                                break;
                            case "2":
                                rt = cre.getRenderTexture(1024, 1024);
                                break;
                            case "3":
                                rt = cre.getRenderTexture(2048 , 2048);
                                break;
                        }  
                        t = cre.renTexToTex(rt);
                        break;
                    }
                }//如果有texNames列表中的贴图时，定义贴图
                else
                {
                    t = new Texture2D(1, 1);
                    t.SetPixel(0, 0, matList[j].color);
                }//如果都没，返回像素1，color颜色的贴图
            }
            cre.saveTex(t, "Texture/_tex", matList[j].name,texFormat); 
            AssetDatabase.Refresh();
            t = (Texture2D)Resources.Load ("Texture/_tex/" + matList[j].name,typeof(Texture2D)) ;
            cre.SaveMatAssest(matList[j].name,t, "Materials/_mat");
        }
        AssetDatabase.Refresh();

        switch (useMat + "")
        {
            case "0":
                for (int k = 0; k < matList.Count; k++)
                {
                    for (int l = 0; l < nameBox[matList[k].name].Count; l++)
                    {
                        nameBox[matList[k].name][l].renderer.material = (Material)Resources.Load("Materials/_mat/" + matList[k].name, typeof(Material));//用Resources.Load载入时，路径的最开头不可有/
                    }
                }//将场景上的对象材质重定义为新保存的材质
                break;
        }
        
           
            cre.breakScene();

    }
}
public class MatAndTexFunction 
{

    public void getAllChild(Transform tran, ref string matName, ref Dictionary<string, List<Transform>> nameBox, ref List<Material> matList)
   {
       for (int i = 0; i < tran.renderer.sharedMaterials.Length; i++)
       {
           matName = tran.renderer.sharedMaterials[i].name;
           if(!nameBox.ContainsKey(matName))
           {
               matList.Add(tran.renderer.sharedMaterials[i]);
               nameBox.Add(matName,new List<Transform>());//如果此名字的材质未被使用，则创建一个以此名称命名的字典，并创建一个新的对象列表用于记录使用此材质的对象。
               nameBox[matName].Add(tran);
           }else{
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
    public void saveTex(Texture2D t,string savePath,string texName,int tf)
    {
        byte[] tex = null;
        switch (tf + "")
        {
            case "0":
                tex = t.EncodeToPNG();
                File.WriteAllBytes(Application.dataPath + "/Resources/" + savePath + "/" + texName + ".png", tex);
                break;
            case"1":
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
        GameObject cam = new GameObject ();
        GameObject plane;
        List<GameObject> light = new List<GameObject>();

        plane = (GameObject)GameObject.Instantiate(Resources.Load("Prefab/Plane"));
        plane.transform.position = new Vector3 (0,0,0);
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
        RenderTexture rt = new RenderTexture(width,hight,24);
        GameObject cam = GameObject.Find("_renderCamera");
        cam.camera.targetTexture = rt;
        cam.camera.Render();
        return rt;

    }//通过摄像机取得renderTexture
    public Texture2D renTexToTex(RenderTexture m)
    {
        RenderTexture.active = m;
        Texture2D tex = new Texture2D(m.width, m.height, TextureFormat.ARGB32, false);
        tex.ReadPixels (new Rect (0, 0, m.width, m.height), 0, 0);
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
            GameObject.DestroyImmediate(GameObject.Find("_renderLight_"+i));
        }
            
    }
    public void setMat(Material m)
    {
        GameObject.Find("_renderPlane").transform.renderer.material = m;
    }

    public void SaveMatAssest(string matName,Texture2D tex,string savePath)
    {
        Material mat = new Material(Shader.Find("Diffuse"));
        mat.name = matName;
        mat.SetTexture("_MainTex", tex);
        mat.SetColor("_Color",Color.white);
        //将材质放入创建的文件夹中  
        AssetDatabase.CreateAsset(mat, "Assets/Resources/" + savePath + "/" + matName + ".mat"); 
        AssetDatabase.Refresh();
    }//保存材质
    public Texture2D checkCameraCa(Camera cam)
    {
        Rect r = cam.camera.pixelRect;
        cam.Render();
        Texture2D t = new Texture2D(Mathf.FloorToInt(r.xMax),Mathf.FloorToInt( r.yMax));
        t.ReadPixels(r,0,0);
        return t;
    }
    public void pathSet()
    {
        string path = Application.dataPath;
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
        }//如果有则检查是否有Texture和Materials文件夹，没则创建
        Directory.CreateDirectory(path + "/Resources" + "/Texture"+"/_tex");
        Directory.CreateDirectory(path + "/Resources" + "/Materials"+"/_mat");
        AssetDatabase.Refresh();
    }//判断是否存在文件夹存在，然后创建
    
}
