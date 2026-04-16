using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Kontract;

namespace Level5ResourceEditor.Level5
{
    class AnmcNamedImageResource
    {
        public string Name { get; }

        public IList<AnmcImageResource> ImageResources { get; }

        public SizeF Size => GetSize();

        public PointF Location => GetLocation();

        public AnmcNamedImageResource(string name, IList<AnmcImageResource> imageResources)
        {
            ContractAssertions.IsNotNull(name, nameof(name));
            ContractAssertions.IsNotNull(imageResources, nameof(imageResources));

            Name = name;
            ImageResources = imageResources;
        }

        public void DrawOnImage(Image image)
        {
            foreach (var imageResource in ImageResources)
                imageResource.DrawOnImage(image);
        }

        public Image GetImage()
        {
            var image = new Bitmap((int)Size.Width, (int)Size.Height);
            using var g = Graphics.FromImage(image);

            var location = Location;
            foreach (var imageResource in ImageResources)
            {
                var dest = new PointF(Math.Abs(location.X - imageResource.Location.X), Math.Abs(location.Y - imageResource.Location.Y));

                var drawImg = imageResource.GetImage();
                g.DrawImage(drawImg, dest);
            }

            return image;
        }

        public override string ToString()
        {
            return Name;
        }

        private SizeF GetSize()
        {
            if (!ImageResources.Any())
                return SizeF.Empty;

            var maxWidth = ImageResources.Max(x => x.Location.X + x.Size.Width);
            var maxHeight = ImageResources.Max(x => x.Location.Y + x.Size.Height);

            var location = Location;
            return new SizeF(maxWidth - location.X, maxHeight - location.Y);
        }

        private PointF GetLocation()
        {
            if (!ImageResources.Any())
                return PointF.Empty;

            var minX = ImageResources.Min(x => x.Location.X);
            var minY = ImageResources.Min(x => x.Location.Y);

            return new PointF(minX, minY);
        }
    }
}
