using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Storages.BaseStorages;
using GMechanics.Core.Classes.Types;
using GMechanics.Core.Helpers;

namespace GMechanics.Core.Classes.Storages
{
    [Serializable]
    public sealed class GameObjects : BaseGameObjects<GameObject, GameObjects>
    {
        public GameObjects() { }

        public GameObjects(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

#region SQLite

        protected override void CreateDbStructure()
        {
            base.CreateDbStructure();

            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {

                    #region T_GAME_OBJECT_EXT

                    //Create T_GAME_OBJECT_EXT table
                    command.CommandText = @"
CREATE TABLE T_GAME_OBJECT_EXT
(
    game_object_id BIGINT NOT NULL,
    game_object_size_packed VARBINARY(12) NOT NULL,
    game_object_weight FLOAT NOT NULL,
    game_object_direction FLOAT NOT NULL,
    game_object_location_packed VARBINARY(12) NOT NULL
)";
                    command.ExecuteNonQuery();

                    #endregion

                    //Commit
                    transaction.Commit();
                }
            }
        }

        protected override void ClearDatabase(SQLiteCommand command)
        {
            base.ClearDatabase(command);
            command.CommandText = @"
DELETE FROM T_GAME_OBJECT_EXT;";
            command.ExecuteNonQuery();
        }

        protected override void InsertGameObject(SQLiteCommand command,
            GameObject gameObject)
        {
            if (!gameObject.IsDestroyed)
            {
                //Call base method
                base.InsertGameObject(command, gameObject);

                //Insert extended game object data
                command.CommandText = @"
INSERT INTO T_GAME_OBJECT_EXT
    (game_object_id, game_object_size_packed, game_object_weight, game_object_direction, 
     game_object_location_packed) 
VALUES 
    (@game_object_id, @game_object_size_packed, @game_object_weight, @game_object_direction, 
     @game_object_location_packed)";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObject.ObjectId;
                command.Parameters.Add("@game_object_size_packed", DbType.Binary).Value =
                    gameObject.Size.ToBytes();
                command.Parameters.Add("@game_object_weight", DbType.Single).Value =
                    gameObject.Weight;
                command.Parameters.Add("@game_object_direction", DbType.Single).Value =
                    gameObject.Direction;
                command.Parameters.Add("@game_object_location_packed", DbType.Binary).Value =
                    gameObject.Location.ToBytes();

                command.ExecuteNonQuery();
            }
        }

        protected override GameObject PopulateGameObjectFromDataReader(SQLiteDataReader dr)
        {
            //Populate game object
            GameObject obj = new GameObject();
            obj.ObjectId = (long)dr["game_object_id"];
            obj.Name = (string)dr["game_object_name"];
            obj.Transcription = (string)dr["game_object_transcription"];
            obj.Description = (string)dr["game_object_description"];

            //Populate extended game data
            obj.Size = Size3D.FromBytes((byte[])dr["game_object_size_packed"]);
            obj.Weight = (float)(double)dr["game_object_weight"];
            obj.Direction = (float)(double)dr["game_object_direction"];
            obj.Location = Location3D.FromBytes((byte[])dr["game_object_location_packed"]);

            //Return game object
            return obj;
        }

        protected override void PopulateGameObjects(List<object[]> inheritanceTable)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_GAME_OBJECT go
JOIN T_GAME_OBJECT_EXT goext ON goext.game_object_id = go.game_object_id";
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        GameObject obj = PopulateGameObjectFromDataReader(dr);
                        PopulateGameObjectMembers(obj, inheritanceTable);
                        Add(obj);
                    }
                }
            }
        }

#endregion

        public bool Add(string name, string transcription, string description,
                        ElementaryGameObject parent)
        {
            return Add(new GameObject(name, transcription, description, parent));
        }

        public override void RemoveDestroyedItems()
        {
            base.RemoveDestroyedItems();
            foreach (string key in Keys)
            {
                Helper.RemoveDestroyedItemsFromListsOwner(this[key]);
            }
        }
    }
}