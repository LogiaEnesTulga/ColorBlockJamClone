using System.IO;

namespace RollicGames.ColorBlockJamClone.Level.Editor
{
    public readonly struct LevelSaveResult
    {
        public readonly bool IsSuccess;
        public readonly string Message;

        private LevelSaveResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static LevelSaveResult Success(string message) => new(true, message);
        public static LevelSaveResult Failure(string message) => new(false, message);
    }

    public class LevelSaveService
    {
        private readonly LevelFileRepository _repository;

        public LevelSaveService(LevelFileRepository repository)
        {
            _repository = repository;
        }

        /// <summary>Saves the level. A previously saved level is replaced, and renamed when its id changed.</summary>
        public LevelSaveResult Save(LevelEditorSession session)
        {
            return Write(session, session.FilePath);
        }

        /// <summary>Saves the level as a separate file. The id must differ from every saved level, including the loaded one.</summary>
        public LevelSaveResult SaveAsNew(LevelEditorSession session)
        {
            return Write(session, null);
        }

        private LevelSaveResult Write(LevelEditorSession session, string replacedPath)
        {
            var levelId = session.LevelId;
            var conflict = _repository.FindLevel(levelId, replacedPath);
            if(conflict != null)
            {
                return LevelSaveResult.Failure($"Level ID {levelId} is not unique. It is already used by {conflict.FileName}.");
            }

            var targetPath = _repository.GetLevelPath(levelId);
            var isReplacingTarget = LevelFileRepository.IsSamePath(targetPath, replacedPath);
            if(!isReplacingTarget && _repository.Exists(targetPath))
            {
                return LevelSaveResult.Failure($"{Path.GetFileName(targetPath)} already exists.");
            }

            _repository.Write(targetPath, LevelFileMapper.ToFileData(session));
            if(replacedPath != null && !isReplacingTarget)
            {
                _repository.Delete(replacedPath);
            }

            session.MarkSaved(targetPath);
            return LevelSaveResult.Success($"Saved {Path.GetFileName(targetPath)}");
        }
    }
}
