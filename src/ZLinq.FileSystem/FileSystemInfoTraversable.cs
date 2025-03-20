using ZLinq.Traversables;

namespace ZLinq;

public static class FileSystemInfoExtensions
{
    public static FileSystemInfoTraverser AsTraverser(this FileSystemInfo origin) => new(origin);

    // type inference helper

    public static ValueEnumerable<Children<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> Children(this FileSystemInfoTraverser traverser) => traverser.Children<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<Children<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> ChildrenAndSelf(this FileSystemInfoTraverser traverser) => traverser.ChildrenAndSelf<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<Descendants<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> Descendants(this FileSystemInfoTraverser traverser) => traverser.Descendants<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<Descendants<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> DescendantsAndSelf(this FileSystemInfoTraverser traverser) => traverser.DescendantsAndSelf<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<Ancestors<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> Ancestors(this FileSystemInfoTraverser traverser) => traverser.Ancestors<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<Ancestors<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> AncestorsAndSelf(this FileSystemInfoTraverser traverser) => traverser.AncestorsAndSelf<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<BeforeSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> BeforeSelf(this FileSystemInfoTraverser traverser) => traverser.BeforeSelf<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<BeforeSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> BeforeSelfAndSelf(this FileSystemInfoTraverser traverser) => traverser.BeforeSelfAndSelf<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<AfterSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> AfterSelf(this FileSystemInfoTraverser traverser) => traverser.AfterSelf<FileSystemInfoTraverser, FileSystemInfo>();
    public static ValueEnumerable<AfterSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> AfterSelfAndSelf(this FileSystemInfoTraverser traverser) => traverser.AfterSelfAndSelf<FileSystemInfoTraverser, FileSystemInfo>();

    // direct shortcut

    public static ValueEnumerable<Children<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> Children(this FileSystemInfo origin) => origin.AsTraverser().Children();
    public static ValueEnumerable<Children<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> ChildrenAndSelf(this FileSystemInfo origin) => origin.AsTraverser().ChildrenAndSelf();
    public static ValueEnumerable<Descendants<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> Descendants(this FileSystemInfo origin) => origin.AsTraverser().Descendants();
    public static ValueEnumerable<Descendants<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> DescendantsAndSelf(this FileSystemInfo origin) => origin.AsTraverser().DescendantsAndSelf();
    public static ValueEnumerable<Ancestors<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> Ancestors(this FileSystemInfo origin) => origin.AsTraverser().Ancestors();
    public static ValueEnumerable<Ancestors<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> AncestorsAndSelf(this FileSystemInfo origin) => origin.AsTraverser().AncestorsAndSelf();
    public static ValueEnumerable<BeforeSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> BeforeSelf(this FileSystemInfo origin) => origin.AsTraverser().BeforeSelf();
    public static ValueEnumerable<BeforeSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> BeforeSelfAndSelf(this FileSystemInfo origin) => origin.AsTraverser().BeforeSelfAndSelf();
    public static ValueEnumerable<AfterSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> AfterSelf(this FileSystemInfo origin) => origin.AsTraverser().AfterSelf();
    public static ValueEnumerable<AfterSelf<FileSystemInfoTraverser, FileSystemInfo>, FileSystemInfo> AfterSelfAndSelf(this FileSystemInfo origin) => origin.AsTraverser().AfterSelfAndSelf();
}

[StructLayout(LayoutKind.Auto)]
public struct FileSystemInfoTraverser(FileSystemInfo origin) : ITraverser<FileSystemInfoTraverser, FileSystemInfo>
{
    IEnumerator<FileSystemInfo>? fileSystemInfoEnumerator; // state for TryGet...

    public FileSystemInfo Origin => origin;

    public FileSystemInfoTraverser ConvertToTraverser(FileSystemInfo next) => new(next);

    // TryGetChildCount and TryGetHasChild is optimize path and allows return false.
    // We should avoid to call EnumerateFileSystemInfos, so return false.

    public bool TryGetChildCount(out int count)
    {
        count = 0;
        return false;
    }

    public bool TryGetHasChild(out bool hasChild)
    {
        hasChild = default;
        return false;
    }

    public bool TryGetParent(out FileSystemInfo parent)
    {
        parent = origin switch
        {
            DirectoryInfo di => di.Parent!,
            FileInfo fi => fi.Directory!,
            _ => null!
        };

        return (parent != null);
    }

    public bool TryGetNextChild(out FileSystemInfo child)
    {
        if (origin is not DirectoryInfo dir)
        {
            child = null!;
            return false;
        }

        fileSystemInfoEnumerator ??= dir.EnumerateFileSystemInfos().GetEnumerator();

        if (fileSystemInfoEnumerator.MoveNext())
        {
            child = fileSystemInfoEnumerator.Current;
            return true;
        }

        child = null!;
        return false;
    }

    public bool TryGetNextSibling(out FileSystemInfo next)
    {
    BEGIN:
        if (fileSystemInfoEnumerator != null)
        {
            if (fileSystemInfoEnumerator.MoveNext())
            {
                next = fileSystemInfoEnumerator.Current;
                return true;
            }
        }
        else if (TryGetParent(out var parent) && parent is DirectoryInfo dir)
        {
            var enumerator = fileSystemInfoEnumerator = dir.EnumerateFileSystemInfos().GetEnumerator();

            var originName = origin.Name;
            while (enumerator.MoveNext())
            {
                if (enumerator.Current.Name == originName)
                {
                    // ok to skip self
                    goto BEGIN;
                }
            }
        }

        next = null!;
        return false;
    }

    public bool TryGetPreviousSibling(out FileSystemInfo previous)
    {
    BEGIN:
        if (fileSystemInfoEnumerator != null)
        {
            if (fileSystemInfoEnumerator.MoveNext())
            {
                previous = fileSystemInfoEnumerator.Current;
                if (previous.Name != origin.Name)
                {
                    return true;
                }
                // else: find self, stop
            }
        }
        else if (TryGetParent(out var parent) && parent is DirectoryInfo dir)
        {
            fileSystemInfoEnumerator = dir.EnumerateFileSystemInfos().GetEnumerator();
            goto BEGIN;
        }

        previous = null!;
        return false;
    }

    public void Dispose()
    {
        if (fileSystemInfoEnumerator != null)
        {
            fileSystemInfoEnumerator.Dispose();
            fileSystemInfoEnumerator = null;
        }
    }
}
