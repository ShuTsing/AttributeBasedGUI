

using System.Reflection;
using Godot;
namespace AttributeBasedGUI;

public partial class ClassWindowContainer : WindowContainer
{
    [Export]
    public Resource Content { get; set; }
    
    private FieldTreeNode _fieldTree;

    private void BuildTreeByContent()
    {
        if (Content == null)
        {
            return;
        }

        var ownType = typeof(ClassWindowContainer);
        var contentMemberInfo = ownType.GetProperty("Content");
        _fieldTree = new FieldTreeNode(Content.GetType());
        GD.Print(_fieldTree.ToString());
    }
    
    public ClassWindowContainer() : base()
    {
    }
    
    public override void _EnterTree()
    {
        base._EnterTree();
        BuildTreeByContent();
    }
}