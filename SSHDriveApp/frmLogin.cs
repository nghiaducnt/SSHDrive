using System;
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
using System.Security.Cryptography.X509Certificates;

using FubarDev.FtpServer;
using FubarDev.FtpServer.AccountManagement;
using FubarDev.FtpServer.AccountManagement.Anonymous;
using FubarDev.FtpServer.AuthTls;
using System.IO;
using TestFtpServer.Logging;

namespace SSHDriveApp
{
    public partial class frmLogin : Form
    {
        
        public frmLogin()
        {
            InitializeComponent();
        }
        System.Threading.Thread myThread;
        SshClient _client;
        SSHCmdProvider _sshProvider;
        bool _stop;
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
                _client = new SshClient(userConn);
                {
                    SshCommand cmd;
                    _client.Connect();
                    cmd = _client.RunCommand("echo "  + test_string);
                    if (String.Compare(test_string, 0, cmd.Result, 0, test_string.Length) == 0)
                        MessageBox.Show("Log in successfully\n", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else
                        MessageBox.Show("Login successfully but could not execute Unix Shell: \n" + cmd.Result, "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //test Regex
                    try
                    {
                        _sshProvider = new SSHCmdProvider(_client);
                        MessageBox.Show(_sshProvider.ToString());

                    } catch (Exception ex)
                    {
                        throw ex;
                    }
                    finally
                    {
                        //_client.Disconnect();
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Login unsuccessfully\n", "Testing...", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void myStartingMethod()
        {
            // Load server certificate
            var cert = new X509Certificate2("test.pfx");
            AuthTlsCommandHandler.ServerCertificate = cert;

            // Only allow anonymous login
            var membershipProvider = new AnonymousMembershipProvider(new NoValidation());

            // Use the .NET file system
#if SSHDRIVE
            var fsProvider = new DotNetFileSystemProvider(Path.Combine(Path.GetTempPath(), "TestFtpServer"));
#else
            var fsProvider = new SSHFileSystemProvider(this._sshProvider, "");
#endif
            Console.WriteLine(Path.Combine(Path.GetTempPath(), "TestFtpServer").ToString());
            Console.WriteLine(fsProvider.ToString());

            // Use all commands from the FtpServer assembly and the one(s) from the AuthTls assembly
            var commandFactory = new AssemblyFtpCommandHandlerFactory(typeof(FtpServer).Assembly, typeof(AuthTlsCommandHandler).Assembly);

            // Initialize the FTP server
            using (var ftpServer = new FtpServer(fsProvider, membershipProvider, "127.0.0.1", 5000, commandFactory)
            {
                DefaultEncoding = Encoding.ASCII,
                LogManager = new FtpLogManager(),
            })
            {
#if USE_FTPS_IMPLICIT
                // Use an implicit SSL connection (without the AUTHTLS command)
                ftpServer.ConfigureConnection += (s, e) =>
                {
                    var sslStream = new FixedSslStream(e.Connection.OriginalStream);
                    sslStream.AuthenticateAsServer(cert);
                    e.Connection.SocketStream = sslStream;
                };
#endif

                // Create the default logger
                var log = ftpServer.LogManager?.CreateLog(typeof(Program));

                try
                {
                    // Start the FTP server
                    
                    ftpServer.Start();
                    while (_stop == false) ;
                    this._client.Disconnect();
                    // Stop the FTP server
                    ftpServer.Stop();
                }
                catch (Exception ex)
                {
                    log?.Error(ex, "Error during main FTP server loop");
                    this._client.Disconnect();
                }
                finally
                {
                    
                }
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            myThread = new System.Threading.Thread(new  System.Threading.ThreadStart(myStartingMethod));
            _stop = false;
            myThread.Start();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _stop = true;
        }
    }
}
