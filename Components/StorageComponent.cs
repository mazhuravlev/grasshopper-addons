using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Grasshopper.Kernel;
using LiteDB;

namespace GHAddons.Components
{
    using Grasshopper.Kernel.Special;

    // ReSharper disable once UnusedMember.Global
    // ReSharper disable once ClassNeverInstantiated.Global
    public class StorageComponent : GH_Component
    {
        private string _dbPath;
        private int _pathIn;

        public StorageComponent()
            : base("Storage", "Storage", "Storage", "Test", "Test")
        {
        }

        public override Guid ComponentGuid => new Guid("1a1d1f12-61ab-49d3-b977-37dc65a13333");

        public delegate void StorageChangedHandler(object sender, StorageChangedEventArgs e);

        public event StorageChangedHandler OnStorageChanged;

        public void SetString(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key is null or empty");
            }

            using (var db = GetDb())
            {
                var collection = db.GetCollection<KeyValue>();
                var kv = collection.Find(x => x.Key == key).FirstOrDefault();
                if (kv == null)
                {
                    collection.Insert(new KeyValue {Key = key, Value = value});
                }
                else
                {
                    kv.Value = value;
                    collection.Update(kv);
                }
            }

            OnStorageChanged?.Invoke(this, new StorageChangedEventArgs {Key = key, Value = value});
        }

        public void SetStrings(List<string> keys, List<string> values)
        {
            if (keys.Count != values.Count)
            {
                throw new ArgumentException($"{nameof(keys)}.Count != {nameof(values)}.Count");
            }

            using (var db = GetDb())
            {
                var collection = db.GetCollection<KeyValue>();
                var existingKeyValues = collection.Find(x => keys.Contains(x.Key)).ToDictionary(x => x.Key, x => x);
                var toInsert = new List<KeyValue>();
                var toUpdate = new List<KeyValue>();
                foreach (var (key, value) in keys.Zip(values, (k, v) => (k, v)))
                {
                    if (string.IsNullOrEmpty(key))
                    {
                        throw new ArgumentException("Key is null or empty");
                    }

                    if (existingKeyValues.ContainsKey(key))
                    {
                        var kv = existingKeyValues[key];
                        kv.Value = value;
                        toUpdate.Add(kv);
                    }
                    else
                    {
                        toInsert.Add(new KeyValue {Key = key, Value = value});
                    }

                    OnStorageChanged?.Invoke(this, new StorageChangedEventArgs {Key = key, Value = value});
                }

                collection.InsertBulk(toInsert);
                collection.Update(toUpdate);
            }
        }

        public string GetString(string key)
        {
            using (var db = GetDb())
            {
                var collection = db.GetCollection<KeyValue>();
                var kv = collection.Find(x => x.Key == key).FirstOrDefault();
                return kv?.Value;
            }
        }

        public IEnumerable<string> GetStrings(IEnumerable<string> keys)
        {
            using (var db = GetDb())
            {
                var collection = db.GetCollection<KeyValue>();
                var kv = collection.Find(x => keys.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
                return keys.Select(x => kv.ContainsKey(x) ? kv[x] : null);
            }
        }

        public void DeleteKeys(IEnumerable<string> keys)
        {
            using (var db = GetDb())
            {
                var collection = db.GetCollection<KeyValue>();
                collection.Delete(x => keys.Contains(x.Key));
            }

            foreach (var key in keys)
            {
                OnStorageChanged?.Invoke(this, new StorageChangedEventArgs {Key = key, Value = null});
            }
        }

        private LiteDatabase GetDb()
        {
            LiteDatabase LiteDatabase()
            {
                if (!string.IsNullOrEmpty(_dbPath))
                {
                    return new LiteDatabase(_dbPath);
                }

                var panel = Params.Input[_pathIn].Sources.FirstOrDefault() as GH_Panel;
                if (panel == null)
                {
                    throw new Exception("Db path not set");
                }

                return new LiteDatabase(panel.UserText);
            }

            var db = LiteDatabase();
            var collection = db.GetCollection<KeyValue>();
            collection.EnsureIndex(x => x.Key);
            return db;
        }

        protected override Bitmap Icon => new Bitmap(24, 24);

        protected override void RegisterInputParams(GH_InputParamManager pManager)
        {
            _pathIn = pManager.AddTextParameter("DB Path", "DB", "DB Path", GH_ParamAccess.item);
        }

        protected override void RegisterOutputParams(GH_OutputParamManager pManager)
        {
        }

        protected override void SolveInstance(IGH_DataAccess da)
        {
            var dbPath = string.Empty;
            da.GetData(_pathIn, ref dbPath);
            _dbPath = dbPath;
        }

        protected override void ExpireDownStreamObjects()
        {
            //
        }

        public IEnumerable<string> SearchKeys(string search)
        {
            using (var db = GetDb())
            {
                var collection = db.GetCollection<KeyValue>();
                return collection.Find(x => x.Key.StartsWith(search)).Select(x => x.Key).ToList();
            }
        }
    }

    public class KeyValue
    {
        public int Id { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    public class StorageChangedEventArgs
    {
        public string Key { get; set; }

        public string Value { get; set; }
    }
}