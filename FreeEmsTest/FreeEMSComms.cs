using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Threading;
namespace FreeEmsTest
{
    class FreeEMSComms
    {
        SerialPort port;
        Thread myThread;
        public delegate void MessageRecievedDelegate(List<byte> message);
        public event MessageRecievedDelegate MessageRecieved;

        public delegate void InvalidChecksumDelegate();
        public event InvalidChecksumDelegate InvalidChecksum;

        public delegate void OutOfPacketByteDelegate();
        public event OutOfPacketByteDelegate OutOfPacketByte;

        public delegate void InvalidEscapeCharDelegate();
        public event InvalidEscapeCharDelegate InvalidEscapeChar;


        public FreeEMSComms()
        {
            m_portName = "COM1";
            m_baud = 115200;
        }
        public void Start()
        {
            myThread = new Thread(new ThreadStart(threadLoop));
            myThread.Start();
        }
        public void SetComSettings(String port, int baud)
        {
            m_portName = port;
            m_baud = baud;
        }
        string m_portName;
        int m_baud;
        private void threadLoop()
        {
            port = new SerialPort();
            port.Parity = Parity.Odd;
            port.StopBits = StopBits.One;
            port.BaudRate = m_baud;
            port.PortName = m_portName;
            port.Open();
            byte[] buffer = new byte[1024];
            bool inescape = false;
            bool inmessage = false;
            int count = 0;
            List<byte> messageBuffer = new List<byte>();
            while (true)
            {
                count = port.Read(buffer, 0, 1024);
                for (int i = 0; i < count; i++)
                {
                    if (buffer[i] == 0xAA)
                    {
                        if (inmessage)
                        {
                            //Start byte while currently in message
                            messageBuffer.Clear();
                        }
                        inmessage = true;
                    }
                    else if (buffer[i] == 0xCC && inmessage)
                    {
                        inmessage = false;
                        byte sum = 0;
                        for (int j = 0; j < messageBuffer.Count - 1; j++)
                        {
                            sum += messageBuffer[j];
                        }
                        if (sum != messageBuffer[messageBuffer.Count - 1])
                        {
                            InvalidChecksum();
                            //BAD CHEKCSUM
                        }
                        else
                        {
                            //GOOD PACKET in messageBuffer
                            MessageRecieved(messageBuffer);
                        }
                        messageBuffer.Clear();
                        //bufferList.Add(buffer[i]);
                    }
                    else
                    {
                        if (inmessage && !inescape)
                        {
                            if (buffer[i] == 0xBB)
                            {
                                //Need to escape the next byte
                                //retval = logfile.read(1);
                                inescape = true;
                            }
                            else
                            {
                                messageBuffer.Add(buffer[i]);
                            }

                        }
                        else if (inmessage && inescape)
                        {
                            if (buffer[i] == 0x55)
                            {
                                messageBuffer.Add(0xAA);
                            }
                            else if (buffer[i] == 0x44)
                            {
                                messageBuffer.Add(0xBB);
                            }
                            else if (buffer[i] == 0x33)
                            {
                                messageBuffer.Add(0xCC);
                            }
                            else
                            {
                                InvalidEscapeChar();
                                //Invalid escape char
                            }
                            inescape = false;
                        }
                        else
                        {
                            OutOfPacketByte();
                            //Out of packet byte
                        }
                    }

                }
            }
        }
    }
}
