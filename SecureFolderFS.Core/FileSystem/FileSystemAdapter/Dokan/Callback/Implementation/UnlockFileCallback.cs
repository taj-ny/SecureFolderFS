﻿using DokanNet;
using System.IO;
using SecureFolderFS.Core.FileSystem.OpenHandles;

namespace SecureFolderFS.Core.FileSystem.FileSystemAdapter.Dokan.Callback.Implementation
{
    internal sealed class UnlockFileCallback : BaseDokanOperationsCallback, IUnlockFileCallback
    {
        public UnlockFileCallback(HandlesCollection handles)
            : base(handles)
        {
        }

        public NtStatus UnlockFile(string fileName, long offset, long length, IDokanFileInfo info)
        {
            try
            {
                if (IsContextInvalid(info))
                {
                    return DokanResult.InvalidHandle;
                }
                else if (info.IsDirectory)
                {
                    return DokanResult.AccessDenied;
                }

                if (handles.GetHandle(GetContextValue(info)) is FileHandle fileHandle)
                {
                    fileHandle.CleartextFileStream.Unlock(offset, length);
                    return DokanResult.Success;
                }
                else
                {
                    return DokanResult.InvalidHandle;
                }
            }
            catch (IOException)
            {
                return DokanResult.AccessDenied;
            }
        }
    }
}
