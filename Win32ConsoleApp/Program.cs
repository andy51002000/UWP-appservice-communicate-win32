using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.AppService; //AppService
using Windows.Foundation.Collections;


namespace Win32ConsoleApp
{
    class Program
    {
        static AppServiceConnection connection = null;
        static void Main(string[] args)
        {
            new Thread(ThreadProc).Start();
            Console.Title = "Hello World";
            Console.WriteLine("This process runs at the full privileges of the user and has access to the entire public desktop API surface");
            Console.WriteLine("\r\nPress any key to exit ...");
            Console.ReadLine();

        }

        /// <summary>
        /// Creates the app service connection
        /// </summary>
        static async void ThreadProc()
        {
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "\t(a)ThreadProc Start!!");

            connection = new AppServiceConnection();
            connection.AppServiceName = "CommunicationService";
            connection.PackageFamilyName = Package.Current.Id.FamilyName;
            connection.RequestReceived += Connection_RequestReceived;

            AppServiceConnectionStatus status = await connection.OpenAsync();
            switch (status)
            {
                case AppServiceConnectionStatus.Success:
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Connection established - waiting for requests");
                    Console.WriteLine();
                    break;
                case AppServiceConnectionStatus.AppNotInstalled:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not installed.");
                    return;
                case AppServiceConnectionStatus.AppUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("The app AppServicesProvider is not available.");
                    return;
                case AppServiceConnectionStatus.AppServiceUnavailable:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("The app AppServicesProvider is installed but it does not provide the app service {0}.", connection.AppServiceName));
                    return;
                case AppServiceConnectionStatus.Unknown:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(string.Format("An unkown error occurred while we were trying to open an AppServiceConnection."));
                    return;
            }

        }

        /// <summary>
        /// Receives message from UWP app and sends a response back
        /// </summary>
        private static void Connection_RequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "\t(b) Get message from UWP!");

            string key = args.Request.Message.First().Key;

            ValueSet valueSet = new ValueSet();

 
            valueSet.Add("serialNumber", "12345");
 
            //Send back message to UWP
            args.Request.SendResponseAsync(valueSet).Completed += delegate { };
            Console.WriteLine(DateTime.Now.TimeOfDay.ToString() + "\tMessage to UWP has been sent!!");
        }

    }
}
