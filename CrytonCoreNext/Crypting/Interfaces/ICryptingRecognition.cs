using CrytonCoreNext.Crypting.Enums;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingRecognition
    {
        byte[] PrepareRerecognizableBytes(EMethod method, string extension, string keys);

        (bool succes, (EMethod method, string extension, string keys)) RecognizeBytes(byte[] bytes);
    }
}
