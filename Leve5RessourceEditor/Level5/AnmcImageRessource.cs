using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Kontract;
using Leve5RessourceEditor.Level5.Models;

namespace Leve5RessourceEditor.Level5
{
    class AnmcImageRessource
    {
        private readonly ImageProvider _imageProvider;
        private readonly IList<PointMapping> _mappings;

        public string Name { get; }

        public PointF Location { get; private set; }
        public SizeF Size { get; private set; }

        public PointF UvLocation { get; private set; }
        public SizeF UvSize { get; private set; }

        public AnmcImageRessource(ImageProvider imageProvider, IList<PointMapping> mappings, string name)
        {
            ContractAssertions.IsNotNull(imageProvider, nameof(imageProvider));
            ContractAssertions.IsNotNull(mappings, nameof(mappings));

            if (mappings.Count != 6)
                throw new InvalidOperationException("One image contains of 6 mapping points.");

            Name = name;

            _imageProvider = imageProvider;
            _mappings = mappings.Distinct().ToArray();

            LoadLocations(mappings);
            LoadSizes(mappings);
        }

        public void SetLocation(PointF newLocation)
        {
            var deltaX = newLocation.X - Location.X;
            var deltaY = newLocation.Y - Location.Y;

            foreach (var mapping in _mappings)
            {
                mapping.x += deltaX;
                mapping.y += deltaY;
            }

            LoadLocations(_mappings);
        }

        public void SetUvLocation(PointF newLocation)
        {
            var deltaU = newLocation.X - UvLocation.X;
            var deltaV = newLocation.Y - UvLocation.Y;

            foreach (var mapping in _mappings)
            {
                mapping.u += deltaU;
                mapping.v += deltaV;
            }

            LoadLocations(_mappings);
        }

        public void SetSize(SizeF newSize)
        {
            var deltaWidth = newSize.Width - Size.Width;
            var deltaHeight = newSize.Height - Size.Height;

            _mappings[1].y += deltaHeight;
            _mappings[2].x += deltaWidth;

            _mappings[3].x += deltaWidth;
            _mappings[3].y += deltaHeight;

            LoadSizes(_mappings);
        }

        public void SetUvSize(SizeF newSize)
        {
            var deltaWidth = newSize.Width - UvSize.Width;
            var deltaHeight = newSize.Height - UvSize.Height;

            _mappings[1].v += deltaHeight;
            _mappings[2].u += deltaWidth;

            _mappings[3].u += deltaWidth;
            _mappings[3].v += deltaHeight;

            LoadSizes(_mappings);
        }

        public void DrawOnImage(Image image)
        {
            using var g = Graphics.FromImage(image);
            var centeredLocation = new PointF(image.Width / 2 + Location.X, image.Height / 2 + Location.Y);

            var map = _imageProvider.Image;
            g.DrawImage(map, new RectangleF(centeredLocation, Size), new RectangleF(UvLocation, UvSize), GraphicsUnit.Pixel);
        }

        public Image GetImage()
        {
            var image = new Bitmap((int)Size.Width, (int)Size.Height);
            using var g = Graphics.FromImage(image);

            var map = _imageProvider.Image;
            g.DrawImage(map, new RectangleF(Point.Empty, Size), new RectangleF(UvLocation, UvSize), GraphicsUnit.Pixel);

            return image;
        }

        public override string ToString()
        {
            return Name;
        }

        private void LoadLocations(IList<PointMapping> mappings)
        {
            var minLocation = mappings[0];

            Location = new PointF(minLocation.x, minLocation.y);
            UvLocation = new PointF(minLocation.u, minLocation.v);
        }

        private void LoadSizes(IList<PointMapping> mappings)
        {
            var maxPoint = mappings[3];

            Size = new SizeF(maxPoint.x - Location.X, maxPoint.y - Location.Y);
            UvSize = new SizeF(maxPoint.u - UvLocation.X, maxPoint.v - UvLocation.Y);
        }
    }
}
