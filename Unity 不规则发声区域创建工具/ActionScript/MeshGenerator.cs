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
    public GameObject SplinerTools;//Splineê�㸸������
    public string MeshName;
    private Transform[] Localvertices;//ê��λ��
    public float height;
    Mesh mesh;//����
    MeshCollider meshCollider;//������ײ��
    public Vector3[] InputVertices;//��ȡ��ê����Ϊmesh����
    public int[] triangles;//������

    public void Tool_Act()//������Ϊ����
    {
        SetPositions();//��ȡê��
        SetTriangles();//����������

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;//��������MeshFilter
        meshCollider = new MeshCollider();
        GetComponent<MeshCollider>().sharedMesh = mesh;//�����ڱ༭ģʽ����������Ϊ����


        UpdateMesh();//����mesh
        SaveMesh();
        DeleteKnot();//ɾ���ӽڵ�

        DestroyImmediate(this.GetComponent<MeshGenerator>());//��ɲ����󽫱������������ɾ��
    }
    public void SaveMesh()//����mesh�ļ���Ŀ��·��
    {
        Mesh _mesh = GetComponent<MeshFilter>().sharedMesh;
        AssetDatabase.CreateAsset(_mesh, "Assets/AudioFile/Wwise_TestTools/Tool2/AkAudioLimiterMesh/" + MeshName + ".asset");
    }
    public void DeleteKnot()//���������ƺ�ɾ��Spline���ɵ�Knot
    {
        for(int i = Localvertices.Length - 1; i >= 0; i--)
        {
            if (Localvertices[i]!=this.transform)
            {
                DestroyImmediate(Localvertices[i].gameObject);
            }
        }
    }
    private void UpdateMesh()//�������
    {
        //mesh.Clear();
        mesh.vertices = InputVertices;
        mesh.triangles = triangles;
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }
    public void SetPositions()//����Knotê���ȡ��������
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
    public void SetTriangles()//��knotê��������������������Ⱦ˳��
    {
        //�������ɶ���������˳ʱ������ƽ��Mesh
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
