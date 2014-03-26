/*
Copyright (C) 2013 David Braam
Copyright (c) 2014, Lars Brubaker

This file is part of MatterSlice.

MatterSlice is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

MatterSlice is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with MatterSlice.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;

using MatterSlice.ClipperLib;

namespace MatterHackers.MatterSlice
{
    using Polygon = List<IntPoint>;
    using Polygons = List<List<IntPoint>>;

    public static class Inset
    {
        public static void generateInsets(SliceLayerPart part, int offset, int insetCount)
        {
            part.combBoundery = part.outline.Offset(-offset);
            if (insetCount == 0)
            {
                part.insets.Add(part.outline);
                return;
            }

            for (int i = 0; i < insetCount; i++)
            {
                part.insets.Add(new Polygons());
                part.insets[i] = part.outline.Offset(-offset * i - offset / 2);
                PolygonOptimizer.optimizePolygons(part.insets[i]);
                if (part.insets[i].Count < 1)
                {
                    part.insets.RemoveAt(part.insets.Count - 1);
                    break;
                }
            }
        }

        public static void generateInsets(SliceLayer layer, int offset, int insetCount)
        {
            for (int partNr = 0; partNr < layer.parts.Count; partNr++)
            {
                generateInsets(layer.parts[partNr], offset, insetCount);
            }

            //Remove the parts which did not generate an inset. As these parts are too small to print,
            // and later code can now assume that there is always minimum 1 inset line.
            for (int partIndex = 0; partIndex < layer.parts.Count; partIndex++)
            {
                if (layer.parts[partIndex].insets.Count < 1)
                {
                    layer.parts.RemoveAt(partIndex);
                    partIndex -= 1;
                }
            }
        }
    }
}
