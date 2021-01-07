using System;
using System.Text;
using System.IO.Ports;

namespace MODBUS_SERIAR
{
    class Master
    {
        private static bool _bConnected = false;

        private static SerialPort mSerial = null;
        private byte[] mBuffer = new byte[2048];

        public delegate void ReceivedData(byte[] data);
        public event ReceivedData OnReceivedData;

        public bool bConnected
        {
            get { return _bConnected; }
        }

        public Master()
        {
        }

        public Master(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            Connect( portName, baudRate, parity, dataBits, stopBits);
        }

        public void Connect(string portName, int baudRate, Parity parity, int dataBits, StopBits stopBits)
        {
            try
            {
                if (mSerial != null)
                    return;

                mSerial = new SerialPort(portName, baudRate, parity, dataBits, stopBits);

                mSerial.Open();

                mSerial.DataReceived += new SerialDataReceivedEventHandler(OnReceived);

                _bConnected = true;
            }
            catch (System.IO.IOException error)
            {
                _bConnected = false;
                throw (error);
            }
        }

        ~Master()
        {
            Dispose();
        }

        public void Disconnect()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (mSerial != null)
            {
                if (mSerial.IsOpen)
                    mSerial.Close();

                _bConnected = false;
            }
        }

        private void OnReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (sender == null) return;

            try
            {
                SerialPort sp = (SerialPort)sender;

                mBuffer = Encoding.UTF8.GetBytes(sp.ReadExisting());

                OnReceivedData((byte[])mBuffer.Clone());
            }
            catch (System.IO.IOException error)
            {
                throw (error);
            }
        }

        public void WriteData(byte[] write_data)
        {
            if(mSerial.IsOpen)
            {
                try
                {
                    mSerial.Write(write_data, 0 , write_data.Length);
                }
                catch (System.IO.IOException error)
                {
                    throw (error);
                }
            }
            else return;
        }
    }
}
