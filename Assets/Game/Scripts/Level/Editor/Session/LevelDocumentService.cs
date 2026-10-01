namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public class LevelDocumentService
    {
        private readonly LevelEditorSession _session;
        private readonly LevelFileRepository _repository;
        private readonly LevelGridResizer _resizer;

        public LevelDocumentService(LevelEditorSession session, LevelFileRepository repository, LevelGridResizer resizer)
        {
            _session = session;
            _repository = repository;
            _resizer = resizer;
        }

        public void CreateNew(int levelId, float duration, int width, int height)
        {
            LevelGridRegistry.Clear(_session.Grid);
            _resizer.Resize(width, height);
            _session.Open(levelId, duration, null);
        }

        public void Load(string path)
        {
            var data = _repository.Read(path);
            LevelFileMapper.FillGrid(_session.Grid, data);
            _session.Open(data.LevelId, data.Duration, path);
        }
    }
}
