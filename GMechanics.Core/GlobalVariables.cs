using System.IO;
using GMechanics.Core.Helpers;

namespace GMechanics.Core
{
    public static class GlobalVariables
    {
        public const string DataFolderName = "Data";
        public const string BackupsFolderName = "Backups";
        public const string GameObjectPropertiesClassesFileName = "gopc.dat";
        public const string GameObjectPropertiesFileName = "gop.dat";
        public const string GameObjectFeaturesClassesFileName = "gofc.dat";
        public const string GameObjectFeaturesFileName = "gof.dat";
        public const string GameObjectAttributesFileName = "goa.dat";
        public const string GameObjectsFileName = "go.dat";
        public const string ElementaryGameObjectsFileName = "ego.dat";
        public const string ScriptsSourcesFileName = "scripts.dat";
        public const string FlowhartItemsFileName = "fchartitems.dat";
        public const string GameObjectGroupsFileName = "gog.dat";
        public const string ElementaryGameObjectGroupsFileName = "egog.dat";

        public static string DataFolderPath = Path.Combine(Helper.ApplicationPath, 
                                DataFolderName);

        public static string BackupsFolderPath = Path.Combine(Helper.ApplicationPath,
                                BackupsFolderName);

        public static string GameObjectPropertiesClassesFilePath = Path.Combine(DataFolderPath,
                                GameObjectPropertiesClassesFileName);

        public static string GameObjectPropertiesFilePath = Path.Combine(DataFolderPath,
                                GameObjectPropertiesFileName);

        public static string GameObjectFeaturesClassesFilePath = Path.Combine(DataFolderPath,
                                GameObjectFeaturesClassesFileName);

        public static string GameObjectFeaturesFilePath = Path.Combine(DataFolderPath,
                                GameObjectFeaturesFileName);

        public static string GameObjectAttributesFilePath = Path.Combine(DataFolderPath,
                                GameObjectAttributesFileName);

        public static string ElementaryGameObjectsFilePath = Path.Combine(DataFolderPath,
                                ElementaryGameObjectsFileName);

        public static string GameObjectsFilePath = Path.Combine(DataFolderPath,
                                GameObjectsFileName);

        public static string ScriptsSourcesFilePath = Path.Combine(DataFolderPath,
                                ScriptsSourcesFileName);

        public static string FlowhartItemsFilePath = Path.Combine(DataFolderPath,
                                FlowhartItemsFileName);

        public static string ElementaryGameObjectGroupsFilePath = Path.Combine(DataFolderPath,
                                ElementaryGameObjectGroupsFileName);

        public static string GameObjectGroupsFilePath = Path.Combine(DataFolderPath,
                                GameObjectGroupsFileName);
    }
}