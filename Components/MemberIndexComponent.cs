using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class MemberIndexComponent : GH_Component
    {
        private int _listIn;
        private int _membersIn;
        private int _indicesOut;
        private int _countOut;

        public MemberIndexComponent()
            : base("MemberIndex", "MemberIndex", "MemberIndex", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("e43f56b2-0a3a-4381-a4d4-e70098f8d1f4");

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _listIn = pManager.AddGenericParameter("List", "L", "List", GH_ParamAccess.list);
            _membersIn = pManager.AddGenericParameter("Members", "M", "Members", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _indicesOut = pManager.AddIntegerParameter("Indices", "I", "Indices", GH_ParamAccess.tree);
            _countOut = pManager.AddIntegerParameter("Count", "C", "Count", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var list = new List<object>();
            var members = new List<object>();
            da.GetDataList(_listIn, list);
            da.GetDataList(_membersIn, members);
            var dict = new Dictionary<object, (List<int>,int)>();
            for (var i = 0; i < list.Count; i++)
            {
                var currentItem = list[i];
                if (dict.ContainsKey(currentItem))
                {
                    var rec = dict[currentItem];
                    dict[currentItem] = (rec.Item1.Concat(new List<int> { i }).ToList(), rec.Item2 + 1);
                }
                else
                {
                    dict[currentItem] = (new List<int> { i }, 1);
                }
            }

            var indices = new DataTree<int>();
            var count = new DataTree<int>();
            for (var i = 0; i < members.Count; i++)
            {
                var path = new GH_Path(i);
                var member = members[i];
                if (dict.ContainsKey(member))
                {
                    indices.AddRange(dict[member].Item1, path);
                    count.Add(dict[member].Item2);
                }
                else
                {
                    indices.AddRange(new int[]{}, path);
                    count.Add(0, path);
                }
            }

            da.SetDataTree(_indicesOut, indices);
            da.SetDataTree(_countOut, count);
        }
    }
}