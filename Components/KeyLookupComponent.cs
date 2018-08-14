using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class KeyLookupComponent : GH_Component
    {
        private int _keysIn;
        private int _valuesOut;
        private int _valuesIn;
        private int _searchIn;

        public KeyLookupComponent()
            : base("KeyLookup", "KeyLookup", "KeyLookup", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("1a1d1f12-61ab-49d3-b171-37dc65a182a2");

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _keysIn = pManager.AddTextParameter("Keys", "K", "Keys", GH_ParamAccess.list);
            _valuesIn = pManager.AddGenericParameter("Values", "V", "Values", GH_ParamAccess.list);
            _searchIn = pManager.AddTextParameter("Search", "S", "Search", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _valuesOut = pManager.AddGenericParameter("Values", "V", "Values", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var keys = new List<string>();
            da.GetDataList(_keysIn, keys);
            var values = new List<object>();
            da.GetDataList(_valuesIn, values);
            if (values.Count != keys.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "keys.Count != values.Count");
                return;
            }

            var search = new List<string>();
            da.GetDataList(_searchIn, search);

            var dict = new Dictionary<string, object>();
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                if(key == null) continue;
                if (dict.ContainsKey(key)) continue;
                dict[key] = values[i];
            }

            var result = new object[search.Count];
            for (var i = 0; i < search.Count; i++)
            {
                var searchKey = search[i];
                if (dict.ContainsKey(searchKey))
                {
                    result[i] = dict[searchKey];
                }
                else
                {
                    result[i] = null;
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Warning, $"Search key [{searchKey}] not found");
                }
            }
            da.SetDataList(_valuesOut, result);
        }
    }
}