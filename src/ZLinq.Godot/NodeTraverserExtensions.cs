using Godot;
using ZLinq.Traversables;

namespace ZLinq;

public static class NodeTraverserExtensions
{
    public static NodeTraverser AsTraversable(this Node origin) => new(origin);

    // type inference helper
    public static ValueEnumerable<Children<NodeTraverser, Node>, Node> Children(this NodeTraverser traverser) => traverser.Children<NodeTraverser, Node>();
    public static ValueEnumerable<Children<NodeTraverser, Node>, Node> ChildrenAndSelf(this NodeTraverser traverser) => traverser.ChildrenAndSelf<NodeTraverser, Node>();
    public static ValueEnumerable<Descendants<NodeTraverser, Node>, Node> Descendants(this NodeTraverser traverser) => traverser.Descendants<NodeTraverser, Node>();
    public static ValueEnumerable<Descendants<NodeTraverser, Node>, Node> DescendantsAndSelf(this NodeTraverser traverser) => traverser.DescendantsAndSelf<NodeTraverser, Node>();
    public static ValueEnumerable<Ancestors<NodeTraverser, Node>, Node> Ancestors(this NodeTraverser traverser) => traverser.Ancestors<NodeTraverser, Node>();
    public static ValueEnumerable<Ancestors<NodeTraverser, Node>, Node> AncestorsAndSelf(this NodeTraverser traverser) => traverser.AncestorsAndSelf<NodeTraverser, Node>();
    public static ValueEnumerable<BeforeSelf<NodeTraverser, Node>, Node> BeforeSelf(this NodeTraverser traverser) => traverser.BeforeSelf<NodeTraverser, Node>();
    public static ValueEnumerable<BeforeSelf<NodeTraverser, Node>, Node> BeforeSelfAndSelf(this NodeTraverser traverser) => traverser.BeforeSelfAndSelf<NodeTraverser, Node>();
    public static ValueEnumerable<AfterSelf<NodeTraverser, Node>, Node> AfterSelf(this NodeTraverser traverser) => traverser.AfterSelf<NodeTraverser, Node>();
    public static ValueEnumerable<AfterSelf<NodeTraverser, Node>, Node> AfterSelfAndSelf(this NodeTraverser traverser) => traverser.AfterSelfAndSelf<NodeTraverser, Node>();

    // direct shortcut
    public static ValueEnumerable<Children<NodeTraverser, Node>, Node> Children(this Node origin) => origin.AsTraversable().Children();
    public static ValueEnumerable<Children<NodeTraverser, Node>, Node> ChildrenAndSelf(this Node origin) => origin.AsTraversable().ChildrenAndSelf();
    public static ValueEnumerable<Descendants<NodeTraverser, Node>, Node> Descendants(this Node origin) => origin.AsTraversable().Descendants();
    public static ValueEnumerable<Descendants<NodeTraverser, Node>, Node> DescendantsAndSelf(this Node origin) => origin.AsTraversable().DescendantsAndSelf();
    public static ValueEnumerable<Ancestors<NodeTraverser, Node>, Node> Ancestors(this Node origin) => origin.AsTraversable().Ancestors();
    public static ValueEnumerable<Ancestors<NodeTraverser, Node>, Node> AncestorsAndSelf(this Node origin) => origin.AsTraversable().AncestorsAndSelf();
    public static ValueEnumerable<BeforeSelf<NodeTraverser, Node>, Node> BeforeSelf(this Node origin) => origin.AsTraversable().BeforeSelf();
    public static ValueEnumerable<BeforeSelf<NodeTraverser, Node>, Node> BeforeSelfAndSelf(this Node origin) => origin.AsTraversable().BeforeSelfAndSelf();
    public static ValueEnumerable<AfterSelf<NodeTraverser, Node>, Node> AfterSelf(this Node origin) => origin.AsTraversable().AfterSelf();
    public static ValueEnumerable<AfterSelf<NodeTraverser, Node>, Node> AfterSelfAndSelf(this Node origin) => origin.AsTraversable().AfterSelfAndSelf();
}
