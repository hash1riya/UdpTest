using System.Text;
using System.Net;
using System.Net.Sockets;

class Program
{
    // змінні, необхідні для налаштування підключення:
    // віддалений хост та порти - віддалений та локальний
    static UdpClient client = new("127.0.0.1", 1025);
    //static IPEndPoint local = new(IPAddress.Parse("127.0.0.1"), 1025);
    static IPEndPoint remote = new(IPAddress.Parse("127.0.0.1"), 1024);

    [STAThread]
    static void Main(string[] args)
    {
        try
        {
            // окремий потік для зчитування у методі ThreadFuncReceive
            // цей метод викликає метод Receive() класу UdpClient,
            // який блокує поточний потік, тому необхідний окремий
            // потік
            Thread thread = new Thread(new ThreadStart(ThreadFuncReceive));
            // створення фонового потоку
            thread.IsBackground = true;
            // запуск потоку
            thread.Start();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Client started...");
            Console.ForegroundColor = ConsoleColor.Red;
            while (true)
            {
                SendData(Console.ReadLine());
            }
        }
        catch (FormatException formExc)
        {
            Console.WriteLine("Перетворення неможливе :" + formExc);
        }
        catch (Exception exc)
        {
            Console.WriteLine("Помилка : " + exc.Message);
        }
    }

    static void ThreadFuncReceive()
    {
        try
        {
            while (true)
            {
                // отримання дейтаграми
                byte[] responce = client.Receive(ref remote);
                // перетворення на рядок
                string strResult = Encoding.Unicode.GetString(responce);
                Console.ForegroundColor = ConsoleColor.Green;
                // виведення на екран
                Console.WriteLine($"{client.Client.RemoteEndPoint}: {strResult}");
                Console.ForegroundColor = ConsoleColor.Red;
            }
        }
        catch (SocketException sockEx)
        {
            Console.WriteLine("Помилка сокета: " + sockEx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Помилка : " + ex.Message);
        }
    }

    static void SendData(string datagramm)
    {
        try
        {
            client.Connect(remote);
            byte[] bytes = Encoding.Unicode.GetBytes(datagramm);
            client.Send(bytes);
        }
        catch (SocketException sockEx)
        {
            Console.WriteLine("Помилка сокета: " + sockEx.Message);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Помилка : " + ex.Message);
        }
    }
}