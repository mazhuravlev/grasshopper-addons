using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GateButtonComponent : GH_Component
    {
        private int _in;
        private int _out;

        public GateButtonComponent()
            : base("GateButton", "GateButton", "GateButton", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("1a1d1f12-61ab-49d3-b977-37dc65a182a2");

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _in = pManager.AddBooleanParameter("Button", "B", "Button input", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _out = pManager.AddBooleanParameter("Gate", "G", "Gate", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var a = false;
            da.GetData(_in, ref a);
            da.SetData(_out, true);
            if (a)
            {
                this.ExpireOutput(_out);
            }
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }

     
    }
}