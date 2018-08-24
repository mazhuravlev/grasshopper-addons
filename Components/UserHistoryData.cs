using System;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global

    public class UserHistoryData
    {
        public Guid ComponentGuid { get; set; }

        public string UserName { get; set; }

        public DateTime DateTime { get; set; } = DateTime.Now;
    }
}