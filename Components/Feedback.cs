using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using Grasshopper.Kernel;
using Rhino;
using Rhino.Geometry;
using Rhino.UI;

namespace GHAddons.Components
{
    public class Feedback : MouseCallback
    {
        private readonly ClickablePreviewComponent _cm;

        public Feedback(ClickablePreviewComponent cm)
        {
            _cm = cm;
            Enabled = true;
        }

        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        protected override void OnMouseDown(MouseCallbackEventArgs e)
        {
            if (e.Button == MouseButtons.Left && _cm != null && (_cm.EnabledGlobal && _cm.InActiveDocument) && GetForegroundWindow() == RhinoApp.MainWindowHandle())
            {
                _cm.MouseLine = e.View.ActiveViewport.ClientToWorld(e.ViewportPoint);
                _cm.ShiftPressed = Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift);
                _cm.ExpireSolution(true);
                e.Cancel = true;
            }
            else
            {
                base.OnMouseDown(e);
            }
        }
    }
}
