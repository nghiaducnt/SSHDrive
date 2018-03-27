using System;
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
        public string _rootPath;
        public string _currentPath;
        SshClient _client;
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
            _currentDir = new SSHDirectoryEntry(this, _currentPath);
            _currentDir.EnumerateSSHFileSystemInfos();
        }
        public SSHCmdProvider([NotNull] SshClient client, [NotNull] string currentPath)
        {
            _client = client;
            /* get the current home of user */
            _rootPath = SSHGetHome();
            _currentPath = currentPath;
            _currentDir = new SSHDirectoryEntry(this, _currentPath);
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
        /// <summary>
        /// List from path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string SSHGetLS(string path)
        {
            try
            {
                SshCommand cmd;
                cmd = _client.RunCommand("ls " + path);
                return cmd.Result;
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

        #endregion
    }
}
