using Akka.Actor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Hashcode2018.Models;
public class MoveTracker : ReceiveActor
{
    private Dictionary<int, List<Point>> _movement;

    public MoveTracker()
    {
        _movement = new Dictionary<int, List<Point>>();
        Receive<Moved>(AddMovement);
    }

    public void AddMovement(Moved move)
    {
        Point prev = new Point(0, 0);
        var dest = new Point(move.X, move.Y);
        if (_movement.TryGetValue(move.CarId, out List<Point> val))
            prev = val.Last();
        else
            _movement.Add(move.CarId, new List<Point> { prev });
        _movement[move.CarId].Add(dest);
        //Console.WriteLine($"on i {move.Iteration}, car {move.CarId} moves from {prev} to {dest}");
    }
}