﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using FubarDev.FtpServer.FileSystem;

namespace DavinciInc.FtpServer.FileSystem.SSH
{
    /// <summary>
    /// A <see cref="IUnixFileSystem"/> implementation that uses the
    /// standard .NET functionality to access the file system.
    /// </summary>
    public class SSHFileSystem : IUnixFileSystem
    {
        public static readonly int DefaultStreamBufferSize = 4096;
        private bool _disposedValue;

        private int _streamBufferSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystem"/> class.
        /// </summary>
        /// <param name="rootPath">The path to use as root</param>
        /// <param name="allowNonEmptyDirectoryDelete">Allow deletion of non-empty directories?</param>
        public SSHFileSystem(string rootPath, bool allowNonEmptyDirectoryDelete)
            : this(rootPath, allowNonEmptyDirectoryDelete, DefaultStreamBufferSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DotNetFileSystem"/> class.
        /// </summary>
        /// <param name="rootPath">The path to use as root</param>
        /// <param name="allowNonEmptyDirectoryDelete">Allow deletion of non-empty directories?</param>
        /// <param name="streamBufferSize">Buffer size to be used in async IO methods</param>
        public SSHFileSystem(string rootPath, bool allowNonEmptyDirectoryDelete, int streamBufferSize)
        {
          
        }

        /// <inheritdoc/>
        public bool SupportsNonEmptyDirectoryDelete { get; }

        /// <inheritdoc/>
        public StringComparer FileSystemEntryComparer { get; }

        /// <inheritdoc/>
        public IUnixDirectoryEntry Root { get; }

        /// <inheritdoc/>
        public bool SupportsAppend => true;

        /// <inheritdoc/>
        public Task<IReadOnlyList<IUnixFileSystemEntry>> GetEntriesAsync(IUnixDirectoryEntry directoryEntry, CancellationToken cancellationToken)
        {
            return null;
        }

        /// <inheritdoc/>
        public Task<IUnixFileSystemEntry> GetEntryByNameAsync(IUnixDirectoryEntry directoryEntry, string name, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <inheritdoc/>
        public Task<IUnixFileSystemEntry> MoveAsync(IUnixDirectoryEntry parent, IUnixFileSystemEntry source, IUnixDirectoryEntry target, string fileName, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <inheritdoc/>
        public Task UnlinkAsync(IUnixFileSystemEntry entry, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <inheritdoc/>
        public Task<IUnixDirectoryEntry> CreateDirectoryAsync(IUnixDirectoryEntry targetDirectory, string directoryName, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <inheritdoc/>
        public Task<Stream> OpenReadAsync(IUnixFileEntry fileEntry, long startPosition, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer> AppendAsync(IUnixFileEntry fileEntry, long? startPosition, Stream data, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer> CreateAsync(IUnixDirectoryEntry targetDirectory, string fileName, Stream data, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer> ReplaceAsync(IUnixFileEntry fileEntry, Stream data, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this function");
        }

        /// <summary>
        /// Sets the modify/access/create timestamp of a file system item
        /// </summary>
        /// <param name="entry">The <see cref="IUnixFileSystemEntry"/> to change the timestamp for</param>
        /// <param name="modify">The modification timestamp</param>
        /// <param name="access">The access timestamp</param>
        /// <param name="create">The creation timestamp</param>
        /// <param name="cancellationToken">The cancellation token</param>
        /// <returns>The modified <see cref="IUnixFileSystemEntry"/></returns>
        public Task<IUnixFileSystemEntry> SetMacTimeAsync(IUnixFileSystemEntry entry, DateTimeOffset? modify, DateTimeOffset? access, DateTimeOffset? create, CancellationToken cancellationToken)
        {
            
            
             throw new ArgumentException("Please implement this function");
            
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Dispose the object
        /// </summary>
        /// <param name="disposing"><code>true</code> when called from <see cref="Dispose()"/></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // Nothing to dispose
                }
                _disposedValue = true;
            }
        }
    }
}
