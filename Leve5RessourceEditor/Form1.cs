using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Kontract.Extensions;
using Kontract.Models.IO;
using Kore.Managers.Plugins;
using Leve5RessourceEditor.Level5;

namespace Leve5RessourceEditor
{
    public partial class Form1 : Form
    {
        private UPath _openedFile;

        private readonly AnmcRessource _ressourceManager;
        private readonly IDictionary<AnmcImageRessource, bool> _selectedRessourceParts;

        private readonly IDictionary<string, Size> _imageSizeDictionary = new Dictionary<string, Size>
        {
            ["Top Screen"] = new Size(400, 240),
            ["Bottom Screen"] = new Size(320, 240)
        };

        public Form1()
        {
            InitializeComponent();

            var assemblyLoader = new Kore.Managers.Plugins.PluginLoader.AssemblyFilePluginLoader(Assembly.GetAssembly(typeof(plugin_level5.Archives.Arc0Plugin)));
            var pluginManager = new PluginManager(assemblyLoader);
            _ressourceManager = new AnmcRessource(pluginManager);
            _selectedRessourceParts = new Dictionary<AnmcImageRessource, bool>();

            DrawGridColor(tsbGridColor1, pb.GridColor);
            DrawGridColor(tsbGridColor2, pb.GridColorAlternate);
        }

        #region Open File

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ofd = new OpenFileDialog();
            if (ofd.ShowDialog() != DialogResult.OK)
                return;

            if (string.IsNullOrEmpty(ofd.FileName) || !File.Exists(ofd.FileName))
                return;

            OpenFile(ofd.FileName);
        }

        private async void OpenFile(string file)
        {
            IList<AnmcNamedImageRessource> namedImageRessources;
            try
            {
                namedImageRessources = await _ressourceManager.Load(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK);
                return;
            }

            saveToolStripMenuItem.Enabled = true;
            _openedFile = file;

            clbImageRessources.ItemCheck -= clbImageRessources_ItemCheck;
            FillCheckList(namedImageRessources);
            FillRessourceParts(Array.Empty<AnmcImageRessource>());
            clbImageRessources.ItemCheck += clbImageRessources_ItemCheck;

            UpdateImage();
        }

        #endregion

        #region Fill Methods

        private void FillCheckList(IList<AnmcNamedImageRessource> namedImageRessources)
        {
            clbImageRessources.Items.Clear();

            foreach (var namedImageRessource in namedImageRessources)
            {
                clbImageRessources.Items.Add(namedImageRessource, true);

                foreach (var imageRessource in namedImageRessource.ImageRessources)
                    _selectedRessourceParts[imageRessource] = true;
            }
        }

        private void FillRessourceParts(IList<AnmcImageRessource> imageRessources)
        {
            clbRessourceParts.SelectedIndexChanged -= clbRessourceParts_SelectedIndexChanged;
            clbRessourceParts.ItemCheck -= clbRessourceParts_ItemCheck;

            clbRessourceParts.Items.Clear();

            foreach (var imageRessource in imageRessources)
                clbRessourceParts.Items.Add(imageRessource, _selectedRessourceParts[imageRessource]);

            clbRessourceParts.ItemCheck += clbRessourceParts_ItemCheck;
            clbRessourceParts.SelectedIndexChanged += clbRessourceParts_SelectedIndexChanged;
        }

        #endregion

        #region Events

        private void clbImageRessources_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkedItems = GetCheckedItems<AnmcNamedImageRessource>(clbImageRessources);

            if (e.NewValue == CheckState.Checked)
                checkedItems.Add((AnmcNamedImageRessource)clbImageRessources.Items[e.Index]);

            if (e.NewValue == CheckState.Unchecked)
                checkedItems.Remove((AnmcNamedImageRessource)clbImageRessources.Items[e.Index]);

