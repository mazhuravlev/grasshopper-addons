using System;
using System.Drawing;
using System.Linq;
using Grasshopper;
using Grasshopper.Kernel;
using Grasshopper.Kernel.Data;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class GateComponent : GH_Component, IGH_VariableParameterComponent
    {
        private int[] _inputState;
        private GateState _state = GateState.Closed;

        /// <inheritdoc />
        /// <summary>
        /// Each implementation of GH_Component must provide a public 
        /// constructor without any arguments.
        /// Category represents the Tab in which the component will appear, 
        /// Subcategory the panel. If you use non-existing tab or panel names, 
        /// new tabs/panels will automatically be created.
        /// </summary>
        public GateComponent()
            : base("Gate", "Gate", "Gate", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("1a3d9f12-61ab-49d3-b977-37dc65a182a2");

        protected override Bitmap Icon => new Bitmap(24, 24);

        public void SetActivation(GateParam param)
        {
            var index = Params.Input.IndexOf(param);
            _inputState[index] = 1;
            if (_inputState.All(x => x == 1))
            {
                _state = GateState.Activated;
                _inputState = new int[Params.Input.Count];
            }
        }

        public bool CanInsertParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public bool CanRemoveParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public IGH_Param CreateParameter(GH_ParameterSide side, int index)
        {
            if (side == GH_ParameterSide.Output)
            {
                var param = new Param_GenericObject { Access = GH_ParamAccess.tree };
                return param;
            }
            else
            {
                var param = new GateParam { Access = GH_ParamAccess.tree };
                return param;
            }
        }

        public bool DestroyParameter(GH_ParameterSide side, int index)
        {
            return true;
        }

        public void VariableParameterMaintenance()
        {
            _inputState = new int[Params.Input.Count];
        }

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            if (Params.Input.Count != Params.Output.Count)
            {
                AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Input and output count must be equal");
                return;
            }

            switch (_state)
            {
                case GateState.Closed:
                    SetBlankData(da);
                    break;
                case GateState.Activated:
                    SetData(da);
                    _inputState = new int[Params.Input.Count];
                    _state = GateState.Reset;
                    break;
                case GateState.Reset:
                    SetBlankData(da);
                    ExpireAllOutputs();
                    _state = GateState.Closed;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }

        private void ExpireAllOutputs()
        {
            this.ExpireOutputs(Enumerable.Range(0, Params.Output.Count));
        }

        private void SetBlankData(IGH_DataAccess da)
        {
            var emptyData = new DataTree<IGH_Goo>();
            for (var i = 0; i < Params.Output.Count; i++)
            {
                da.SetDataTree(i, emptyData);
            }
        }

        private void SetData(IGH_DataAccess da)
        {
            for (var i = 0; i < Params.Input.Count; i++)
            {
                da.GetDataTree(i, out GH_Structure<IGH_Goo> data);
                da.SetDataTree(i, data);
            }

            ExpireAllOutputs();
        }
    }
}