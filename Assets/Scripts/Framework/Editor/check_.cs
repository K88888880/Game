using System;
using System.Collections;
using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestShouCheck))]
public class check_ : BaseInspector
{
    private SerializedProperty proceduresProperty;
    private SerializedProperty defaultProcedureProperty;
    //SerializedObject serializedObject;
    List<string> list=new List<string>();

    protected override void OnInspectorEnable()
    {
        list.Clear();

        for (int i = 0; i < 6; i++)
        {
            list.Add(i.ToString());
        }
        base.OnInspectorEnable();
        //按名称查找序列化属性
        proceduresProperty = serializedObject.FindProperty("proceduresNames");

        defaultProcedureProperty = serializedObject.FindProperty("defaultProcedureName");
        UpdateProcedures();
    }

    protected override void OnCompileComplete()
    {
        base.OnCompileComplete();
        UpdateProcedures();
    }

    private void UpdateProcedures()
    {
        //list = Utility.Types.GetAllSubclasses(typeof(BaseProcedure), false, Utility.Types.GAME_CSHARP_ASSEMBLY).ConvertAll((Type t) => { return t.FullName; });

        //移除不存在的procedure
        for (int i = proceduresProperty.arraySize - 1; i >= 0; i--)
        {

            string procedureTypeName = proceduresProperty.GetArrayElementAtIndex(i).stringValue;
            if (!list.Contains(procedureTypeName))
            {
                //删除指定索引处元素
                proceduresProperty.DeleteArrayElementAtIndex(i);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        //禁用组内的控件
        EditorGUI.BeginDisabledGroup(Application.isPlaying);
        {
            if (list.Count > 0)
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        GUI.changed = false;
                        int? index = FindProcedureTypeIndex(list[i]);
                        bool selected = EditorGUILayout.ToggleLeft(list[i], index.HasValue);
                        if (GUI.changed)
                        {
                            if (selected)
                            {
                                AddProcedure(list[i]);
                            }
                            else
                            {
                                RemoveProcedure(index.Value);
                            }
                        }
                    }
                }
                EditorGUILayout.EndVertical();
            }
        }
        EditorGUI.EndDisabledGroup();

        if (proceduresProperty.arraySize == 0)
        {
            if (list.Count == 0)
            {
                EditorGUILayout.HelpBox("Can't find any procedure", UnityEditor.MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Please select a procedure at least", UnityEditor.MessageType.Info);
            }
        }
        else
        {
            if (Application.isPlaying)
            {
                //播放中显示当前状态
                EditorGUILayout.LabelField("Current Procedure", TGameFramework.Instance.GetModule<ProcedureModule>().CurrentProcedure?.GetType().FullName);
            }
            else
            {
                //显示默认状态
                List<string> selectedProcedures = new List<string>();
                for (int i = 0; i < proceduresProperty.arraySize; i++)
                {
                    selectedProcedures.Add(proceduresProperty.GetArrayElementAtIndex(i).stringValue);
                }
                selectedProcedures.Sort();
                int defaultProcedureIndex = selectedProcedures.IndexOf(defaultProcedureProperty.stringValue);
                defaultProcedureIndex = EditorGUILayout.Popup("Default Procedure", defaultProcedureIndex, selectedProcedures.ToArray());
                if (defaultProcedureIndex >= 0)
                {
                    defaultProcedureProperty.stringValue = selectedProcedures[defaultProcedureIndex];
                }
            }
        }
        //应用属性修改
        serializedObject.ApplyModifiedProperties();
    }

    private void AddProcedure(string procedureType)
    {
        //在数组中的指定索引处插入空元素
        proceduresProperty.InsertArrayElementAtIndex(0);
        //赋值指定索引处的元素
        proceduresProperty.GetArrayElementAtIndex(0).stringValue = procedureType;
    }

    private void RemoveProcedure(int index)
    {
        string procedureType = proceduresProperty.GetArrayElementAtIndex(index).stringValue;
        if (procedureType == defaultProcedureProperty.stringValue)
        {
            Debug.LogWarning("Can't remove default procedure");
            return;
        }
        proceduresProperty.DeleteArrayElementAtIndex(index);
    }

    private int? FindProcedureTypeIndex(string procedureType)
    {
        for (int i = 0; i < proceduresProperty.arraySize; i++)
        {                                           //返回数组中指定索引处的元素
            SerializedProperty p = proceduresProperty.GetArrayElementAtIndex(i);
            //p.stringValue选中的元素
             
            if (p.stringValue == procedureType)
            {
                return i;
            }
        }
        return null;
    }
}
