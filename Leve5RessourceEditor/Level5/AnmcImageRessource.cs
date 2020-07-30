using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
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
            _mappings = mappings.ToArray();

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
            _mappings[5].y += deltaHeight;

            _mappings[2].x += deltaWidth;
            _mappings[4].x += deltaWidth;

            _mappings[3].x += deltaWidth;
            _mappings[3].y += deltaHeight;

            LoadSizes(_mappings);
        }

        public void SetUvSize(SizeF newSize)
        {
            var deltaWidth = newSize.Width - UvSize.Width;
            var deltaHeight = newSize.Height - UvSize.Height;

            _mappings[1].v += deltaHeight;
            _mappings[5].v += deltaHeight;

            _mappings[2].u += deltaWidth;
            _mappings[4].u += deltaWidth;

            _mappings[3].u += deltaWidth;
            _mappings[3].v += deltaHeight;

            LoadSizes(_mappings);
        }

        public void DrawOnImage(Image image)
        {
            using var g = Graphics.FromImage(image);
            //g.InterpolationMode = InterpolationMode.NearestNeighbor;

            //var imageCenter = new PointF(image.Width / 2, image.Height / 2);
            var topLeftLocation = new PointF(image.Width / 2 + Location.X, image.Height / 2 + Location.Y);

            // Draw triangles
            //var srcPoints = new[] { _mappings[0].GetUv(), _mappings[1].GetUv(), _mappings[2].GetUv() };
            //var destPoints = new[]
            //{
            //    PointF.Add(_mappings[0].GetLocation(), new SizeF(imageCenter)),
            //    PointF.Add(_mappings[1].GetLocation(), new SizeF(imageCenter)),
            //    PointF.Add(_mappings[2].GetLocation(), new SizeF(imageCenter))
            //};
            //DrawTriangle(g, srcPoints, destPoints);

            //srcPoints = new[] { _mappings[3].GetUv(), _mappings[4].GetUv(), _mappings[5].GetUv() };
            //destPoints = new[]
            //{
            //    PointF.Add(_mappings[3].GetLocation(), new SizeF(imageCenter)),
            //    PointF.Add(_mappings[4].GetLocation(), new SizeF(imageCenter)),
            //    PointF.Add(_mappings[5].GetLocation(), new SizeF(imageCenter))
            //};
            //DrawTriangle(g, srcPoints, destPoints);

            var map = _imageProvider.Image;
            g.DrawImage(map, new RectangleF(topLeftLocation, Size), new RectangleF(UvLocation, UvSize), GraphicsUnit.Pixel);
        }

        public Image GetImage()
        {
            var image = new Bitmap((int)Size.Width, (int)Size.Height);
            using var g = Graphics.FromImage(image);

            // Draw triangles
            //var srcPoints = new[] { _mappings[0].GetUv(), _mappings[1].GetUv(), _mappings[2].GetUv() };
            //var destPoints = new[] { _mappings[0].GetLocation(), _mappings[1].GetLocation(), _mappings[2].GetLocation() };
            //DrawTriangle(g, srcPoints, destPoints);

            //srcPoints = new[] { _mappings[3].GetUv(), _mappings[4].GetUv(), _mappings[5].GetUv() };
            //destPoints = new[] { _mappings[3].GetLocation(), _mappings[4].GetLocation(), _mappings[5].GetLocation() };
            //DrawTriangle(g, srcPoints, destPoints);

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

        // TODO: Transform source triangle to destiantion triangle
        private void DrawTriangle(Graphics g, PointF[] srcPoints, PointF[] destPoints)
        {
            // Create buffer image
            var srcRect = ComputeRectangle(srcPoints);
            var newBmp = new Bitmap((int)srcRect.Width, (int)srcRect.Height);
            using var bufferG = Graphics.FromImage(newBmp);

            // Restrict area to draw the image to
            using var gp = new GraphicsPath();
            gp.AddPolygon(srcPoints.Select(x=>PointF.Subtract(x,new SizeF(srcRect.Location))).ToArray());
            bufferG.Clip = new Region(gp);

            // Cut image from source
            bufferG.DrawImage(_imageProvider.Image, new RectangleF(0, 0, srcRect.Width, srcRect.Height), srcRect, GraphicsUnit.Pixel);

            // Draw buffer image to final image
            var destRect = ComputeRectangle(destPoints);
            g.DrawImage(newBmp, destRect);
        }

        private RectangleF ComputeRectangle(PointF[] points)
        {
            var minX = points.Min(x => x.X);
            var maxX = points.Max(x => x.X);

            var minY = points.Min(x => x.Y);
            var maxY = points.Max(x => x.Y);

            return new RectangleF(minX, minY, maxX - minX, maxY - minY);
        }
    }
}
