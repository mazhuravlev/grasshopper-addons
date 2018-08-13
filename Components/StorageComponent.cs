using System;
using System.Drawing;
using Grasshopper.Kernel;
using LiteDB;

namespace GHAddons.Components
{
    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StorageComponent : GH_Component
    {
        private int _in;
        private int _pathIn;

        public StorageComponent()
            : base("Storage", "Storage", "Storage", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("1a1d1f12-61ab-49d3-b977-37dc65a13333");

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _in = pManager.AddBooleanParameter("Button", "B", "Button input", GH_ParamAccess.item);
            _pathIn = pManager.AddTextParameter("Db path", "DB", "Path to db file", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var dbPath = string.Empty;
            da.GetData(_pathIn, ref dbPath);
            using(var db = new LiteDatabase(dbPath))
            {
                // Get customer collection
                var customers = db.GetCollection<Customer>("customers");

                // Create your new customer instance
                var customer = new Customer
                { 
                    Name = "John Doe", 
                    Phones = new string[] { "8000-0000", "9000-0000" }, 
                    IsActive = true
                };

                // Insert new customer document (Id will be auto-incremented)
                customers.Insert(customer);

                // Update a document inside a collection
                customer.Name = "Joana Doe";

                customers.Update(customer);

                // Index document using a document property
                customers.EnsureIndex(x => x.Name);

                // Use Linq to query documents
                var results = customers.Find(x => x.Name.StartsWith("Jo"));
            }  
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }

     
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string[] Phones { get; set; }
        public bool IsActive { get; set; }
    }

}