using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;

namespace GMechanics.Core.Classes.Storages.BaseStorages
{
    public class BaseGameObjectGroups<TParentType> : 
        BaseSQLiteContainer<GameObjectGroup, TParentType>
        where TParentType : class, new()
    {

#region Properties

        protected virtual string GroupTableName
        {
            get { return null; }
        }

        protected virtual string GroupElementsTableName
        {
            get { return null; }
        }

        public override string[] AtomsNamesList
        {
            get
            {
                List<string> names = new List<string>(Values.Count);
                foreach (GameObjectGroup obj in Values)
                {
                    names.Add(obj.Path);
                }
                names.Sort();
                return names.ToArray();
            }
        }

#endregion

#region Class methods

        protected BaseGameObjectGroups() { }

        protected BaseGameObjectGroups(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        public override bool Add(GameObjectGroup obj)
        {
            if (obj != null && !string.IsNullOrEmpty(obj.Path) &&
                !ContainsKey(obj.Path))
            {
                Add(obj.Path, obj);
                return true;
            }
            return false;
        }

        public override bool Remove(GameObjectGroup obj, bool destroy)
        {
            if (obj != null)
            {
                if (destroy)
                {
                    obj.Destroy();
                }
                Remove(obj.Path);
                return true;
            }
            return false;
        }

#endregion

#region SQLite

        protected override void CreateDbStructure()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {

                    #region T_GAME_OBJECT_GROUP

                    //Create T_GAME_OBJECT_GROUP table
                    command.CommandText = string.Format(@"
CREATE TABLE {0} 
(
    group_id BIGINT NOT NULL, 
    parent_group_id BIGINT,
    group_name VARCHAR(50) NOT NULL,
    group_transcription NVARCHAR(100) NOT NULL,
    group_description NVARCHAR(1000) NOT NULL
)
", GroupTableName);
                    command.ExecuteNonQuery();

                    #endregion

                    #region T_GAME_OBJECT_GROUP_ELEMENTS

                    //Create T_GAME_OBJECT_GROUP_ELEMENTS table
                    command.CommandText = string.Format(@"
CREATE TABLE {0}
(
    group_element_name VARCHAR(50) NOT NULL,
    group_element_type TINYINT NOT NULL,
    group_id BIGINT NOT NULL
)
", GroupElementsTableName);
                    command.ExecuteNonQuery();

                    //Create T_GAME_OBJECT_GROUP_ELEMENTS index
                    command.CommandText = string.Format(@"
CREATE UNIQUE INDEX IDX_T_{0}_GROUP_ID_ELEMENT_NAME 
ON {0} (group_id, group_element_name)
", GroupElementsTableName);
                    command.ExecuteNonQuery();

                    #endregion

                    //Commit
                    transaction.Commit();
                }
            }
        }

#endregion

#region Container

        protected void InsertGameObjectGroupElements(SQLiteCommand command, long groupId,
            ElementaryGameObject element)
        {
            if (!element.IsDestroyed)
            {
                command.CommandText = string.Format(@"
INSERT INTO {0} (group_element_name, group_element_type, group_id) 
VALUES (@group_element_name, @group_element_type, @group_id) 
", GroupElementsTableName);
                command.Parameters.Add("@group_element_name", DbType.AnsiStringFixedLength, 50).Value = 
                    element.Name;
                command.Parameters.Add("@group_element_type", DbType.Byte).Value =
                    (byte)GameEntityTypesTable.TypeOf(element);
                command.Parameters.Add("@group_id", DbType.Int64).Value =
                    groupId;
                command.ExecuteNonQuery();
            }
        }

        protected void InsertGameObjectGroup(SQLiteCommand command, GameObjectGroup group)
        {
            if (!group.IsDestroyed)
            {
                command.CommandText = string.Format(@"
INSERT INTO {0} (group_id, parent_group_id, group_name, group_transcription, group_description) 
VALUES (@group_id, @parent_group_id, @group_name, @group_transcription, @group_description) 
", GroupTableName);
                command.Parameters.Add("@group_id", DbType.Int64).Value =
                    group.ObjectId;
                command.Parameters.Add("@parent_group_id", DbType.Int64).Value =
                    group.Parent == null ? (object)DBNull.Value : group.Parent.ObjectId;
                command.Parameters.Add("@group_name", DbType.AnsiStringFixedLength, 50).Value =
                    group.Name;
                command.Parameters.Add("@group_transcription", DbType.StringFixedLength, 100).Value =
                    group.Transcription;
                command.Parameters.Add("@group_description", DbType.StringFixedLength, 1000).Value =
                    group.Description;
                command.ExecuteNonQuery();

                foreach (ElementaryGameObject element in group.GameObjects)
                {
                    InsertGameObjectGroupElements(command, group.ObjectId, element);
                }
            }
        }

        public override void Save()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    command.CommandText = string.Format(@"
DELETE FROM {0};
DELETE FROM {1};
", GroupTableName, GroupElementsTableName);
                    command.ExecuteNonQuery();

                    foreach (GameObjectGroup obj in Values)
                    {
                        InsertGameObjectGroup(command, obj);
                    }

                    //Commit
                    transaction.Commit();
                }
            }
        }

        protected void PopulateGameObjectGroupElementFromDataReader(GameObjectGroup group,
            SQLiteDataReader dr)
        {
            GameEntityType type = (GameEntityType)dr["group_element_type"];
            string elementName = (string)dr["group_element_name"];
            ElementaryGameObject gameObject = type == GameEntityType.ElementaryGameObject
                                                  ? GlobalStorage.Instance.ElementaryGameObjects[elementName]
                                                  : GlobalStorage.Instance.GameObjects[elementName];
            if (gameObject != null)
            {
                group.AddGameObject(gameObject);
            }
        }

        protected void ReadGameObjectGroupElements(GameObjectGroup group)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = string.Format(@"
SELECT *
FROM {0}
WHERE group_id = @group_id
", GroupElementsTableName);
                command.Parameters.Add("@group_id", DbType.Int64).Value = group.ObjectId;
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        PopulateGameObjectGroupElementFromDataReader(group, dr);
                    }
                }
            }
        }

