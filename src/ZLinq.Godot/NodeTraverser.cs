using Godot;

namespace ZLinq;

[StructLayout(LayoutKind.Auto)]
public struct NodeTraverser : ITraverser<NodeTraverser, Node>
{
    static readonly object CalledTryGetNextChild = new object();
    static readonly object ParentNotFound = new object();

    readonly Node node;
    object? initializedState; // CalledTryGetNext or Parent(for sibling operations)
    int childCount; // self childCount(TryGetNextChild) or parent childCount(TryGetSibling)
    int index;

    public NodeTraverser(Node origin)
    {
        this.node = origin;
        this.initializedState = null;
        this.childCount = 0;
        this.index = 0;
    }

    public Node Origin => node;
    public NodeTraverser ConvertToTraverser(Node next) => new(next);

    public bool TryGetParent(out Node parent)
    {
        var maybeParent = node.GetParent();
        if (maybeParent is null)
        {
            parent = default!;
            return false;
        }

        parent = maybeParent;
        return true;
    }

    public bool TryGetChildCount(out int count)
    {
        count = node.GetChildCount();
        return true;
    }

    public bool TryGetHasChild(out bool hasChild)
    {
        hasChild = node.GetChildCount() != 0;
        return true;
    }

    public bool TryGetNextChild(out Node child)
    {
        if (initializedState is null)
        {
            initializedState = CalledTryGetNextChild;
            childCount = node.GetChildCount();
        }

        if (index < childCount)
        {
            child = node.GetChild(index++);
            return true;
        }

        child = default!;
        return false;
    }

    public bool TryGetNextSibling(out Node next)
    {
        if (initializedState is null)
        {
            var maybeParent = node.GetParent();
            if (maybeParent is null)
            {
                initializedState = ParentNotFound;
                next = default!;
                return false;
            }

            // cache parent and childCount
            initializedState = maybeParent;
            childCount = maybeParent.GetChildCount(); // parent's childCount
            index = node.GetIndex() + 1;
        }
        else if (initializedState == ParentNotFound)
        {
            next = default!;
            return false;
        }

        var parent = (Node)initializedState;
        if (index < childCount)
        {
            next = parent.GetChild(index++);
            return true;
        }

        next = default!;
        return false;
    }

    public bool TryGetPreviousSibling(out Node previous)
    {
        if (initializedState is null)
        {
            var maybeParent = node.GetParent();
            if (maybeParent is null)
            {
                initializedState = ParentNotFound;
                previous = default!;
                return false;
            }

            initializedState = maybeParent;
            childCount = node.GetIndex(); // not childCount but means `to`
            index = 0; // 0 to siblingIndex
        }
        else if (initializedState == ParentNotFound)
        {
            previous = default!;
            return false;
        }

        var parent = (Node)initializedState;
        if (index < childCount)
        {
            previous = parent.GetChild(index++);
            return true;
        }

        previous = default!;
        return false;
    }

    public void Dispose() { }
}
