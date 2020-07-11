using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Kore.Managers.Plugins;
using Kore.Progress;
using Leve5RessourceEditor.Level5;

namespace Leve5RessourceEditor
{
    public partial class Form1 : Form
    {
        private readonly AnmcRessource _ressourceManager;

        private readonly IDictionary<string, Size> _imageSizeDictionary = new Dictionary<string, Size>
        {
            ["Top Screen"] = new Size(400, 240),
            ["Bottom Screen"] = new Size(320, 240)
        };

        public Form1()
        {
            InitializeComponent();

            var assemblyLoader=new Kore.Managers.Plugins.PluginLoader.AssemblyFilePluginLoader(Assembly.GetAssembly(typeof(plugin_level5.Archives.Arc0Plugin)));
            var pluginManager = new PluginManager(assemblyLoader);
            _ressourceManager = new AnmcRessource(pluginManager);

            DrawGridColor(tsbGridColor1, pb.GridColor);
            DrawGridColor(tsbGridColor2, pb.GridColorAlternate);
        }

        #region Open File

        private async void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (string.IsNullOrEmpty(ofd.FileName) || !File.Exists(ofd.FileName))
                return;

            IList<AnmcNamedImageRessource> namedImageRessources;
            try
            {
                namedImageRessources = await _ressourceManager.Load(ofd.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK);
                return;
            }

            pbiList.ItemCheck -= pbiList_ItemCheck;
            FillCheckList(namedImageRessources);
            pbiList.ItemCheck += pbiList_ItemCheck;

            UpdateImage();
        }

        private void FillCheckList(IList<AnmcNamedImageRessource> imageRessources)
        {
            pbiList.Items.Clear();

            // TODO: Make another listbox to separate named ressource from image ressource properly
            foreach (var imageRessource in imageRessources.SelectMany(x => x.ImageRessources))
                pbiList.Items.Add(imageRessource, true);
        }

        #endregion

        #region Events

        private void pbiList_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkedItems = GetCheckedItems<AnmcImageRessource>(pbiList);

            if (e.NewValue == CheckState.Checked)
                checkedItems.Add((AnmcImageRessource)pbiList.Items[e.Index]);

            if (e.NewValue == CheckState.Unchecked)
                checkedItems.Remove((AnmcImageRessource)pbiList.Items[e.Index]);

