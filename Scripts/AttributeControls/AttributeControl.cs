using System.Collections.Generic;
using System.Reflection;
using System;
using Godot;

namespace AttributeBasedGUI;
[Tool]
public partial class AttributeControl : HBoxContainer
{
    public string ThemePath { get; set; } = "res://addons/AttributeBasedGUI/Resources/ControlStyles/Themes/TH_MainTheme.tres";
    public Theme ControlTheme => _theme ??= GD.Load<Theme>(ThemePath);
    private Theme _theme;
    
    protected FieldTreeNode FieldTreeNode;
    protected System.Object MasterObject;
    
    private void Init()
    {
        LayoutMode = 1;
        AnchorsPreset = (int)LayoutPreset.FullRect;
        SizeFlagsHorizontal = SizeFlags.ExpandFill;
        Theme = ControlTheme;
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        Init();
    }
    
    // Set all children theme
    private void SetChildrenTheme(Control control)
    {
        foreach (Node child in control.GetChildren())
        {
            if (child is Control childControl)
            {
                childControl.Theme = ControlTheme;
                SetChildrenTheme(childControl);
            }
        }
    }
    
    public override void _Ready()
    {
        base._Ready();
        SetChildrenTheme(this);
    }
    
    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateControlValue();
    }


    protected AttributeControl(FieldTreeNode fieldTreeNode, System.Object masterObject)
    {
        FieldTreeNode = fieldTreeNode;
        MasterObject = masterObject;
    }


    protected virtual void UpdateControlValue() {}
}