using System;
using System.IO;
using System.Reflection;
using GMechanics.Core;

namespace GMechanics.Editor.Data
{
    internal sealed class LocalStorage
    {
        private static readonly LocalStorage StaticInstance = new LocalStorage();

#region Accessors

        public ScriptsSources ScriptsSources
        {
            get { return ScriptsSources.Instance; }
        }

        public FlowcharItems FlowcharItems
        {
            get { return FlowcharItems.Instance; }
        }

        public static LocalStorage Instance
        {
            get { return StaticInstance; }
        }

#endregion Accessors

        private LocalStorage() { }

        internal bool LoadSQLiteStorageFromFile(string filePath, object container)
        {
            try
            {
                Type cType = container.GetType();
                MethodInfo mLoad = cType.GetMethod("Load");
                mLoad.Invoke(container, new object[] {filePath});
                return true;
            }
            catch
            {
                return false;
            }
        }

        public void Save()
        {
            if (!Directory.Exists(GlobalVariables.DataFolderPath))
            {
                Directory.CreateDirectory(GlobalVariables.DataFolderPath);
            }
        }

        public void Load()
        {
            CloseSQLiteConnections();

            LoadSQLiteStorageFromFile(GlobalVariables.ScriptsSourcesFilePath,
                                      ScriptsSources);
            LoadSQLiteStorageFromFile(GlobalVariables.FlowhartItemsFilePath,
                                      FlowcharItems);
        }

        public void CloseSQLiteConnections()
        {
            ScriptsSources.CloseConnection();
            FlowcharItems.CloseConnection();
        }

        public void RestoreSQLiteConnections()
        {
            ScriptsSources.RestorePreviousConnection();
            FlowcharItems.RestorePreviousConnection();
        }
    }
}