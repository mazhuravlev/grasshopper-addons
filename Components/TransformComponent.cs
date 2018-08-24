using System;
using System.Drawing;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Types;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class TransformComponent : GH_Component
    {
        private int _transformIn;
        private int _geometryIn;
        private int _geometryOut;

        public TransformComponent()
            : base("Transform", "Transform", "Transform", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("36262171-fa6a-4dc9-b85b-7fc69684f416");

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _geometryIn = pManager.AddGeometryParameter("Geometry", "G", "Geometry", GH_ParamAccess.tree);
            _transformIn = pManager.AddTransformParameter("Transform", "T", "Transform", GH_ParamAccess.tree);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _geometryOut = pManager.AddGeometryParameter("Geometry", "G", "Geometry", GH_ParamAccess.tree);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            da.GetDataTree(_transformIn, out GH_Structure<GH_Transform> transforms);
            da.GetDataTree(_geometryIn, out GH_Structure<IGH_GeometricGoo> geometris);
            var result = new DataTree<IGH_GeometricGoo>();
            foreach (var ghPath in transforms.Paths)
            {
                var branchTransforms = transforms[ghPath];
                var branchGeometries = geometris[ghPath];
                for (var i = 0; i < branchGeometries.Count; i++)
                {
                    var ghGeometricGoo = branchGeometries[i];
                    var transformedGeometry = ghGeometricGoo.Transform(branchTransforms[i].Value);
                    result.Add(transformedGeometry, ghPath);
                }
            }

            da.SetDataTree(_geometryOut, result);
           this.ExpireOutput(_geometryOut);;
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }

     
    }
}