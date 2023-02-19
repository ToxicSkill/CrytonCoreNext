namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingRecognition
    {
        byte[] PrepareRerecognizableBytes(string method, string extension);

        (bool succes, (string method, string extension)) RecognizeBytes(byte[] bytes);
    }
}
