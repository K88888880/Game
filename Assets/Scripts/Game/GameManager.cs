using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
     
    [ModuleAttrbute(1)]
    public static MessageModule Message { get => TGameFramework.Instance.GetModule<MessageModule>(); }
    [ModuleAttrbute(2)]
    public static ProcedureModule Procedure { get => TGameFramework.Instance.GetModule<ProcedureModule>(); }

    private bool activing;
    private void Awake()
    {
        if (TGameFramework.Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        activing = true;
        DontDestroyOnLoad(gameObject);
#if UNITY_EDITOR

#else

#endif
        Application.logMessageReceived += OnReceiveLog;
        TGameFramework.Initialize();
        StartupModules();
        TGameFramework.Instance.InitModules();
    }
    private void Start()
    {
        TGameFramework.Instance.StartModule();

    }
    private void Update()
    {
        TGameFramework.Instance.Update();
    }
    private void LateUpdate()
    {
        TGameFramework.Instance.LateUpdate();
    }
    private void FixedUpdate()
    {
        TGameFramework.Instance.FixedUpdate();
    }

    private void OnDestroy()
    {
        if (activing)
        {
            Application.logMessageReceived -= OnReceiveLog;
            TGameFramework.Instance.Destroy();
        }
    }

    public void StartupModules()
    {
        List<ModuleAttrbute> moduleAttrbutes = new List<ModuleAttrbute>();
        //��������ָ�����ƵĹ�������
        PropertyInfo[] propertyInfos = GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        Type basecompType = typeof(BaseGameModule);
        for (int i = 0; i < propertyInfos.Length; i++)
        {
            PropertyInfo property = propertyInfos[i];
            if (!basecompType.IsAssignableFrom(property.PropertyType)) continue;
            //��ȡ�Զ�������
            //GetCustomAttributes(Ҫ�������������ͣ��Ƿ������ó�Ա�ļ̳����Բ�����Щ����)
            object[] attrs = property.GetCustomAttributes(typeof(ModuleAttrbute), false);
            if (attrs.Length == 0) continue;
            Component comp = GetComponentInChildren(property.PropertyType);
            if (comp == null)
            {
                Debug.LogError($"Can't Find GameModule:{property.PropertyType}");
                continue;
            }
            ModuleAttrbute moduleAttrbute = attrs[0] as ModuleAttrbute;
            moduleAttrbute.Module = comp as BaseGameModule;
            moduleAttrbutes.Add(moduleAttrbute);
        }
        moduleAttrbutes.Sort();
        for (int i = 0; i < moduleAttrbutes.Count; i++)
        {
            TGameFramework.Instance.AddModule(moduleAttrbutes[i].Module);
        }
    }

    //�Զ���������(Ӧ�����Եĳ���Ԫ�أ������ṹ��,����Ԫ�ؿ���ָ���������)  
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class ModuleAttrbute : Attribute, IComparable<ModuleAttrbute>
    {
        /// <summary>
        /// ���ȼ�
        /// </summary>
        public int Priority { get; private set; }
        /// <summary>
        /// ģ��
        /// </summary>
        public BaseGameModule Module { get; set; }

        public ModuleAttrbute(int priority)
        {
            Priority = priority;
        }

        int IComparable<ModuleAttrbute>.CompareTo(ModuleAttrbute other)
        {
            return Priority.CompareTo(other.Priority);
        }
    }
    private void OnReceiveLog(string condition, string stackTrace, LogType type)
    {
#if !UNITY_EDITOR
            if (type == LogType.Exception)
            {
                UnityLog.Fatal($"{condition}\n{stackTrace}");
            }
#endif
    }
}
