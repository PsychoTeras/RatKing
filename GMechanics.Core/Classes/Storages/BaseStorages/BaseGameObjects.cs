using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Runtime.Serialization;
using GMechanics.Core.Classes.Entities.GameObjectAttributeClasses;
using GMechanics.Core.Classes.Entities.GameObjectFeatureClasses;
using GMechanics.Core.Classes.Entities.GameObjectPropertyClasses;
using GMechanics.Core.Classes.Entities.GameObjects;
using GMechanics.Core.Classes.Enums;

namespace GMechanics.Core.Classes.Storages.BaseStorages
{
    public class BaseGameObjects<TObjType, TParentType> : 
        BaseSQLiteContainer<TObjType, TParentType>
        where TObjType : ElementaryGameObject, new() 
        where TParentType : class, new()
    {

#region SQLite

        protected override void CreateDbStructure()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {

                    #region T_GAME_OBJECT

                    //Create T_GAME_OBJECT table
                    command.CommandText = @"
CREATE TABLE T_GAME_OBJECT 
(
    game_object_id BIGINT NOT NULL,
    game_object_name VARCHAR(50) NOT NULL,
    game_object_transcription NVARCHAR(100) NOT NULL,
    game_object_description NVARCHAR(1000) NOT NULL
)";
                    command.ExecuteNonQuery();

                    #endregion

                    #region T_GAME_OBJECT_PARENTS

                    //Create T_GAME_OBJECT_PARENTS table
                    command.CommandText = @"
CREATE TABLE T_GAME_OBJECT_PARENTS 
(
    game_object_id BIGINT NOT NULL,
    parent_game_object_id BIGINT NOT NULL,
    parent_game_object_type TINYINT NOT NULL
)";
                    command.ExecuteNonQuery();

                    #endregion

                    #region T_GAME_OBJECT_ATTRIBUTES

                    //Create T_GAME_OBJECT_ATTRIBUTES table
                    command.CommandText = @"
CREATE TABLE T_GAME_OBJECT_ATTRIBUTES
(
    attribute_id BIGINT NOT NULL,
    game_object_id BIGINT NOT NULL,
    attribute_name VARCHAR(50) NOT NULL
)";
                    command.ExecuteNonQuery();

                    #endregion

                    #region T_GAME_OBJECT_ATTRIBUTE_VALUES

                    //Create T_GAME_OBJECT_ATTRIBUTE_VALUES table
                    command.CommandText = @"
CREATE TABLE T_GAME_OBJECT_ATTRIBUTE_VALUES
(
    attribute_value_id BIGINT NOT NULL,
    game_object_id BIGINT NOT NULL,
    attribute_id BIGINT NOT NULL,
    attribute_value_name VARCHAR(50) NOT NULL,
    attribute_value_value FLOAT NOT NULL
)";
                    command.ExecuteNonQuery();

                    #endregion

                    #region T_GAME_OBJECT_PROPERTIES

                    //Create T_GAME_OBJECT_PROPERTIES table
                    command.CommandText = @"
CREATE TABLE T_GAME_OBJECT_PROPERTIES
(
    property_id BIGINT NOT NULL,
    game_object_id BIGINT NOT NULL,
    property_name VARCHAR(50) NOT NULL,
    property_max_value FLOAT NOT NULL,
    property_value FLOAT NOT NULL
)";
                    command.ExecuteNonQuery();

                    #endregion

                    #region T_GAME_OBJECT_FEATURES

                    //Create T_GAME_OBJECT_FEATURES table
                    command.CommandText = @"
CREATE TABLE T_GAME_OBJECT_FEATURES
(
    feature_id BIGINT NOT NULL,
    game_object_id BIGINT NOT NULL,
    feature_name VARCHAR(50) NOT NULL
)";
                    command.ExecuteNonQuery();

                    #endregion

                    //Commit
                    transaction.Commit();
                }
            }
        }

#endregion

