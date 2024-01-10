// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.InteropServices;
using Windows.Win32.NetworkManagement.WindowsFirewall;
using Windows.Win32.Security;
using static Windows.Win32.PInvoke;

namespace Snap.Hutao.Core.IO.Http.Loopback;

[Injection(InjectAs.Singleton)]
internal sealed class LoopbackManager : ObservableObject, IDisposable
{
    private static IntPtr appContainerSids;

    private readonly RuntimeOptions runtimeOptions;
    private readonly ITaskContext taskContext;

    private readonly List<UnsafeSidAndAttributes> appContainerConfigs;
    private readonly UnsafeAppContainer hutaoAppContainer;
    private UnsafeSidAndAttributes hutaoAppContainerConfig;

    public LoopbackManager(IServiceProvider serviceProvider)
    {
        runtimeOptions = serviceProvider.GetRequiredService<RuntimeOptions>();
        taskContext = serviceProvider.GetRequiredService<ITaskContext>();

        List<UnsafeAppContainer> appContainers = UnsafeNetworkIsolationEnumAppContainers();
        appContainerConfigs = UnsafeNetworkIsolationGetAppContainerConfig();

        hutaoAppContainer = appContainers.Single(container => container.AppContainerName == runtimeOptions.FamilyName);
        hutaoAppContainerConfig = appContainerConfigs.SingleOrDefault(config => config.StringSid == hutaoAppContainer.AppContainerStringSid);
        IsLoopbackEnabled = hutaoAppContainerConfig != default;
    }

    public bool IsLoopbackEnabled { get; private set; }

    public async ValueTask EnableLoopbackAsync()
    {
        hutaoAppContainerConfig = UnsafeSidAndAttributes.Create(hutaoAppContainer.AppContainerStringSid, 0);
        appContainerConfigs.Add(hutaoAppContainerConfig);
        NetworkIsolationSetAppContainerConfig(appContainerConfigs.ConvertAll(config => (SID_AND_ATTRIBUTES)config).ToArray());

        IsLoopbackEnabled = true;

        await taskContext.SwitchToMainThreadAsync();
        OnPropertyChanged(nameof(IsLoopbackEnabled));
    }

    public void Dispose()
    {
        UnsafeNetworkIsolationFreeAppContainers(appContainerSids);
    }

    private static unsafe List<UnsafeSidAndAttributes> UnsafeNetworkIsolationGetAppContainerConfig()
    {
        List<UnsafeSidAndAttributes> list = [];

        NetworkIsolationGetAppContainerConfig(out uint count, out SID_AND_ATTRIBUTES* sids);

        int structSize = Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES));
        for (uint i = 0; i < count; i++)
        {
            list.Add(Marshal.PtrToStructure<UnsafeSidAndAttributes>((IntPtr)((IntPtr)sids + (i * structSize))));
        }

        return list;
    }

    private static unsafe List<UnsafeAppContainer> UnsafeNetworkIsolationEnumAppContainers()
    {
        List<UnsafeAppContainer> list = [];

        NetworkIsolationEnumAppContainers((uint)NETISO_FLAG.NETISO_FLAG_MAX, out uint count, out INET_FIREWALL_APP_CONTAINER* containers);
        appContainerSids = (IntPtr)containers;

        int structSize = Marshal.SizeOf(typeof(INET_FIREWALL_APP_CONTAINER));
        for (uint i = 0; i < count; i++)
        {
            list.Add(Marshal.PtrToStructure<UnsafeAppContainer>((IntPtr)((IntPtr)containers + (i * structSize))));
        }

        return list;
    }

    private static unsafe void UnsafeNetworkIsolationFreeAppContainers(IntPtr appContainerSids)
    {
        _ = NetworkIsolationFreeAppContainers((INET_FIREWALL_APP_CONTAINER*)appContainerSids);
    }
}
