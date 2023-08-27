namespace CrytonCoreNext.Crypting.Models
{
    public class CryptingStatistics
    {
        public int Speed { get; set; }

        public int Strenght { get; set; }

        public int Application { get; set; }

        public CryptingStatistics(int speed, int strenght, int application)
        {
            Speed = speed;
            Strenght = strenght;
            Application = application;
        }
    }
}