            var checkedRessources = checkedItems
                .SelectMany(x => x.ImageRessources)
                .Where(x => _selectedRessourceParts[x])
                .ToArray();
            UpdateImage(checkedRessources);
        }

        private void clbRessourceParts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkedItems = GetCheckedItems<AnmcNamedImageRessource>(clbImageRessources);
            var checkedRessources = checkedItems
                .SelectMany(x => x.ImageRessources)
                .Where(x => _selectedRessourceParts[x])
                .ToList();

            if (e.NewValue == CheckState.Checked)
            {
                var ressource = (AnmcImageRessource)clbRessourceParts.Items[e.Index];
                checkedRessources.Add(ressource);
                _selectedRessourceParts[ressource] = true;
            }

            if (e.NewValue == CheckState.Unchecked)
            {
                var ressource = (AnmcImageRessource)clbRessourceParts.Items[e.Index];
                checkedRessources.Remove(ressource);
                _selectedRessourceParts[ressource] = false;
            }

            UpdateImage(checkedRessources);
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
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtWidth, value => imageRessource.SetSize(new SizeF(value, imageRessource.Size.Height)));

            UpdateImage();
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtHeight,
                value => imageRessource.SetSize(new SizeF(imageRessource.Size.Width, value)));

            UpdateImage();
        }

        private void txtLocationX_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtLocationX, value => imageRessource.SetLocation(new PointF(value, imageRessource.Location.Y)));

            UpdateImage();
        }

        private void txtLocationY_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtLocationY, value => imageRessource.SetLocation(new PointF(imageRessource.Location.X, value)));

            UpdateImage();
        }

        private void txtUvWidth_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtUvWidth, value => imageRessource.SetUvSize(new SizeF(value, imageRessource.UvSize.Height)));

            UpdateImage();
        }

        private void txtUvHeight_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtUvHeight, value => imageRessource.SetUvSize(new SizeF(imageRessource.UvSize.Width, value)));

            UpdateImage();
        }

        private void txtUvLocationX_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtUvLocationX, value => imageRessource.SetUvLocation(new PointF(value, imageRessource.UvLocation.Y)));

            UpdateImage();
        }

        private void txtUvLocationY_TextChanged(object sender, EventArgs e)
        {
            var imageRessource = (AnmcImageRessource)((Control)sender).Tag;
            UpdateImageInformation(txtUvLocationY, value => imageRessource.SetUvLocation(new PointF(imageRessource.UvLocation.X, value)));

            UpdateImage();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sfd = new SaveFileDialog
            {
                InitialDirectory = _openedFile.GetDirectory().FullName,
                FileName = _openedFile.GetName()
            };

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

        private void clbImageRessources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbImageRessources.SelectedIndex < 0)
                return;

            ClearImageInformation();

            var namedImagedRessource = GetSelectedNamedImageRessource();
            FillRessourceParts(namedImagedRessource.ImageRessources);
        }

        private void clbRessourceParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbRessourceParts.SelectedIndex < 0)
                return;

            var imagedRessource = GetSelectedImageRessource();
            FillImageInformation(imagedRessource);
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            var files = (string[])e.Data.GetData(DataFormats.FileDrop, false);
            OpenFile(files.First());
        }

        #endregion

        #region Update Functions

        private void UpdateImage()
        {
            var checkedItems = GetCheckedItems<AnmcNamedImageRessource>(clbImageRessources);
            var checkedRessources = checkedItems
                .SelectMany(x => x.ImageRessources)
                .Where(x => _selectedRessourceParts[x])
                .ToArray();
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
            clbImageRessources.ItemCheck -= clbImageRessources_ItemCheck;

            for (var i = 0; i < clbImageRessources.Items.Count; i++)
                clbImageRessources.SetItemCheckState(i, CheckState.Checked);

            clbImageRessources.ItemCheck += clbImageRessources_ItemCheck;
        }

        private void UncheckAll()
        {
            clbImageRessources.ItemCheck -= clbImageRessources_ItemCheck;

            for (var i = 0; i < clbImageRessources.Items.Count; i++)
                clbImageRessources.SetItemCheckState(i, CheckState.Unchecked);

            clbImageRessources.ItemCheck += clbImageRessources_ItemCheck;
        }

        #endregion

        private AnmcNamedImageRessource GetSelectedNamedImageRessource()
        {
            return (AnmcNamedImageRessource)clbImageRessources.SelectedItem;
        }

        private AnmcImageRessource GetSelectedImageRessource()
        {
            return (AnmcImageRessource)clbRessourceParts.SelectedItem;
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

            txtWidth.Tag = anmcImageRessource;
            txtHeight.Tag = anmcImageRessource;
            txtUvWidth.Tag = anmcImageRessource;
            txtUvHeight.Tag = anmcImageRessource;
            txtLocationX.Tag = anmcImageRessource;
            txtLocationY.Tag = anmcImageRessource;
            txtUvLocationX.Tag = anmcImageRessource;
            txtUvLocationY.Tag = anmcImageRessource;

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

        private void ClearImageInformation()
        {
            txtWidth.TextChanged -= txtWidth_TextChanged;
            txtHeight.TextChanged -= txtHeight_TextChanged;
            txtUvWidth.TextChanged -= txtUvWidth_TextChanged;
            txtUvHeight.TextChanged -= txtUvHeight_TextChanged;
            txtLocationX.TextChanged -= txtLocationX_TextChanged;
            txtLocationY.TextChanged -= txtLocationY_TextChanged;
            txtUvLocationX.TextChanged -= txtUvLocationX_TextChanged;
            txtUvLocationY.TextChanged -= txtUvLocationY_TextChanged;

            txtWidth.Enabled = false;
            txtHeight.Enabled = false;
            txtUvWidth.Enabled = false;
            txtUvHeight.Enabled = false;
            txtLocationX.Enabled = false;
            txtLocationY.Enabled = false;
            txtUvLocationX.Enabled = false;
            txtUvLocationY.Enabled = false;

            txtWidth.Text = string.Empty;
            txtHeight.Text = string.Empty;
            txtUvWidth.Text = string.Empty;
            txtUvHeight.Text = string.Empty;
            txtLocationX.Text = string.Empty;
            txtLocationY.Text = string.Empty;
            txtUvLocationX.Text = string.Empty;
            txtUvLocationY.Text = string.Empty;

            txtWidth.TextChanged += txtWidth_TextChanged;
            txtHeight.TextChanged += txtHeight_TextChanged;
            txtUvWidth.TextChanged += txtUvWidth_TextChanged;
            txtUvHeight.TextChanged += txtUvHeight_TextChanged;
            txtLocationX.TextChanged += txtLocationX_TextChanged;
            txtLocationY.TextChanged += txtLocationY_TextChanged;
            txtUvLocationX.TextChanged += txtUvLocationX_TextChanged;
            txtUvLocationY.TextChanged += txtUvLocationY_TextChanged;
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
