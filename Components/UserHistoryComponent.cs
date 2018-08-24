using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GH_IO.Serialization;
using Grasshopper.Kernel;
using Newtonsoft.Json;

namespace GHAddons.Components
{
    public class UserHistoryComponent : GH_Component
    {
        private Dictionary<Guid, UserHistoryData> _userHistory = new Dictionary<Guid, UserHistoryData>();

        public UserHistoryComponent()
            : base("UserHistory", "UserHistory", "UserHistory", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("80e7ead8-d09c-4459-884d-23501229cad8");
        
        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
           
        }

        public override bool Write(GH_IWriter writer)
        {
            writer.SetString("history", JsonConvert.SerializeObject(_userHistory.Values.ToList()));
            return base.Write(writer);
        }

        public override bool Read(GH_IReader reader)
        {
            var historyJson = reader.GetString("history");
            if (!string.IsNullOrWhiteSpace(historyJson))
            {
                try
                {
                    var historyData = JsonConvert.DeserializeObject<List<UserHistoryData>>(historyJson);
                    _userHistory = historyData.ToDictionary(x => x.ComponentGuid);
                }
                catch
                {
                    AddRuntimeMessage(GH_RuntimeMessageLevel.Error, "Failed to read saved data");
                }
            }
            return base.Read(reader);
        }

        public UserHistoryData GetHistory(Guid componentGuid)
        {
            return _userHistory.ContainsKey(componentGuid) ? _userHistory[componentGuid] : null;
        }

        public override void AddedToDocument(GH_Document document)
        {
            document.ObjectsAdded += DocumentOnObjectsAdded;
        }

        public override void RemovedFromDocument(GH_Document document)
        {
            document.ObjectsAdded -= DocumentOnObjectsAdded;
        }

        private void DocumentOnObjectsAdded(object sender, GH_DocObjectEventArgs e)
        {
            foreach (var userHistoryData in e.Objects.Select(x => x.ComponentGuid)
                .Where(x => !_userHistory.ContainsKey(x))
                .Select(x => new UserHistoryData {ComponentGuid = x, UserName = Environment.UserName}))
            {
                _userHistory[userHistoryData.ComponentGuid] = userHistoryData;
            }
        }
    }
}