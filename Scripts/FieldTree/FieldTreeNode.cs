

using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

namespace AttributeBasedGUI;

public class FieldTreeNode
{
    private readonly List<FieldTreeNode> _children = new List<FieldTreeNode>();
    private readonly Dictionary<string, int> _childIndex = new Dictionary<string, int>();
    
    public bool IsLeaf => _children.Count == 0;
    public int ChildCount => _children.Count;
    public FieldTreeNode this[int index] => _children[index];
    
    public MemberInfo MemberInfo { get; protected set; }
    
    //public System.Object MasterObject { get; protected set; }
    
    public FieldTreeNodeType NodeType { get; protected set; }
    
    public FieldTreeNode Parent { get; protected set; }
    
    public string Name { get; protected set; }
    
    public GroupAttribute NodeGroupAttribute
    {
        get => ((int)NodeType & GroupTypeMask) != 0 ? _groupAttribute : null;
        protected set => _groupAttribute = value;
    }
    private GroupAttribute _groupAttribute;
    
    private const int MemberTypeMask = 1 << 8;
    private const int GroupTypeMask = 1 << 9;
    private const int CollectionTypeMask = 1 << 10;
    private const int ObjectTypeMask = 1 << 11;
    private const int MethodTypeMask = 1 << 12;
    
    public enum FieldTreeNodeType
    {
        Unknown = 0,
        
        Integer = 1 | 1 << 8,
        Float = 2 | 1 << 8,
        String = 3 | 1 << 8,
        Boolean = 4 | 1 << 8,
        Enum = 5 | 1 << 8,
        Color = 6 | 1 << 8,
        Vector2 = 7 | 1 << 8,
        Vector3 = 8 | 1 << 8,
        Vector4 = 9 | 1 << 8,
        Vector2I = 10 | 1 << 8,
        Vector3I = 11 | 1 << 8,
        Vector4I = 12 | 1 << 8,
        
        UnknownGroup = 1 | 1 << 9,
        BoxGroup = 2 | 1 << 9,
        TabGroup = 3 | 1 << 9,
        FoldoutGroup = 4 | 1 << 9,
        TitleGroup = 5 | 1 << 9,
        VerticalGroup = 6 | 1 << 9,
        HorizontalGroup = 7 | 1 << 9,
        
        Array = 1 | 1 << 10,
        List = 2 | 1 << 10,
        Dictionary = 3 | 1 << 10,
        HashSet = 4 | 1 << 10,
        
        Object = 1 | 1 << 11,
        
        Method = 1 << 12,
    }

    private FieldTreeNode()
    {
        NodeType = FieldTreeNodeType.Unknown;
    }

    private FieldTreeNodeType NodeTypeByValueType(Type valueType)
    {
        if (valueType == typeof(int) || valueType == typeof(short) || valueType == typeof(long) || valueType == typeof(byte) || valueType == typeof(sbyte) || valueType == typeof(ushort) || valueType == typeof(uint) || valueType == typeof(ulong))
            return FieldTreeNodeType.Integer;
        if (valueType == typeof(float) || valueType == typeof(double))
            return FieldTreeNodeType.Float;
        if (valueType == typeof(string))
            return FieldTreeNodeType.String;
        if (valueType == typeof(bool))
            return FieldTreeNodeType.Boolean;
        if (valueType.IsEnum)
            return FieldTreeNodeType.Enum;
        if (valueType == typeof(Color))
            return FieldTreeNodeType.Color;
        if (valueType == typeof(Vector2))
            return FieldTreeNodeType.Vector2;
        if (valueType == typeof(Vector3))
            return FieldTreeNodeType.Vector3;
        if (valueType == typeof(Vector4))
            return FieldTreeNodeType.Vector4;
        if (valueType == typeof(Vector2I))
            return FieldTreeNodeType.Vector2I;
        if (valueType == typeof(Vector3I))
            return FieldTreeNodeType.Vector3I;
        if (valueType == typeof(Vector4I))
            return FieldTreeNodeType.Vector4I;
        if (valueType.IsArray)
            return FieldTreeNodeType.Array;
        if (valueType.IsGenericType)
        {
            var genericType = valueType.GetGenericTypeDefinition();
            if (genericType == typeof(List<>))
                return FieldTreeNodeType.List;
            if (genericType == typeof(Dictionary<,>))
                return FieldTreeNodeType.Dictionary;
            if (genericType == typeof(HashSet<>))
                return FieldTreeNodeType.HashSet;
        }
        if (valueType.IsClass || valueType.IsValueType && valueType.IsPrimitive == false)
            return FieldTreeNodeType.Object;

        return FieldTreeNodeType.Unknown;
    }

