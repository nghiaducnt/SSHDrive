using System;
using System.Collections.Generic;
using System.IO;

using JetBrains.Annotations;
using FubarDev.FtpServer.FileSystem;
using FubarDev.FtpServer.FileSystem.Generic;
using System.Text.RegularExpressions;

namespace DavinciInc.FtpServer.FileSystem.SSH
{
    class SSHFileEntry : IUnixFileEntry
    {
        private SSHCmdProvider _sshCmd;
        private string _path;
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

        public SSHFileEntry([NotNull] SSHCmdProvider sshCmd, string path)
        {
            _sshCmd = sshCmd;
            IsValid = true;
            _path = path;
            string statString = _sshCmd.SSHGetStat(path);
            Match match = Regex.Match(statString, @"File:\s\W([A-Za-z0-9\-\.\\\/\-_]+)\W");
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
                    Size = long.Parse(match.Groups[1].Value);
                else
                {
                    Size = 0;
                    IsValid = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                Size = 0;
            }


            //Check for Directory
            match = Regex.Match(statString, @"Size:.*Blocks:.*IO.*Block:.*[0-9]+\s+([a-zA-Z]+)");
            if (match.Success)
                if (String.Compare(match.Groups[1].Value.ToLower(), 0, "directory", 0, "directory".Length) == 0)
                {
                    IsValid = false;
                }
                else
                    IsValid = true;
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
            match = Regex.Match(statString, @"Access:.*Uid.*[0-9]/(.*)\)\s+Gid");
            if (match.Success)
            {
                Owner = match.Groups[1].Value;
            }
            //Group
            match = Regex.Match(statString, @"Access:.*Uid:.*Gid:\s+\(.*[0-9]+/(.*)\)");
            if (match.Success)
            {
                Group = match.Groups[1].Value;
            }


        }

        /// <summary>
        /// Gets the underlying <see cref="FileInfo"/>
        /// </summary>
        public FileInfo Info { get; }

        /// <inheritdoc/>
        public string Name { get; }

        /// <inheritdoc/>
        public IUnixPermissions Permissions { get; }

        /// <inheritdoc/>
        public DateTimeOffset? LastWriteTime { get; }

        /// <inheritdoc/>
        public DateTimeOffset? CreatedTime { get; }

        /// <inheritdoc/>
        public DateTimeOffset? LastAccessTime { get; }

        /// <inheritdoc/>
        public long NumberOfLinks => 1;

        /// <inheritdoc/>
        public IUnixFileSystem FileSystem { get; }

        /// <inheritdoc/>
        public string Owner { get; }

        /// <inheritdoc/>
        public string Group { get; }

        /// <inheritdoc/>
        public long Size { get; }
        public bool IsValid { get; }

        public override string ToString()
        {
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
