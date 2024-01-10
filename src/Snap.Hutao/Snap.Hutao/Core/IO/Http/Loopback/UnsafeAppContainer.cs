using System.Runtime.InteropServices;
using Windows.Win32.Foundation;
using Windows.Win32.NetworkManagement.WindowsFirewall;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct UnsafeAppContainer
{
    private readonly PSID appContainerSid;
    private readonly PSID userSid;
    [MarshalAs(UnmanagedType.LPWStr)]
    private readonly string appContainerName;
    [MarshalAs(UnmanagedType.LPWStr)]
    private readonly string displayName;
    [MarshalAs(UnmanagedType.LPWStr)]
    private readonly string description;
    private readonly INET_FIREWALL_AC_CAPABILITIES capabilities;
    private readonly INET_FIREWALL_AC_BINARIES binaries;
    [MarshalAs(UnmanagedType.LPWStr)]
    private readonly string workingDirectory;
    [MarshalAs(UnmanagedType.LPWStr)]
    private readonly string packageFullName;

    public readonly string AppContainerStringSid
    {
        get
        {
            ConvertSidToStringSid(appContainerSid, out PWSTR stringSid);
            return stringSid.ToString();
        }
    }

    public readonly string AppContainerName
    {
        get => appContainerName;
    }
}
