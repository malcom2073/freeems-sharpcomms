using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;
using System.Threading;
namespace FreeEmsTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        FreeEMSComms comms;
        DataPacketDecoder packetDecoder= new DataPacketDecoder();
        private void Form1_Load(object sender, EventArgs e)
        {
            listView1.Columns.Add("Key");
            listView1.Columns.Add("Value");
            comms = new FreeEMSComms();
            comms.SetComSettings("COM3", 115200);
            comms.MessageRecieved += new FreeEMSComms.MessageRecievedDelegate(comms_MessageRecieved);
            comms.InvalidChecksum += new FreeEMSComms.InvalidChecksumDelegate(comms_InvalidChecksum);
            comms.InvalidEscapeChar += new FreeEMSComms.InvalidEscapeCharDelegate(comms_InvalidEscapeChar);
            comms.OutOfPacketByte += new FreeEMSComms.OutOfPacketByteDelegate(comms_OutOfPacketByte);
            comms.Start();
            packetDecoder.PacketDecoded += new DataPacketDecoder.PacketDecodedDelegate(packetDecoder_PacketDecoded);
        }

        void comms_OutOfPacketByte()
        {
            
        }

        void comms_InvalidEscapeChar()
        {
            throw new NotImplementedException();
        }

        void comms_InvalidChecksum()
        {
            throw new NotImplementedException();
        }

        void packetDecoder_PacketDecoded(Dictionary<string, string> valuemap)
        {
            if (listView1.Items.Count == 0)
            {
                foreach (KeyValuePair<string, string> kvp in valuemap)
                {
                    listView1.Items.Add(kvp.Key);
                    listView1.Items[listView1.Items.Count - 1].SubItems.Add(kvp.Value);
                }
            }

            for (int i = 0; i < listView1.Items.Count; i++)
            {
                if (valuemap.ContainsKey(listView1.Items[i].Text))
                {
                    listView1.Items[i].SubItems[1].Text = valuemap[listView1.Items[i].Text];
                }
            }
        }

        void comms_MessageRecieved(List<byte> message)
        {
            
            this.Invoke(new displayMessageDelegate(Invoked_displayMessage), new object[] { message });
        }
        int msgcounter = 0;
        delegate void displayMessageDelegate(List<byte> message);
        void Invoked_displayMessage(List<byte> message)
        {
            packetDecoder.decodePayload(message);
            msgcounter++;
            this.Text = msgcounter.ToString();
        }
    }
}
