using System.Diagnostics;

namespace Coworkee.Client.Utils;

public static class Debug
{
    public static bool IsDebug()
    {
        #if DEBUG
                return true;
        #else
              return false;
        #endif
    }

    public static bool DebuggerIsAttached()
    {
        return Debugger.IsAttached;
    }

}