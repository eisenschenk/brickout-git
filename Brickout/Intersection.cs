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
        public Lines BallLine;
        public Lines IntersectionLine;
        public int IndexGObject;
        public float DistanceToIntersection;
        public float LengthAfterIntersection;
        public float IntersectionLength;
        public GameObject IntersectingObject;

        public Intersection(GameObject gObject, Lines intersectionLine, Lines ballLine, int index)
        {
            IntersectionLine = intersectionLine;
            BallLine = ballLine;
            IntersectionPoint = intersectionLine.LineSegmentIntersection(ballLine);
            IndexGObject = index;
            IntersectionLength = intersectionLine.Getlength();
            IntersectingObject = gObject;
            DistanceToIntersection = new Lines(ballLine.Start, IntersectionPoint).Getlength();
            LengthAfterIntersection = ballLine.Getlength() - DistanceToIntersection;
        }


    }
}
