using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class BakeTex : EditorWindow {
    string selNumText = null;

    string[] texSizeSelText = new string[4] { "256", "512", "1024", "2048" };
    int texSizeSelNum = 2;//设置贴图尺寸

    string[] texFormat = new string[2] { "PNG", "JPG" };
    int texFormatNum = 0;

    string[] useMat = new string[2] { "Yes", "No" };
    int useMatNum = 0;

    string texPath = "Assets" + "/Resources" + "/Texture" + "/_tex";
    string matPath = "Assets" + "/Resources" + "/Materials" + "/_mat";


    //string assetPath = null;//临时记录材质路径用于判断是否已经被更新

    [MenuItem("TexTools/BakeTex")]
    static void TexWindow()
    {
        Rect re = new Rect(0, 0, 300, 350);//定义窗口大小 
        BakeTex window = (BakeTex)EditorWindow.GetWindowWithRect(typeof(BakeTex), re, true, "BakeTex");
        window.Show();
    }


    void OnGUI(){

        GUILayout.BeginHorizontal();
        if (GUI.Button(new Rect(3, position.height - 55, position.width - 6, 25), "Bake"))
        {
            if (texPath.Length != 0 && matPath.Length != 0)
            {
                if (texPath == "Assets" + "/Resources" + "/Texture" + "/_tex")
                {
                    Directory.CreateDirectory("Assets" + "/Resources" + "/Texture" + "/_tex");
                    AssetDatabase.Refresh();
                }//如果贴图地址为默认，则判断？创建默认保存文件夹
                if (matPath == "Assets" + "/Resources" + "/Materials" + "/_mat")
                {
                    Directory.CreateDirectory("Assets" + "/Resources" + "/Materials" + "/_mat");
                    AssetDatabase.Refresh();
                }//同贴图
                if (Directory.Exists(texPath) == true && Directory.Exists(matPath) == true)
                {
                    if (getAllChileInfo()[0] != 0)
                    {
                        create(texSizeSelNum, texFormatNum, useMatNum, texPath, matPath); //贴图文件夹和材质文件夹都有效的时候，执行烘焙
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("警告！", "请至少选择一个以上的对象\nヽ(≧Д≦)ノ", "好的，我去选");
                    }//如果没有选中对象
                }
                else//如果不是同时具有有效地址，则弹出警告，并将路径重置为有效路径
                {
                    EditorUtility.DisplayDialog("警告！", "必须设置一个保存位置，点击“确认”使用默认位置", "确认");
                    if (texPath.Length == 0 || !Directory.Exists(texPath))
                    {
                        texPath = "Assets" + "/Resources" + "/Texture" + "/_tex";
                    }
                    if (matPath.Length == 0 || !Directory.Exists(matPath))
                    {
                        matPath = "Assets" + "/Resources" + "/Materials" + "/_mat";
                    }

                }
                
            }
            else//如果不是同时具有地址，则弹出警告，并将路径重置为有效路径
            {
                EditorUtility.DisplayDialog("警告！", "必须设置一个保存位置，点击“确认”使用默认位置", "确认");
                if (texPath.Length == 0 || !Directory.Exists(texPath))
                {
                    texPath = "Assets" + "/Resources" + "/Texture" + "/_tex";
                }
                if (matPath.Length == 0 || !Directory.Exists(matPath))
                {
                    matPath = "Assets" + "/Resources" + "/Materials" + "/_mat";
                }
                
            }
        }

        if (GUI.Button(new Rect(3,position.height-28,position.width-6,25),"Close"))
        {
            this.Close();
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
        GUILayout.Label("设置保存路径:");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        //GUILayout.Label("说明：\n材质默认保存于\n Resources\\Materials\\_mat \n贴图保存于\n Resources\\Texture\\_tex");
        if (GUILayout.Button("Texture Save"))
        {
            string texPathTemp = EditorUtility.SaveFolderPanel("Selection The Floder to Save Texture", "Assets", "");
            texPath = texPathTemp.Substring(Application.dataPath.Length - 6, texPathTemp.Length-(Application.dataPath.Length-6));
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("贴图位置：\n" + texPath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Materials Save"))
        {
            string matPathTemp = EditorUtility.SaveFolderPanel("Selection The Floder to Save Material", "Assets", "");
            matPath = matPathTemp.Substring(Application.dataPath.Length - 6, matPathTemp.Length - (Application.dataPath.Length - 6));
            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("材质位置：\n" + matPath);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("使用默认地址"))
        {
            texPath = Application.dataPath.Substring(Application.dataPath.Length - 6, 6) + "/Resources" + "/Texture" + "/_tex";
            matPath = Application.dataPath.Substring(Application.dataPath.Length - 6, 6) + "/Resources" + "/Materials" + "/_mat";
        }
        GUILayout.EndHorizontal();


        Repaint();  //重绘窗口
    }


    public int[] getAllChileInfo()
    {
        MatAndTexFunction gac = new MatAndTexFunction();
        //string matName = null;
        //Dictionary<string, List<Transform>> nameBox = new Dictionary<string, List<Transform>>();
        List<Material> matList = new List<Material>();
        List<Transform> objMatRefList = new List<Transform>();

        GameObject[] obj = Selection.gameObjects;

        //int num = 0;
        int[] res = new int[2] {0 ,0};


        for (int i = 0; i < obj.Length; i++)
        {

            gac.getAllChild(obj[i].transform, ref matList, ref objMatRefList);
               
        }
        res[0] = objMatRefList.Count;
        res[1] = matList.Count;
        return res;
    }//取得对象总数和材质总数

    public void create(int texSiez,int texFormat,int useMat,string texPath,string matPath)
    {
        GameObject[] obj = Selection.gameObjects;
        //将选择的对象读入

        List<Material> matList = new List<Material>();
        //创建材质名列表
        //string matName = null;
        //创建临时材质名变量
        //Dictionary<string, List<Transform>> nameBox = new Dictionary<string, List<Transform>>();//创建一个字典用以关联材质名与使用此材质的对象与列表
        //创建材质名与对象关联字典
        Texture2D t = null;
        //临时贴图

        Material[] matTmp = null;//创建用于最后更新对象材质的时候的材质数组
        List<Transform> objMatRefList = new List<Transform>(); // 用于记录有render的对象

        List<GameObject> lightList = new List<GameObject>();//用于取得全部灯光
        GameObject[] lightTmp = (GameObject[])GameObject.FindObjectsOfType<GameObject>();

        string[] texNames = new string[5] { "_MainTex", "_BumpMap", "_Detail", "_ParallaxMap", "_Parallax" };


        MatAndTexFunction cre = new MatAndTexFunction();
        //调用

        foreach (GameObject o in lightTmp)
        {
            if (o.GetComponent<Light>()){
                lightList.Add(o);
            }
        }//获取全部灯光
        for (int l = 0; l < lightList.Count;l++)
        {
            lightList[l].light.enabled = false;
        }//关闭场景全部灯光


        for (int i = 0; i < obj.Length; i++)
        {
            cre.getAllChild(obj[i].transform, ref matList, ref objMatRefList);
                //cre.getAllChildMix(obj[i].transform,ref objMatRefList);
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
            cre.saveTex(t, texPath, matList[j].name,texFormat); 
            AssetDatabase.Refresh();
            switch (texFormatNum + "")
            {
                case "0":
                    t = (Texture2D)Resources.LoadAssetAtPath(texPath + "/" + matList[j].name + ".png", typeof(Texture2D));
                    break;
                case "1":
                    t = (Texture2D)Resources.LoadAssetAtPath(texPath + "/" + matList[j].name + ".jpg", typeof(Texture2D));
                    break;
            }
            
            cre.SaveMatAssest(matList[j].name,t, matPath);
        }
        AssetDatabase.Refresh();

        switch (useMat + "")
        {
            case "0":

                for (int k = 0; k < objMatRefList.Count; k++)
                {
                        matTmp = new Material[objMatRefList[k].renderer.sharedMaterials.Length];

                        for (int l = 0; l < objMatRefList[k].renderer.sharedMaterials.Length; l++)
                        {
                            //obj[k].renderer.sharedMaterials[l] = (Material)Resources.LoadAssetAtPath(matPath + "/" + obj[k].renderer.sharedMaterials[l].name + ".mat", typeof(Material));

                            matTmp[l] = (Material)Resources.LoadAssetAtPath(matPath + "/" + objMatRefList[k].renderer.sharedMaterials[l].name + ".mat", typeof(Material));

                        }
                        objMatRefList[k].renderer.sharedMaterials = matTmp;
                        matTmp = null;
                }
                    break;
        }
            cre.breakScene();
            for (int l = 0; l < lightList.Count; l++)
            {
                lightList[l].light.enabled = true;
            }//打开场景全部灯光
    }
    
}