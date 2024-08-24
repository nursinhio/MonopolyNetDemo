using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Monopoly2
{
    public partial class FormServer : Form
    {
        private MainWindow mainWindow;
        public FormServer(MainWindow mainWindow)
        {

            InitializeComponent();
            this.Text = $"Server: {Dns.GetHostName()}";
            btnStart.Enabled = true;
            btnStop.Enabled = false;
            txtAddress.Text = "127.0.0.1";
            txtPort.Text = "12345";
            this.DialogResult = DialogResult.Cancel;
            this.mainWindow = mainWindow;
        }
        public TcpClient socClient;
        public TcpListener socSrv;
        private void btnStart_Click(object sender, EventArgs e)
        {
            int port = int.Parse(txtPort.Text);
            string ip = txtAddress.Text;
            socSrv = new TcpListener(IPAddress.Parse(ip), port);
            try
            {
                socSrv.Start();
                btnStart.Enabled = false;
                btnStop.Enabled = true;
                socClient = socSrv.AcceptTcpClient();
                byte[] buf = new byte[1024];
                string msg;
                int size = socClient.Client.Receive(buf);
                msg = Encoding.UTF8.GetString(buf, 0, size);
                socClient.Client.Send(Encoding.UTF8.GetBytes(msg));
                if (socClient.Connected)
                {
                    this.DialogResult = DialogResult.OK;
                    mainWindow.socPlayer = socClient;
                    MessageBox.Show("OK! Game is start.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            socSrv.Stop();
            btnStop.Enabled = false;
            btnStart.Enabled = true;
        }
    }
}
