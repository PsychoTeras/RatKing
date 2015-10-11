namespace GMechanics.Editor.Data
{
    public sealed class ScriptSourceData
    {
        public string Source;
        public bool IsActive;

        public ScriptSourceData() {}

        public ScriptSourceData(string source, bool isActive)
        {
            Source = source;
            IsActive = isActive;
        }
    }
}
