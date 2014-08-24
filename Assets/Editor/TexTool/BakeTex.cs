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
