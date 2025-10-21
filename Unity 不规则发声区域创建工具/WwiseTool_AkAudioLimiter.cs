using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.Shapes;

//�����������Ĺ��ߴ���UI���
public class WwiseTool_AkAudioLimiter : EditorWindow
{
    [Header("�ڴ˷��������ê��ʵ������Spline����")]
    public GameObject splineObject;//�����ê��ʵ������Spline
    public SerializedObject serialized_Splineobject;
    private SerializedProperty serialized_Splineobject_property;
    //public GameObject mySerializedObject;
    [Header("�ڴ�����������Trigger�߶�")]
    public float triggerHeight;//Mesh���ɵĸ߶�
    public string meshName;//Mesh�ı�����


    [MenuItem("Tools/����һ��������˥����Ƶ����")]
    public static void ShowWindow()
    {
        GetWindow<WwiseTool_AkAudioLimiter>("˥�����򴴽�����");
    }
    private void OnEnable()
    {
        serialized_Splineobject = new SerializedObject(this);
        serialized_Splineobject_property = serialized_Splineobject.FindProperty("splineObject");
    }
    private void OnGUI()
    {
        //serialized_Splineobject = new SerializedObject(this);
        //serialized_Splineobject_property = serialized_Splineobject.FindProperty("splineObject");

        serialized_Splineobject.Update();
        GUILayout.Label("\n==ʹ��ǰ��Ҫ����һЩǰ�ò������������İ�ť�鿴==", EditorStyles.boldLabel);

        if (GUILayout.Button("ǰ�ù���"))
        {
            AnotherEditorWindow.ShowWindow();
        }

        GUILayout.Space(20);
        EditorGUILayout.PropertyField(serialized_Splineobject_property, true);
        serialized_Splineobject.ApplyModifiedProperties();

        GUILayout.Label("����������Trigger�߶�", EditorStyles.boldLabel);

        triggerHeight = Mathf.Max(0.0f, EditorGUILayout.FloatField("������Mesh�߶ȣ�", triggerHeight));

        meshName = EditorGUILayout.TextField("Mesh�ı�������", meshName);
        serialized_Splineobject.ApplyModifiedProperties();
        if (GUILayout.Button("\n���ɲ����� Trigger MeshCollider\n"))
        {
            if (splineObject == null)
            //if(mySerializedObject == null)
            {
                Debug.LogWarning("����˥�����򴴽����ߣ�δ�ڹ����з��������ê��ʵ������Spline����");
                return;
            }
            if(triggerHeight<0.5f)
            {
                Debug.LogWarning("����˥�����򴴽����ߣ������Trigger�߶ȹ�С");
                return;
            }

            if(string.IsNullOrEmpty(meshName))
            {
                Debug.LogWarning("����˥�����򴴽����ߣ�������Mesh�ı�������");
                return;
            }
            //����Trigger���ƺͱ���
            Debug.Log("����Mesh�����ɺͱ��棬�������˹��ߴ���ͬ�ļ����µ�AkAudioLimiterMesh�ļ����У�������Ϊ" + meshName);
            CreateTriggerMesh();
            return;
        }
    }
    //��splineObject������һ������Camera�Ľű�����
    public void CreatePostObject()
    {
        GameObject NewpostObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        NewpostObject.transform.position = splineObject.transform.position;

        NewpostObject.AddComponent<LimitPostObject>();

        NewpostObject.GetComponent<LimitPostObject>().boundaryCollider = splineObject.GetComponent<MeshCollider>();
        NewpostObject.GetComponent<LimitPostObject>().postEventGameobject = NewpostObject.transform;
        NewpostObject.GetComponent<LimitPostObject>().cameraTransform = Camera.main.transform;

        Undo.RegisterCreatedObjectUndo(NewpostObject, "˥����Post����");
    }
    public void CreateTriggerMesh()
    {
        //����Mesh�Ļ��ƺͱ���
        splineObject.AddComponent<MeshCollider>();
        splineObject.AddComponent<MeshFilter>();
        splineObject.AddComponent<MeshGenerator>();

        splineObject.GetComponent<MeshCollider>().convex = true;
        splineObject.GetComponent<MeshGenerator>().SplinerTools = splineObject;
        splineObject.GetComponent<MeshGenerator>().MeshName = meshName;
        splineObject.GetComponent<MeshGenerator>().height = triggerHeight;
        
        splineObject.GetComponent<MeshGenerator>().Tool_Act();
        CreatePostObject();//�������Ƹ�������


        //DestroyImmediate(splineObject.GetComponent<MeshGenerator>());
    }
}
public class AnotherEditorWindow : EditorWindow
{
    public static void ShowWindow()
    {
        GetWindow<AnotherEditorWindow>("Another Editor Window");
    }

    private void OnGUI()
    {
        GUILayout.Label("\n\n1).����һ��Spline����Ȧ��˥��������(�����λ��Ӻ�\n��Esc�������򻮶�)\n\n2).�ڡ�Spline����������ӡ�Spline Instantiate�����" +
            "\n\n3).�ڡ�  Items To Instantiate  �������һ�����壬\n����������Ϊ ��Tool2���ļ����µġ�Wwise_ToolKnot��Ԥ����" +
            "\n\n4).���á�Spacing��Spline��   Dist�����ʵ�����\n(����Խ��Trigger����Խ�٣���������Խ��)��" +
            "\n\n5).����� Bake Instances ���������ê��Ĵ���\n(�����ٴ�������ê��)" +
            "\n\n\n\n\n=====���ǰ�ù��������Թرո���ʾ���ڻص����������=====", EditorStyles.boldLabel);
    }
}

