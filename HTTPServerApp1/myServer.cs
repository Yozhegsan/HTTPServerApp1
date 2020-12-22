using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HTTPServerApp1
{
    class myServer
    {
        TcpListener Listener; // Объект, принимающий TCP-клиентов

        // Запуск сервера
        public myServer(int Port)
        {
            Listener = new TcpListener(IPAddress.Any, Port); // Создаем "слушателя" для указанного порта
            Listener.Start(); // Запускаем его

            // В бесконечном цикле
            while (true)
            {
                // Принимаем нового клиента
                TcpClient Client = Listener.AcceptTcpClient();
                // Создаем поток
                Thread thread = new Thread(new ParameterizedThreadStart(ClientThread));
                // И запускаем этот поток, передавая ему принятого клиента
                thread.Start(Client);
            }
        }

        static void ClientThread(Object StateInfo)
        {
            // Просто создаем новый экземпляр класса Client и передаем ему приведенный к классу TcpClient объект StateInfo
            new myClient((TcpClient)StateInfo);
        }

        // Остановка сервера
        ~myServer()
        {
            // Если "слушатель" был создан
            if (Listener != null)
            {
                // Остановим его
                Listener.Stop();
            }
        }

        static void Main(string[] args)
        {
            // Создадим новый сервер на порту 80
            new myServer(80);
        }
    }
}
