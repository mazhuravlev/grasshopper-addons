using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class WriterComponent : GH_Component
    {
        private int _keyIn;
        private int _valueIn;

        public WriterComponent()
            : base("Writer", "Writer", "Writer", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("df246d1a4d584dbeb88303b33f1ba0eb");
        
        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _keyIn = pManager.AddTextParameter("Key", "K", "Key", GH_ParamAccess.list);
            _valueIn = pManager.AddTextParameter("Value", "V", "Value", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var keys = new List<string>();
            var values = new List<string>();
            da.GetDataList(_keyIn, keys);
            da.GetDataList(_valueIn, values);
            var storage = OnPingDocument().Objects.SingleOrDefault(x => x is StorageComponent) as StorageComponent;
            if (storage == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There must be exactly 1 Storage Component in document");
                return;
            }
            storage.SetStrings(keys, values);
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }
    }
}