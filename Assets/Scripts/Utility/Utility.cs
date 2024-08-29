using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public static partial class Utility
{
    public static class Types
    {
        public readonly static Assembly GAME_CSHARP_ASSEMBLY = Assembly.Load("Assembly-CSharp");
        public readonly static Assembly GAME_EDITOR_ASSEMBLY = Assembly.Load("Assembly-CSharp-Editor");

        /// <summary>
        /// ��ȡ�����ܴ�ĳ�����ͷ���������б�
        /// </summary>
        public static List<PropertyInfo> GetAllAssignablePropertiesFromType(Type basePropertyType, Type objType, BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
        {
            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            PropertyInfo[] properties = objType.GetProperties(bindingFlags);
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo propertyInfo = properties[i];
                if (basePropertyType.IsAssignableFrom(propertyInfo.PropertyType))
                {
                    propertyInfos.Add(propertyInfo);
                }
            }
            return propertyInfos;
        }

        /// <summary>
        /// ��ȡĳ�����͵�����������
        /// </summary>
        /// <param name="baseClass">����</param>
        /// <param name="assemblies">����,���Ϊnull����ҵ�ǰ����</param>
        /// <returns></returns>
        public static List<Type> GetAllSubclasses(Type baseClass, bool allowAbstractClass, params Assembly[] assemblies)
        {
            List<Type> subclasses = new List<Type>();
            if (assemblies == null)
            {
                assemblies = new Assembly[] { Assembly.GetCallingAssembly() };
            }
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!baseClass.IsAssignableFrom(type))
                        continue;

                    if (!allowAbstractClass && type.IsAbstract)
                        continue;

                    subclasses.Add(type);
                }
            }
            return subclasses;
        }
    }
}
