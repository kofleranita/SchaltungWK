using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;          // added to include serial ports
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Management;
using System.Threading;
using System.Configuration;

namespace NS_SchaltungWK
{
  public partial class Form1 : Form
  {
    public delegate void AsyncCallback(IAsyncResult ar);
    public delegate void AddTolstDiscoveredDevices(object o);

    enum commands
    {
      GET_VER = 0x10,
      DIG_ACTIVE = 0x20, DIG_INACTIVE, DIG_DIRECTION, DIG_SET_OUTPUTS, DIG_GET_OUTPUTS, DIG_GET_INPUTS,
      VAR_SET_OUTPUT = 0x30, VAR_GET_OUTPUT, VAR_GET_INPUT,

      GET_SER_NUM = 0x77, GET_VOLTS, PASSWORD_ENTRY, GET_UNLOCK_TIME, LOG_OUT,
    };

    bool device_found = false;
    byte[] SerBuf = new byte[70];
    NetworkStream ns;
    Button[] RelayButtons = new Button[8];
    Button[] DimmButtons = new Button[8];
    Button ConnButton;
    Button RebootButton;

    public Form1()
    {
      const int cWidth = 100;
      const int cHeight = 40;
      int vertical,horizontal;
      string s;

      InitializeComponent();

      vertical = 5;
      horizontal = 3;
      for (int i = 0; i < RelayButtons.Length; i++)   //add all the relay buttons to the form as an array, this allows the buttons to be accessed easily within loops
      {
        RelayButtons[i] = new Button();
        RelayButtons[i].Size = new Size(cWidth, cHeight);
        RelayButtons[i].Location = new Point(horizontal, vertical);
        RelayButtons[i].Tag = (i + 1).ToString();

        DimmButtons[i] = new Button();
        DimmButtons[i].Size = new Size(cWidth, cHeight);
        DimmButtons[i].Location = new Point(horizontal + cWidth + horizontal, vertical);
        DimmButtons[i].Tag = (i + 1).ToString();

        vertical += 45;

        s = (i + 1).ToString();
        RelayButtons[i].Text = ConfigurationManager.AppSettings["RelayBez" +s];
          
        //wenn Button kein Text hat, dann nicht zeichnen!
        if (RelayButtons[i].Text != "")
        {
            this.Controls.Add(RelayButtons[i]);
            RelayButtons[i].Click += new EventHandler(button_relay_Click); 
        }

        if (Convert.ToInt32(ConfigurationManager.AppSettings["RelayDimmen" + s]) == 1)
        {
          DimmButtons[i].Text = "Dimmen";
          this.Controls.Add(DimmButtons[i]);
          DimmButtons[i].Click += new EventHandler(button_dimmen_Click);
        }
      }

      ConnButton = new Button();
      ConnButton.Size = new Size(cWidth, cHeight);
      ConnButton.Location = new Point(horizontal, vertical);
      ConnButton.Text = "Verbindung";
      this.Controls.Add(ConnButton);
      ConnButton.Click += new EventHandler(btConnection_Click);

      RebootButton = new Button();
      RebootButton.Size = new Size(cWidth, cHeight);
      vertical += 45;
      RebootButton.Location = new Point(horizontal, vertical);
      RebootButton.Text = "Neu starten";
      this.Controls.Add(RebootButton);
      RebootButton.Click += new EventHandler(btReboot_Click);
      RebootButton.Visible = false; //wurde nur für Testzwecke benötigt

    }

    private void timer1_Tick(object sender, EventArgs e)
    {
      byte x;
      int temp;

      if (device_found == true)
      {
        SerBuf[0] = (byte)commands.DIG_GET_OUTPUTS;      // get states command
        transmit(1);
        receive(1);

        temp = SerBuf[2] << 16;
        temp += SerBuf[1] << 8;
        temp += SerBuf[0];
        for (x = 0; x < RelayButtons.Length; x++)
        {
          if ((temp & (0x01 << x)) > 0)
          {
            RelayButtons[x].BackColor = Color.Red;
            DimmButtons[x].BackColor = Color.Red;
          }
          else
          {
            RelayButtons[x].BackColor = Color.White;
            DimmButtons[x].BackColor = Color.White;
          }
        }
      }
      UpdateConnectionStatus();
    }

    private void UpdateConnectionStatus()
    {
      int i;
      if (device_found == true)
        ConnButton.BackColor = Color.Green;
      else
      {
        ConnButton.BackColor = Color.Red;
        for (i=0; i<RelayButtons.Length; i++)
        {
          RelayButtons[i].BackColor = Color.White;
          DimmButtons[i].BackColor = Color.White;
        }
      }
    }

    private void open_ethernet_connection(bool ShowMessage)
    {
      if (ns != null)
      {
        ns.Close();
        ns.Dispose();
      }
      TcpClient client = new TcpClient();
      try
      {
        IAsyncResult result = client.BeginConnect(GlobalClass.ipaddress, GlobalClass.port, null, null);
        result.AsyncWaitHandle.WaitOne(2000, true);
        Thread.Sleep(2000);
        //if (!success)
        //{
        /*connected = 0;
        client.Close();
        device_found = false;
        if (ShowMessage)
          MessageBox.Show("Failed to connect to " + GlobalClass.ipaddress + ":" + GlobalClass.port, Application.ProductName);*/
        //}
        //else 
       
        ns = client.GetStream();
        ns.ReadTimeout = 5000;
        ns.WriteTimeout = 5000;
        SerBuf[0] = (byte)commands.GET_VER;     // get version command for ETH RLY16/02, returns software version
        transmit(1);
        receive(3);
        device_found = true;
        UpdateConnectionStatus();        
      }
      catch 
      {
        client.Close();
        device_found = false;
        if (ShowMessage)
        {
          DialogResult mb = MessageBox.Show("Verbindungsfehler zu " + GlobalClass.ipaddress + ":" + GlobalClass.port + "; " + "Soll die Schnittstelle neugestartet werden?", Application.ProductName, MessageBoxButtons.YesNo);
          if (mb == DialogResult.Yes)
          {
            DoReboot();
          }
        }
        UpdateConnectionStatus();
        return;
      } 
    }

