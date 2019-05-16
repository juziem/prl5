using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace lab5
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    /// 

    public partial class MainWindow : Window
    {
        const int port = 7532;
        static TcpListener listener;

        public MainWindow()
        {
            InitializeComponent();
        }

        static string Ch(string str)
        {
            char[] ch = str.ToCharArray();
            Array.Reverse(ch);
            string strRe = new string(ch);
            return strRe;
        }

        public void Process(TcpClient tcpClient)
        {
            TcpClient client = tcpClient;
            NetworkStream stream = null;

            try
            {
                //получение потока для обмена сообщениями 
                stream = client.GetStream();
                // буфер для получаемых данных 
                byte[] data = new byte[64];
                //цикл обработки сообщений 
                while (true)
                {
                    //объект, для формирования строк 
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    //до тех пор, пока в потоке есть данные 
                    do
                    {
                        //из потока считываются 64 байта и записываются в data 
                        bytes = stream.Read(data, 0, data.Length);
                        //из считанных данных формируется строка 
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));


                    }
                    while (stream.DataAvailable);

                    if (!builder.ToString().Equals("Клиент отключен"))
                    {
                        string message = Ch(builder.ToString());
                        data = Encoding.Unicode.GetBytes(message);
                        stream.Write(data, 0, data.Length);
                    }
                    else
                    {
                        list.Items.Add("Отключен");
                        break;
                    }
                }
            }

            catch 
            {
                //MessageBox(ex.Message); 
            }
            finally
            {
                if (stream != null)
                    stream.Close();
                if (client != null)
                    client.Close();
            }
        }

        public void listen()
        {
            try
            {
                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Thread clientThread = new Thread(() => Process(client));
                    Dispatcher.BeginInvoke(new Action(() => list.Items.Add("Подключено")));
                    clientThread.Start();
                }
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("Ошибка!");
            }
            catch (SocketException)
            {
                MessageBox.Show("Ошибка!");
            }

        }


        private void bt1_Click(object sender, RoutedEventArgs e) //подключение ip 
        {
            try
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
                listener.Start();
                Dispatcher.BeginInvoke(new Action(() => list.Items.Add("Сервер включен")));
                Thread clientTread = new Thread(() => listen());
                clientTread.Start();
            }
            catch (SocketException)
            {
                MessageBox.Show("Ошибка!");
            }
        }

        private void bt2_Click(object sender, RoutedEventArgs e)
        {
            if (listener != null)
                listener.Stop();
        }
    }
}
