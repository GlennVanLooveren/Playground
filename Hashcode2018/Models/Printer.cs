using Akka.Actor;
using System;

namespace Hashcode2018.Models;

public class Printer : ReceiveActor
{
    public Printer()
    {
        Receive<string>(Console.WriteLine);
    }
}

