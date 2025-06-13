using System.Drawing;
using System.Windows.Forms;

namespace PhotoManager
{
    public partial class PhotoShowForm : Form
    {
        private PictureBox pictureBox;
        private Image originalImage;
        private int currentWidth, currentHeight;

        public PhotoShowForm(string imagePath)
        {
            Text = "Просмотр фотографии";
            Size = new Size(800, 600);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;

            pictureBox = new PictureBox
            {
                Location = new Point(10, 10),
                Size = new Size(760, 500),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BorderStyle = BorderStyle.Fixed3D
            };

            var zoomInButton = new Button
            {
                Location = new Point(10, 520),
                Size = new Size(100, 30),
                Text = "Приблизить"
            };
            zoomInButton.Click += (s, e) =>
            {
                currentWidth += 50;
                currentHeight += 50;
                pictureBox.Image?.Dispose();
                pictureBox.Image = new Bitmap(originalImage, currentWidth, currentHeight);
            };

            var zoomOutButton = new Button
            {
                Location = new Point(120, 520),
                Size = new Size(100, 30),
                Text = "Отдалить"
            };
            zoomOutButton.Click += (s, e) =>
            {
                if (currentWidth > 50 && currentHeight > 50)
                {
                    currentWidth -= 50;
                    currentHeight -= 50;
                    pictureBox.Image?.Dispose();
                    pictureBox.Image = new Bitmap(originalImage, currentWidth, currentHeight);
                }
            };

            var closeButton = new Button
            {
                Location = new Point(670, 520),
                Size = new Size(100, 30),
                Text = "Закрыть"
            };
            closeButton.Click += (s, e) => Close();

            Controls.Add(pictureBox);
            Controls.Add(zoomInButton);
            Controls.Add(zoomOutButton);
            Controls.Add(closeButton);

            originalImage = Image.FromFile(imagePath);
            currentWidth = originalImage.Width;
            currentHeight = originalImage.Height;
            pictureBox.Image = new Bitmap(originalImage, currentWidth, currentHeight);
        }
    }
}