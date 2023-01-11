/* This file is part of SilverBot.
SilverBot is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
SilverBot is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
You should have received a copy of the GNU General Public License along with SilverBot. If not, see <https://www.gnu.org/licenses/>.
*/
using SilverConfig;
using System.ComponentModel;
using System.Diagnostics;

namespace SilverBot.Shared;

public interface ICanBeToldThatAPartOfMeIsChanged
{
    public void PropertyChanged(object sender, PropertyChangedEventArgs e);
    public bool AllowedToRead { get; }
}
public class CommentXmlConfigReaderNotifyWhenChanged<T> : CommentXmlConfigReader<T>, IDisposable
    where T : INotifyPropertyChanged, ICanBeToldThatAPartOfMeIsChanged
{
    private readonly List<FileSystemWatcher> fileSystemWatchers = new();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            foreach (var fsw in fileSystemWatchers) fsw.Dispose();
        }
    }
    public override T? Read(string path)
    {
        var c = base.Read(path);
        var fp = Path.GetFullPath(path);
        var fpdir = Path.GetDirectoryName(fp)??"";
        var fpnm = Path.GetFileName(fp);

        FileSystemWatcher j = new()
        {
            Path = fpdir,
            EnableRaisingEvents = true,
            Filter = fpnm,
            NotifyFilter = NotifyFilters.CreationTime
                           | NotifyFilters.DirectoryName
                           | NotifyFilters.LastWrite
                           | NotifyFilters.Size
        };
        j.Changed += (x, y) =>
        {
            if (y.ChangeType != WatcherChangeTypes.Changed) return;
            if (y.FullPath != fp || !c.AllowedToRead)
            {
                return;
            }

            var c2 = base.Read(path);
            var t = typeof(T);
            foreach (var a in t.GetProperties())
                if (a.Name != "AllowedToRead" && a.CanRead && a.GetValue(c)?.Equals(a.GetValue(c2)) != true)
                {
                    if (a.CanWrite)
                        a.SetValue(c, a.GetValue(c2));
                    else
                        Debug.WriteLine("CommentXmlConfigReaderNotifyWhenChanged had an issue setting c." + a.Name);
                    c?.PropertyChanged(this, new PropertyChangedEventArgs(a.Name));
                }
        };
        fileSystemWatchers.Add(j);
        return c;
    }
}