using System.Drawing;
using Kontract;
using Kontract.Kanvas;

namespace Leve5RessourceEditor.Level5
{
    class ImageProvider
    {
        private readonly IKanvasImage _kanvasImage;

        public Image Image
        {
            get => _kanvasImage.GetImage();
            set => _kanvasImage.SetImage((Bitmap)value);
        }

        public ImageProvider(IKanvasImage image)
        {
            ContractAssertions.IsNotNull(image, nameof(image));

            _kanvasImage = image;
        }
    }
}
