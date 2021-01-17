﻿using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Globalization;
using System.Resources;
using TeelSys.Globalization;
using TeelSys.Web;

namespace WebSocketExample
{
    public partial class Form1 : Form
    {
        /*** Fields and Constants ***/
        #region
        private bool _connected = false;
        private WsClient _client;
        private readonly ResourceManager rm;
        #endregion

        /*** Properties ***/
        #region
        #endregion

        /*** Constructor & Initialization ***/
        #region
        public Form1()
        {
            InitializeComponent();

            string t = this.GetType().FullName;

            rm = new ResourceManager(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly());
            LocalizeFormText();
        }

        public Form1(CultureInfo culture)
        {
            InitializeComponent();

            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            rm = new ResourceManager(this.GetType().FullName, System.Reflection.Assembly.GetExecutingAssembly());
            LocalizeFormText();
        }

        public void LocalizeFormText()
        {
            if(rm == null)
                return;

            LocalizedResourceHelper.LocalizeControlText(rm, this, "$this.Text");
            LocalizedResourceHelper.LocalizeControlText(rm, cmdConnect);
            LocalizedResourceHelper.LocalizeControlText(rm, cmdDisconnect);
            LocalizedResourceHelper.LocalizeControlText(rm, cmdSend);
            LocalizedResourceHelper.LocalizeControlText(rm, exitToolStripMenuItem);
            LocalizedResourceHelper.LocalizeControlText(rm, fileToolStripMenuItem);
            LocalizedResourceHelper.LocalizeControlText(rm, githubProjectToolStripMenuItem);
            LocalizedResourceHelper.LocalizeControlText(rm, helpToolStripMenuItem);
            LocalizedResourceHelper.LocalizeControlText(rm, lblConnection);
            LocalizedResourceHelper.LocalizeControlText(rm, lblMessageLog);
            LocalizedResourceHelper.LocalizeControlText(rm, lblPort);
            LocalizedResourceHelper.LocalizeControlText(rm, lblSend);
            LocalizedResourceHelper.LocalizeControlText(rm, lblServer);
        }
        #endregion

        /*** Public Events ***/
        #region
        #endregion

        /*** Public Methods ***/
        #region
        #endregion

        /*** Protected Methods ***/
        #region
        #endregion

        /*** Private Methods ***/
        #region
        private delegate void AppendMessageDelegate(string Message, bool sending, bool connectionStatus = false);

        private void AppendMessage(string Message, bool sending, bool connectionStatus = false)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new AppendMessageDelegate(AppendMessage), new object[] { Message, sending, connectionStatus });
            }
            else
            {
                if (txtLog.Text.Length > 0)
                    txtLog.AppendText("\r\n");
                else
                    txtLog.AppendText(LocalizedResourceHelper.GetLocalizedText(rm, "MessageLogHeaders") + "\r\n");

                txtLog.Select(txtLog.TextLength, 0);
                if (connectionStatus)
                    txtLog.SelectionColor = Color.Purple;
                else if (sending)
                    txtLog.SelectionColor = Color.Red;
                else
                    txtLog.SelectionColor = Color.Green;

                txtLog.AppendText(string.Format("{0}\t{1}\t{2}", DateTime.Now, connectionStatus ? "" : sending ? LocalizedResourceHelper.GetLocalizedText(rm, "Sent") : LocalizedResourceHelper.GetLocalizedText(rm, "Received"), Message));
            }
        }

        private bool IsUriValid(string server, int port)
        {
            try
            {
                Uri connectionUri = new Uri(string.Format("ws://{0}:{1}", server, port));
                return true;
            }
            catch (UriFormatException e)
            {
                MessageBox.Show(this, string.Format("{0}\r\n{1}", LocalizedResourceHelper.GetLocalizedText(rm, "InvalidServerOrPort"), e.Message),
                    LocalizedResourceHelper.GetLocalizedText(rm, "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private void UpdateButtons()
        {
            cmdDisconnect.Enabled = _connected;
            cmdConnect.Enabled = !cmdDisconnect.Enabled;

            lblConnectedStatus.Text = _connected ? LocalizedResourceHelper.GetLocalizedText(rm, "Connected") : LocalizedResourceHelper.GetLocalizedText(rm, "NotConnected");
        }
        #endregion

        /*** Event Handlers ***/
        #region
        private void Client_ConnectionError(object sender, ErrorEventArgs e)
        {
            AppendMessage(e.Message, false, true);
        }

        private void Client_ConnectionStateChanged(object sender, ConnectionStateChangedEventArgs e)
        {
            _connected = e.IsConnected;

            UpdateButtons();

            AppendMessage(lblConnectedStatus.Text, false, true);
        }

        private void Client_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            AppendMessage(e.Message, false);
        }

        private void CmdConnect_Click(object sender, EventArgs e)
        {
            if (_connected) // Disconnect
            {
                if (_client == null)
                {
                    _connected = false;
                    UpdateButtons();
                    return;
                }
                _client.CloseClientAsync();
            }
            else // Connect
            {
                if (int.TryParse(txtPort.Text, out int port))
                {
                    if (!IsUriValid(txtServer.Text, port))
                    {
                        return;
                    }
                    _client = new WsClient(txtServer.Text, int.Parse(txtPort.Text));
                    _client.ConnectionStateChanged += Client_ConnectionStateChanged;
                    _client.MessageReceived += Client_MessageReceived;
                    _client.ConnectionError += Client_ConnectionError;
                }
                else
                {
                    MessageBox.Show(this, LocalizedResourceHelper.GetLocalizedText(rm, "PortMustBeAnInteger"), LocalizedResourceHelper.GetLocalizedText(rm, "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            UpdateButtons();
        }

        private void CmdSend_Click(object sender, EventArgs e)
        {
            if (_client == null)
                return;

            AppendMessage(txtSend.Text, true);
            _client.SendAsync(txtSend.Text);

            txtSend.Text = "";
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Form1_Resize(sender, e);
            UpdateButtons();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            int space = 10;

            cmdConnect.Top = cmdDisconnect.Top = (panButtons.Height - cmdConnect.Height) / 2;
            cmdConnect.Left = (panButtons.Width - cmdConnect.Width - cmdDisconnect.Width - space)/ 2;
            cmdDisconnect.Left = cmdConnect.Left + cmdConnect.Width + space;
        }

        private void GithubProjectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(Properties.Resources.GitHubProjectURL);
        }
        #endregion

        private void TextPort_Validating(object sender, CancelEventArgs e)
        {
            if (!int.TryParse(txtPort.Text, out _))
            {
                MessageBox.Show(this, LocalizedResourceHelper.GetLocalizedText(rm, "PortMustBeAnInteger"), LocalizedResourceHelper.GetLocalizedText(rm, "Error"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                e.Cancel = true;
            }
        }
    }
}