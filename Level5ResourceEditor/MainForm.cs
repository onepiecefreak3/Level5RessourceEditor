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
using Level5ResourceEditor.Level5;

namespace Level5ResourceEditor
{
    public partial class MainForm : Form
    {
        private UPath _openedFile;

        private readonly AnmcResource _resourceManager;
        private readonly IDictionary<AnmcImageResource, bool> _selectedResourceParts;

        private readonly IDictionary<string, Size> _imageSizeDictionary = new Dictionary<string, Size>
        {
            ["Top Screen"] = new Size(400, 240),
            ["Bottom Screen"] = new Size(320, 240)
        };

        public MainForm()
        {
            InitializeComponent();

            var assemblyLoader = new Kore.Managers.Plugins.PluginLoader.AssemblyFilePluginLoader(Assembly.GetAssembly(typeof(plugin_level5.Archives.Arc0Plugin)));
            var pluginManager = new PluginManager(assemblyLoader);
            _resourceManager = new AnmcResource(pluginManager);
            _selectedResourceParts = new Dictionary<AnmcImageResource, bool>();

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
            IList<AnmcNamedImageResource> namedImageResources;
            try
            {
                namedImageResources = await _resourceManager.Load(file);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Load Error", MessageBoxButtons.OK);
                return;
            }

            saveToolStripMenuItem.Enabled = true;
            _openedFile = file;

            clbImageResources.ItemCheck -= clbImageResources_ItemCheck;
            FillCheckList(namedImageResources);
            FillResourceParts(Array.Empty<AnmcImageResource>());
            clbImageResources.ItemCheck += clbImageResources_ItemCheck;

            UpdateImage();
        }

        #endregion

        #region Save File

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileAs();
        }

        private void SaveFileAs()
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

