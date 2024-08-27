using System;
using System.Collections.Generic;
using UnityEngine;

public class TGameFramework : MonoBehaviour
{
    public static TGameFramework Instance { get; private set; }

    public static bool Initialized { get; private set; }

    private Dictionary<Type, BaseGameModule> m_modulesdic = new Dictionary<Type, BaseGameModule>();


    private void Awake()
    {
        Instance = new TGameFramework();
    }
    /// <summary>
    /// 获取模块
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetModule<T>() where T : BaseGameModule
    {
        if (m_modulesdic.TryGetValue(typeof(T), out BaseGameModule baseGameModule))
        {
            return baseGameModule as T;
        }
        return default(T);
    }

    /// <summary>
    /// 添加模块
    /// </summary>
    /// <param name="module"></param>
    public void AddModule(BaseGameModule module)
    {
        Type type = module.GetType();
        if (m_modulesdic.ContainsKey(type))
        {
            Debug.Log("已经添加，不能重复添加");
            return;
        }
        m_modulesdic.Add(type, module);
    }

    private void Update()
    {
        IsRun();
        float deltaTime = Time.deltaTime;
        foreach (var item in m_modulesdic.Values)
        {
            item.OnModuleUpdate(deltaTime);
        }
    }
    private void LateUpdate()
    {
        IsRun();
        float deltaTime = Time.deltaTime;
        foreach (var item in m_modulesdic.Values)
        {
            item.OnModuleLateUpdate(deltaTime);
        }
    }
    private void FixedUpdate()
    {
        IsRun();
        float deltaTime = Time.deltaTime;
        foreach (var item in m_modulesdic.Values)
        {
            item.OnModuleFixedUpdate(deltaTime);
        }
    }
    public void InitModules()
    {
        if (Initialized) return;
        Initialized = true;
        foreach (var item in m_modulesdic.Values)
        {
            item.OnModuleInit();
        }
    }

    public void StartModule()
    {
        if (m_modulesdic == null) return;
        if (!Initialized) return;
        foreach (var item in m_modulesdic.Values)
        {
            item.OnModuleStart();
        }
    }

    public void Destroy()
    {
        if (!Initialized) return;
        if (Instance != this) return;
        if (Instance.m_modulesdic == null) return;
        foreach (var item in m_modulesdic.Values)
        {
            item.OnModuleStop();
        }
        Instance = null;
        Initialized = false;
    }



    void IsRun()
    {
        if (!Initialized) return;
        if (m_modulesdic == null) return;
        if (!Initialized) return;
    }

}
