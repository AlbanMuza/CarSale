namespace CarSale.Models
{
    public class Enumerators
    {

        public enum Make : Byte
        {
            Toyota=1,Mazda,Ford,Honda,Mitsubishi
        }
        public enum Colour : Byte
        {
            Red, Black, White
        }

        public enum NewUsed : Byte
        {
            New,
            Used
        }
        public enum Sstates : Byte
        {
            Published,
            Draft,
            Archived
        }

    }
}
