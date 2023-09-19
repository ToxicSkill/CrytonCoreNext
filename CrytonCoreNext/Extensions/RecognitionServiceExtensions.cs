using CrytonCoreNext.Crypting.Interfaces;
using CrytonCoreNext.Enums;

namespace CrytonCoreNext.Extensions
{
    public static class RecognitionServiceExtensions
    {
        public static ICryptingRecognition Add(this ICryptingRecognition cryptingRecognition, ERObject rObject)
        {
            cryptingRecognition.AddObject(rObject);
            return cryptingRecognition;
        }

        public static ICryptingRecognition Remove(this ICryptingRecognition cryptingRecognition, ERObject rObject)
        {
            cryptingRecognition.RemoveObject(rObject);
            return cryptingRecognition;
        }
    }
}