    private void transmit(byte write_bytes)
    {
      try
      {
        ns.Write(SerBuf, 0, write_bytes);
        ns.Flush();
      }
      catch
      {
        device_found = false;
        UpdateConnectionStatus();
      }
    }

    private void receive(byte read_bytes)
    {
      byte x;

      for (x = 0; x < read_bytes; x++)       // this will call the read function for the passed number times, 
      {                                      // this way it ensures each byte has been correctly recieved while
        try                                // still using timeouts
        {
          ns.Read(SerBuf, x, 1);     // retrieves 1 byte at a time and places in SerBuf at position x
        }
        catch                 // timeout or other error occured, set lost comms indicator
        {
          device_found = false;
          UpdateConnectionStatus();
        }
      }
    }

    private void button_relay_Click(object sender, EventArgs e)
    {
      int tag, pulse;
      tag = 0;
      pulse = 0;

      //Ermittle den Button anhand des Tags
      if (sender is Button)
          tag = Convert.ToInt32(((Button)sender).Tag);

      if (tag > 0)
      {
        pulse = Convert.ToInt32(ConfigurationManager.AppSettings["RelayPulse" + tag.ToString()]);
        if (device_found == false)
          open_ethernet_connection(false);
        if (device_found == true)
        {
          if (RelayButtons[tag - 1].BackColor == Color.Red) SerBuf[0] = (byte)commands.DIG_INACTIVE;
          else SerBuf[0] = (byte)commands.DIG_ACTIVE;
          SerBuf[1] = (byte)tag;
          SerBuf[2] = (byte)pulse;
          transmit(3);
          receive(1);
        }         
      }
    }

    private void button_dimmen_Click(object sender, EventArgs e)
    {
      const int pulse = 0;
      int tag;
      tag = 0;

      //Ermittle den Button anhand des Tags
      if (sender is Button)
        tag = Convert.ToInt32(((Button)sender).Tag);

      if (tag > 0)
      {
        if (device_found == false)
          open_ethernet_connection(false);
        if (device_found == true)
        {
          if (DimmButtons[tag - 1].BackColor == Color.Red) SerBuf[0] = (byte)commands.DIG_INACTIVE;
          else SerBuf[0] = (byte)commands.DIG_ACTIVE;
          SerBuf[1] = (byte)tag;
          SerBuf[2] = (byte)pulse;
          transmit(3);
          receive(1);
        }
      }
    }

    private void Form1_FormClosing(object sender, FormClosingEventArgs e) // attempt to close any open network stream on form closure
    {
      try
      {
        SerBuf[0] = (byte)commands.LOG_OUT;
        transmit(1);
        receive(1);
        ns.Close();
        ns.Dispose();
      }
      catch { }
    }

    private bool CheckIP(string myIP, string myPort)
    {
      if (IsAddressValid(myIP) == true)
      {
        GlobalClass.ipaddress = myIP;
        GlobalClass.port = Convert.ToInt16(myPort);
        return true;
      }
      else
      {
        MessageBox.Show("INVALID IP ADDRESS", Application.ProductName);
        return false;
      }
    }

    private bool IsAddressValid(string addrString)
    {
      IPAddress address;
      return IPAddress.TryParse(addrString, out address);
    }

    public void Form1_Load(object sender, EventArgs e)
    {
      string ip;
      string port;
      //IP von Config laden

      ip = ConfigurationManager.AppSettings["IPAdresse"];
      port = ConfigurationManager.AppSettings["Port"];

      if (CheckIP(ip, port) == true)
      {
        open_ethernet_connection(true);
      }
      else
        Close();
      UpdateConnectionStatus();
    }

    private void btConnection_Click(object sender, EventArgs e)
    {
      TimerConnection.Start();
      ConnButton.Enabled = false;
      open_ethernet_connection(true);
      UpdateConnectionStatus();
    }

    private void btReboot_Click(object sender, EventArgs e)
    {
      DoReboot();
    }

    private void DoReboot()
    {
      try
      {
        var request = (HttpWebRequest)WebRequest.Create("http://" + GlobalClass.ipaddress + "/reboot.htm");

        var postData = "reboot=Reset ETH008";
        var data = Encoding.ASCII.GetBytes(postData);

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;

        using (var stream = request.GetRequestStream())
        {
          stream.Write(data, 0, data.Length);
        }
        backgroundWorker.RunWorkerAsync();
      }
      catch
      {
        MessageBox.Show("Fehler beim Neustart");
      }
    }

    private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
    {
      for (int i = 1; i <= 35; i++)
      {
        backgroundWorker.ReportProgress(i);
        Thread.Sleep(1000);
      }
    }

    private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
    {
      progressBar.Value = e.ProgressPercentage;
      progressBar.Visible = true;
      progressBar.Maximum = 35;
    }

    private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
      progressBar.Visible = false;
      open_ethernet_connection(false);
    }

    private void TimerConnection_Tick(object sender, EventArgs e)
    {
      ConnButton.Enabled = true;
      TimerConnection.Stop();
    }
  }
}
