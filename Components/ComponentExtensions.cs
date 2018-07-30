using System.Collections.Generic;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    public static class ComponentExtensions
    {
        

        public static void ExpireOutput(this IGH_Component component, int output)
        {
            foreach (var receiver in component.Params.Output[output].Recipients)
            {
                receiver.ExpireSolution(true);
            }
        }

        public static void ExpireOutputs(this IGH_Component component, IEnumerable<int> outputs)
        {
            foreach (var output in outputs)
            {
                component.ExpireOutput(output);
            }
        }

        public static void ExpireOutputs(this IGH_Component component, params int[] outputs)
        {
            foreach (var output in outputs)
            {
                component.ExpireOutput(output);
            }
        }
    }
}