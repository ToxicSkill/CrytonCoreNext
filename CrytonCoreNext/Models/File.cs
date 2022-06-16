using System;

namespace CrytonCoreNext.Models
{
    public class File
    {
        public string Name { get; init; }

        public string NameWithExtension { get; init; }

        public string Path { get; init; }

        public string Size { get; init; }

        public DateTime Date { get; init; }

        public string Extension { get; init; }

        public int Id { get; set; }

        public byte[] Bytes { get; init; }

        //public string Text { get; init; }

        public Guid Guid { get; init; }

        public File() { }

        public File(string name, string path, string size, string nameWithExtension, DateTime date, string extension, int id, byte[] bytes)
        {
            Name = name;
            NameWithExtension = nameWithExtension;
            Path = path;
            Size = size;
            Date = date;
            Extension = extension;
            Id = id;
            Bytes = bytes;
            //Text = Parsers.FileContentParser.GetStringFromBytes(bytes);
            Guid = new Guid();
        }
    }
}
