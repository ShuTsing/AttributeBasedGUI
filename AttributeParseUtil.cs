using System.Collections.Generic;
using System.Reflection;
using System;
using Godot;

namespace AttributeBasedGUI;

public static class AttributeParseUtil
{
    public static List<MemberInfo> GetShowInInspectorMembers(Type targetType)
    {
        var members = targetType.GetMembers(BindingFlags.Instance);
        List<MemberInfo> showInInspectorMembers = new List<MemberInfo>();
        foreach (var member in members)
        {
            if (member is FieldInfo fieldInfo)
            {
                if (fieldInfo.GetCustomAttribute<HideInInspectorAttribute>() != null)
                    continue;
                
                if (fieldInfo.IsPublic == false && fieldInfo.GetCustomAttribute<ShowInInspectorAttribute>() == null)
                    continue;
                
                showInInspectorMembers.Add(fieldInfo);
            }
            
            if (member is PropertyInfo propertyInfo)
            {
                if (propertyInfo.GetCustomAttribute<HideInInspectorAttribute>() != null)
                    continue;
                
                var getter = propertyInfo.GetGetMethod(true);
                var setter = propertyInfo.GetSetMethod(true);
                
                if (getter == null || setter == null)
                    continue;

                if (getter.IsPublic == false && setter.IsPublic == false)
                {
                    if (propertyInfo.GetCustomAttribute<ShowInInspectorAttribute>() == null)
                        continue;
                }
                
                showInInspectorMembers.Add(propertyInfo);
            }

            if (member is MethodInfo methodInfo)
            {
                if (methodInfo.GetCustomAttribute<ButtonAttribute>() == null)
                    continue;
                
                if (methodInfo.GetParameters().Length > 0)
                    continue;
                
                showInInspectorMembers.Add(methodInfo);
            }
        }
        return showInInspectorMembers;
    }
    
    public static List<MemberInfo> GetShowInInspectorMembers(System.Object target)
    {
        return GetShowInInspectorMembers(target.GetType());
    }
    
    public static void SortMembersByPropertyOrder(in List<MemberInfo> members)
    {
        members.Sort((a, b) =>
        {
            var orderAttrA = a.GetCustomAttribute<PropertyOrderAttribute>();
            var orderAttrB = b.GetCustomAttribute<PropertyOrderAttribute>();
			
            int orderA = orderAttrA?.Order ?? 0;
            int orderB = orderAttrB?.Order ?? 0;
			
            if (orderA < orderB)
            {
                return -1;
            }
            else if (orderA > orderB)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        });
    }
    
}