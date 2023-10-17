// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.


using System;
using System.Runtime.InteropServices;
using Windows.UI.Core;
using Windows.Win32.Foundation;

namespace Snap.Hutao.Win32;

internal static class UnsafePInvoke
{
    private enum WINDOW_TYPE : uint
    {
        IMMERSIVE_BODY,
        IMMERSIVE_DOCK,
        IMMERSIVE_HOSTED,
        IMMERSIVE_TEST,
        IMMERSIVE_BODY_ACTIVE,
        IMMERSIVE_DOCK_ACTIVE,
        NOT_IMMERSIVE,
    }

    [DllImport("Windows.UI.dll", CharSet = CharSet.None, EntryPoint = "#1500", ExactSpelling = false, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    private static extern HRESULT PrivateCreateCoreWindow(WINDOW_TYPE WindowType, PWSTR pWindowTitle, int x, int y, uint uWidth, uint uHeight, uint dwAttributes, HWND hOwnerWindow, Guid riid, out nint ppv);

    public static unsafe CoreWindow PrivateCreateCoreWindow(string title, HWND hOwnerWindow)
    {
        fixed(char* pTitle = title)
        {
            PrivateCreateCoreWindow(WINDOW_TYPE.NOT_IMMERSIVE, pTitle, 0, 0, 400, 400, 0, hOwnerWindow, typeof(ICoreWindow).GUID, out nint thisPtr);
            return CoreWindow.FromAbi(thisPtr);
        }
    }
}