            SaveFile(sfd.FileName);
        }

        private void SaveFile()
        {
            SaveFile(_openedFile);
        }

        private void SaveFile(UPath savePath)
        {
            _resourceManager.Save(savePath);
        }

        #endregion

        #region Fill Methods

        private void FillCheckList(IList<AnmcNamedImageResource> namedImageResources)
        {
            clbImageResources.Items.Clear();

            foreach (var namedImageResource in namedImageResources)
            {
                clbImageResources.Items.Add(namedImageResource, true);

                foreach (var imageResource in namedImageResource.ImageResources)
                    _selectedResourceParts[imageResource] = true;
            }
        }

        private void FillResourceParts(IList<AnmcImageResource> imageResources)
        {
            clbResourceParts.SelectedIndexChanged -= clbResourceParts_SelectedIndexChanged;
            clbResourceParts.ItemCheck -= clbResourceParts_ItemCheck;

            clbResourceParts.Items.Clear();

            foreach (var imageResource in imageResources)
                clbResourceParts.Items.Add(imageResource, _selectedResourceParts[imageResource]);

            clbResourceParts.ItemCheck += clbResourceParts_ItemCheck;
            clbResourceParts.SelectedIndexChanged += clbResourceParts_SelectedIndexChanged;
        }

        #endregion

        #region Events

        private void clbImageResources_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkedItems = GetCheckedItems<AnmcNamedImageResource>(clbImageResources);

            if (e.NewValue == CheckState.Checked)
                checkedItems.Add((AnmcNamedImageResource)clbImageResources.Items[e.Index]);

            if (e.NewValue == CheckState.Unchecked)
                checkedItems.Remove((AnmcNamedImageResource)clbImageResources.Items[e.Index]);

            var checkedResources = checkedItems
                .SelectMany(x => x.ImageResources)
                .Where(x => _selectedResourceParts[x])
                .ToArray();
            UpdateImage(checkedResources);
        }

        private void clbResourceParts_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            var checkedItems = GetCheckedItems<AnmcNamedImageResource>(clbImageResources);
            var checkedResources = checkedItems
                .SelectMany(x => x.ImageResources)
                .Where(x => _selectedResourceParts[x])
                .ToList();

            if (e.NewValue == CheckState.Checked)
            {
                var Resource = (AnmcImageResource)clbResourceParts.Items[e.Index];
                checkedResources.Add(Resource);
                _selectedResourceParts[Resource] = true;
            }

            if (e.NewValue == CheckState.Unchecked)
            {
                var Resource = (AnmcImageResource)clbResourceParts.Items[e.Index];
                checkedResources.Remove(Resource);
                _selectedResourceParts[Resource] = false;
            }

            UpdateImage(checkedResources);
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
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtWidth, value => imageResource.SetSize(new SizeF(value, imageResource.Size.Height)));

            UpdateImage();
        }

        private void txtHeight_TextChanged(object sender, EventArgs e)
        {
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtHeight,
                value => imageResource.SetSize(new SizeF(imageResource.Size.Width, value)));

            UpdateImage();
        }

        private void txtLocationX_TextChanged(object sender, EventArgs e)
        {
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtLocationX, value => imageResource.SetLocation(new PointF(value, imageResource.Location.Y)));

            UpdateImage();
        }

        private void txtLocationY_TextChanged(object sender, EventArgs e)
        {
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtLocationY, value => imageResource.SetLocation(new PointF(imageResource.Location.X, value)));

            UpdateImage();
        }

        private void txtUvWidth_TextChanged(object sender, EventArgs e)
        {
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtUvWidth, value => imageResource.SetUvSize(new SizeF(value, imageResource.UvSize.Height)));

            UpdateImage();
        }

        private void txtUvHeight_TextChanged(object sender, EventArgs e)
        {
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtUvHeight, value => imageResource.SetUvSize(new SizeF(imageResource.UvSize.Width, value)));

            UpdateImage();
        }

        private void txtUvLocationX_TextChanged(object sender, EventArgs e)
        {
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtUvLocationX, value => imageResource.SetUvLocation(new PointF(value, imageResource.UvLocation.Y)));

            UpdateImage();
        }

        private void txtUvLocationY_TextChanged(object sender, EventArgs e)
        {
            var imageResource = (AnmcImageResource)((Control)sender).Tag;
            UpdateImageInformation(txtUvLocationY, value => imageResource.SetUvLocation(new PointF(imageResource.UvLocation.X, value)));

            UpdateImage();
        }

        private void resolutionList_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateImage();
        }

        private void clbImageResources_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbImageResources.SelectedIndex < 0)
                return;

            ClearImageInformation();

            var namedImagedResource = GetSelectedNamedImageResource();
            FillResourceParts(namedImagedResource.ImageResources);
        }

        private void clbResourceParts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (clbResourceParts.SelectedIndex < 0)
                return;

            var imagedResource = GetSelectedImageResource();
            FillImageInformation(imagedResource);
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

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.Shift && e.KeyCode == Keys.S)
            {
                // Save As
                SaveFileAs();
            }

            if (e.Control && e.KeyCode == Keys.S)
            {
                // Save
                SaveFile();
            }
        }

        #endregion

        #region Update Functions

        private void UpdateImage()
        {
            var checkedItems = GetCheckedItems<AnmcNamedImageResource>(clbImageResources);
            var checkedResources = checkedItems
                .SelectMany(x => x.ImageResources)
                .Where(x => _selectedResourceParts[x])
                .ToArray();
            UpdateImage(checkedResources);
        }

        private void UpdateImage(IList<AnmcImageResource> imageResources)
        {
            var imageSize = _imageSizeDictionary[resolutionList.Text];
            var image = new Bitmap(imageSize.Width, imageSize.Height);

            foreach (var imageResource in imageResources)
                imageResource.DrawOnImage(image);

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
            clbImageResources.ItemCheck -= clbImageResources_ItemCheck;

            for (var i = 0; i < clbImageResources.Items.Count; i++)
                clbImageResources.SetItemCheckState(i, CheckState.Checked);

            clbImageResources.ItemCheck += clbImageResources_ItemCheck;
        }

        private void UncheckAll()
        {
            clbImageResources.ItemCheck -= clbImageResources_ItemCheck;

            for (var i = 0; i < clbImageResources.Items.Count; i++)
                clbImageResources.SetItemCheckState(i, CheckState.Unchecked);

            clbImageResources.ItemCheck += clbImageResources_ItemCheck;
        }

        #endregion

        private AnmcNamedImageResource GetSelectedNamedImageResource()
        {
            return (AnmcNamedImageResource)clbImageResources.SelectedItem;
        }

        private AnmcImageResource GetSelectedImageResource()
        {
            return (AnmcImageResource)clbResourceParts.SelectedItem;
        }

        private void UpdateImageInformation(Control textBox, Action<float> valueSetter)
        {
            if (string.IsNullOrEmpty(textBox.Text) || !float.TryParse(textBox.Text, out var value))
                return;

            valueSetter(value);
        }

        private void FillImageInformation(AnmcImageResource anmcImageResource)
        {
            txtWidth.TextChanged -= txtWidth_TextChanged;
            txtHeight.TextChanged -= txtHeight_TextChanged;
            txtUvWidth.TextChanged -= txtUvWidth_TextChanged;
            txtUvHeight.TextChanged -= txtUvHeight_TextChanged;
            txtLocationX.TextChanged -= txtLocationX_TextChanged;
            txtLocationY.TextChanged -= txtLocationY_TextChanged;
            txtUvLocationX.TextChanged -= txtUvLocationX_TextChanged;
            txtUvLocationY.TextChanged -= txtUvLocationY_TextChanged;

            txtWidth.Tag = anmcImageResource;
            txtHeight.Tag = anmcImageResource;
            txtUvWidth.Tag = anmcImageResource;
            txtUvHeight.Tag = anmcImageResource;
            txtLocationX.Tag = anmcImageResource;
            txtLocationY.Tag = anmcImageResource;
            txtUvLocationX.Tag = anmcImageResource;
            txtUvLocationY.Tag = anmcImageResource;

            txtWidth.Enabled = true;
            txtHeight.Enabled = true;
            txtUvWidth.Enabled = true;
            txtUvHeight.Enabled = true;
            txtLocationX.Enabled = true;
            txtLocationY.Enabled = true;
            txtUvLocationX.Enabled = true;
            txtUvLocationY.Enabled = true;

            txtWidth.Text = anmcImageResource.Size.Width.ToString(CultureInfo.InvariantCulture);
            txtHeight.Text = anmcImageResource.Size.Height.ToString(CultureInfo.InvariantCulture);
            txtUvWidth.Text = anmcImageResource.UvSize.Width.ToString(CultureInfo.InvariantCulture);
            txtUvHeight.Text = anmcImageResource.UvSize.Height.ToString(CultureInfo.InvariantCulture);
            txtLocationX.Text = anmcImageResource.Location.X.ToString(CultureInfo.InvariantCulture);
            txtLocationY.Text = anmcImageResource.Location.Y.ToString(CultureInfo.InvariantCulture);
            txtUvLocationX.Text = anmcImageResource.UvLocation.X.ToString(CultureInfo.InvariantCulture);
            txtUvLocationY.Text = anmcImageResource.UvLocation.Y.ToString(CultureInfo.InvariantCulture);

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
