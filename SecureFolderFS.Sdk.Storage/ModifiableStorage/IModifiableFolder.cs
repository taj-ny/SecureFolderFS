﻿using SecureFolderFS.Sdk.Storage.NestedStorage;
using System.Threading;
using System.Threading.Tasks;

namespace SecureFolderFS.Sdk.Storage.ModifiableStorage
{
    /// <summary>
    /// Represents a folder that can be modified.
    /// </summary>
    public interface IModifiableFolder : IFolder, IModifiableStorable
    {
        /// <summary>
        /// Deletes the provided storable item from this folder.
        /// </summary>
        Task DeleteAsync(INestedStorable item, bool permanently = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new file with the desired name inside this folder.
        /// </summary>
        Task<INestedFile> CreateFileAsync(string desiredName, bool overwrite = default, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new folder with the desired name inside this folder.
        /// </summary>
        Task<INestedFolder> CreateFolderAsync(string desiredName, bool overwrite = default, CancellationToken cancellationToken = default);
    }
}
