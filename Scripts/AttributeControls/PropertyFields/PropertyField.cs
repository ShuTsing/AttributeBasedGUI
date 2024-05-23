using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace AttributeBasedGUI;
[Tool]
public partial class PropertyField : AttributeControl
{
    [Export]
    public string LabelText
    {
        get => _label.Text;
        set => _label.Text = value;
    }
    
    [Export]
    public float LabelWidthScale
    {
        get => _label.SizeFlagsStretchRatio / DefaultLabelWidthScale;
        set => _label.SizeFlagsStretchRatio = value * DefaultLabelWidthScale;
    }
    
    [Export]
    public bool HideLabel
    {
        get => !_label.Visible;
        set => _label.Visible = !value;
    }
    
    private const float DefaultLabelWidthScale = 0.6f;

    private Label _label;
    private HBoxContainer _entity;
    private readonly FieldInfo _fieldInfo;
    private readonly object _target;
    private readonly List<Button> _inlineButtons = new List<Button>();

    private void CreateInlineButtons()
    {
        if (_fieldInfo == null) return;
        var inlineButtons = _fieldInfo.GetCustomAttributes<InlineButtonAttribute>();
        foreach (var inlineButton in inlineButtons)
        {
            var button = new Button();
            button.Text = inlineButton.Label;
            var classType = _target.GetType();
            var method = classType.GetMethod(inlineButton.Action);
            if (method == null || method.GetParameters().Length > 0)
            {
                GD.PrintErr($"Method {inlineButton.Action} not found in class {classType.Name} or has parameters");
                continue;
            }
            button.Pressed += () => method.Invoke(_target, null);
            _inlineButtons.Add(button);
        }
    }

    private void GetAttributeInfo()
    {
        if (_fieldInfo == null)
        {
            _label.Text = "FieldInfo is null";
            _label.Visible = true;
            _label.SizeFlagsStretchRatio = DefaultLabelWidthScale;
        }
        else
        {
            _label.Text = _fieldInfo.GetCustomAttribute<LabelTextAttribute>()?.Text ?? _fieldInfo.Name;
            _label.Visible = _fieldInfo.GetCustomAttribute<HideLabelAttribute>() == null;
            _label.SizeFlagsStretchRatio = _fieldInfo.GetCustomAttribute<LabelWidthAttribute>()?.Width ?? DefaultLabelWidthScale;
        }
        _label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        CreateInlineButtons();
    }

    public override void _EnterTree()
    {
        base._EnterTree();
        GD.Print("PropertyField _EnterTree");
        AddChild(_label);
        
        AddChild(_entity);

        AddChildControlsToEntity();

        foreach (var button in _inlineButtons)
        {
            _entity.AddChild(button);
        }

    }
    
    public PropertyField(FieldTreeNode fieldTreeNode, System.Object masterObject) : base(fieldTreeNode, masterObject)
    {
    }

    protected virtual void AddChildControlsToEntity() {}
    
    protected virtual void ProcessOnValueChanged() {}
}
