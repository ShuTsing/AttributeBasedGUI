using System.Collections.Generic;
using System.Reflection;
using System;
using Godot;

[Tool]
public partial class AttributeControl : HBoxContainer
{
    private void Init()
    {
        LayoutMode = 1;
        AnchorsPreset = (int)LayoutPreset.FullRect;
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        Theme = GD.Load<Theme>("res://addons/AttributeBasedGUI/Resources/ControlStyles/Themes/TH_MainTheme.tres");
    }
    

    protected AttributeControl()
    {
        Init();
    }

    protected virtual void UpdateControlValue() {}
}