using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Reflection;
using System.Linq;

public class CreateKeys
{
    /// <summary>
    /// 脚本的程序集
    /// </summary>
    public const string AssemblyCSharpKey = "Assembly-CSharp";
}

public class CreatePrefs
{
    /// <summary>
    /// 是否开启创建脚本功能
    /// </summary>
    private static string editorCreateStateKey = "editorCreateStateKey";//是否开启创建脚本功能
    /// <summary>
    /// 创建的预设 InstanceID
    /// </summary>
    private static string editorPrefabIdKey = "editorPrefabIdKey";//创建的预设 InstanceID
    /// <summary>
    /// 用户输入的脚本名
    /// </summary>
    private static string inputScriptNameKey = "inputScriptName";//用户输入的脚本名
    
    public static void Record(bool createState, string inputScriptName, int instanceID)
    {
        EditorPrefs.SetBool(CreatePrefs.editorCreateStateKey, createState);
        EditorPrefs.SetString(CreatePrefs.inputScriptNameKey, inputScriptName);
        EditorPrefs.SetInt(CreatePrefs.editorPrefabIdKey, instanceID);
    }
    public static void DeleteKeys()
    {
        if (EditorPrefs.HasKey(editorCreateStateKey)) { EditorPrefs.DeleteKey(editorCreateStateKey); }
        if (EditorPrefs.HasKey(inputScriptNameKey)) { EditorPrefs.DeleteKey(inputScriptNameKey); }
        if (EditorPrefs.HasKey(editorPrefabIdKey)) { EditorPrefs.DeleteKey(editorPrefabIdKey); }
    }

    public static bool GetCreateState() { return EditorPrefs.GetBool(editorCreateStateKey); }
    public static int GetPrefabID() { return EditorPrefs.GetInt(editorPrefabIdKey); }
    public static string GetInputScriptName() { return EditorPrefs.GetString(inputScriptNameKey); }



}

public class UIPanelType
{
    public const string Canvas = "EditorCanvas";
    public const string Panel左右对话 = "EditorPanel左右对话";
    public const string Panel填空题 = "EditorPanel填空题";
    public const string Panel选择题 = "EditorPanel选择题";
    public const string Panel系统提示1 = "EditorPanel系统提示1";
    public const string Panel系统提示2 = "EditorPanel系统提示2";
    public const string Panel开始实验 = "EditorPanel开始实验";
    public const string Panel登录 = "EditorPanel登录";
    public const string Panel显示图片 = "EditorPanel显示图片";
    public const string Panel自动提示 = "EditorPanel自动提示";
    public const string Panel帮助 = "EditorPanel帮助";
    public const string Panel环形按钮列表 = "EditorPanel环形按钮列表";
    public const string Panel纵向按钮列表 = "EditorPanel纵向按钮列表";
    public const string Panel横向按钮列表 = "EditorPanel横向按钮列表";
    public const string Panel屏中按钮1 = "EditorPanel屏中按钮1";
    public const string Panel标题栏 = "EditorPanel标题栏";
    public const string Panel场景加载 = "EditorPanel场景加载";
    public const string Panel黑屏效果 = "EditorPanel黑屏效果";

}

public class EditorCreateUIWindow : EditorWindow
{
    private string scriptName;
    public string InputScriptName { get { return scriptName; } }

    public string InputBaseScriptName { get { return baseClass; } }


    public Action event_ClickCreateButton;

    public string createUIType = "";

    private string baseClass = "";
    //private bool changeBaseClassState = false;

    private void OnEnable()
    {
        //changeBaseClassState = false;
        
        baseClass = EditorCreateKeys.BaseClass;
        enableShowPanel = false;
        tipContent = "";
    }

