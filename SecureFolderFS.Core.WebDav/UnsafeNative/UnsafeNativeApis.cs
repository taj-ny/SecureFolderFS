﻿using System.Runtime.InteropServices;
using System.Text;

namespace SecureFolderFS.Core.WebDav.UnsafeNative
{
    internal static class UnsafeNativeApis
    {
        public const int CONNECT_TEMPORARY = 4;
        public const int RESOURCETYPE_DISK = 1;

        [DllImport("Mpr.dll")]
        public static extern int WNetAddConnection2(
            [In] NETRESOURCE lpNetResource,
            [In] string lpPassword,
            [In] string lpUserName,
            [In] uint dwFlags);

        [DllImport("Mpr.dll")]
        public static extern int WNetCancelConnection2(
            [In] string lpName,
            [In] uint dwFlags,
            [In] bool fForce);

        [DllImport("Mpr.dll")]
        public static extern int WNetGetConnection(
            [In] string lpLocalName,
            [Out] StringBuilder lpRemoteName,
            [In, Out] ref int lpnLength);
    }

    [StructLayout(LayoutKind.Sequential)]
    public class NETRESOURCE
    {
        public uint dwScope;
        public uint dwType;
        public uint dwDisplayType;
        public uint dwUsage;
        public string lpLocalName = null!;
        public string lpRemoteName = null!;
        public string lpComment = null!;
        public string lpProvider = null!;
    }
}
