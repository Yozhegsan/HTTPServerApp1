using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HTTPClientApp1
{
    public class socketctl
    {


        private IPEndPoint ipPoint;
        private Socket socket;

        private bool Connected = false;

        //###########################################################################################

        public socketctl()
        {

        }

        public bool Connect(string ip, int port)
        {
            if (Connected) return false;
            try
            {
                ipPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(ipPoint);
                Connected = true;
                return true;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                return false;
            }
        }

        public bool Disconnect()
        {
            if (!Connected) return false;
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                socket.Dispose();
                ipPoint = null;
                Connected = false;
                return true;
            }
            catch
            {
                return false;
            }

        }

        private string SendData(string data)
        {
            byte[] SendMas = Encoding.ASCII.GetBytes(data);                                   
            string Headers = "HTTP/1.1 200 OK\nContent-Type: text/html\nContent-Length: " + data.Length + "\n\n";
            byte[] HeadersBuffer = Encoding.ASCII.GetBytes(Headers);

            socket.GetStream().Write(HeadersBuffer, 0, HeadersBuffer.Length);
            socket.GetStream().Write(SendMas, 0, SendMas.Count());
            return "";
        }

        private byte[] SendCommand(ref byte[] data, byte answerSize)
        {
            if (!Connected)
            {
                byte[] b = new byte[1];
                b[0] = 255;
                return b;
            }
            try
            {
                socket.Send(data);
                data = new byte[1 + answerSize];
                StringBuilder builder = new StringBuilder();
                int bytes = 0;
                do
                {
                    bytes = socket.Receive(data, data.Length, 0);
                }
                while (socket.Available > 0);

                return data;
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                byte[] b = new byte[1];
                b[0] = 254;
                return b;
            }
        }

        private string ByteToHex(byte b)
        {
            return (b < 16 ? "0x0" : "0x") + Convert.ToString(b, 16);
        }

        private short TwoByteToShort(byte hi, byte lo)
        {
            return (short)(hi * 256 + lo);
        }

        /// <summary>
        /// command: 0x01
        /// </summary>
        /// <returns></returns>
        public bool TestConnection()
        {
            byte[] mas = new byte[1];
            mas[0] = 0x01;
            byte[] answer = SendCommand(ref mas, 0);
            return answer[0] == 0x01 ? true : false;
        }

        /// <summary>
        /// command: 0x22
        /// </summary>
        /// <param name="relayNum">0 or 1</param>
        /// <param name="state">0 or 1</param>
        /// <param name="length">0 - 255 seconds. 0 - infinity</param>
        /// <returns></returns>
        public string RelaySetState(byte relayNum, byte state, byte length)
        {
            byte[] mas = new byte[4];
            mas[0] = 0x22;
            mas[1] = relayNum;
            mas[2] = state;
            mas[3] = length;
            string result = "";
            byte[] answer = SendCommand(ref mas, 3);
            string nl = Environment.NewLine;
            if (answer[0] == 0x22)
            {
                result += "Command: " + ByteToHex(answer[0]) + nl;
                result += "Relay number: " + answer[1] + nl;
                result += "Relay state: " + answer[2] + nl;
                result += "Length: " + answer[3] + "s";
            }
            else
            {
                result = "Error # " + ByteToHex(answer[0]);
            }
            return result;
        }

        public string GetTypeAndVersion()
        {
            byte[] mas = new byte[1];
            mas[0] = 0x03;
            string result = "";
            byte[] answer = SendCommand(ref mas, 4);
            string nl = Environment.NewLine;
            short n = TwoByteToShort(answer[2], answer[3]);
            result += "Command: " + ByteToHex(answer[0]) + nl;
            result += "Equipment type: " + answer[1] + nl;
            result += "Firmvare version: " + n + nl;
            result += "Firmware type: " + answer[4];
            return result;
        }

        public string GetSN()
        {
            byte[] mas = new byte[1];
            mas[0] = 0x04;
            string result = "";
            byte[] answer = SendCommand(ref mas, 2);
            string nl = Environment.NewLine;
            short n = TwoByteToShort(answer[1], answer[2]);
            result += "Command: " + ByteToHex(answer[0]) + nl;
            result += "Serial number: " + n + nl;
            return result;
        }

        public string GetInfo()
        {
            byte[] mas = new byte[1];
            mas[0] = 0x23;
            string result = "";
            byte[] answer = SendCommand(ref mas, 4);
            string nl = Environment.NewLine;
            short n = TwoByteToShort(answer[1], answer[2]);
            result += "Command: " + ByteToHex(answer[0]) + nl;
            result += "Digital input 0 - " + answer[1] + nl;
            result += "Digital input 1 - " + answer[2] + nl;
            result += "Relay 0 - " + answer[3] + nl;
            result += "Relay 1 - " + answer[4];
            return result;
        }

    }
}
