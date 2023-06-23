using CrytonCoreNext.Crypting.Enums;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingRecognition
    {
        byte[] PrepareRerecognizableBytes(EMethod method, string extension);

        (bool succes, (EMethod method, string extension)) RecognizeBytes(byte[] bytes);
    }
}
