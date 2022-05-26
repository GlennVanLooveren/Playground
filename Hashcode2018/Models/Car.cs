using Akka.Actor;
using System.Drawing;

namespace Hashcode2018.Models;
public class Car : ReceiveActor
{
    private bool _active;
    public Car(int id)
    {
        X = 0;
        Y = 0;
        _active = true;
        Receive<Tick>(Tick);
        Receive<FoundRide>(SetRide);
        Receive<NoRidesFound>(HandleNoRidesFound);
        Id = id;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public Ride Ride { get; set; }
    public int Id { get; set; }
    private void Tick(Tick tick)
    {
        if (!_active)
            Context.Parent.Tell(new Moved(tick.Iteration, Id, X, Y));
        else if (Ride == null)
        {
            Context.Parent.Tell(new FindRide(X, Y, tick, Self));
        }
        else
            Move(tick);
    }

    private void Move(Tick tick)
    {
        if (Ride.Active)
            MoveToDestination(tick);
        else
            MoveToRide(tick);
    }

    private void MoveToRide(Tick tick)
    {
        if (tick.Iteration >= Ride.Start && X == Ride.Src.X && Y == Ride.Src.Y)
        {
            //System.Console.WriteLine($"{Id} starting ride {Ride.Id} on i {tick.Iteration}");
            Ride.Active = true;
            MoveToDestination(tick);
        }
        else
        {
            MoveToPoint(Ride.Src, tick);
        }
    }

    private void MoveToDestination(Tick tick)
    {
        MoveToPoint(Ride.Dest, tick);
        if (X == Ride.Dest.X && Y == Ride.Dest.Y)
            CompleteRide(tick);
    }

    private void MoveToPoint(Point point, Tick tick)
    {
        if (X < point.X)
            X++;
        else if (X > point.X)
            X--;
        else if (Y < point.Y)
            Y++;
        else if (Y > point.Y)
            Y--;
        Context.Parent.Tell(new Moved(tick.Iteration, Id, X, Y));
    }

    private void CompleteRide(Tick tick)
    {
        Context.Parent.Tell(new CompletedRide(tick.Iteration + 1, Ride, Id));
        Ride = null;
    }

    private void SetRide(FoundRide found)
    {
        Ride = found.Ride;
        Self.Tell(found.Tick);
    }

    private void HandleNoRidesFound(NoRidesFound msg)
    {
        _active = false;
        Self.Tell(msg.Tick);
    }
}

public class FindRide
{
    public FindRide(int x, int y, Tick i, IActorRef car)
    {
        X = x;
        Y = y;
        Tick = i;
        Car = car;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public Tick Tick { get; set; }
    public IActorRef Car { get; set; }
}


public class CompletedRide
{
    public CompletedRide(int i, Ride ride, int carid)
    {
        Iteration = i;
        Ride = ride;
        CarId = carid;
    }

    public int CarId { get; set; }
    public int Iteration { get; set; }
    public Ride Ride { get; set; }
}

public class Moved
{
    public Moved(int i, int carId, int x, int y)
    {
        Iteration = i;
        CarId = carId;
        X = x;
        Y = y;
    }

    public int Iteration { get; set; }
    public int CarId { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
}

public class FoundRide
{
    public FoundRide(Ride ride, Tick tick)
    {
        Ride = ride;
        Tick = tick;
    }

    public Ride Ride { get; set; }
    public Tick Tick { get; set; }
}
