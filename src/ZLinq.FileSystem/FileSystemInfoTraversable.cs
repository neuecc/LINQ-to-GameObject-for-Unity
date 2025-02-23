using ZLinq.Traversables;

namespace ZLinq;

public static class FileSystemInfoExtensions
{
    public static FileSystemInfoTraversable AsTraversable(this FileSystemInfo origin) => new(origin);

    // type inference helper

    public static ChildrenEnumerable<FileSystemInfoTraversable, FileSystemInfo> Children(this FileSystemInfoTraversable traversable) => traversable.Children<FileSystemInfoTraversable, FileSystemInfo>();
    public static ChildrenEnumerable<FileSystemInfoTraversable, FileSystemInfo> ChildrenAndSelf(this FileSystemInfoTraversable traversable) => traversable.ChildrenAndSelf<FileSystemInfoTraversable, FileSystemInfo>();
    public static DescendantsEnumerable<FileSystemInfoTraversable, FileSystemInfo> Descendants(this FileSystemInfoTraversable traversable) => traversable.Descendants<FileSystemInfoTraversable, FileSystemInfo>();
    public static DescendantsEnumerable<FileSystemInfoTraversable, FileSystemInfo> DescendantsAndSelf(this FileSystemInfoTraversable traversable) => traversable.DescendantsAndSelf<FileSystemInfoTraversable, FileSystemInfo>();
    public static AncestorsEnumerable<FileSystemInfoTraversable, FileSystemInfo> Ancestors(this FileSystemInfoTraversable traversable) => traversable.Ancestors<FileSystemInfoTraversable, FileSystemInfo>();
    public static AncestorsEnumerable<FileSystemInfoTraversable, FileSystemInfo> AncestorsAndSelf(this FileSystemInfoTraversable traversable) => traversable.AncestorsAndSelf<FileSystemInfoTraversable, FileSystemInfo>();
    public static BeforeSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> BeforeSelf(this FileSystemInfoTraversable traversable) => traversable.BeforeSelf<FileSystemInfoTraversable, FileSystemInfo>();
    public static BeforeSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> BeforeSelfAndSelf(this FileSystemInfoTraversable traversable) => traversable.BeforeSelfAndSelf<FileSystemInfoTraversable, FileSystemInfo>();
    public static AfterSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> AfterSelf(this FileSystemInfoTraversable traversable) => traversable.AfterSelf<FileSystemInfoTraversable, FileSystemInfo>();
    public static AfterSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> AfterSelfAndSelf(this FileSystemInfoTraversable traversable) => traversable.AfterSelfAndSelf<FileSystemInfoTraversable, FileSystemInfo>();

    public static StructEnumerator<ChildrenEnumerable<FileSystemInfoTraversable, FileSystemInfo>, FileSystemInfo> GetEnumerator(this ChildrenEnumerable<FileSystemInfoTraversable, FileSystemInfo> source) => new(source);
    public static StructEnumerator<DescendantsEnumerable<FileSystemInfoTraversable, FileSystemInfo>, FileSystemInfo> GetEnumerator(this DescendantsEnumerable<FileSystemInfoTraversable, FileSystemInfo> source) => new(source);
    public static StructEnumerator<AncestorsEnumerable<FileSystemInfoTraversable, FileSystemInfo>, FileSystemInfo> GetEnumerator(this AncestorsEnumerable<FileSystemInfoTraversable, FileSystemInfo> source) => new(source);
    public static StructEnumerator<BeforeSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo>, FileSystemInfo> GetEnumerator(this BeforeSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> source) => new(source);
    public static StructEnumerator<AfterSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo>, FileSystemInfo> GetEnumerator(this AfterSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> source) => new(source);

    // direct shortcut

    public static ChildrenEnumerable<FileSystemInfoTraversable, FileSystemInfo> Children(this FileSystemInfo origin) => origin.AsTraversable().Children();
    public static ChildrenEnumerable<FileSystemInfoTraversable, FileSystemInfo> ChildrenAndSelf(this FileSystemInfo origin) => origin.AsTraversable().ChildrenAndSelf();
    public static DescendantsEnumerable<FileSystemInfoTraversable, FileSystemInfo> Descendants(this FileSystemInfo origin) => origin.AsTraversable().Descendants();
    public static DescendantsEnumerable<FileSystemInfoTraversable, FileSystemInfo> DescendantsAndSelf(this FileSystemInfo origin) => origin.AsTraversable().DescendantsAndSelf();
    public static AncestorsEnumerable<FileSystemInfoTraversable, FileSystemInfo> Ancestors(this FileSystemInfo origin) => origin.AsTraversable().Ancestors();
    public static AncestorsEnumerable<FileSystemInfoTraversable, FileSystemInfo> AncestorsAndSelf(this FileSystemInfo origin) => origin.AsTraversable().AncestorsAndSelf();
    public static BeforeSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> BeforeSelf(this FileSystemInfo origin) => origin.AsTraversable().BeforeSelf();
    public static BeforeSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> BeforeSelfAndSelf(this FileSystemInfo origin) => origin.AsTraversable().BeforeSelfAndSelf();
    public static AfterSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> AfterSelf(this FileSystemInfo origin) => origin.AsTraversable().AfterSelf();
    public static AfterSelfEnumerable<FileSystemInfoTraversable, FileSystemInfo> AfterSelfAndSelf(this FileSystemInfo origin) => origin.AsTraversable().AfterSelfAndSelf();

    // helper for OfType

    // TODO:...OfFileInfo(), OfDirectoryInfo
}

[StructLayout(LayoutKind.Auto)]
public struct FileSystemInfoTraversable(FileSystemInfo origin) : ITraversable<FileSystemInfoTraversable, FileSystemInfo>
{
    IEnumerator<FileSystemInfo>? fileSystemInfoEnumerator; // state for TryGet...

    public FileSystemInfo Origin => origin;

    public FileSystemInfoTraversable ConvertToTraversable(FileSystemInfo next) => new(next);

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
