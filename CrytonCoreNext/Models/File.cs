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

        public string Extension { get; set; }

        public int Id { get; set; }

        public byte[] Bytes { get; set; }

        public Guid Guid { get; init; } = Guid.NewGuid();

        public File(string name, string nameWithExtension, string path, string size, DateTime date, string extension, int id, byte[] bytes)
        {
            Name = name;
            NameWithExtension = nameWithExtension;
            Path = path;
            Size = size;
            Date = date;
            Extension = extension;
            Id = id;
            Bytes = bytes;
        }

        public File(File file, string name, byte[] bytes, int id)
        {
            Name = name;
            NameWithExtension = name + '.' + file.Extension;
            Path = file.Path;
            Size = file.Size;
            Date = file.Date;
            Extension = file.Extension;
            Id = id;
            Bytes = bytes;
        }

        public File(File file)
        {
            Name = file.Name;
            NameWithExtension = file.NameWithExtension;
            Path = file.Path;
            Size = file.Size;
            Date = file.Date;
            Extension = file.Extension;
            Id = file.Id;
            Bytes = file.Bytes;
        }
    }
}
