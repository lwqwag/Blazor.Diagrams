﻿using Blazor.Diagrams.Core.Geometry;
using Blazor.Diagrams.Core.Models;
using Blazor.Diagrams.Core.Models.Base;

namespace Blazor.Diagrams.Core.Anchors
{
    public class ShapeIntersectionAnchor : Anchor
    {
        public ShapeIntersectionAnchor(NodeModel node, Point? offset = null) : base(node, offset) { }

        public override Point? GetPosition(BaseLinkModel link, Point[] route)
        {
            if (Node.Size == null)
                return null;

            var isTarget = link.Target == this;
            var nodeCenter = Node.GetBounds()!.Center;
            Point? pt;
            if (route.Length > 0)
            {
                pt = route[isTarget ? ^1 : 0];
            }
            else
            {
                pt = GetOtherPosition(link, isTarget);
            }

            if (pt is null) return null;

            var line = new Line(pt, nodeCenter);
            var intersections = Node.GetShape().GetIntersectionsWithLine(line);
            return GetClosestPointTo(intersections, pt);
        }
    }
}
