namespace CrytonCoreNext.Models
{
    public class File
    {
        public File(File file)
        {
            Name = file.Name;
            Path = file.Path;
            Size = file.Size;
            Id = file.Id;
            Bytes = file.Bytes;
        }

        public File() { }

        public string Name { get; init; }
        public string Path { get; init; }
        public string Size { get; init; }
        public int Id { get; set; }
        public byte[] Bytes { get; init; }
    }
}
