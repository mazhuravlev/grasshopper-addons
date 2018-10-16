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
    public class DeletorComponent : GH_Component
    {
        private int _keyIn;
        private List<string> _keys = new List<string>();

        public DeletorComponent()
            : base("Deletor", "Deletor", "Deletor", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("3b65ace05ffd47ea9d07ada1038f2a66");
        
        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _keyIn = pManager.AddTextParameter("Keys", "K", "Keys", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            da.GetDataTree(_keyIn, out GH_Structure<GH_String> keysTree);
            _keys = keysTree.Where(x => x.IsValid).Select(x => x.Value).ToList();
            var storage = OnPingDocument().Objects.SingleOrDefault(x => x is StorageComponent) as StorageComponent;
            if (storage == null)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "There must be exactly 1 Storage Component in document");
                return;
            }
            storage.DeleteKeys(_keys);
        }
        
        protected override void ExpireDownStreamObjects()
        {
            //
        }
    }
}