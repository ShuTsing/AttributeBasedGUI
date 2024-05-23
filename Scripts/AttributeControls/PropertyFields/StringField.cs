using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AttributeBasedGUI;
[Tool]
public partial class StringField : PropertyField
{
    public StringField(FieldTreeNode fieldTreeNode, System.Object masterObject) : base(fieldTreeNode, masterObject)
    {}
    
}