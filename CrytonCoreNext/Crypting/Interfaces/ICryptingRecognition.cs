using CrytonCoreNext.Crypting.Enums;
using CrytonCoreNext.Models;

namespace CrytonCoreNext.Crypting.Interfaces
{
    public interface ICryptingRecognition
    {
        byte[] PrepareRerecognizableBytes(Recognition recon);

        Recognition RecognizeBytes(byte[] bytes);
    }
}
