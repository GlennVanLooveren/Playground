using Akka.Actor;
using Akka.Util.Internal;
using System;
using System.Collections.Generic;

namespace Hashcode2018.Models;
public class CarPark : ReceiveActor
{
    private IActorRef _rideManager;
    private IActorRef _ridePublisher;
    private IActorRef _moveTracker;
    private IActorRef _printer;
    private IActorRef _scorer;
    private readonly HashSet<IActorRef> _cars;
    private readonly int _nCars;
    private int _carCounter;
    private int _maxI;
    private Ride[] _rides;
    private int _bonus;
    public CarPark(int cars, int iterations, int bonus, Ride[] rides)
    {
        Receive<Tick>(Tick);
        Receive<FindRide>(Dispatch);
        Receive<Moved>(CarMoved);
        Receive<Start>(start => Self.Tell(new Tick(0)));
        Receive<CompletedRide>(ride => _ridePublisher.Tell(ride));
        _cars = new HashSet<IActorRef>();
        _nCars = cars;
        _carCounter = 0;
        _maxI = iterations;
        _rides = rides;
        _bonus = bonus;
    }

    protected override void PreStart()
    {
        for (int i = 0; i < _nCars; i++)
        {
            _cars.Add(Context.ActorOf(Props.Create(() => new Car(i)), $"car-{i}"));
        }
        _ridePublisher = Context.ActorOf(Props.Create(() => new RidePublisher()));
        _rideManager = Context.ActorOf(Props.Create(() => new RideManager(_rides, _bonus)));
        _scorer = Context.ActorOf(Props.Create(() => new ScoreCalculator(_bonus, _ridePublisher)));
        _moveTracker = Context.ActorOf(Props.Create(() => new MoveTracker()));
        _printer = Context.ActorOf(Props.Create(() => new Printer()));
    }

    public void Tick(Tick tick)
    {
        _cars.ForEach(car => car.Tell(tick));
    }

    public void Dispatch(FindRide ride)
    {
        _rideManager.Tell(ride);
    }

    public void CarMoved(Moved moved)
    {
        _carCounter++;
        //_moveTracker.Tell(moved);
        if (_carCounter == _nCars)
        {
            _carCounter = 0;
            if (moved.Iteration == _maxI)
            {
                _scorer.Tell(new PrintScore(_printer, false));
                Context.System.Terminate();
            }
            else
                Self.Tell(new Tick(moved.Iteration + 1));
        }
    }
}

public class Tick
{
    public Tick(int i)
    {
        Iteration = i;
    }

    public int Iteration { get; set; }
}

public class Start
{

}
