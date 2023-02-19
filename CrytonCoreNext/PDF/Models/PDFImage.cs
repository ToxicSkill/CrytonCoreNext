using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace CrytonCoreNext.PDF.Models
{
    public class PDFImage
    {
        public Guid Guid { get; set; }

        public List<ImageSource> Images { get; set; }

        public PDFImage(List<ImageSource> images, Guid owner)
        {
            Guid = owner;
            Images = images;
        }
    }
}
