using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Renci.SshNet;


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
        /// <param name="streamBufferSize">Buffer size to be used in async IO methods</param>
        public SSHFileSystem(SSHCmdProvider client, string rootPath, bool allowNonEmptyDirectoryDelete, int streamBufferSize)
        {
            FileSystemEntryComparer = StringComparer.OrdinalIgnoreCase;
            if (rootPath == null || rootPath.Length == 0)
                rootPath = client._rootPath;
            Root = new SSHDirectoryEntry(client, this, rootPath, true);
            SupportsNonEmptyDirectoryDelete = allowNonEmptyDirectoryDelete;
            _streamBufferSize = streamBufferSize;
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
            var result = new List<IUnixFileSystemEntry>();
            var searchDirInfo = ((SSHDirectoryEntry)directoryEntry);
            foreach (var info in searchDirInfo.EnumerateSSHFileSystemInfos())
            {
                result.Add(info);
            }
            return Task.FromResult<IReadOnlyList<IUnixFileSystemEntry>>(result);
        }

        public static string PathCombine(string str, string str1)
        {
            return str + "/" + str1;
        }
        /// <inheritdoc/>
        public Task<IUnixFileSystemEntry> GetEntryByNameAsync(IUnixDirectoryEntry directoryEntry, string name, CancellationToken cancellationToken)
        {
            SSHDirectoryEntry searchDirInfo = (SSHDirectoryEntry)directoryEntry;
            var fullPath = SSHFileSystem.PathCombine(searchDirInfo.FullName, name);
            IUnixFileSystemEntry result;
            if (SSHDirectoryEntry.Exists(searchDirInfo._sshCmd, fullPath))
                result = new SSHDirectoryEntry(searchDirInfo._sshCmd, this, fullPath, false);
            else if (SSHFileEntry.Exists(searchDirInfo._sshCmd, fullPath))
                result = new SSHFileEntry(searchDirInfo._sshCmd, this, fullPath);
            else
                result = null;
            return Task.FromResult(result);
        }

        /// <inheritdoc/>
        public Task<IUnixFileSystemEntry> MoveAsync(IUnixDirectoryEntry parent, IUnixFileSystemEntry source, IUnixDirectoryEntry target, string fileName, CancellationToken cancellationToken)
        {
            var targetEntry = (SSHDirectoryEntry)target;
            var targetName = SSHFileSystem.PathCombine(targetEntry.FullName, fileName);

            var sourceFileEntry = source as SSHFileEntry;
            if (sourceFileEntry != null)
            {
                SSHFileEntry.MoveTo(sourceFileEntry, targetName);
                return Task.FromResult<IUnixFileSystemEntry>(new SSHFileEntry(targetEntry._sshCmd, this, targetName));
            }

            var sourceDirEntry = (SSHDirectoryEntry)source;
            SSHDirectoryEntry.MoveTo(sourceDirEntry, targetName);
            return Task.FromResult<IUnixFileSystemEntry>(new SSHDirectoryEntry(targetEntry._sshCmd, this, targetName, false));
        }

        /// <inheritdoc/>
        public Task UnlinkAsync(IUnixFileSystemEntry entry, CancellationToken cancellationToken)
        {
            var dirEntry = entry as SSHDirectoryEntry;
            if (dirEntry != null)
            {
                dirEntry._sshCmd.SSHDeleteAll(dirEntry.FullName);
            }
            else
            {
                var fileEntry = (SSHFileEntry)entry;
                fileEntry._sshCmd.SSHDeleteAll(fileEntry.FullName);
            }
            return Task.FromResult(0);
        }

        /// <inheritdoc/>
        public Task<IUnixDirectoryEntry> CreateDirectoryAsync(IUnixDirectoryEntry targetDirectory, string directoryName, CancellationToken cancellationToken)
        {
            var targetEntry = (SSHDirectoryEntry)targetDirectory;
            string newDirPath = SSHFileSystem.PathCombine(targetEntry.FullName, directoryName);
            var newDirInfo = targetEntry._sshCmd.CreateSubdirectory(newDirPath);
            return Task.FromResult<IUnixDirectoryEntry>(new SSHDirectoryEntry(targetEntry._sshCmd, this, newDirPath, false));
        }

        /// <inheritdoc/>
        public Task<Stream> OpenReadAsync(IUnixFileEntry fileEntry, long startPosition, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this OpenReadAsync");
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer> AppendAsync(IUnixFileEntry fileEntry, long? startPosition, Stream data, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this AppendAsync");
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer> CreateAsync(IUnixDirectoryEntry targetDirectory, string fileName, Stream data, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this CreateAsync");
        }

        /// <inheritdoc/>
        public async Task<IBackgroundTransfer> ReplaceAsync(IUnixFileEntry fileEntry, Stream data, CancellationToken cancellationToken)
        {
            throw new ArgumentException("Please implement this ReplaceAsync");
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
            
            
             throw new ArgumentException("Please implement this SetMacTimeAsync");
            
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
