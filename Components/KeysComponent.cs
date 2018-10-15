using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    using Grasshopper.Kernel.Data;
    using Grasshopper.Kernel.Types;

    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class KeysComponent : GH_Component
    {
        private int _searchIn;
        private int _keysOut;
        private string _search;

        public KeysComponent()
            : base("Keys", "Keys", "Keys", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("801a613cfb8b4eab8514cf6997cfe25d");
        
        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _searchIn = pManager.AddTextParameter("Search", "S", "Search", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _keysOut = pManager.AddTextParameter("Keys", "K", "Keys", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            _search = "";
            da.GetData(_searchIn, ref _search);
            if (string.IsNullOrWhiteSpace(_search))
            {
                return;
            }
            
            var storage = OnPingDocument().Objects.SingleOrDefault(x => x is StorageComponent) as StorageComponent;
            if (storage == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There must be exactly 1 Storage Component in document");
                return;
            }
            storage.OnStorageChanged -= StorageChanged;
            storage.OnStorageChanged += StorageChanged;
            var result = storage.SearchKeys(_search);
            da.SetDataList(_keysOut, result);
        }
        
        private void StorageChanged(object sender, StorageChangedEventArgs e)
        {
            if (e.Key.StartsWith(_search))
            {
                ExpireSolution(true);
            }
        }
    }
}