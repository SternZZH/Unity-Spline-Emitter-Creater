using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public GameObject SplinerTools;//Spline锚点父级物体
    public string MeshName;
    private Transform[] Localvertices;//锚点位置
    public float height;
    Mesh mesh;//网格
    MeshCollider meshCollider;//网格碰撞体
    public Vector3[] InputVertices;//获取的锚点作为mesh顶点
    public int[] triangles;//三角形

    public void Tool_Act()//工具行为方法
    {
        SetPositions();//获取锚点
        SetTriangles();//绘制三角面

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;//设置网格到MeshFilter
        meshCollider = new MeshCollider();
        GetComponent<MeshCollider>().sharedMesh = mesh;//用于在编辑模式进行网格行为操作


        UpdateMesh();//绘制mesh
        SaveMesh();
        DeleteKnot();//删除子节点

        DestroyImmediate(this.GetComponent<MeshGenerator>());//完成操作后将本组件在物体上删除
    }
    public void SaveMesh()//保存mesh文件到目标路径
    {
        Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
        AssetDatabase.CreateAsset(_mesh, "Assets/AudioFile/Wwise_TestTools/Tool2/AkAudioLimiterMesh/" + MeshName + ".asset");
    }
    public void DeleteKnot()//完成网格绘制后删除Spline生成的Knot
    {
        for(int i = Localvertices.Length - 1; i >= 0; i--)
        {
            if (Localvertices[i]!=this.transform)
            {
                DestroyImmediate(Localvertices[i].gameObject);
            }
        }
    }
    private void UpdateMesh()//网格更新
    {
        //mesh.Clear();
        mesh.vertices = InputVertices;
        mesh.triangles = triangles;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    public void SetPositions()//按照Knot锚点读取本地坐标
    {
        Localvertices = SplinerTools.GetComponentsInChildren<Transform>();
        InputVertices = new Vector3[2 * Localvertices.Length];

        for (int i = 0; i < Localvertices.Length; i++)
        {
            if (Localvertices[i]!=this.transform)
            {
                InputVertices[i] = Localvertices[i].transform.localPosition;
                InputVertices[i + Localvertices.Length] = Localvertices[i].transform.localPosition + new Vector3(0, height, 0);
            }
        }
    }
    public void SetTriangles()//以knot锚点数量来生成三角面渲染顺序
    {
        //按照生成顶点数量，顺时针生成平面Mesh
        int i =  3 * ((InputVertices.Length) - 2);
        triangles = new int[i];
        for (int j = 0; j < (InputVertices.Length/2) - 2; j++)
        {
            triangles[3 * j] = 1;
            triangles[(3 * j) + 1] = j + 1;
            triangles[(3 * j) + 2] = j + 2;

            triangles[3 * (j + (InputVertices.Length/2) - 2)] = InputVertices.Length/2;
            triangles[3 * (j + (InputVertices.Length/2) - 2) + 1] = InputVertices.Length/2+j+1;
            triangles[3 * (j + (InputVertices.Length/2) - 2) + 2] = InputVertices.Length/2+j+2;
        }
    }
}
