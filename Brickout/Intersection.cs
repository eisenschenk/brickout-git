using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brickout
{
    class Intersection
    {
        public Vector2 IntersectionPoint;
        public LineSegment BallLine;
        public LineSegment IntersectionLine;
        public int IndexGObject;
        public float DistanceToIntersection;
        public float LengthAfterIntersection;
        public float IntersectionLength;
        public GameObject IntersectingObject;

        public Intersection(GameObject gObject, LineSegment intersectionLine, LineSegment ballLine, int index)
        {
            IntersectionLine = intersectionLine;
            BallLine = ballLine;
            IntersectionPoint = intersectionLine.LineSegmentIntersection(ballLine);
            IndexGObject = index;
            IntersectionLength = intersectionLine.Getlength();
            IntersectingObject = gObject;
            DistanceToIntersection = new LineSegment(ballLine.Start, IntersectionPoint).Getlength();
            LengthAfterIntersection = ballLine.Getlength() - DistanceToIntersection;
        }


    }
}
