using CrytonCoreNext.Extensions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CrytonCoreNext.Models
{
    public class File : INotifyPropertyChanged
    {
        public string Name { get; private set; }

        public string NameWithExtension { get => $"{Name}.{Extension}"; }

        public string Path { get; init; }

        public string Size { get; init; }

        public string Suffix { get; set; }

        public DateTime Date { get; init; }

        public string Extension { get; set; }

        public int Id { get; set; }

        public byte[] Bytes { get; set; }

        /// <summary>
        /// Set value of that property to false when testing (no need STA UI thread)
        /// </summary>
        public bool LoadMetadata { get; set; } = true;

        public Guid Guid { get; init; } = Guid.NewGuid();

        public event PropertyChangedEventHandler? PropertyChanged;

        public File(string name, string path, DateTime date, string extension, int id, byte[] bytes)
        {
            Name = name;
            Path = path;
            Size = bytes.GetSizeString();
            Date = date;
            Extension = extension;
            Id = id;
            Bytes = bytes;
        }

        public File(File file, string name, byte[] bytes, int id)
        {
            Name = name;
            Path = file.Path;
            Size = bytes.GetSizeString();
            Date = file.Date;
            Extension = file.Extension;
            Id = id;
            Bytes = bytes;
        }

        public File(File file)
        {
            Name = file.Name;
            Path = file.Path;
            Size = file.Bytes.GetSizeString();
            Date = file.Date;
            Extension = file.Extension;
            Id = file.Id;
            Bytes = file.Bytes;
            LoadMetadata = file.LoadMetadata;
        }

        public void Rename(string newName)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                this.Name = newName;
            }
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
