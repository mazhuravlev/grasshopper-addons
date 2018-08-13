using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Rhino.Geometry.Intersect;

namespace GHAddons.Components
{
    public class ClickablePreviewComponent : GH_Component
    {
        internal bool EnabledGlobal;
        internal bool InActiveDocument;
        public bool SingleSelect = true;
        internal Line? MouseLine;
        private Feedback _feed;
        private GH_Document _doc;
        private List<int> _selectedIndices = new List<int>();
        private int _selectedMeshesOut;
        private int _selectedIndicesOut;

        public ClickablePreviewComponent() : base("ClickablePreview", "ClickablePreview", "ClickablePreview", "Test", "Test")
        {
            MouseLine = new Line?();
            _feed = new Feedback(this);
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            pManager.AddBooleanParameter("Enable", "E", "Enable click-to-select mode.", 0);
            pManager.AddMeshParameter("Meshes", "M", "The meshes you want to be able to click on", GH_ParamAccess.list);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _selectedMeshesOut = pManager.AddMeshParameter("Selected Meshes", "M", "The meshes that have been clicked on", GH_ParamAccess.list);
            _selectedIndicesOut = pManager.AddIntegerParameter("Selected Indices", "I", "The indices of the selected meshes", (GH_ParamAccess)1);
            //pManager.AddLineParameter("Mouse Line", "ML", "The line that represents the mouse's location", (GH_ParamAccess)2);
        }

        protected override void BeforeSolveInstance()
        {
            _doc = OnPingDocument();
            InActiveDocument = Instances.ActiveCanvas.Document == _doc && Instances.ActiveCanvas.Document.Context == GH_DocumentContext.Loaded;
            // ISSUE: method pointer
            _doc.ObjectsDeleted -= ObjectsDeleted;
            _doc.ObjectsDeleted += ObjectsDeleted;
            Instances.ActiveCanvas.Document.ContextChanged -= ContextChanged;
            Instances.ActiveCanvas.Document.ContextChanged += ContextChanged;
            base.BeforeSolveInstance();
        }

        private void ContextChanged(object sender, GH_DocContextEventArgs e)
        {
            InActiveDocument = e.Document == _doc && e.Context == GH_DocumentContext.Loaded;
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var flag = false;
            var meshList = new List<Mesh>();
            // ISSUE: cast to a reference type
            if (!da.GetData("Enable", ref flag) || !da.GetDataList("Meshes", meshList))
                return;
            EnabledGlobal = flag;
            var selectedMeshes = ShiftPressed ? _selectedIndices.Select(x => meshList[x]).ToList() : new List<Mesh>();
            var intersectParams = new List<double>();
            var selectedIndices = new List<int>();
            if (!MouseLine.HasValue)
            {
                return;
            }

            var meshesToIntersect = meshList.Where(x => !selectedMeshes.Contains(x)).ToList();
            Debug.Assert(meshesToIntersect.Count + selectedMeshes.Count == meshList.Count);
            foreach (var mesh in meshesToIntersect)
            {
                var line = MouseLine.Value;
                var @from = line.From;
                var to = line.To;
                var vector3D = to - @from;
                var ray3D = new Ray3d(@from, vector3D);
                var num = Intersection.MeshRay(mesh, ray3D);
                // ReSharper disable once InvertIf
                if (num >= 0.0)
                {
                    intersectParams.Add(num);
                    selectedMeshes.Add(mesh);
                }
            }
            var selectedSet = new HashSet<Mesh>(selectedMeshes);
            selectedIndices = meshList.Select((x, i) => selectedSet.Contains(x) ? i : -1).Where(x => x >= 0).ToList();

            //var ghStructure = new GH_Structure<GH_Line>();
            // ghStructure.Append(new GH_Line(MouseLine.Value));
            //da.SetDataTree(2, (IGH_Structure)ghStructure);
            da.SetDataList(_selectedMeshesOut, selectedMeshes);
            da.SetDataList(_selectedIndicesOut, selectedIndices);
            _selectedIndices = selectedIndices;
        }

        private void ObjectsDeleted(object sender, GH_DocObjectEventArgs e)
        {
            if (!e.Objects.Contains(this))
            {
                return;
            }

            EnabledGlobal = false;
        }

        protected override Bitmap Icon => new Bitmap(24, 24);

        public override Guid ComponentGuid => new Guid("5be96ccb-eaa5-4c0e-a09a-95973ba08ef5");
        public bool ShiftPressed { get; set; } = false;
    }
}