#region Container

        protected virtual void ClearDatabase(SQLiteCommand command)
        {
            command.CommandText = @"
DELETE FROM T_GAME_OBJECT_PARENTS;
DELETE FROM T_GAME_OBJECT_ATTRIBUTES;
DELETE FROM T_GAME_OBJECT_ATTRIBUTE_VALUES;
DELETE FROM T_GAME_OBJECT_PROPERTIES;
DELETE FROM T_GAME_OBJECT_FEATURES;
DELETE FROM T_GAME_OBJECT;
";
            command.ExecuteNonQuery();
        }

        protected void InsertGameObjectAttribute(SQLiteCommand command, 
            long gameObjectId, GameObjectAttribute attribute)
        {
            if (!attribute.ClassAsAtom.IsDestroyed)
            {
                command.CommandText = @"
INSERT INTO T_GAME_OBJECT_ATTRIBUTES 
    (attribute_id, game_object_id, attribute_name) 
VALUES 
    (@attribute_id, @game_object_id, @attribute_name)";

                command.Parameters.Add("@attribute_id", DbType.Int64).Value =
                    attribute.ClassAsAtom.ObjectId;
                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObjectId;
                command.Parameters.Add("@attribute_name", DbType.AnsiStringFixedLength, 50).Value =
                    attribute.Name;

                command.ExecuteNonQuery();

                if (attribute.Values != null)
                {
                    foreach (GameObjectAttributeValue value in attribute.Values)
                    {
                        InsertGameObjectAttributeValue(command, gameObjectId,
                            attribute.ClassAsAtom.ObjectId, value);
                    }
                }
            }
        }

        protected void InsertGameObjectAttributeValue(SQLiteCommand command,
            long gameObjectId, long attributeId, GameObjectAttributeValue value)
        {
            if (!value.ClassAsAtom.IsDestroyed)
            {
                command.CommandText = @"
INSERT INTO T_GAME_OBJECT_ATTRIBUTE_VALUES 
    (attribute_value_id, game_object_id, attribute_id, attribute_value_name, 
     attribute_value_value) 
VALUES 
    (@attribute_value_id, @game_object_id, @attribute_id, @attribute_value_name, 
     @attribute_value_value)";

                command.Parameters.Add("@attribute_value_id", DbType.Int64).Value =
                    value.ClassAsAtom.ObjectId;
                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObjectId;
                command.Parameters.Add("@attribute_id", DbType.Int64).Value =
                    attributeId;
                command.Parameters.Add("@attribute_value_name", DbType.AnsiStringFixedLength, 50).Value =
                    value.Name;
                command.Parameters.Add("@attribute_value_value", DbType.Single).Value =
                    value.Value;

                command.ExecuteNonQuery();
            }
        }

        protected void InsertGameObjectProperty(SQLiteCommand command,
            long gameObjectId, GameObjectProperty property)
        {
            if (!property.ClassAsAtom.IsDestroyed)
            {
                command.CommandText = @"
INSERT INTO T_GAME_OBJECT_PROPERTIES 
    (property_id, game_object_id, property_name, property_max_value, property_value) 
VALUES 
    (@property_id, @game_object_id, @property_name, @property_max_value, @property_value)";

                command.Parameters.Add("@property_id", DbType.Int64).Value =
                    property.ClassAsAtom.ObjectId;
                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObjectId;
                command.Parameters.Add("@property_name", DbType.AnsiStringFixedLength, 50).Value =
                    property.Name;
                command.Parameters.Add("@property_max_value", DbType.Single).Value =
                    property.MaxValue;
                command.Parameters.Add("@property_value", DbType.Single).Value =
                    property.Value;

                command.ExecuteNonQuery();
            }
        }

        protected void InsertGameObjectFeature(SQLiteCommand command,
            long gameObjectId, GameObjectFeature feature)
        {
            if (!feature.ClassAsAtom.IsDestroyed)
            {
                command.CommandText = @"
INSERT INTO T_GAME_OBJECT_FEATURES 
    (feature_id, game_object_id, feature_name) 
VALUES 
    (@feature_id, @game_object_id, @feature_name)";

                command.Parameters.Add("@feature_id", DbType.Int64).Value =
                    feature.ClassAsAtom.ObjectId;
                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObjectId;
                command.Parameters.Add("@feature_name", DbType.AnsiStringFixedLength, 50).Value =
                    feature.Name;

                command.ExecuteNonQuery();
            }
        }

        protected void InsertGameObjectParent(SQLiteCommand command, long gameObjectId,
            ElementaryGameObject parent)
        {
            command.CommandText = @"
INSERT INTO T_GAME_OBJECT_PARENTS 
    (game_object_id, parent_game_object_id, parent_game_object_type) 
VALUES 
    (@game_object_id, @parent_game_object_id, @parent_game_object_type)";

            command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                gameObjectId;
            command.Parameters.Add("@parent_game_object_id", DbType.AnsiStringFixedLength, 50).Value =
                parent.ObjectId;
            command.Parameters.Add("@parent_game_object_type", DbType.Byte, 50).Value =
                (byte) GameEntityTypesTable.TypeOf(parent);

            command.ExecuteNonQuery();
        }

        protected virtual void InsertGameObject(SQLiteCommand command,
            TObjType gameObject)
        {
            if (!gameObject.IsDestroyed)
            {
                command.CommandText = @"
INSERT INTO T_GAME_OBJECT 
    (game_object_id, game_object_name, game_object_transcription, game_object_description) 
VALUES 
    (@game_object_id, @game_object_name, @game_object_transcription, @game_object_description)";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObject.ObjectId;
                command.Parameters.Add("@game_object_name", DbType.AnsiStringFixedLength, 50).Value =
                    gameObject.Name;
                command.Parameters.Add("@game_object_transcription", DbType.StringFixedLength, 100).Value =
                    gameObject.Transcription;
                command.Parameters.Add("@game_object_description", DbType.StringFixedLength, 1000).Value =
                    gameObject.Description;

                command.ExecuteNonQuery();

                //Insert parents
                if (gameObject.Parents != null)
                {
                    foreach (ElementaryGameObject parent in gameObject.Parents.OwnParents)
                    {
                        InsertGameObjectParent(command, gameObject.ObjectId, parent);
                    }
                }

                //Insert attributes
                if (gameObject.Attributes != null)
                {
                    foreach (GameObjectAttribute attribute in gameObject.Attributes)
                    {
                        InsertGameObjectAttribute(command, gameObject.ObjectId, attribute);
                    }
                }

                //Insert properties
                if (gameObject.Properties != null)
                {
                    foreach (GameObjectProperty property in gameObject.Properties)
                    {
                        InsertGameObjectProperty(command, gameObject.ObjectId, property);
                    }
                }

                //Insert features
                if (gameObject.Features != null)
                {
                    foreach (GameObjectFeature feature in gameObject.Features)
                    {
                        InsertGameObjectFeature(command, gameObject.ObjectId, feature);
                    }
                }
            }
        }

        public override void Save()
        {
            using (SQLiteTransaction transaction = Connection.BeginTransaction())
            {
                using (SQLiteCommand command = new SQLiteCommand(Connection))
                {
                    //Clear database
                    ClearDatabase(command);

                    //Store game objects
                    foreach (TObjType obj in Values)
                    {
                        InsertGameObject(command, obj);
                    }

                    //Commit
                    transaction.Commit();
                }
            }
        }

        protected void EstablishGameObjectsInheritances(List<object[]> inheritanceTable)
        {
            ElementaryGameObjects egoList = GlobalStorage.Instance.ElementaryGameObjects;
            GameObjects goList = GlobalStorage.Instance.GameObjects;
            foreach (object[] data in inheritanceTable)
            {
                ElementaryGameObject gameObject = (ElementaryGameObject) data[0];
                long parentId = (long) data[1];
                GameEntityType parentType = (GameEntityType)data[2];
                gameObject.AddParent(parentType == GameEntityType.ElementaryGameObject
                    ? egoList[parentId]
                    : goList[parentId]);
            }
        }

        protected void PopulateGameObjectAttributeValues(TObjType obj, 
            GameObjectAttribute attribute)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_GAME_OBJECT_ATTRIBUTE_VALUES
