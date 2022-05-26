using System;
using System.Drawing;

namespace Hashcode2018.Models;
public class Ride
{
    public int Id { get; set; }
    public Point Src { get; set; }
    public Point Dest { get; set; }
    public int Start { get; set; }
    public int End { get; set; }
    public bool Active { get; set; }
    public int Distance
    {
        get
        {
            return Math.Abs(Src.X - Dest.X) + Math.Abs(Src.Y - Dest.Y);
        }
    }
    public Ride(params int[] ps)
    {
        Src = new Point(ps[0], ps[1]);
        Dest = new Point(ps[2], ps[3]);
        Start = ps[4];
        End = ps[5];
    }
}
