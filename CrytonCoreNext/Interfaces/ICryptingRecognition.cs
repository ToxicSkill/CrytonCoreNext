namespace CrytonCoreNext.Interfaces
{
    public interface ICryptingRecognition
    {
        byte[] PrepareRerecognizableBytes(string method, string extension);
    }
}
