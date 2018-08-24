using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using Anna;
using Grasshopper.Kernel;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class ServerComponent : GH_Component
    {
        private int _in;
        private int _out;
        private string _serverData = "hello";
        private string _clientData = string.Empty;
        private HttpServer _server;
        private int _dataIn;
        private int _portIn;

        public ServerComponent()
            : base("Server", "Server", "Server", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("75556ffb-aeb3-4718-b684-bf589a15ba78");

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _in = pManager.AddBooleanParameter("In", "In", "In", GH_ParamAccess.item);
            _portIn = pManager.AddIntegerParameter("Port", "Port", "Port", GH_ParamAccess.item);
            _dataIn = pManager.AddTextParameter("Data", "Data", "Data", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
            _out = pManager.AddTextParameter("Out", "Out", "Out", GH_ParamAccess.item);
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var on = false;
            da.GetData(_in, ref on);
            var port = 0;
            da.GetData(_portIn, ref port);
            da.GetData(_dataIn, ref _serverData);
            if (on)
            {
                if (_server == null)
                {
                    var url = new Uri($"http://localhost:{port}/");
                    _server = MakeServer(url);
                    Process.Start(url.ToString());
                }
            }
            else
            {
                _server?.Dispose();
                _server = null;
            }

            da.SetData(_out, _clientData);
            this.ExpireOutput(_out);
        }

        private HttpServer MakeServer(Uri url)
        {
            var server = new HttpServer(url.ToString());
            server.GET("/").Subscribe(ctx =>
            {
                _clientData = ctx.Request.QueryString.Data;
                ctx.Respond($"<p>{_serverData}</p>  {Html}");
                ExpireSolution(true);
            });
            server.OPTIONS("/bricks").Subscribe(ctx => { ctx.Respond(200, new Dictionary<string, string>
            {
                {"Access-Control-Allow-Origin", "*"},
                {"Access-Control-Allow-Headers", "*"},
                {"Access-Control-Allow-Methods", "*"}
            }); });
            server.POST("/bricks").Subscribe(ctx =>
            {
                ctx.Request.GetBody().Subscribe(x => _clientData = x);
                ctx.Respond();
                ExpireSolution(true);
            });
            return server;
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }

        private const string Html = @"
<form method=""get"">
<input name=""Data"">
<button>ok</button>
</form>
";
     
    }
}