    private void OnGUI()
    {
        GUIStyle WindowBackground = GUI.skin.FindStyle("WindowBackground");
        GUIStyle ErrorLabel = GUI.skin.FindStyle("ErrorLabel");
        GUIStyle ButtonStyle = GUI.skin.FindStyle("toolbarbutton");
        GUIStyle LargeTextField = GUI.skin.FindStyle("LargeTextField");

        GUILayout.Space(8);
        if (createUIType.Length >= "Editor".Length && createUIType.Substring(0, "Editor".Length) == "Editor")
        {
            createUIType = createUIType.Substring("Editor".Length);
        }
        if (createUIType.Length >= "Panel".Length && createUIType.Substring(0, "Panel".Length) == "Panel")
        {
            createUIType = createUIType.Substring("Panel".Length);
        }
        GUILayout.Label("当前要创建的UI类型：" + createUIType);
        
        GUIStyle areaStyle = new GUIStyle(WindowBackground);
        GUILayout.BeginArea(new Rect(20, 30, 400, 300), areaStyle);

        GUILayout.BeginHorizontal();
        GUILayout.Label("当前父类：", GUILayout.Width(60), GUILayout.Height(20));
        baseClass = EditorGUILayout.TextField(baseClass, GUILayout.Width(240), GUILayout.Height(20));
        GUILayout.EndHorizontal();

        GUIStyle labelStyle = new GUIStyle(EditorStyles.label);
        labelStyle.fontSize = 18;
        GUILayout.Label("请输入脚本名称：", labelStyle);
        
        GUILayout.Space(8);
        GUIStyle scriptNameInputStyle = new GUIStyle(LargeTextField);
        scriptNameInputStyle.fixedHeight = 26;
        scriptNameInputStyle.fontSize = 18;
        scriptNameInputStyle.alignment = TextAnchor.MiddleLeft;
        scriptName = EditorGUILayout.TextField(scriptName, scriptNameInputStyle);

        GUILayout.Space(8);
        GUIStyle endLabelStyle = new GUIStyle(ErrorLabel);
        GUILayout.Label("（为空表示创建UI物体，但是不创建脚本）", endLabelStyle);

        GUILayout.Space(position.height - 200);
        GUILayout.BeginHorizontal();
        GUILayout.Space(position.width - 165);
        GUIStyle buttonStyle = new GUIStyle(ButtonStyle);
        buttonStyle.fixedHeight = 30;
        buttonStyle.fixedWidth = 100;
        buttonStyle.fontSize = 14;
        if (GUILayout.Button("创建", buttonStyle))
        {
            if (event_ClickCreateButton != null)
            {
                event_ClickCreateButton();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.EndArea();

        if (enableShowPanel)
        {
            GUI.color = Color.red;
            EditorGUILayout.LabelField(tipContent);
        }
        
    }

    private bool enableShowPanel = false;
    private string tipContent = "";
    public void ShowTip(string tip)
    {
        tipContent = tip;
        enableShowPanel = true;
    }

}

public class EditorCreateUI
{
    public static GameObject CraeteUI(string panelName, Transform parent)
    {
        UnityEngine.Object uiPrefab = Resources.Load("UIPrefabs/" + panelName);
        if (uiPrefab != null)
        {
            GameObject uiObject = UnityEngine.Object.Instantiate(uiPrefab, parent) as GameObject;
            uiObject.name = panelName.Substring("Editor".Length);
            uiObject.transform.SetAsLastSibling();
            Undo.RegisterCreatedObjectUndo(uiObject, "");
            return uiObject;
        }
        return null;
    }
    public static string GetPanelScriptContent(string panelName)
    {
        string scriptContent = "";
        panelName = "Script" + panelName.Substring("Editor".Length);
        TextAsset textAsset = Resources.Load<TextAsset>("UIPrefabScripts/" + panelName);
        if (textAsset != null)
        {
            scriptContent = textAsset.text;
        }
        return scriptContent;
    }
    public static string GetPanelScriptName(string panelName)
    {
        panelName = "Script" + panelName.Substring("Editor".Length);
        return panelName;
    }

    public static GameObject Create(string inputScriptName, string inputBaseScriptName, string panelPrefabName)
    {
        Transform uiParent = null;
        if (Selection.activeGameObject != null) { uiParent = Selection.activeGameObject.transform; }

        GameObject uiObject = EditorCreateUI.CraeteUI(panelPrefabName, uiParent);
        if (uiObject != null)
        {
            //UnityEngine.Debug.Log(inputScriptName != null);
            if (inputScriptName != null && inputScriptName != "")
            {
                //UnityEngine.Debug.Log(inputScriptName);
                CreatePrefs.Record(true, inputScriptName, uiObject.GetInstanceID());
                //创建脚本文件
                string scriptPath = Application.dataPath + "/Scripts/";
                string scriptContent = EditorCreateUI.GetPanelScriptContent(panelPrefabName);
                scriptContent = scriptContent.Replace("#BaseClass#", inputBaseScriptName);
                scriptContent = scriptContent.Replace("#ClassName#", inputScriptName);
                CreateScriptFile(scriptPath, inputScriptName, scriptContent);
            }
            else
            {//
                //UnityEngine.Debug.Log("输入的脚本名为空");
            }
        }

        return uiObject;
    }
    
    private static void CreateScriptFile(string path, string scriptName, string content)
    {
        // 检查是否存在文件夹
        if (!System.IO.Directory.Exists(path))
        {
            //创建pic文件夹
            System.IO.Directory.CreateDirectory(path);
            //UnityEngine.Debug.Log("创建文件夹");
        }
        else
        {
            //UnityEngine.Debug.Log("文件夹已存在");
        }
        
        StreamWriter writer = File.CreateText(path + scriptName + ".cs");
        writer.Write(content);
        writer.Close();

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 检查程序集中是否包含类
    /// </summary>
    /// <param name="assemblyName"></param>
    /// <param name="className"></param>
    /// <returns></returns>
    public static bool AssemblyExistClass(string assemblyName, string className)
    {
        Assembly ass = Assembly.Load(assemblyName);
        //UnityEngine.Debug.Log(ass);
        Type[] types = ass.GetTypes();
        for (int i = 0; i < types.Length; i++)
        {
            //UnityEngine.Debug.Log(types[i].FullName);
            if (types[i].Name == className)
            {
                return true;
            }
        }
        return false;
    }
    
    [InitializeOnLoadMethod]
    static void ComplierRun()
    {
        //bool isExits = EditorCreateUI.AssemblyExistClass("Assembly-CSharp", "qq");

        //UnityEngine.Debug.Log("ComplierRun");
        bool createState = CreatePrefs.GetCreateState();
        int instanceID = CreatePrefs.GetPrefabID();
        string inputScriptName = CreatePrefs.GetInputScriptName();

        if (createState)
        {
            //EditorPrefs.SetBool(CreatePrefs.editorEnableCreateKey, false);
            //int id = int.Parse(instanceID);
            UnityEngine.GameObject obj = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            //UnityEngine.Debug.Log(obj, obj);
            //UnityEngine.Debug.Log("物体ID：" + id);
            //UnityEngine.Debug.Log("创建脚本：" + inputScriptName);
            Assembly ass = Assembly.Load(CreateKeys.AssemblyCSharpKey);
            Type type = ass.GetType(inputScriptName);
            //Type type = Type.GetType(EditorCreateUI.AssemblyCSharpKey + "." + inputScriptName);
            //UnityEngine.Debug.Log(type);
            Component comp = obj.AddComponent(type);

            FieldInfo[] fieldInfos = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                //UnityEngine.Debug.Log(fieldInfos[i].Name);
                FieldInfo fieldInfo = fieldInfos[i];
                string fieldObjectName = fieldInfo.Name.Substring(1);
                Type fieldType = fieldInfo.FieldType;
                GameObject childObject = FindChild(obj, fieldObjectName);
                if (childObject != null)
                {
                    //UnityEngine.Debug.Log(fieldType == typeof(GameObject));
                    if (fieldType == typeof(GameObject))
                    {
                        fieldInfo.SetValue(comp, childObject);
                    }
                    else
                    {
                        Component fieldT = childObject.GetComponent(fieldType);
                        fieldInfo.SetValue(comp, fieldT);
                    }
                }
            }

            CreatePrefs.DeleteKeys();
        }
        else
        {
            //UnityEngine.Debug.Log("不能创建");
        }
    }

    private static GameObject FindChild(GameObject parent, string childName)
    {
        List<Transform> childs = parent.GetComponentsInChildren<Transform>(true).ToList();
        Transform child = childs.Find(x => x.name == childName);
        return child == null ? null : child.gameObject;
    }


}

public class EditorCreateUIPanel : Editor
{



    #region 创建 UI 布局

    [MenuItem("Tool/Create UI/Reset Create State(重置创建功能)")]
    static void ResetCreateUIFunction()
    {
        bool state = EditorUtility.DisplayDialog("提示", "确定重置创建功能吗？\n（如果创建功能出现问题不能创建时应用，此功能不会删除已经创建的 UI 内容）", "确定", "取消");
        if (state)
        {
            CreatePrefs.DeleteKeys();
        }
    }

    [MenuItem("Tool/Create UI/Create Canvas")]
    static void CreateUICanvas()
    {
        string panelPrefabName = UIPanelType.Canvas;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 填空题")]
    static void CreateUIPanel填空题()
    {
        string panelPrefabName = UIPanelType.Panel填空题;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 屏中按钮1(一个按钮)")]
    static void CreateUIPanel屏中按钮1()
    {
        string panelPrefabName = UIPanelType.Panel屏中按钮1;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 左右对话")]
    static void CreateUIPanel左右对话()
    {
        string panelPrefabName = UIPanelType.Panel左右对话;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 场景加载")]
    static void CreateUIPanel场景加载()
    {
        string panelPrefabName = UIPanelType.Panel场景加载;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 黑屏效果")]
    static void CreateUIPanel黑屏效果()
    {
        string panelPrefabName = UIPanelType.Panel黑屏效果;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 帮助")]
    static void CreateUIPanel帮助()
    {
        string panelPrefabName = UIPanelType.Panel帮助;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 开始实验")]
    static void CreateUIPanel开始实验()
    {
        string panelPrefabName = UIPanelType.Panel开始实验;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 显示图片")]
    static void CreateUIPanel显示图片()
    {
        string panelPrefabName = UIPanelType.Panel显示图片;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 标题栏")]
    static void CreateUIPanel标题栏()
    {
        string panelPrefabName = UIPanelType.Panel标题栏;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 横向按钮列表")]
    static void CreateUIPanel横向按钮列表()
    {
        string panelPrefabName = UIPanelType.Panel横向按钮列表;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 环形按钮列表")]
    static void CreateUIPanel环形按钮列表()
    {
        string panelPrefabName = UIPanelType.Panel环形按钮列表;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 登录")]
    static void CreateUIPanel登录()
    {
        string panelPrefabName = UIPanelType.Panel登录;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 系统提示1(一个按钮)")]
    static void CreateUIPanel系统提示1()
    {
        string panelPrefabName = UIPanelType.Panel系统提示1;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 系统提示2(两个按钮)")]
    static void CreateUIPanel系统提示2()
    {
        string panelPrefabName = UIPanelType.Panel系统提示2;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 纵向按钮列表")]
    static void CreateUIPanel纵向按钮列表()
    {
        string panelPrefabName = UIPanelType.Panel纵向按钮列表;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 自动提示")]
    static void CreateUIPanel自动提示()
    {
        string panelPrefabName = UIPanelType.Panel自动提示;
        InvokeCreateUI(panelPrefabName);
    }

    [MenuItem("Tool/Create UI/Create 选择题")]
    static void CreateUIPanel选择题()
    {
        string panelPrefabName = UIPanelType.Panel选择题;
        InvokeCreateUI(panelPrefabName);
    }

    #endregion


    private static EditorCreateUIWindow uiWin;
    private static void InvokeCreateUI(string panelPrefabName)
    {
        if (CreatePrefs.GetCreateState())
        {
            //UnityEngine.Debug.Log("正在创建中，请等待脚本创建完成");
            return;
        }

        if (uiWin != null)
        {
            uiWin.Close();
        }

        GUIContent titleContent = new GUIContent("创建面板");
        Rect re = new Rect(0, 0, 440, 340);
        //固定窗口大小
        uiWin = EditorWindow.GetWindowWithRect<EditorCreateUIWindow>(re);
        uiWin.titleContent = titleContent;
        //可更改大小窗口
        //uiWin = EditorCreateUIWindow.CreateInstance<EditorCreateUIWindow>();
        //uiWin.position = new Rect(20, 20, 440, 340);
        uiWin.Show();
        uiWin.createUIType = panelPrefabName;
        uiWin.event_ClickCreateButton = () =>
        {
            bool isExits = EditorCreateUI.AssemblyExistClass(CreateKeys.AssemblyCSharpKey, uiWin.InputScriptName);
            if (!isExits)
            {
                uiWin.Close();
                //string panelPrefabName = UIPanelType.Canvas;
                EditorCreateUI.Create(uiWin.InputScriptName, uiWin.InputBaseScriptName, panelPrefabName);
            }
            else
            {
                //UnityEngine.Debug.Log("类已经存在");
                uiWin.ShowTip("类已经存在，请更换类名");
            }
        };
    }

}