WHERE game_object_id = @game_object_id AND attribute_id = @attribute_id";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    obj.ObjectId;
                command.Parameters.Add("@attribute_id", DbType.Int64).Value =
                    attribute.ClassAsAtom.ObjectId;

                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        GameObjectAttributeValue value = obj.AddAttributeValue(attribute.Name, 
                            (string)dr["attribute_value_name"]);
                        value.Value = (float)(double)dr["attribute_value_value"];
                    }
                }
            }
        }

        protected void PopulateGameObjectAttributes(TObjType obj)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_GAME_OBJECT_ATTRIBUTES
WHERE game_object_id = @game_object_id";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    obj.ObjectId;

                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        GameObjectAttribute attribute = obj.AddAttribute((string) dr["attribute_name"]);
                        PopulateGameObjectAttributeValues(obj, attribute);
                    }
                }
            }
        }

        protected void PopulateGameObjectProperties(TObjType obj)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_GAME_OBJECT_PROPERTIES
WHERE game_object_id = @game_object_id";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    obj.ObjectId;

                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        GameObjectProperty propety = obj.AddProperty((string)dr["property_name"]);
                        propety.MaxValue = (float)(double)dr["property_max_value"];
                        propety.Value = (float)(double)dr["property_value"];
                    }
                }
            }
        }

        protected void PopulateGameObjectFeatures(TObjType obj)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_GAME_OBJECT_FEATURES
