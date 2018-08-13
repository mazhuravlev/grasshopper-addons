using System;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Attributes;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Special;

namespace GHAddons.Components
{
    public class GateParam : Param_GenericObject
    {
        public override Guid ComponentGuid => new Guid("{273e5834-2f3f-47f4-a1c1-e01ca4851a32}");

        public override string Name => "GateParam";

        public override string Description => "GateParam";

        public override GH_Exposure Exposure => GH_Exposure.hidden;

        protected override void OnVolatileDataCollected()
        {
            var gate = (GateComponent)Attributes.GetTopLevel.DocObject;
            gate.SetActivation(this);
            base.OnVolatileDataCollected();
        }
    }
}