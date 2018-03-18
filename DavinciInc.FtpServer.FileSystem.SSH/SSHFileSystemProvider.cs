using System;
using System.Threading.Tasks;

using JetBrains.Annotations;
using FubarDev.FtpServer.FileSystem;
using System.IO;

namespace DavinciInc.FtpServer.FileSystem.SSH
{
    public class SSHFileSystemProvider : IFileSystemClassFactory
    {
        private readonly string _rootPath;

        private readonly bool _useUserIdAsSubFolder;

        private readonly int _streamBufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="SSHFileSystemProvider"/> class.
        /// </summary>
        /// <param name="rootPath">The root path for all users</param>
        public SSHFileSystemProvider([NotNull] string rootPath)
            : this(rootPath, false)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SSHFileSystemProvider"/> class.
        /// </summary>
        /// <param name="rootPath">The root path for all users</param>
        /// <param name="useUserIdAsSubFolder">Use the user id as subfolder?</param>
        public SSHFileSystemProvider([NotNull] string rootPath, bool useUserIdAsSubFolder)
            : this(rootPath, useUserIdAsSubFolder, SSHFileSystem.DefaultStreamBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SSHFileSystemProvider"/> class.
        /// </summary>
        /// <param name="rootPath">The root path for all users</param>
        /// <param name="useUserIdAsSubFolder">Use the user id as subfolder?</param>
        /// <param name="streamBufferSize">Buffer size to be used in async IO methods</param>
        public SSHFileSystemProvider([NotNull] string rootPath, bool useUserIdAsSubFolder, int streamBufferSize)
        {
            _rootPath = rootPath;
            _useUserIdAsSubFolder = useUserIdAsSubFolder;
            _streamBufferSize = streamBufferSize;
        }

        /// <summary>
        /// Gets or sets a value indicating whether deletion of non-empty directories is allowed.
        /// </summary>
        public bool AllowNonEmptyDirectoryDelete { get; set; }

        /// <inheritdoc/>
        public Task<IUnixFileSystem> Create(string userId, bool isAnonymous)
        {
            var path = _rootPath;
            if (_useUserIdAsSubFolder)
            {
                if (isAnonymous)
                    userId = "anonymous";
                path = Path.Combine(path, userId);
            }

            return Task.FromResult<IUnixFileSystem>(new SSHFileSystem(path, AllowNonEmptyDirectoryDelete, _streamBufferSize));
        }
    }
}
