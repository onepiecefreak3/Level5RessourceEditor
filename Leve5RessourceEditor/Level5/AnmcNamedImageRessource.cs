using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Kontract;

namespace Leve5RessourceEditor.Level5
{
    class AnmcNamedImageRessource
    {
        public string Name { get; }

        public IList<AnmcImageRessource> ImageRessources { get; }

        public SizeF Size => GetSize();

        public PointF Location => GetLocation();

        public AnmcNamedImageRessource(string name, IList<AnmcImageRessource> imageRessources)
        {
            ContractAssertions.IsNotNull(name, nameof(name));
            ContractAssertions.IsNotNull(imageRessources, nameof(imageRessources));

            Name = name;
            ImageRessources = imageRessources;
        }

        public void DrawOnImage(Image image)
        {
            foreach (var imageRessource in ImageRessources)
                imageRessource.DrawOnImage(image);
        }

        public Image GetImage()
        {
            var image = new Bitmap((int)Size.Width, (int)Size.Height);
            using var g = Graphics.FromImage(image);

            var location = Location;
            foreach (var imageRessource in ImageRessources)
            {
                var dest = new PointF(Math.Abs(location.X - imageRessource.Location.X), Math.Abs(location.Y - imageRessource.Location.Y));

                var drawImg = imageRessource.GetImage();
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
            if (!ImageRessources.Any())
                return SizeF.Empty;

            var maxWidth = ImageRessources.Max(x => x.Location.X + x.Size.Width);
            var maxHeight = ImageRessources.Max(x => x.Location.Y + x.Size.Height);

            var location = Location;
            return new SizeF(maxWidth - location.X, maxHeight - location.Y);
        }

        private PointF GetLocation()
        {
            if (!ImageRessources.Any())
                return PointF.Empty;

            var minX = ImageRessources.Min(x => x.Location.X);
            var minY = ImageRessources.Min(x => x.Location.Y);

            return new PointF(minX, minY);
        }
    }
}
