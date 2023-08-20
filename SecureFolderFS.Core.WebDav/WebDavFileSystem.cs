using SecureFolderFS.Core.FileSystem;
using SecureFolderFS.Core.FileSystem.Enums;
using SecureFolderFS.Core.FileSystem.Helpers;
using SecureFolderFS.Core.WebDav.UnsafeNative;
using SecureFolderFS.Sdk.Storage;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace SecureFolderFS.Core.WebDav
{
    /// <inheritdoc cref="IVirtualFileSystem"/>
    internal sealed class WebDavFileSystem : IVirtualFileSystem
    {
        private readonly WebDavWrapper _webDavWrapper;

        /// <inheritdoc/>
        public IFolder RootFolder { get; }

        /// <inheritdoc/>
        public bool IsOperational { get; private set; }

        public WebDavFileSystem(IFolder rootFolder, WebDavWrapper webDavWrapper)
        {
            _webDavWrapper = webDavWrapper;

            RootFolder = rootFolder;
            IsOperational = true;
        }

        /// <inheritdoc/>
        public async Task<bool> CloseAsync(FileSystemCloseMethod closeMethod)
        {
            if (IsOperational)
            {
                var closeResult = await Task.Run(() => _webDavWrapper.CloseFileSystem(closeMethod));
                IsOperational = !closeResult;

                if (closeResult && OperatingSystem.IsWindows()) // Closed successfully
                {
                    // Close all file explorer windows
                    await CloseExplorerShellAsync(RootFolder.Id);
                }
            }

            return !IsOperational;
        }

        /// <inheritdoc/>
        public async ValueTask DisposeAsync()
        {
            _ = await CloseAsync(FileSystemCloseMethod.CloseForcefully);
        }

        private static async Task CloseExplorerShellAsync(string path)
        {
         
        }
    }
}
