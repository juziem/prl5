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

namespace clie
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const int port = 4168;
        const string address = "127.0.0.1";
        public MainWindow()
        {
            InitializeComponent();

            Console.Write("Введите свое имя:");
            string userName = Console.ReadLine();
            TcpClient client = null;
            try
            {
                client = new TcpClient(address, port);
                NetworkStream stream = client.GetStream();
                while (true)
                {
                    Console.Write(userName + ": ");
                    string message = Console.ReadLine();
                    message = String.Format("{0}: {1}", userName, message);
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                    data = new byte[64];
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);
                    message = builder.ToString();
                    Console.WriteLine("Сервер: {0}", message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                client.Close();
            }
        }
            
        
        private void msg_Click(object sender, RoutedEventArgs e)
        {
            Thread myThread1 = new Thread(new ThreadStart(Count1));
            myThread1.Start(); 
            Thread myThread2 = new Thread(new ThreadStart(Count2));
            myThread2.Start(); 
        }
         public void Count2()
        {
            for (int j = 1; j < 9; j++)
            {
                Dispatcher.BeginInvoke(new Action(
                () => list.Items.Add("Второй поток: " + j * j)));
                Thread.Sleep(100);
            }
        }
        public void Count1()
        {
            for (int j = 1; j < 9; j++)
            {
                Dispatcher.BeginInvoke(new Action(
                () => list.Items.Add("Первый поток: " + j * j)));
                Thread.Sleep(100);
            }
        

        }
    }
}
