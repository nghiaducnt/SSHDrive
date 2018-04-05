﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Renci.SshNet;

namespace DavinciInc.FtpServer.FileSystem.SSH
{
    public class SSHCmdProvider
    {
        public static int XXDOctetPerLine = 32;
        public string _currentPath;
        SshClient _client;

        /// <inheritdoc/>
        public string _rootPath { get; }

        public SSHDirectoryEntry _currentDir;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public SSHCmdProvider([NotNull] SshClient client)
        {
            _client = client;
            /* get the current home of user */
            _rootPath = SSHGetHome();
            _currentPath = _rootPath;
            _currentDir = new SSHDirectoryEntry(this, null, _currentPath);
            _currentDir.EnumerateSSHFileSystemInfos();
        }
        public SSHCmdProvider([NotNull] SshClient client, [NotNull] string currentPath)
        {
            _client = client;
            /* get the current home of user */
            _rootPath = SSHGetHome();
            if (currentPath.Length > 0)
                _currentPath = currentPath;
            else
                _rootPath = _rootPath;
            _currentDir = new SSHDirectoryEntry(this, null, _currentPath);
            _currentDir.EnumerateSSHFileSystemInfos();
        }
        #region Internal command
        /// <summary>
        /// Get home directory of current user
        /// </summary>
        /// <returns></returns>
        public string SSHGetHome()
        {
            try
            {
                SshCommand cmd;
                cmd = _client.RunCommand("cd ~");
                cmd = _client.RunCommand("pwd");
                return cmd.Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// Get status of current path, it can be directory or file
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string SSHGetStat(string path)
        {
            try
            {
                SshCommand cmd;
                cmd = _client.RunCommand("stat " + path);
                return cmd.Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SSHMoveTo(string abs_path_source, string abs_path_dest)
        {
            try
            {
                SshCommand cmd;
                cmd = _client.RunCommand("mv " + abs_path_source + " "  + abs_path_dest);
                return cmd.Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// List from path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string SSHGetLSAll(string path)
        {
            try
            {
                SshCommand cmd;
                if (path != null && path.Length > 0)
                    cmd = _client.RunCommand("ls -la " + path);
                else
                    cmd = _client.RunCommand("ls -la " + this._currentPath);
                return cmd.Result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string SSHDeleteAll(string path)
        {
            try
            {
                SshCommand cmd;
                if (path != null && path.Length > 0)
                {
                    cmd = _client.RunCommand("rm -rf " + path);
                    return cmd.Result;
                }
                    
                return "Error";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public override string ToString()
        {
            string ret = "";
            ret = _currentDir.ToString();
            return ret;
        }

        public string CreateSubdirectory(string newDirPath)
        {
            try
            {
                SshCommand cmd;
                if (newDirPath != null && newDirPath.Length > 0)
                {
                    cmd = _client.RunCommand("mkdir " + newDirPath);
                    return cmd.Result;
                }

                return "Error";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Return a XXD octet stream with 32 octets per line
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string SSHXXD(string fullName, long start, long length)
        {
            try
            {
                SshCommand cmd;
                if (fullName != null && fullName.Length > 0)
                {
                    cmd = _client.RunCommand("xxd -ps -c " + SSHCmdProvider.XXDOctetPerLine.ToString() + " -s +" + start.ToString() + " -l " + length.ToString() + " " + fullName);
                    return cmd.Result;
                }

                return "Error";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
    }
}
