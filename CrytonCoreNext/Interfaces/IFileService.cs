namespace CrytonCoreNext.Interfaces
{
    public interface IFileService : IFilesLoader, IFilesSaver, IFilesManager
    {
        bool HasBytes(Models.File? file);
    }
}
