using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Grasshopper.GUI;
using Grasshopper.GUI.Canvas;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    public class UserHistoryWidget : Grasshopper.GUI.Widgets.GH_Widget
{
	public RectangleF Bounds = new RectangleF(100, 100, 24, 24);
    private bool _drag;
	private PointF _clickOffset = new PointF(0, 0);

	public override string Description => "User history widget";

    public override string Name => "User history widget";

	public override bool Visible { get; set; } = true;

    public override Bitmap Icon_24x24 => Grasshopper.Plugin.GH_ResourceGate.Info_24x24;

    public override void Render(GH_Canvas canvas)
	{
	    var document = canvas.Document;
	    if (document == null) return;
	    var zoomFadeMedium = GH_Canvas.ZoomFadeMedium;
	    if (zoomFadeMedium == 0) return;
	    var historyComponent = document.Objects.SingleOrDefault(x => x is UserHistoryComponent) as UserHistoryComponent;
        if(historyComponent == null) return;
	    var solidBrush = new SolidBrush(Color.FromArgb(zoomFadeMedium, Color.Black));
	    foreach (var ghDocumentObject in document.Objects)
	    {
	        if (ghDocumentObject is IGH_ActiveObject)
	        {
	            var historyRecord = historyComponent.GetHistory(ghDocumentObject.ComponentGuid);
                if(historyRecord == null) continue;
	            var bounds = ghDocumentObject.Attributes.Bounds;
	            if (canvas.Viewport.IsVisible(ref bounds, 10f))
	            {
	                var str = $"{historyRecord.UserName}: {historyRecord.DateTime:dd.MM.yy HH:mm}";
	                var sizeF = (SizeF) GH_FontServer.MeasureString(str, GH_FontServer.ConsoleSmall);
	                var rectangleF = bounds;
	                rectangleF.Height = sizeF.Height + 2f;
	                rectangleF.Width = sizeF.Width + 2f;
	                rectangleF.X = (float) (0.5 * ((double) bounds.Left + (double) bounds.Right) -
	                                        0.5 * (double) rectangleF.Width);
	                rectangleF.Y = bounds.Top - 15f;
	                canvas.Graphics.DrawString(str, GH_FontServer.ConsoleSmall, (Brush) solidBrush, rectangleF, GH_TextRenderingConstants.CenterCenter);
	            }
	        }
	    }
	}

	public override bool Contains(Point ptControl, PointF ptCanvas)
	{
		if (Bounds.Contains(ptControl) || Bounds.Contains(Owner.Viewport.ProjectPoint(ptCanvas))) return true;
		else return false;
	}

	public override GH_ObjectResponse RespondToMouseDown(GH_Canvas sender, GH_CanvasMouseEvent e)
	{
		if (e.Button == MouseButtons.Left && Bounds.Contains(e.ControlLocation))
		{
			_drag = true;
			Grasshopper.Instances.CursorServer.AttachCursor(Owner, "GH_HandClosed");
			Owner.Refresh();
			_clickOffset = new PointF (e.ControlX - Bounds.X, e.ControlY - Bounds.Y);
			return GH_ObjectResponse.Capture;
		}
		return GH_ObjectResponse.Ignore;
	}

	public override GH_ObjectResponse RespondToMouseUp(GH_Canvas sender, GH_CanvasMouseEvent e)
	{
		if (e.Button == MouseButtons.Left)
		{
			_drag = false;
		}
		return GH_ObjectResponse.Release;
	}

	public override GH_ObjectResponse RespondToMouseMove(GH_Canvas sender, GH_CanvasMouseEvent e)
	{
		if(_drag)
		{
			Bounds.X = e.ControlX - _clickOffset.X;
			Bounds.Y = e.ControlY - _clickOffset.Y;
			Owner.Refresh();
			return GH_ObjectResponse.Handled;
		}
		if (Bounds.Contains(e.ControlLocation))
		{
			Grasshopper.Instances.CursorServer.AttachCursor(Owner, "GH_HandOpen");
			Owner.Refresh();
			return GH_ObjectResponse.Handled;
		}
		return GH_ObjectResponse.Ignore;	
	}

	public override bool IsTooltipRegion(PointF canvasCoordinate)
	{
		// Transform canvas coordinate to control coordinate.
		var p = Owner.Viewport.ProjectPoint(canvasCoordinate);
		return Bounds.Contains(p);
	}
}
}
