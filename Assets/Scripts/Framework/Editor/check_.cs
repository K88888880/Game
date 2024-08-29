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
        //�����Ʋ������л�����
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

        //�Ƴ������ڵ�procedure
        for (int i = proceduresProperty.arraySize - 1; i >= 0; i--)
        {

            string procedureTypeName = proceduresProperty.GetArrayElementAtIndex(i).stringValue;
            if (!list.Contains(procedureTypeName))
            {
                //ɾ��ָ��������Ԫ��
                proceduresProperty.DeleteArrayElementAtIndex(i);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }

    public override void OnInspectorGUI()
    {
        //�������ڵĿؼ�
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
                //��������ʾ��ǰ״̬
                EditorGUILayout.LabelField("Current Procedure", TGameFramework.Instance.GetModule<ProcedureModule>().CurrentProcedure?.GetType().FullName);
            }
            else
            {
                //��ʾĬ��״̬
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
        //Ӧ�������޸�
        serializedObject.ApplyModifiedProperties();
    }

    private void AddProcedure(string procedureType)
    {
        //�������е�ָ�������������Ԫ��
        proceduresProperty.InsertArrayElementAtIndex(0);
        //��ֵָ����������Ԫ��
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
        {                                           //����������ָ����������Ԫ��
            SerializedProperty p = proceduresProperty.GetArrayElementAtIndex(i);
            //p.stringValueѡ�е�Ԫ��
             
            if (p.stringValue == procedureType)
            {
                return i;
            }
        }
        return null;
    }
}
