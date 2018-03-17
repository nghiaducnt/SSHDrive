using System;
using System.Collections.Generic;
using System.IO;

using JetBrains.Annotations;
using FubarDev.FtpServer.FileSystem;
using FubarDev.FtpServer.FileSystem.Generic;

namespace DavinciInc.FtpServer.FileSystem.SSH
{
    class SSHFileEntry : IUnixFileEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SSHFileEntry"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this entry belongs to</param>
        /// <param name="info">The <see cref="FileInfo"/> to extract the information from</param>
        public SSHFileEntry([NotNull] SSHFileSystem fileSystem, [NotNull] FileInfo info)
        {
            FileSystem = fileSystem;
            Info = info;
            LastWriteTime = new DateTimeOffset(Info.LastWriteTime);
            CreatedTime = new DateTimeOffset(Info.CreationTimeUtc);
            var accessMode = new GenericAccessMode(true, true, true);
            Permissions = new GenericUnixPermissions(accessMode, accessMode, accessMode);
        }

        /// <summary>
        /// Gets the underlying <see cref="FileInfo"/>
        /// </summary>
        public FileInfo Info { get; }

        /// <inheritdoc/>
        public string Name => Info.Name;

        /// <inheritdoc/>
        public IUnixPermissions Permissions { get; }

        /// <inheritdoc/>
        public DateTimeOffset? LastWriteTime { get; }

        /// <inheritdoc/>
        public DateTimeOffset? CreatedTime { get; }

        /// <inheritdoc/>
        public long NumberOfLinks => 1;

        /// <inheritdoc/>
        public IUnixFileSystem FileSystem { get; }

        /// <inheritdoc/>
        public string Owner => "owner";

        /// <inheritdoc/>
        public string Group => "group";

        /// <inheritdoc/>
        public long Size => Info.Length;
    }
}
