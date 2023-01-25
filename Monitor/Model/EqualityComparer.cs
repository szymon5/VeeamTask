using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Monitor.Model
{

    public class EqualityComparer : IEqualityComparer<Process>
    {
        public bool Equals(Process? x, Process? y)
        {
            if (x.MainWindowTitle.Equals(y.MainWindowTitle)) return true;
            else return false;
        }

        public int GetHashCode([DisallowNull] Process obj)
        {
            return 0;
        }
    }
}
