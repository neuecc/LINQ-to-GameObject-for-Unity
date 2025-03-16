using Godot;
using ZLinq.Traversables;

namespace ZLinq;

public static class NodeExtensions
{
    public static NodeTraversable AsTraversable(this Node origin) => new(origin);

    // type inference helper
    public static ChildrenEnumerable<NodeTraversable, Node> Children(this NodeTraversable traversable) => traversable.Children<NodeTraversable, Node>();
    public static ChildrenEnumerable<NodeTraversable, Node> ChildrenAndSelf(this NodeTraversable traversable) => traversable.ChildrenAndSelf<NodeTraversable, Node>();
    public static DescendantsEnumerable<NodeTraversable, Node> Descendants(this NodeTraversable traversable) => traversable.Descendants<NodeTraversable, Node>();
    public static DescendantsEnumerable<NodeTraversable, Node> DescendantsAndSelf(this NodeTraversable traversable) => traversable.DescendantsAndSelf<NodeTraversable, Node>();
    public static AncestorsEnumerable<NodeTraversable, Node> Ancestors(this NodeTraversable traversable) => traversable.Ancestors<NodeTraversable, Node>();
    public static AncestorsEnumerable<NodeTraversable, Node> AncestorsAndSelf(this NodeTraversable traversable) => traversable.AncestorsAndSelf<NodeTraversable, Node>();
    public static BeforeSelfEnumerable<NodeTraversable, Node> BeforeSelf(this NodeTraversable traversable) => traversable.BeforeSelf<NodeTraversable, Node>();
    public static BeforeSelfEnumerable<NodeTraversable, Node> BeforeSelfAndSelf(this NodeTraversable traversable) => traversable.BeforeSelfAndSelf<NodeTraversable, Node>();
    public static AfterSelfEnumerable<NodeTraversable, Node> AfterSelf(this NodeTraversable traversable) => traversable.AfterSelf<NodeTraversable, Node>();
    public static AfterSelfEnumerable<NodeTraversable, Node> AfterSelfAndSelf(this NodeTraversable traversable) => traversable.AfterSelfAndSelf<NodeTraversable, Node>();

    public static ValueEnumerator<ChildrenEnumerable<NodeTraversable, Node>, Node> GetEnumerator(this ChildrenEnumerable<NodeTraversable, Node> source) => new(source);
    public static ValueEnumerator<DescendantsEnumerable<NodeTraversable, Node>, Node> GetEnumerator(this DescendantsEnumerable<NodeTraversable, Node> source) => new(source);
    public static ValueEnumerator<AncestorsEnumerable<NodeTraversable, Node>, Node> GetEnumerator(this AncestorsEnumerable<NodeTraversable, Node> source) => new(source);
    public static ValueEnumerator<BeforeSelfEnumerable<NodeTraversable, Node>, Node> GetEnumerator(this BeforeSelfEnumerable<NodeTraversable, Node> source) => new(source);
    public static ValueEnumerator<AfterSelfEnumerable<NodeTraversable, Node>, Node> GetEnumerator(this AfterSelfEnumerable<NodeTraversable, Node> source) => new(source);

    // direct shortcut
    public static ChildrenEnumerable<NodeTraversable, Node> Children(this Node origin) => origin.AsTraversable().Children();
    public static ChildrenEnumerable<NodeTraversable, Node> ChildrenAndSelf(this Node origin) => origin.AsTraversable().ChildrenAndSelf();
    public static DescendantsEnumerable<NodeTraversable, Node> Descendants(this Node origin) => origin.AsTraversable().Descendants();
    public static DescendantsEnumerable<NodeTraversable, Node> DescendantsAndSelf(this Node origin) => origin.AsTraversable().DescendantsAndSelf();
    public static AncestorsEnumerable<NodeTraversable, Node> Ancestors(this Node origin) => origin.AsTraversable().Ancestors();
    public static AncestorsEnumerable<NodeTraversable, Node> AncestorsAndSelf(this Node origin) => origin.AsTraversable().AncestorsAndSelf();
    public static BeforeSelfEnumerable<NodeTraversable, Node> BeforeSelf(this Node origin) => origin.AsTraversable().BeforeSelf();
    public static BeforeSelfEnumerable<NodeTraversable, Node> BeforeSelfAndSelf(this Node origin) => origin.AsTraversable().BeforeSelfAndSelf();
    public static AfterSelfEnumerable<NodeTraversable, Node> AfterSelf(this Node origin) => origin.AsTraversable().AfterSelf();
    public static AfterSelfEnumerable<NodeTraversable, Node> AfterSelfAndSelf(this Node origin) => origin.AsTraversable().AfterSelfAndSelf();
}