WHERE game_object_id = @game_object_id";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    obj.ObjectId;

                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        obj.AddFeature((string)dr["feature_name"]);
                    }
                }
            }
        }

        protected void PopulateGameObjectParents(TObjType obj, List<object[]> inheritanceTable)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_GAME_OBJECT_PARENTS
WHERE game_object_id = @game_object_id";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    obj.ObjectId;

                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        long parentObjId = (long) dr["parent_game_object_id"];
                        GameEntityType parentObjType = (GameEntityType) dr["parent_game_object_type"];
                        inheritanceTable.Add(new object[] {obj, parentObjId, parentObjType});
                    }
                }
            }
        }

        protected void PopulateGameObjectMembers(TObjType obj, List<object[]> inheritanceTable)
        {
            //Populate parents
            PopulateGameObjectParents(obj, inheritanceTable);

            //Populate attributes
            PopulateGameObjectAttributes(obj);

            //Populate properties
            PopulateGameObjectProperties(obj);

            //Populate features
            PopulateGameObjectFeatures(obj);
        }

        protected virtual TObjType PopulateGameObjectFromDataReader(SQLiteDataReader dr)
        {
            //Populate game object
            TObjType obj = new TObjType();
            obj.ObjectId = (long)dr["game_object_id"];
            obj.Name = (string)dr["game_object_name"];
            obj.Transcription = (string)dr["game_object_transcription"];
            obj.Description = (string)dr["game_object_description"];

            //Return game object
            return obj;
        }

        protected virtual void PopulateGameObjects(List<object[]> inheritanceTable)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_GAME_OBJECT";
                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        TObjType obj = PopulateGameObjectFromDataReader(dr);
                        PopulateGameObjectMembers(obj, inheritanceTable);
                        Add(obj);
                    }
                }
            }
        }

        public override void Load(string baseFileName)
        {
            //Call base load method
            base.Load(baseFileName);

            //Clear self
            Clear();

            List<object[]> inheritanceTable = new List<object[]>();

            //Populate game objects
            PopulateGameObjects(inheritanceTable);

            //Establish game objects inheritances
            EstablishGameObjectsInheritances(inheritanceTable);
        }

#endregion

#region Class methods

        protected BaseGameObjects() { }

        protected BaseGameObjects(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

#endregion

    }
}
