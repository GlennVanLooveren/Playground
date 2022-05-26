using Akka.Actor;
using Akka.Util.Internal;
using System.Collections.Generic;

namespace Hashcode2018.Models;
public class RidePublisher : ReceiveActor
{
    private HashSet<IActorRef> _subscribers;
    public RidePublisher()
    {
        _subscribers = new HashSet<IActorRef>();
        Receive<CompletedRide>(Publish);
        Receive<SubscribeToRide>(Subscribe);
        Receive<UnSubScribeToRides>(UnSubscribe);
    }

    private void Publish(CompletedRide msg)
    {
        _subscribers.ForEach(sub => sub.Tell(msg));
    }

    private void Subscribe(SubscribeToRide msg)
    {
        _subscribers.Add(msg.Sub);
    }

    private void UnSubscribe(UnSubScribeToRides msg)
    {
        _subscribers.Remove(msg.UnSub);
    }
}

public class SubscribeToRide
{
    public SubscribeToRide(IActorRef sub)
    {
        Sub = sub;
    }

    public IActorRef Sub { get; set; }
}

public class UnSubScribeToRides
{

    public IActorRef UnSub { get; set; }
}