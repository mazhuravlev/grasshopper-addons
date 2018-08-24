using Grasshopper.Kernel;

namespace GHAddons.Components
{
    public class CustomAssemblyPriority : GH_AssemblyPriority
    {
        public override GH_LoadingInstruction PriorityLoad()
        {
            Grasshopper.GUI.Canvas.GH_Canvas.WidgetListCreated += AddWidgets;
            return GH_LoadingInstruction.Proceed;
        }

        private static void AddWidgets(object o, Grasshopper.GUI.Canvas.GH_CanvasWidgetListEventArgs args)
        {
            var w = new UserHistoryWidget();
            args.AddWidget(w);
        }
    }
}
