namespace GMechanics.Editor.Data
{
    public class GameObjectEventScriptDBRecord
    {
        public string Source;
        public bool IsActive;

        public GameObjectEventScriptDBRecord() {}

        public GameObjectEventScriptDBRecord(string source, bool isActive)
        {
            Source = source;
            IsActive = isActive;
        }
    }
}
