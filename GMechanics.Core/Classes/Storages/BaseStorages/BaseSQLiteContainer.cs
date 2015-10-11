using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Interfaces;

namespace GMechanics.Core.Classes.Storages.BaseStorages
{
    public class BaseSQLiteContainer<TObjType, TParentType> : 
        Dictionary<string, TObjType>, IBaseContainer, IDisposable
        where TObjType : Atom
        where TParentType : class, new()
    {

#region Static members

        private static readonly TParentType StaticInstance = new TParentType();

#endregion

#region Protected members

        protected string BaseFileName;
        protected SQLiteConnection Connection;

#endregion

#region SQLite

        public virtual void OpenConnection(string baseFileName)
        {
            if (Connection != null)
            {
                throw new Exception("Close current connection before");
            }

            if (string.IsNullOrEmpty(baseFileName))
            {
                throw new Exception("Incorrect SQLite database file name");
            }

            bool newDatabase = !File.Exists(baseFileName);
            Connection = new SQLiteConnection(string.Format("Data Source={0};Version=3;New={1};Compress=True;",
                baseFileName, newDatabase));
            Connection.Open();

            if (newDatabase)
            {
                CreateDbStructure();
            }

            BaseFileName = baseFileName;
        }

        public virtual void CloseConnection()
        {
            if (Connection != null)
            {
                Connection.Close();
                Connection.Dispose();
                Connection = null;
            }
        }

        public virtual void RestorePreviousConnection()
        {
            if (!string.IsNullOrEmpty(BaseFileName))
            {
                OpenConnection(BaseFileName);
            }
        }

        protected virtual void CreateDbStructure() { }

        protected object GetSQLFieldData(object data)
        {
            return data ?? DBNull.Value;
        }

#endregion

#region Properties

        public static TParentType Instance
        {
            get { return StaticInstance; }
        }

        public virtual string[] AtomsNamesList
        {
            get
            {
                List<string> names = new List<string>(Values.Count);
                foreach (TObjType obj in Values)
                {
                    names.Add(obj.Name);
                }
                names.Sort();
                return names.ToArray();
            }
        }

        public new virtual TObjType this[string name]
        {
            get
            {
                return !string.IsNullOrEmpty(name) && ContainsKey(name)
                           ? base[name]
                           : null;
            }
            set
            {
                Remove(name);
                if (value != null)
                {
                    Add(value);
                }
            }
        }

        public virtual TObjType this[long objectId]
        {
            get { return Values.FirstOrDefault(o => o.ObjectId == objectId); }
        }

#endregion

#region Class functions

        protected BaseSQLiteContainer() { }

        protected BaseSQLiteContainer(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public virtual TObjType[] ToArray()
        {
            List<TObjType> list = new List<TObjType>();
            foreach (TObjType obj in Values)
            {
                list.Add(obj);
            }
            return list.ToArray();
        }

        public virtual bool Add(TObjType obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.Name) &&
                !ContainsKey(obj.Name))
            {
                Add(obj.Name, obj);
                return true;
            }
            return false;
        }

        public virtual void AddRange(IEnumerable<TObjType> list)
        {
            foreach (TObjType obj in list)
            {
                Add(obj);
            }
        }

        public virtual void AddRange(BaseContainer<TObjType, TParentType> list)
        {
            if (list != null)
            {
                foreach (TObjType obj in list.Values)
                {
                    Add(obj);
                }
            }
        }

        public bool Remove(string name, bool destroy)
        {
            if (!string.IsNullOrEmpty(name))
            {
                TObjType obj = this[name];
                if (obj != null)
                {
                    if (destroy)
                    {
                        obj.Destroy();
                    }
                    base.Remove(name);
                    return true;
                }
            }
            return false;
        }

        public new bool Remove(string name)
        {
            return Remove(name, true);
        }

        public virtual bool Remove(TObjType obj, bool destroy)
        {
            if (obj != null)
            {
                if (destroy)
                {
                    obj.Destroy();
                }
                base.Remove(obj.Name);
                return true;
            }
            return false;
        }

        public virtual bool Remove(TObjType obj)
        {
            return Remove(obj, true);
        }

        public virtual bool ChangeName(string oldName)
        {
            if (!string.IsNullOrEmpty(oldName))
            {
                TObjType obj = this[oldName];
                if (obj != null)
                {
                    Remove(oldName, false);
                    Add(obj);
                }
            }
            return false;
        }

        public virtual void RemoveDestroyedItems()
        {
            List<string> forRemoving = new List<string>();
            foreach (string key in Keys)
            {
                TObjType obj = this[key];
                if (obj.IsDestroyed)
                {
                    forRemoving.Add(key);
                }
            }
            foreach (string key in forRemoving)
            {
                Remove(key, false);
            }
        }

        public virtual void Save() {}

        public virtual void Load(string baseFileName)
        {
            OpenConnection(baseFileName);
        }

#endregion

#region IDisposable

        public void Dispose()
        {
            CloseConnection();
        }

#endregion

    }
}
