using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using JetBrains.Annotations;
using FubarDev.FtpServer.FileSystem;
using FubarDev.FtpServer.FileSystem.Generic;
using Renci.SshNet;
using System.Text.RegularExpressions;

namespace DavinciInc.FtpServer.FileSystem.SSH
{
    public class SSHDirectoryEntry: IUnixDirectoryEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SSHDirectoryEntry"/> class.
        /// </summary>
        /// <param name="fileSystem">The file system this entry belongs to</param>
        /// <param name="dirInfo">The <see cref="DirectoryInfo"/> to extract the information from</param>
        /// <param name="isRoot">Is this the root directory?</param>
        public SSHDirectoryEntry([NotNull] SSHFileSystem fileSystem, [NotNull] DirectoryInfo dirInfo, bool isRoot)
        {
            FileSystem = fileSystem;
            Info = dirInfo;
            LastWriteTime = new DateTimeOffset(Info.LastWriteTime);
            CreatedTime = new DateTimeOffset(Info.CreationTimeUtc);
            var accessMode = new GenericAccessMode(true, true, true);
            Permissions = new GenericUnixPermissions(accessMode, accessMode, accessMode);
            IsRoot = isRoot;
        }

        public SSHDirectoryEntry([NotNull] SSHFileSystem fileSystem, [NotNull] string rootPath, bool isRoot)
        {
            FileSystem = fileSystem;
            LastWriteTime = new DateTimeOffset(Info.LastWriteTime);
            CreatedTime = new DateTimeOffset(Info.CreationTimeUtc);
            var accessMode = new GenericAccessMode(true, true, true);
            Permissions = new GenericUnixPermissions(accessMode, accessMode, accessMode);
            IsRoot = isRoot;
        }

        /// <summary>
        /// Create Directory from Unix'status string
        /// For example: http://regexstorm.net/tester
        /// File: 'bin'
        /// Size: 1228  Blocks: 24  IO Block: 262144 directory
        /// Device: 18h/24d Inode: 15089714    Links: 9
        /// Access: (0700/drwx------)  Uid: (  263/     shs)   Gid: (  100/ unixdweebs)
        /// Access: 2014-09-21 03:00:45.000000000 -0400
        /// Modify: 2014-09-15 17:54:41.000000000 -0400
        /// Change: 2014-09-15 17:54:41.000000000 -0400
        /// </summary>
        /// <param name="statString"></param>
        public SSHDirectoryEntry(string statString)
        {
            Match match = Regex.Match(statString, @"File:\s\W([A-Za-z0-9\-\.\\\/\-_]+)\W");
            
            IsValid = true;
            //File field
            if (match.Success)
                Name = match.Groups[1].Value;
            else
            {
                Name = "NULL";
                IsValid = false;
            }
             
            //Size
            match = Regex.Match(statString, @"Size:\s([0-9]+)");
            try
            {
                if (match.Success)
                    Size = UInt32.Parse(match.Groups[1].Value);
                else
                {
                    Size = 0;
                    IsValid = false;
                }
            } catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Size = 0;
            }
            

            //Check for Directory
            match = Regex.Match(statString, @"Size:.*Blocks:.*IO.*Block:.*[0-9]+\s+([a-zA-Z]+)");
            if (match.Success)
                if (String.Compare(match.Groups[1].Value.ToLower(), 0, "directory", 0, "directory".Length) == 0)
                {
                    IsValid = true;
                }
                else
                    IsValid = false;
            //Access timestamp
            match = Regex.Match(statString, @"Access:\s+([0-9].*)");
            if (match.Success)
            {
                LastAccessTime = DateTimeOffset.Parse(match.Groups[1].Value);
            }
            //Create timestamp
            match = Regex.Match(statString, @"Change:\s+([0-9].*)");
            if (match.Success)
            {
                CreatedTime = DateTimeOffset.Parse(match.Groups[1].Value);
            }
            //Write timestamp
            match = Regex.Match(statString, @"Modify:\s+([0-9].*)");
            if (match.Success)
            {
                LastWriteTime = DateTimeOffset.Parse(match.Groups[1].Value);
            }
            //Owner
            match = Regex.Match(statString, @"Access:.*Uid:.*[0-9]/\s+(.*)\)\s+Gid");
            if (match.Success)
            {
                Owner = match.Groups[1].Value;
            }
            //Group
            match = Regex.Match(statString, @"Access:.*Uid:.*Gid:\s+\(\s+[0-9]+/\s+(.*)\)");
            if (match.Success)
            {
                Group = match.Groups[1].Value;
            }

            
        }


        /// <summary>
        /// Gets the underlying <see cref="DirectoryInfo"/>
        /// </summary>
        public DirectoryInfo Info { get; }

        /// <inheritdoc/>
        public bool IsRoot { get; }

        /// <inheritdoc/>
        public bool IsDeletable => !IsRoot && (FileSystem.SupportsNonEmptyDirectoryDelete || !Info.EnumerateFileSystemInfos().Any());

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IUnixPermissions Permissions { get; }

        /// <inheritdoc/>
        public DateTimeOffset? LastAccessTime { get; }

        /// <inheritdoc/>
        public DateTimeOffset? LastWriteTime { get; }

        /// <inheritdoc/>
        public DateTimeOffset? CreatedTime { get; }

        /// <inheritdoc/>
        public long NumberOfLinks => 1;

        /// <inheritdoc/>
        public IUnixFileSystem FileSystem { get; }

        /// <inheritdoc/>
        public string Owner { get; }

        /// <inheritdoc/>
        public string Group { get;  }

        public UInt32 Size { get; }

        public bool IsValid { get;  }

        public override string ToString() {
            string ret;
            ret = "Name: " + this.Name;
            ret += "\nSize: " + this.Size.ToString();
            ret += this.IsValid == true ? "\nDirectory" : "\nNot valid";
            ret += "\nCreated: " + this.CreatedTime.ToString();
            ret += "\nAccess: " + this.LastAccessTime.ToString();
            ret += "\nModifed: " + this.LastWriteTime.ToString();
            ret += "\nOwner: " + this.Owner;
            ret += "\nGroup: " + this.Group;
            return ret;
        }
    }
}
