namespace CrytonCoreNext.Interfaces.Files
{
    public interface IFileService : IFilesLoader, IFilesSaver, IFilesManager
    {
        bool HasBytes(Models.File? file);
    }
}
