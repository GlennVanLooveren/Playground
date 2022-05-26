using Akka.Actor;
using System.Collections.Generic;
using System.Text;

namespace Hashcode2018.Models;
public class ScoreCalculator : ReceiveActor
{
    private int _score;
    private int _bonus;
    private Dictionary<int, int> _allScores;
    public ScoreCalculator(int bonus, IActorRef publisher)
    {
        _score = 0;
        _bonus = bonus;
        _allScores = new Dictionary<int, int>();
        Receive<CompletedRide>(AddToScore);
        Receive<PrintScore>(Print);
        publisher.Tell(new SubscribeToRide(Self));
    }

    private void AddToScore(CompletedRide ride)
    {
        var rideScore = ride.Ride.Distance;
        if (ride.Ride.Start == ride.Iteration - ride.Ride.Distance)
            rideScore += _bonus;
        _allScores.Add(ride.Ride.Id, rideScore);
        _score += rideScore;
    }

    private void Print(PrintScore msg)
    {
        var builder = new StringBuilder();

        if (msg.FullPrint)
            foreach (var pair in _allScores)
                builder.AppendLine($"ride {pair.Key} completed for {pair.Value} points");
        builder.AppendLine($"Total score for round: {_score}");
        msg.Printer.Tell($"{builder.ToString()}");
    }
}

public class PrintScore
{
    public PrintScore(IActorRef printer, bool full)
    {

        Printer = printer;
        FullPrint = full;
    }

    public IActorRef Printer { get; set; }
    public bool FullPrint { get; set; }
}