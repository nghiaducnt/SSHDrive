﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Renci.SshNet;
using DavinciInc.FtpServer.FileSystem.SSH;

namespace SSHDriveApp
{
    public partial class frmLogin : Form
    {
        public frmLogin()
        {
            InitializeComponent();
        }

        private void btnLogIn_Click(object sender, EventArgs e)
        {
            string host, user, password, test_string;
            int port;
            host = txtServer.Text;
            port = Int32.Parse(txtPort.Text);
            user = txtUser.Text;
            password = txtPassword.Text;
            test_string = "Hello";
            ConnectionInfo userConn = new ConnectionInfo(
                host, port, user, new PasswordAuthenticationMethod(user, password));
            userConn.Encoding = new System.Text.ASCIIEncoding();
            try
            {
                using (var client = new SshClient(userConn))
                {
                    SshCommand cmd;
                    client.Connect();
                    cmd = client.RunCommand("echo "  + test_string);
                    if (String.Compare(test_string, 0, cmd.Result, 0, test_string.Length) == 0)
                        MessageBox.Show("Log in successfully\n", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Login successfully but could not execute Unix Shell: \n" + cmd.Result, "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //test Regex
                    SSHCmdProvider sshProvider = new SSHCmdProvider(client);
                    client.Disconnect();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login unsuccessfully\n", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
