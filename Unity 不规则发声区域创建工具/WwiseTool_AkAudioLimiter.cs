using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;
using UnityEngine.ProBuilder.Shapes;

//样条发生器的工具代码UI面板
public class WwiseTool_AkAudioLimiter : EditorWindow
{
    [Header("在此放入完成了锚点实例化的Spline物体")]
    public GameObject splineObject;//完成了锚点实例化的Spline
    public SerializedObject serialized_Splineobject;
    private SerializedProperty serialized_Splineobject_property;
    //public GameObject mySerializedObject;
    [Header("在此输入期望的Trigger高度")]
    public float triggerHeight;//Mesh生成的高度
    public string meshName;//Mesh的保存名


    [MenuItem("Tools/创造一个不规则衰减音频区域")]
    public static void ShowWindow()
    {
        GetWindow<WwiseTool_AkAudioLimiter>("衰减区域创建工具");
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
        GUILayout.Label("\n==使用前需要先做一些前置操作，点击下面的按钮查看==", EditorStyles.boldLabel);

        if (GUILayout.Button("前置工作"))
        {
            AnotherEditorWindow.ShowWindow();
        }

        GUILayout.Space(20);
        EditorGUILayout.PropertyField(serialized_Splineobject_property, true);
        serialized_Splineobject.ApplyModifiedProperties();

        GUILayout.Label("输入期望的Trigger高度", EditorStyles.boldLabel);

        triggerHeight = Mathf.Max(0.0f, EditorGUILayout.FloatField("期望的Mesh高度：", triggerHeight));

        meshName = EditorGUILayout.TextField("Mesh的保存名：", meshName);
        serialized_Splineobject.ApplyModifiedProperties();
        if (GUILayout.Button("\n生成不规则 Trigger MeshCollider\n"))
        {
            if (splineObject == null)
            //if(mySerializedObject == null)
            {
                Debug.LogWarning("来自衰减区域创建工具：未在工具中放入完成了锚点实例化的Spline物体");
                return;
            }
            if(triggerHeight<0.5f)
            {
                Debug.LogWarning("来自衰减区域创建工具：输入的Trigger高度过小");
                return;
            }

            if(string.IsNullOrEmpty(meshName))
            {
                Debug.LogWarning("来自衰减区域创建工具：请输入Mesh的保存名字");
                return;
            }
            //进行Trigger绘制和保存
            Debug.Log("进行Mesh的生成和保存，保存在了工具代码同文件夹下的AkAudioLimiterMesh文件夹中，保存名为" + meshName);
            CreateTriggerMesh();
            return;
        }
    }
    //在splineObject处创建一个跟随Camera的脚本物体
    public void CreatePostObject()
    {
        GameObject NewpostObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        NewpostObject.transform.position = splineObject.transform.position;

        NewpostObject.AddComponent<LimitPostObject>();

        NewpostObject.GetComponent<LimitPostObject>().boundaryCollider = splineObject.GetComponent<MeshCollider>();
        NewpostObject.GetComponent<LimitPostObject>().postEventGameobject = NewpostObject.transform;
        NewpostObject.GetComponent<LimitPostObject>().cameraTransform = Camera.main.transform;

        Undo.RegisterCreatedObjectUndo(NewpostObject, "衰减音Post物体");
    }
    public void CreateTriggerMesh()
    {
        //进行Mesh的绘制和保存
        splineObject.AddComponent<MeshCollider>();
        splineObject.AddComponent<MeshFilter>();
        splineObject.AddComponent<MeshGenerator>();

        splineObject.GetComponent<MeshCollider>().convex = true;
        splineObject.GetComponent<MeshGenerator>().SplinerTools = splineObject;
        splineObject.GetComponent<MeshGenerator>().MeshName = meshName;
        splineObject.GetComponent<MeshGenerator>().height = triggerHeight;
        
        splineObject.GetComponent<MeshGenerator>().Tool_Act();
        CreatePostObject();//创造限制跟随物体


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
        GUILayout.Label("\n\n1).创建一个Spline，并圈好衰减声区域(完成首位相接后，\n按Esc结束区域划定)\n\n2).在“Spline”物体上添加“Spline Instantiate”组件" +
            "\n\n3).在“  Items To Instantiate  ”中添加一个物体，\n并设置物体为 “Tool2”文件夹下的“Wwise_ToolKnot”预制体" +
            "\n\n4).设置“Spacing（Spline）   Dist到合适的数字\n(数字越大，Trigger面数越少，加载性能越好)”" +
            "\n\n5).点击“ Bake Instances ”完成区域锚点的创建\n(请至少创建三个锚点)" +
            "\n\n\n\n\n=====完成前置工作，可以关闭该提示窗口回到工具面板了=====", EditorStyles.boldLabel);
    }
}

