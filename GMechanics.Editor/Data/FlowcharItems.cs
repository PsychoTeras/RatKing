using System.Data;
using System.Data.SQLite;
using System.Drawing;
using GMechanics.Core.Classes.Storages.BaseStorages;
using GMechanics.FlowchartControl;

namespace GMechanics.Editor.Data
{
    public sealed class FlowcharItems : BaseSQLiteConnector<FlowcharItems>
    {
        public FlowcharItemData GetFlowchartItemData(long gameObjectId, 
            long rootGameObjectId)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
SELECT *
FROM T_FLOWCHART_ITEM_DATA
WHERE game_object_id = @game_object_id AND 
      root_game_object_id = @root_game_object_id
";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObjectId;
                command.Parameters.Add("@root_game_object_id", DbType.Int64).Value =
                    rootGameObjectId;

                using (SQLiteDataReader dr = command.ExecuteReader())
                {
                    if (dr.Read())
                    {
                        return new FlowcharItemData(new Rectangle(
                                                        (short) dr["left"],
                                                        (short) dr["top"],
                                                        (short) dr["width"],
                                                        (short) dr["height"]),
                                                    (string) dr["skin_name"]);
                    }
                }
            }

            return null;
        }

        public void StoreFlowchartItemData(FlowchartItem item, long gameObjectId, 
            long rootGameObjectId)
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
REPLACE INTO T_FLOWCHART_ITEM_DATA 
(
    game_object_id, 
    root_game_object_id, 
    left,
    top,
    width,
    height,
    skin_name
)
VALUES 
(
    @game_object_id, 
    @root_game_object_id, 
    @left,
    @top,
    @width,
    @height,
    @skin_name
)";

                command.Parameters.Add("@game_object_id", DbType.Int64).Value =
                    gameObjectId;
                command.Parameters.Add("@root_game_object_id", DbType.Int64).Value =
                    rootGameObjectId;
                command.Parameters.Add("@left", DbType.Int16).Value =
                    item.Left;
                command.Parameters.Add("@top", DbType.Int16).Value =
                    item.Top;
                command.Parameters.Add("@width", DbType.Int16).Value =
                    item.Width;
                command.Parameters.Add("@height", DbType.Int16).Value =
                    item.Height;
                command.Parameters.Add("@skin_name", DbType.StringFixedLength, 30).Value =
                    item.SkinName;

                command.ExecuteNonQuery();
            }
        }

        protected override void CreateDbStructure()
        {
            using (SQLiteCommand command = new SQLiteCommand(Connection))
            {
                command.CommandText = @"
CREATE TABLE T_FLOWCHART_ITEM_DATA
(
    game_object_id BIGINT NOT NULL, 
    root_game_object_id BIGINT NOT NULL, 
    left SMALLINT NOT NULL,
    top SMALLINT NOT NULL,
    width SMALLINT NOT NULL,
    height SMALLINT NOT NULL,
    skin_name NVARCHAR(30) NOT NULL,
    UNIQUE(game_object_id, root_game_object_id) ON CONFLICT REPLACE
)";
                command.ExecuteNonQuery();
            }
        }
    }
}
