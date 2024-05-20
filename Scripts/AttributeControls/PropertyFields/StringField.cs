using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

[Tool]
public partial class StringField : PropertyField
{
    public StringField(object target, FieldInfo fieldInfo) : base(target, fieldInfo)
    {
    }
    
}