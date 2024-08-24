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
    public partial class Form1 : Form
    {

        private MainWindow mainWindow;
        public Form1(MainWindow mainWindow)
        {
            InitializeComponent();
            txtAddress.Text = "127.0.0.1";
            txtPort.Text = "12345";
            this.mainWindow = mainWindow;
            this.DialogResult = DialogResult.Cancel;
        }


        public TcpClient socPlayer;
        private void btnConnect_Click(object sender, EventArgs e)
        {
            socPlayer = new TcpClient();
            try
            {
                socPlayer.Connect(txtAddress.Text, int.Parse(txtPort.Text));
                string msg = "Hello";
                byte[] buf = new byte[1024];
                socPlayer.Client.Send(Encoding.UTF8.GetBytes(msg));
                int size = socPlayer.Client.Receive(buf);
                msg = Encoding.UTF8.GetString(buf, 0, size);
                if (msg == "Hello")
                {
                    mainWindow.socPlayer = socPlayer;
                    this.DialogResult = DialogResult.OK;
                    MessageBox.Show("You are connected", "Success");
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
            }
        }
    }
}
