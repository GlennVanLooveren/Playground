using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Hashcode2018.Models;
public class RideManager : ReceiveActor
{
    private readonly Ride[] _rides;
    private readonly int _bonus;
    private Dictionary<int, List<int>> _carRides;
    public RideManager(Ride[] rides, int bonus)
    {
        _rides = rides;
        _carRides = new Dictionary<int, List<int>>();
        Receive<FindRide>(FindRide);
        Receive<CompletedRide>(CompleteRide);
    }

    public void FindRide(FindRide ride)
    {
        var ordered = _rides.Where(x => CanFinish(ride.X, ride.Y, ride.Tick.Iteration, x)).OrderByDescending(x => RideScore(ride.X, ride.Y, ride.Tick.Iteration, x)).ToList();
        var found = ordered.FirstOrDefault();
        if (found != null)
        {
            _rides[found.Id] = null;
            ride.Car.Tell(new FoundRide(found, ride.Tick));
        }
        else
        {
            ride.Car.Tell(new NoRidesFound(ride.Tick));
        }
    }

    private void CompleteRide(CompletedRide ride)
    {
        if (_carRides.TryGetValue(ride.CarId, out List<int> rides))
            rides.Add(ride.Ride.Id);
        else
            _carRides.Add(ride.CarId, new List<int> { ride.Ride.Id });
    }

    private double RideScore(int x, int y, int i, Ride ride)
    {
        var distanceToStart = Math.Abs(x - ride.Src.X) + Math.Abs(y - ride.Src.Y);
        var idleTime = ride.Start - distanceToStart <= 0 ? 1 : ride.Start - distanceToStart;
        var bonus = i + distanceToStart <= ride.Start ? _bonus : 0;
        var startTime = i + distanceToStart >= ride.Start ? i + distanceToStart : ride.Start;
        var endTime = startTime + ride.Distance;
        return (ride.Distance + bonus) / idleTime - endTime;
    }

    private bool CanFinish(int x, int y, int i, Ride ride)
    {
        if (ride == null)
            return false;
        var distanceToStart = Math.Abs(x - ride.Src.X) + Math.Abs(y - ride.Src.Y);
        var timeNeeded = distanceToStart + ride.Distance;
        if (i + timeNeeded > ride.End)
            return false;
        return true;
    }
}

public class NoRidesFound
{
    public NoRidesFound(Tick tick)
    {
        Tick = tick;
    }

    public Tick Tick { get; set; }
}
