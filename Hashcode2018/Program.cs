using Akka.Actor;
using Hashcode2018.Models;
using System;
using System.IO;
using System.Linq;

namespace Hashcode2019;
class Program
{
    static void Main(string[] args)
    {
        var dir = Directory.GetFiles("./Input");


        foreach (var file in dir)
        {
            var system = ActorSystem.Create($"Hashcode-{file[8]}");
            var lines = File.ReadAllLines(file);
            var fl = lines[0].Split(' ');
            var rows = int.Parse(fl[0]);
            var col = int.Parse(fl[1]);
            var nCars = int.Parse(fl[2]);
            var nRides = int.Parse(fl[3]);
            var bonus = int.Parse(fl[4]);
            var duration = int.Parse(fl[5]);
            var rides = new Ride[nRides];
            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                rides[i - 1] = new Ride(line.Split(' ').Select(x => int.Parse(x)).ToArray());
                rides[i - 1].Id = i - 1;
            }
            var carPark = system.ActorOf(Props.Create(() => new CarPark(nCars, duration, bonus, rides)));
            carPark.Tell(new Start());
            system.WhenTerminated.Wait();
            Console.WriteLine("Click any button to continue");
            Console.ReadLine();
        }
    }
}
