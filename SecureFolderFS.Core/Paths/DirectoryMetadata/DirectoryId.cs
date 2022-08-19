﻿using System;
using System.IO;
using System.Linq;

namespace SecureFolderFS.Core.Paths.DirectoryMetadata
{
    internal sealed class DirectoryId : IEquatable<DirectoryId>
    {
        public byte[] Id { get; }

        private DirectoryId(byte[] id)
        {
            Id = id;
        }

        public static DirectoryId CreateNew()
        {
            return new DirectoryId(Guid.NewGuid().ToByteArray());
        }

        public static DirectoryId GetEmpty()
        {
            return new DirectoryId(Array.Empty<byte>());
        }

        public static DirectoryId FromFileStream(Stream stream)
        {
            var buffer = new byte[Constants.IO.DIRECTORY_ID_MAX_SIZE];
            stream.Read(buffer, 0, buffer.Length);

            return new DirectoryId(buffer);
        }

        public bool Equals(DirectoryId other)
        {
            if (other is null)
                return false;

            return other.Id.SequenceEqual(Id);
        }
    }
}