            UpdateImage(checkedItems);
        }

        private void UncheckAll_Click(object sender, EventArgs e)
        {
            UncheckAll();
            UpdateImage();
        }

        private void CheckAll_Click(object sender, EventArgs e)
        {
            CheckAll();
            UpdateImage();
        }

        private void txtWidth_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtWidth, value => imageRessource.SetSize(new SizeF(value, imageRessource.Size.Height)));

            UpdateImage();
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtHeight,
                value => imageRessource.SetSize(new SizeF(imageRessource.Size.Width, value)));

            UpdateImage();
        }

        private void txtLocationX_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtLocationX, value => imageRessource.SetLocation(new PointF(value, imageRessource.Location.Y)));

            UpdateImage();
        }

        private void txtLocationY_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtLocationY, value => imageRessource.SetLocation(new PointF(imageRessource.Location.X, value)));

            UpdateImage();
        }

        private void txtUvWidth_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtUvWidth, value => imageRessource.SetUvSize(new SizeF(value, imageRessource.UvSize.Height)));

            UpdateImage();
        }

        private void txtUvHeight_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtUvHeight, value => imageRessource.SetUvSize(new SizeF(imageRessource.UvSize.Width, value)));

            UpdateImage();
        }

        private void txtUvLocationX_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtUvLocationX, value => imageRessource.SetUvLocation(new PointF(value, imageRessource.UvLocation.Y)));

            UpdateImage();
        }

        private void txtUvLocationY_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = GetSelectedImageRessource();
            UpdateImageInformation(txtUvLocationY, value => imageRessource.SetUvLocation(new PointF(imageRessource.UvLocation.X, value)));

            UpdateImage();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog();

            if (sfd.ShowDialog() != DialogResult.OK)
            {
                MessageBox.Show("An error occurred when selecting a save path.", "Save Error", MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            var fileName = sfd.FileName;
            _ressourceManager.Save(fileName);
        }

        private void resolutionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void newMapToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Rethink injection of images
            //var ofd = new OpenFileDialog
            //{
            //    InitialDirectory = Environment.CurrentDirectory,
            //    Filter = "Portable Network Graphic (*.png)|*.png",
            //    FilterIndex = 1,
            //    RestoreDirectory = true
            //};

            //if (ofd.ShowDialog() != DialogResult.OK)
            //    return;

            //var newImage = Image.FromFile(ofd.FileName);
            //_ressourceManager.SetImage(newImage);

            //UpdateImage();
        }

        #endregion

        #region Update Functions

        private void UpdateImage()
        {
            var checkedRessources = GetCheckedItems<AnmcImageRessource>(pbiList);
            UpdateImage(checkedRessources);
        }

        private void UpdateImage(IList<AnmcImageRessource> imageRessources)
        {
            var imageSize = _imageSizeDictionary[resolutionList.Text];
            var image = new Bitmap(imageSize.Width, imageSize.Height);

            foreach (var imageRessource in imageRessources)
                imageRessource.DrawOnImage(image);

            pb.Image = image;
        }

        #endregion

        #region Helper Methods

        private IList<T> GetCheckedItems<T>(CheckedListBox checkBox)
        {
            var checkedItems = new List<T>();
            foreach (T checkedItem in checkBox.CheckedItems)
                checkedItems.Add(checkedItem);

            return checkedItems;
        }

        private void CheckAll()
        {
            pbiList.ItemCheck -= pbiList_ItemCheck;

            for (var i = 0; i < pbiList.Items.Count; i++)
                pbiList.SetItemCheckState(i, CheckState.Checked);

            pbiList.ItemCheck += pbiList_ItemCheck;
        }

        private void UncheckAll()
        {
            pbiList.ItemCheck -= pbiList_ItemCheck;

            for (var i = 0; i < pbiList.Items.Count; i++)
                pbiList.SetItemCheckState(i, CheckState.Unchecked);

            pbiList.ItemCheck += pbiList_ItemCheck;
        }

        #endregion

        private AnmcImageRessource GetSelectedImageRessource()
        {
            return (AnmcImageRessource)pbiList.SelectedItem;
        }

        private void UpdateImageInformation(Control textBox, Action<float> valueSetter)
        {
            if (string.IsNullOrEmpty(textBox.Text) || !float.TryParse(textBox.Text, out var value))
                return;

            valueSetter(value);
        }

        private void FillImageInformation(AnmcImageRessource anmcImageRessource)
        {
            txtWidth.TextChanged -= txtWidth_TextChanged;
            txtHeight.TextChanged -= txtHeight_TextChanged;
            txtUvWidth.TextChanged -= txtUvWidth_TextChanged;
            txtUvHeight.TextChanged -= txtUvHeight_TextChanged;
            txtLocationX.TextChanged -= txtLocationX_TextChanged;
            txtLocationY.TextChanged -= txtLocationY_TextChanged;
            txtUvLocationX.TextChanged -= txtUvLocationX_TextChanged;
            txtUvLocationY.TextChanged -= txtUvLocationY_TextChanged;

            txtWidth.Enabled = true;
            txtHeight.Enabled = true;
            txtUvWidth.Enabled = true;
            txtUvHeight.Enabled = true;
            txtLocationX.Enabled = true;
            txtLocationY.Enabled = true;
            txtUvLocationX.Enabled = true;
            txtUvLocationY.Enabled = true;

            txtWidth.Text = anmcImageRessource.Size.Width.ToString(CultureInfo.InvariantCulture);
            txtHeight.Text = anmcImageRessource.Size.Height.ToString(CultureInfo.InvariantCulture);
            txtUvWidth.Text = anmcImageRessource.UvSize.Width.ToString(CultureInfo.InvariantCulture);
            txtUvHeight.Text = anmcImageRessource.UvSize.Height.ToString(CultureInfo.InvariantCulture);
            txtLocationX.Text = anmcImageRessource.Location.X.ToString(CultureInfo.InvariantCulture);
            txtLocationY.Text = anmcImageRessource.Location.Y.ToString(CultureInfo.InvariantCulture);
            txtUvLocationX.Text = anmcImageRessource.UvLocation.X.ToString(CultureInfo.InvariantCulture);
            txtUvLocationY.Text = anmcImageRessource.UvLocation.Y.ToString(CultureInfo.InvariantCulture);

            txtWidth.TextChanged += txtWidth_TextChanged;
            txtHeight.TextChanged += txtHeight_TextChanged;
            txtUvWidth.TextChanged += txtUvWidth_TextChanged;
            txtUvHeight.TextChanged += txtUvHeight_TextChanged;
            txtLocationX.TextChanged += txtLocationX_TextChanged;
            txtLocationY.TextChanged += txtLocationY_TextChanged;
            txtUvLocationX.TextChanged += txtUvLocationX_TextChanged;
            txtUvLocationY.TextChanged += txtUvLocationY_TextChanged;
        }

        private void pbiList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pbiList.SelectedIndex < 0)
                return;

            var imageRessource = GetSelectedImageRessource();
            FillImageInformation(imageRessource);
        }

        private void toolStripDropDownButton1_Click(object sender, EventArgs e)
        {
            SetGridColor(pb.GridColor, clr =>
            {
                pb.GridColor = clr;
                DrawGridColor(tsbGridColor1, clr);
            });
        }

        private void toolStripDropDownButton2_Click(object sender, EventArgs e)
        {
            SetGridColor(pb.GridColorAlternate, clr =>
            {
                pb.GridColorAlternate = clr;
                DrawGridColor(tsbGridColor2, clr);
            });
        }

        private void SetGridColor(Color startColor, Action<Color> setColorToProperties)
        {
            clrDialog.Color = startColor;
            if (clrDialog.ShowDialog() != DialogResult.OK)
                return;

            setColorToProperties(clrDialog.Color);
        }

        private void DrawGridColor(ToolStripDropDownButton tsb, Color color)
        {
            var bmp = new Bitmap(16, 16, PixelFormat.Format24bppRgb);
            using var gfx = Graphics.FromImage(bmp);

            gfx.FillRectangle(new SolidBrush(color), 0, 0, 16, 16);

            tsb.Image = bmp;
        }
    }
}
