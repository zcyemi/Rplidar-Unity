using System;

namespace RplidarNetCore
{
    class Program
    {
        static void Main(string[] args)
        {
            LidarData[] data = new LidarData[720];

            String port;
            while(true){
                Console.WriteLine("input COM port:");
                var comport = Console.ReadLine().ToUpper();
                int p = 0;
                if(comport.StartsWith("COM") && comport.Length ==4 && int.TryParse(comport.Substring(3),out p)){
                    port = comport;
                    break;
                }
            }

            RplidarBinding.OnConnect(port);
            Console.WriteLine("on conn");

            RplidarBinding.StartMotor();
            Console.WriteLine("start motor");

            RplidarBinding.StartScan();
            Console.WriteLine("start scan");

            var datalen = RplidarBinding.GetData(ref data);
            for(var i=0;i<datalen;i++){
                Console.WriteLine($"{data[i].distant} {data[i].theta} {data[i].quality}");
            }

            Console.ReadLine();

            Console.WriteLine("end scan");
            RplidarBinding.EndScan();

            Console.WriteLine("end motor");
            RplidarBinding.EndMotor();

            Console.ReadLine();
            Console.WriteLine("disconnect");
            RplidarBinding.OnDisconnect();
        }
    }
}
