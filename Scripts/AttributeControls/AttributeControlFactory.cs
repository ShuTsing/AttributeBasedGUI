namespace AttributeBasedGUI;

public static partial class AttributeControlFactory
{
    public static AttributeControl CreateControl(FieldTreeNode fieldTreeNode, System.Object masterObject)
    {
        if (fieldTreeNode.NodeType == FieldTreeNode.FieldTreeNodeType.Object)
        {
            return new ObjectControl(fieldTreeNode, masterObject);
        }
        return new AttributeControl();
    }
}