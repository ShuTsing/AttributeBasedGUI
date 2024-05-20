using Godot;
using System;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class ShowInInspectorAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class HideInInspectorAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class PropertyOrderAttribute : Attribute
{
    public int Order { get; private set; }

    public PropertyOrderAttribute(int order)
    {
        Order = order;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class GroupAttribute : Attribute
{
    public string Group { get; private set; }
    public string LabelText { get; private set; }
    public bool ShowLabel { get; private set; }
    public bool CenterLabel { get; private set; }
    
    public GroupAttribute(string group, bool showLabel = true, bool centerLabel = false)
    {
        Group = group;
        ShowLabel = showLabel;
        CenterLabel = centerLabel;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
public class BoxGroupAttribute : GroupAttribute
{
    public BoxGroupAttribute(string labelText, bool showLabel = true, bool centerLabel = false) : base(labelText, showLabel, centerLabel)
    {
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class HideLabelAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class LabelTextAttribute : Attribute
{
    public string Text { get; private set; }

    public LabelTextAttribute(string text)
    {
        Text = text;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class LabelWidthAttribute : Attribute
{
    public float Width { get; private set; }

    public LabelWidthAttribute(float width)
    {
        Width = width;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class OnValueChanged : Attribute
{
    public string Action { get; private set; }

    public OnValueChanged(string action)
    {
        Action = action;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class InlineButtonAttribute : Attribute
{
    public string Action { get; private set; }
    public string Label { get; private set; }
    
    public InlineButtonAttribute(string action, string label)
    {
        Action = action;
        Label = label;
    }
    
    public InlineButtonAttribute(string action)
    {
        Action = action;
        Label = action;
    }
}

[AttributeUsage(AttributeTargets.Field)]
public class ReadOnlyAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class FilePathAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class FolderPathAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class ValueDropdownAttribute : Attribute
{
    public string ValuesGetter { get; private set; }
    
    public ValueDropdownAttribute(string valuesGetter)
    {
        ValuesGetter = valuesGetter;
    }
}

[AttributeUsage(AttributeTargets.Method)]
public class ButtonAttribute : Attribute
{
    public string Name { get; private set; }
    
    public ButtonAttribute(string name)
    {
        Name = name;
    }
    
    public ButtonAttribute()
    {
        Name = null;
    }
}

