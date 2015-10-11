using System;
using System.Data.SQLite;
using System.IO;

namespace GMechanics.Core.Classes.Storages.BaseStorages
{
    public class BaseSQLiteConnector<TParentType> : IDisposable
        where TParentType : class, new()
    {
        private static readonly TParentType StaticInstance = new TParentType();

        protected string BaseFileName;
		protected SQLiteConnection Connection;

        protected BaseSQLiteConnector() { }

        public static TParentType Instance
        {
            get { return StaticInstance; }
        }

        public virtual void OpenConnection(string baseFileName)
        {
            if (Connection != null)
            {
                throw new Exception("Close current connection before.");
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

        public void Dispose()
        {
            CloseConnection();
        }

        public virtual void Load(string baseFileName)
        {
            OpenConnection(baseFileName);
        }
    }
}
