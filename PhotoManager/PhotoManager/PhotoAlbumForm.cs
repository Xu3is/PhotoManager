using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PhotoManager
{
    public partial class PhotoAlbumForm : Form
    {
        private PhotoAlbum photoAlbum;
        private ListView listView;
        private Button addPhotoButton;
        private Button removePhotoButton;
        private Button sortByDateButton;
        public PhotoAlbumForm()
        {
            this.Text = "Управление фотографиями";
            this.Width = 610;
            this.Height = 450;
            CreateControls();
            photoAlbum = new PhotoAlbum(listView);
        }
        private void CreateControls()
        {
            listView = new ListView
            {
                Location = new System.Drawing.Point(10, 10),
                Size = new System.Drawing.Size(580, 350),
                View = View.Details,
                FullRowSelect = true
            };
            listView.Columns.Add("Путь", 300);
            listView.Columns.Add("Описание", 200);
            listView.Columns.Add("Дата съёмки", 100);
            addPhotoButton = new Button
            {
                Location = new System.Drawing.Point(10, 370),
                Text = "Добавить фото",
                Size = new System.Drawing.Size(100, 25)
            };
            addPhotoButton.Click += (sender, e) => photoAlbum.AddPhoto();
            removePhotoButton = new Button
            {
                Location = new System.Drawing.Point(120, 370),
                Text = "Удалить фото",
                Size = new System.Drawing.Size(100, 25)
            };
            removePhotoButton.Click += (sender, e) => photoAlbum.RemovePhoto();
            sortByDateButton = new Button
            {
                Location = new System.Drawing.Point(230, 370),
                Text = "Отсортировать по дате",
                Size = new System.Drawing.Size(120, 25)
            };
            sortByDateButton.Click += (sender, e) => photoAlbum.SortPhotosByDate();
            this.Controls.Add(listView);
            this.Controls.Add(addPhotoButton);
            this.Controls.Add(removePhotoButton);
            this.Controls.Add(sortByDateButton);
        }
    }
}