        protected GameObjectGroup PopulateGameObjectGroupFromDataReader(SQLiteDataReader dr,
            Dictionary<long, List<GameObjectGroup>> inheritanceTable)
        {
            GameObjectGroup group = new GameObjectGroup();
            group.ObjectId = (long)dr["group_id"];
            group.Name = (string)dr["group_name"];
            group.Transcription = (string)dr["group_transcription"];
            group.Description = (string)dr["group_description"];
            StoreGroupInheritanceInfo(group, dr["parent_group_id"], inheritanceTable);
            return group;
        }

        public void StoreGroupInheritanceInfo(GameObjectGroup group, object parentGroupId,
            Dictionary<long, List<GameObjectGroup>> inheritanceTable)
        {
            if (parentGroupId != DBNull.Value)
            {
                long pId = (long)parentGroupId;
                if (!inheritanceTable.ContainsKey(pId))
                {
                    inheritanceTable.Add(pId, new List<GameObjectGroup>());
                }
                List<GameObjectGroup> item = inheritanceTable[pId];
                item.Add(group);
            }
        }

        protected void EstablishGroupsInheritances(List<GameObjectGroup> groups,
            Dictionary<long, List<GameObjectGroup>> inheritanceTable)
        {
            foreach (GameObjectGroup group in groups)
            {
                if (inheritanceTable.ContainsKey(@group.ObjectId))
                {
                    List<GameObjectGroup> item = inheritanceTable[@group.ObjectId];
                    if (item != null)
                    {
                        foreach (GameObjectGroup childGroup in item)
                        {
                            childGroup.Parent = group;
                        }
                    }
                }
            }
        }

        public override void Load(string baseFileName)
        {
            base.Load(baseFileName);

            Clear();

            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                List<GameObjectGroup> groups = new List<GameObjectGroup>();
                Dictionary<long, List<GameObjectGroup>> inheritanceTable = 
                    new Dictionary<long, List<GameObjectGroup>>();
                command.CommandText = string.Format(@"
SELECT *
FROM {0}
", GroupTableName);
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        GameObjectGroup group = PopulateGameObjectGroupFromDataReader(
                            dr, inheritanceTable);
                        ReadGameObjectGroupElements(group);
                        groups.Add(group);
                    }
                }

                EstablishGroupsInheritances(groups, inheritanceTable);

                foreach (GameObjectGroup group in groups)
                {
                    Add(group);
                }
            }
        }

#endregion

    }
}
