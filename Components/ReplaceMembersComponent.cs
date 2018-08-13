using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ReplaceMembersComponent : GH_Component
    {
        private int _listIn;
        private int _findIn;
        private int _replaceIn;
        private int _resultOut;

        public ReplaceMembersComponent()
            : base("ReplaceMembers", "ReplaceMembers", "ReplaceMembers", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("ef9c1a9e-0c95-44b4-b4d5-db84ef239336");

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _listIn = pManager.AddGenericParameter("List", "L", "List", GH_ParamAccess.list);
            _findIn = pManager.AddGenericParameter("Find", "F", "Find", GH_ParamAccess.list);
            _replaceIn = pManager.AddGenericParameter("Replace", "R", "Replace", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
           _resultOut = pManager.AddGenericParameter("Result", "R", "Result", GH_ParamAccess.list);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var list = new List<IGH_Goo>();
            var find = new List<IGH_Goo>();
            var replace = new List<IGH_Goo>();
            da.GetDataList(_listIn, list);
            da.GetDataList(_findIn, find);
            da.GetDataList(_replaceIn, replace);
            if (find.Count != replace.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "find.Count != replace.Count");
                return;
            }
            var dict = new Dictionary<object, object>();
            for (var i = 0; i < find.Count; i++)
            {
                var currentFind = find[i].ScriptVariable();
                if (!dict.ContainsKey(currentFind))
                {
                    dict[currentFind] = replace[i].ScriptVariable();
                }
            }

            var result = list.Select(x => x.ScriptVariable()).Select(x => dict.ContainsKey(x) ? dict[x] : x).ToList();
            da.SetDataList(_resultOut, result);
        }
    }
}