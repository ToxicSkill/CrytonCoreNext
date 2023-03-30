using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrytonCoreNext.Models
{
    public class File : INotifyPropertyChanged
    {
        public string Name { get; init; }

        public string NameWithExtension { get => $"{Name}.{Extension}"; }

        public string Path { get; init; }

        public string Size { get; init; }

        public DateTime Date { get; init; }

        public string Extension { get; set; }

        public int Id { get; set; }

        public byte[] Bytes { get; set; }

        public Guid Guid { get; init; } = Guid.NewGuid();

        public event PropertyChangedEventHandler? PropertyChanged;

        public File(string name, string path, string size, DateTime date, string extension, int id, byte[] bytes)
        {
            Name = name;
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
            Path = file.Path;
            Size = file.Size;
            Date = file.Date;
            Extension = file.Extension;
            Id = file.Id;
            Bytes = file.Bytes;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
