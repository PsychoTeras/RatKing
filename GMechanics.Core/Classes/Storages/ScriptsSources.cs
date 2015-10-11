using System.Data.SQLite;
using GMechanics.Core.Classes.Entities;
using GMechanics.Core.Classes.Entities.GameObjectEventClasses;
using GMechanics.Core.Classes.Enums;
using GMechanics.Core.Classes.Storages.BaseStorages;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Storages
{
    public sealed class ScriptsSources : BaseSQLiteConnector<ScriptsSources>
    {
        private GameObjectEventScriptDBRecord GetScriptSource(string scriptName, 
            InteractiveEventType? eventType)
        {
            string source;
            scriptName = CommonHelper.GetMD5Hash(scriptName);

            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = string.Format(@"
SELECT [source], [is_active]
FROM [T_SCRIPTS_SOURCE]
WHERE [md5] = ""{0}""
", scriptName);
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        source = CommonHelper.DecodeFromBase64((string) dr["source"]);
                        return new GameObjectEventScriptDBRecord(source, (bool)dr["is_active"]);
                    }
                }
            }

            source = DefaultScriptSourcesCode.GetSourceCode(eventType);
            return new GameObjectEventScriptDBRecord(source, false);
        }

        public GameObjectEventScriptDBRecord GetScriptSource(string scriptName)
        {
            return GetScriptSource(scriptName, null);
        }

        public GameObjectEventScriptDBRecord GetScriptSource(Atom atom, InteractiveEventType eventType)
        {
            if (atom != null)
            {
                string name = string.Format("{0}\\{1}", atom.ObjectId, eventType);
                return GetScriptSource(name, eventType);
            }
            return null;
        }

        public void SaveScriptSource(string scriptName, string source, bool isActive)
        {
            scriptName = CommonHelper.GetMD5Hash(scriptName);
            source = CommonHelper.EncodeToBase64(source);
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = string.Format(@"
REPLACE INTO 
[T_SCRIPTS_SOURCE] ([md5], [source], [is_active])
VALUES (""{0}"", ""{1}"", '{2}')
", scriptName, source, isActive ? 1 : 0);
                command.ExecuteNonQuery();
            }
        }

        public void SaveScriptSource(Atom atom, InteractiveEventType eventType, string source, bool isActive)
        {
            if (atom != null)
            {
                string name = string.Format("{0}\\{1}", atom.ObjectId, eventType);
                SaveScriptSource(name, source, isActive);
            }
        }

        protected override void CreateDbStructure()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    //Create T_SCRIPTS_SOURCE table
                    command.CommandText = @"
CREATE TABLE T_SCRIPTS_SOURCE
(
    md5 TEXT NOT NULL, 
    source TEXT NOT NULL, 
    is_active BOOLEAN DEFAULT '1' NOT NULL
)";
                    command.ExecuteNonQuery();

                    //Create T_SCRIPTS_SOURCE index
                    command.CommandText = @"
CREATE UNIQUE INDEX IDX_T_SCRIPTS_SOURCE_MD5 
ON [T_SCRIPTS_SOURCE] ([md5] ASC)
";
                    command.ExecuteNonQuery();

                    //Commit
                    transaction.Commit();
                }
            }
        }
    }

    static class DefaultScriptSourcesCode
    {
        public static string GetSourceCode(InteractiveEventType? eventType)
        {
            string source = string.Empty;

            if (eventType != null)
            {
                source = @"
#region Possible keywords
/*";
                switch (eventType)
                {
                    case InteractiveEventType.Assigning:
                    case InteractiveEventType.Assigned:
                    case InteractiveEventType.Removing:
                    case InteractiveEventType.Removed:
                    case InteractiveEventType.Changed:
                        {
                            source = string.Format(@"{0}
 * @Object    - game object over which the action is performed
 * @Causer    - game object that produces the action
 * @Member    - @Object member over which the action is performed
 * @Value     - current @Member value
 * @Cancelled - set this flag to true to break execution for all scripts 
 *              in current case and cancel all previous execution results
", source);
                            break;
                        }
                    case InteractiveEventType.Changing:
                        {
                            source = string.Format(@"{0}
 * @Object    - game object over which the action is performed
 * @Causer    - game object that produces the action
 * @Member    - @Object member over which the action is performed
 * @OldValue  - current @Member value
 * @NewValue  - new @Member value
 * @Value     - same as @NewValue
 * @Cancelled - set this flag to true to break execution for all scripts 
 *              in current case and cancel all previous execution results
", source);
                            break;
                        }
                    case InteractiveEventType.Interact:
                        {
                            break;
                        }
                }
                source = string.Format(@"{0} */
#endregion
", source);
            }

            source = string.Format(@"{0}
function main()
{{
    // Activate script and place your script code there...
}}", source);

            return source.Trim();
        }
    }
}