using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ReaderComponent : GH_Component
    {
        private int _keysIn;
        private int _valuesOut;
        private List<string> _keys = new List<string>();

        public ReaderComponent()
            : base("Reader", "Reader", "Reader", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("14c00e6e-ffa6-424a-a78f-9a4044b73c16");
        
        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _keysIn = pManager.AddTextParameter("Key", "K", "Key", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _valuesOut = pManager.AddTextParameter("Value", "V", "Value", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var keys = new List<string>();
            da.GetDataList(_keysIn, keys);
            _keys = keys;
            var storage = OnPingDocument().Objects.SingleOrDefault(x => x is StorageComponent) as StorageComponent;          
            if (storage == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There must be exactly one Storage Component in document");
                return;
            }

            storage.OnStorageChanged -= StorageChanged;
            storage.OnStorageChanged += StorageChanged;

            var s = storage.GetStrings(keys);
            da.SetDataList(_valuesOut, s);
            this.ExpireOutput(_valuesOut);
        }

        private void StorageChanged(object sender, StorageChangedEventArgs e)
        {
            if (_keys.Contains(e.Key))
            {
                ExpireSolution(true);
            }
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }
    }
}