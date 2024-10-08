using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BaseInspector : Editor
{
    protected virtual bool DrawBaseGUI { get { return true; } }

    private bool isCompiling = false;
    protected virtual void OnInspectorUpdateInEditor() { }

    private void OnEnable()
    {
        OnInspectorEnable();
        EditorApplication.update += UpdateEditor;
    }
    protected virtual void OnInspectorEnable() { }

    private void OnDisable()
    {
        EditorApplication.update -= UpdateEditor;
        OnInspectorDisable();
    }
    protected virtual void OnInspectorDisable() { }

    private void UpdateEditor()
    {
        if (!isCompiling && EditorApplication.isCompiling)
        {
            isCompiling = true;
            OnCompileStart();
        }
        else if (isCompiling && !EditorApplication.isCompiling)
        {
            isCompiling = false;
            OnCompileComplete();
        }
        OnInspectorUpdateInEditor();
    }

    public override void OnInspectorGUI()
    {
        if (DrawBaseGUI)
        {
            base.OnInspectorGUI();
        }
    }

    protected virtual void OnCompileStart() { }
    protected virtual void OnCompileComplete() { }
}
