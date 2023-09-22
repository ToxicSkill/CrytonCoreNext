using CrytonCoreNext.Enums;
using CrytonCoreNext.Models;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingRecognition
    {
        RecognitionResult GetRecognitionBytes(Recognition recon);

        Recognition RecognizeBytes(byte[] bytes);

        void AddObject(ERObject obj);

        void RemoveObject(ERObject obj);
    }
}
