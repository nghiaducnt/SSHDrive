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
        string _rootPath;
        string _currentPath;
        SshClient _client;
        SSHDirectoryEntry _currentDir;
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
            _currentDir = new SSHDirectoryEntry(SSHGetStat(_currentPath));
        }
        public SSHCmdProvider([NotNull] SshClient client, [NotNull] string currentPath)
        {
            _client = client;
            /* get the current home of user */
            _rootPath = SSHGetHome();
            _currentPath = currentPath;
            _currentDir = new SSHDirectoryEntry(SSHGetStat(_currentPath));
        }
        #region Internal command
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

        public override string ToString()
        {
            string ret = "";
            ret = _currentDir.ToString();
            return ret;
        }

        #endregion
    }
}