    public FieldTreeNode(MemberInfo memberInfo, FieldTreeNode parent = null)
    {
        if (memberInfo == null)
        {
            return;
        }

        Parent = parent;
        Name = memberInfo.Name;
        //MasterObject = masterObject;
        MemberInfo = memberInfo;
        
        if (memberInfo is MethodInfo methodInfo)
        {
            NodeType = FieldTreeNodeType.Method;
            return;
        }
        
        Type valueType = null;
        if (memberInfo is PropertyInfo propertyInfo)
        {
            valueType = propertyInfo.PropertyType;
        }
        else if (memberInfo is FieldInfo fieldInfo)
        {
            valueType = fieldInfo.FieldType;
        }
        else if (memberInfo is Type type)
        {
            valueType = type;
        }
        if (valueType == null)
        {
            return;
        }
        

        if (parent != null)
        {
            var groups = memberInfo.GetCustomAttributes<GroupAttribute>();
            foreach (var group in groups)
            {
                if (group is BoxGroupAttribute boxGroup)
                {
                    var node = parent.GetOrCreateChildGroups(group.Group);
                    node.NodeType = FieldTreeNodeType.BoxGroup;
                    Parent = MaxDepthNode(Parent, node);
                    break;
                }
            }
            Parent.AddChildNode(this);
        }
        
        NodeType = NodeTypeByValueType(valueType);

        if (NodeType == FieldTreeNodeType.Object)
        {
            var members = AttributeParseUtil.GetShowInInspectorMembers(valueType);
            AttributeParseUtil.SortMembersByPropertyOrder(in members);
            foreach (var member in members)
            {
                var childNode = new FieldTreeNode(member, this);
            }
        }
        else if (NodeType == FieldTreeNodeType.Array)
        {
            var elementType = valueType.GetElementType();
            var childNode = new FieldTreeNode(elementType, this);
        }
        else if (((int)NodeType & CollectionTypeMask) != 0)
        {
            var genericTypes = valueType.GetGenericArguments();
            foreach (var genericType in genericTypes)
            {
                var childNode = new FieldTreeNode(genericType, this);
            }
        }
    }

    private void AddChildNode(FieldTreeNode node)
    {
        _children.Add(node);
        _childIndex[node.Name] = _children.Count - 1;
    }
    
    private FieldTreeNode GetOrCreateChildGroups(string[] subPath)
    {
        if (subPath.Length == 0)
        {
            return null;
        }
        
        bool hasNode = _childIndex.TryGetValue(subPath[0], out var index);
        
        var node = hasNode ? _children[index] : new FieldTreeNode
        {
            Parent = this,
            NodeType = FieldTreeNodeType.UnknownGroup,
            Name = subPath[0]
        };
        if (hasNode == false)
        {
            _children.Add(node);
            _childIndex[subPath[0]] = _children.Count - 1;
        }

        subPath = subPath[1..];
        return subPath.Length == 0 ? node : node.GetOrCreateChildGroups(subPath);
    }
    
    private FieldTreeNode GetOrCreateChildGroups(string subPath)
    {
        var names = subPath.Split('/');
        return GetOrCreateChildGroups(names);
    }
    
    private FieldTreeNode MaxDepthNode(FieldTreeNode a, FieldTreeNode b)
    {
        int depthA = 0;
        int depthB = 0;
        
        var nodeA = a;
        while (nodeA != null)
        {
            depthA++;
            nodeA = nodeA.Parent;
        }
        
        var nodeB = b;
        while (nodeB != null)
        {
            depthB++;
            nodeB = nodeB.Parent;
        }
        
        return depthA > depthB ? a : b;
    }
    
    private void RunToString(FieldTreeNode node, ref string result, string tabStr)
    {
        result += $"{tabStr}{node.Name}: {node.NodeType}\n";
        foreach (var child in node._children)
        {
            RunToString(child, ref result, tabStr + "  ");
        }
    }

    public override string ToString()
    {
        string result = "";
        RunToString(this, ref result, "");
        return result;
    }
}