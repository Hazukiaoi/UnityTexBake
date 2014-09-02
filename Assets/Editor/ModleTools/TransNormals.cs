using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using System.Text;
using System.Threading;
using System;

public class TransNormals : EditorWindow{
    public Mesh meshTarget;
    public Mesh meshRoot;

    public int persent;
    public int nowVert;
    public int allVert;

    int[] normalListNum;

    Vector3[] meshRootVert;
    Vector3[] meshTargetVert;
    //记录顶点坐标

    int tarLength;
    int rootLength;

    [MenuItem("ModelTools/顶点不同法线转移")]
    static void NormalTrans()
    {
        TransNormals windows = (TransNormals)GetWindowWithRect(typeof(TransNormals), new Rect(0,0,200,120),true, "NormalTrans");
        windows.Show();
    }
    public void OnGUI()

    {

        GUILayout.BeginHorizontal();
        GUILayout.Label("基础法线模型");
        meshRoot = (Mesh)EditorGUILayout.ObjectField(meshRoot, typeof(Mesh), false);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("法线目标模型");
        meshTarget = (Mesh)EditorGUILayout.ObjectField(meshTarget, typeof(Mesh), false);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label("进度:");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.Label(persenGet(persent) + "|" + persent.ToString() + "%");
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if (GUI.Button(new Rect(10, 80, position.width - 20, 25), "不要按"))
        {

            rootLength = meshRoot.vertices.Length;
            tarLength = meshTarget.vertices.Length;
            //记录目标顶点总数


            meshRootVert = meshRoot.vertices;
            meshTargetVert = meshTarget.vertices;
            //记录顶点坐标

            normalListNum = new int [meshRoot.vertices.Length];
            //记录被转移法线顶点编号

            nowVert = 0;//初始化现在顶点数
            allVert = rootLength;//初始化总顶点数


            WaitHandle[] waitHandles = new WaitHandle[] 
            {
                new AutoResetEvent(false) 
            };

            ThreadPool.QueueUserWorkItem(new WaitCallback(transNormalsThread), waitHandles[0]);
            WaitHandle.WaitAll(waitHandles);

            setNormal(meshRoot, meshTarget, normalListNum);

            AssetDatabase.Refresh();
        }
        GUILayout.EndHorizontal();

        
    }
    public void transNormalsThread(System.Object state)
    {
        AutoResetEvent are = (AutoResetEvent)state;

        float nowDis = 0.0f;
        float prvDis = 0.0f;

        int minVNum = 0;


        //int all = rootM.normals.Length;

        for (int i = 0; i < rootLength; i++)
        {
            for (int j = 0; j < tarLength; j++)
            {
                nowDis = getDistance(meshRootVert[i], meshTargetVert[j]);
                
                if (j < 1)
                {
                    prvDis = nowDis;
                }
                else
                {
                    if (nowDis < prvDis)
                    {//如果现在的坐标距离小于一次距离
                        minVNum = j;//更新最小值下标
                        prvDis = nowDis;//更新上一次距离值
                    }
                }
            }//循环传入的坐标并计算
            normalListNum[i] = minVNum;//将最小值下标记录
            nowVert++;
            persent = Mathf.RoundToInt((nowVert / allVert)*100);
        }
        are.Set();
    }
    public void setNormal(Mesh rootM,Mesh targetM,int[] normalListNum)
    {
        Vector3[] normalL = new Vector3[normalListNum.Length];
        for (int i = 0; i < normalListNum.Length; i++)
        {
            normalL[i] = targetM.normals[normalListNum[i]];
        }
        rootM.normals = normalL;
    }//保存模型法线

    public float getDistance(Vector3 rootV, Vector3 targetV)
    {
        float f = Mathf.Sqrt((rootV.x - targetV.x) * (rootV.x - targetV.x)) + ((rootV.y - targetV.y) * (rootV.y - targetV.y)) + ((rootV.z - targetV.z) * (rootV.z - targetV.z));
        return f;
    }

    public string persenGet(int persent)
    {
        string s = null;
        int tenPer = Mathf.FloorToInt(persent / 10);
        char[] persetSet = new char[12]
        {' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ',' '};

        char[] rob = new char[4]{'/','-','\\','-'};

        persetSet[0] = '[';
        persetSet[11] = ']';

        for (int i = 1; i < tenPer+1; i++)
        {
            persetSet[i] = '=';
        }

        persetSet[tenPer + 1] = rob[persent%4];
         s = new string(persetSet);
        if (persent >= 100)
        {
            s = "[==========]";
        }
        return s;
    }
}