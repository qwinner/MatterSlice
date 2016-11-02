﻿using System.Collections.Generic;
using MSClipperLib;
using QuadTree;

namespace MatterHackers.MatterSlice
{
	public class CloseSegmentsIterator
	{
		QuadTree<int> tree;
		List<int> collisions = new List<int>();
		static bool newMethod = true;

		public CloseSegmentsIterator(List<Segment> polySegments, long overlapAmount)
		{
			IntRect bounds = new IntRect();

			if (newMethod)
			{
				List<Quad> quads = new List<Quad>(polySegments.Count);
				for (int i = 0; i < polySegments.Count; i++)
				{
					var quad = new Quad(polySegments[i].Left - overlapAmount,
						polySegments[i].Bottom - overlapAmount,
						polySegments[i].Right + overlapAmount,
						polySegments[i].Top + overlapAmount);

					if(i==0)
					{
						bounds = new IntRect(quad.MinX, quad.MinY, quad.MaxX, quad.MaxY);
					}
					else
					{
						bounds.ExpandToInclude(new IntRect(quad.MinX, quad.MinY, quad.MaxX, quad.MaxY));
					}

					quads.Add(quad);
				}

				tree = new QuadTree<int>(5, new Quad(bounds.left, bounds.top, bounds.right, bounds.bottom));
				for (int i = 0; i < quads.Count; i++)
				{
					tree.Insert(i, quads[i]);
				}
			}
		}

		public IEnumerable<int> GetTouching(int firstSegmentIndex, int endIndexExclusive)
		{
			if (newMethod)
			{
				if (tree.FindCollisions(firstSegmentIndex, ref collisions))
				{
					for (int collisionIndex = 0; collisionIndex < collisions.Count; collisionIndex++)
					{
						int segmentIndex = collisions[collisionIndex];
						if (segmentIndex <= firstSegmentIndex)
						{
							continue;
						}

						yield return segmentIndex;
					}
				}
			}
			else
			{
				for (int i = firstSegmentIndex; i < endIndexExclusive; i++)
				{
					yield return i;
				}
			}
		}
	}

	public class ClosePointsIterator
	{
		private long distanceNeedingAdd;
		private List<IntPoint> pointsToSplitOn;

		public ClosePointsIterator(List<IntPoint> pointsToSplitOn, long distanceNeedingAdd)
		{
			this.pointsToSplitOn = pointsToSplitOn;
			this.distanceNeedingAdd = distanceNeedingAdd;
		}
	}
}