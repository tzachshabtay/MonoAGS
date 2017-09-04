using System.Collections.Generic;
using AGS.API;

namespace AGS.Engine
{
    public class AGSClippingPlane : IClippingPlane
    {
        public AGSClippingPlane(IObject planeObject, bool isPlaneObjectClipped, 
                                IReadOnlyList<IRenderLayer> layersToClip = null)
        {
            PlaneObject = planeObject;
            IsPlaneObjectClipped = isPlaneObjectClipped;
            LayersToClip = layersToClip;
        }

        public IObject PlaneObject { get; private set; }

        public bool IsPlaneObjectClipped { get; private set; }

        public IReadOnlyList<IRenderLayer> LayersToClip { get; private set; }
    }
}
