using System.IO.Ports;

namespace P1Connect
{
    public partial class Form1 : Form
    {
        static SerialPort? _serialPort;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Get a list of serial port names.
            string[] ports = SerialPort.GetPortNames();

            // Add ports to combo box.
            foreach (string port in ports)
            {
                cmbPorts.Items.Add(port);
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            btnQuit.Enabled = true;
            btnConnect.Enabled = false;
            btnSave.Enabled = true;

            // Create a new SerialPort object with settings.
            _serialPort = new SerialPort
            {
                PortName = cmbPorts.SelectedItem.ToString(),
                BaudRate = 9600,
                Parity = Parity.None,
                StopBits = StopBits.One,
                DataBits = 8,
                Handshake = Handshake.RequestToSend,
            };

            // Set the custom data received event handler.
            _serialPort.DataReceived += new SerialDataReceivedEventHandler(sp_DataReceived);

            // Set the read/write timeouts.
            _serialPort.ReadTimeout = 1500;

            _serialPort.Open();
        }

        private void btnQuit_Click(object sender, EventArgs e)
        {
            btnQuit.Enabled = false;
            btnConnect.Enabled = true;
            txtData.Enabled = true;
            _serialPort.Close();
        }

        // delegate is used to write to a UI control from a non-UI thread  
        private delegate void SetTextDeleg(string text);

        void sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Thread.Sleep(1500);
            string message = _serialPort.ReadLine();
            // Invokes the delegate on the UI thread, and sends the data that was received to the invoked method.  
            // ---- The "si_DataReceived" method will be executed on the UI thread which allows populating of the textbox.  
            this.BeginInvoke(new SetTextDeleg(si_DataReceived), new object[] { message });
        }

        //private void si_DataReceived(string data) { txtData.Text += "\r\n" + data.Trim(); }
        private void si_DataReceived(string data) { txtData.AppendText("\r\n" + data.Trim()); }

        private void cmbPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbPorts.SelectedIndex != -1)
            {
                btnConnect.Enabled = true;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            var dialogResult = sfdSaveData.ShowDialog();
            if (dialogResult == System.Windows.Forms.DialogResult.OK)
            {
                System.IO.File.WriteAllText(sfdSaveData.FileName, txtData.Text);
            }
        }
    }
}