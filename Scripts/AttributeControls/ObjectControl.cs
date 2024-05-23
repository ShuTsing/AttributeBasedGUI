using Godot;

namespace AttributeBasedGUI.Scripts.AttributeControls;

public partial class ObjectControl : AttributeControl
{
    private ScrollContainer _scrollContainer;
    private MarginContainer _marginContainer;
    private VBoxContainer _vBoxContainer;
    public override void _EnterTree()
    {
        base._EnterTree();
        
        _scrollContainer = new ScrollContainer();
        _scrollContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _scrollContainer.LayoutMode = 1;
        _scrollContainer.AnchorsPreset = (int)LayoutPreset.FullRect;
        AddChild(_scrollContainer);
        
        _marginContainer = new MarginContainer();
        _marginContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        _marginContainer.SizeFlagsVertical = SizeFlags.ExpandFill;
        _marginContainer.AddThemeConstantOverride("margin_left", 4);
        _marginContainer.AddThemeConstantOverride("margin_right", 4);
        _marginContainer.AddThemeConstantOverride("margin_bottom", 4);
        _marginContainer.AddThemeConstantOverride("margin_top", 4);
        _scrollContainer.AddChild(_marginContainer);
        
        _vBoxContainer = new VBoxContainer();
        _vBoxContainer.SizeFlagsHorizontal = SizeFlags.Fill;
        _vBoxContainer.SizeFlagsVertical = SizeFlags.Fill;
        _marginContainer.AddChild(_vBoxContainer);
    }
    
    public ObjectControl(FieldTreeNode fieldTreeNode, System.Object masterObject) : base(fieldTreeNode, masterObject)
    {
    }
}