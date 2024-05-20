

using System.Reflection;
using Godot;
namespace AttributeBasedGUI;

public class ClassWindowContainer : WindowContainer
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
        _fieldTree = new FieldTreeNode(contentMemberInfo);
    }
    
    public ClassWindowContainer()
    {
        BuildTreeByContent();
    }
}