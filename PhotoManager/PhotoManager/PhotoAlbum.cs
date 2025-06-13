using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;

namespace PhotoManager
{
    partial class PhotoAlbum
    {
        private List<Photo> photos = new List<Photo>();
        private ListView listView;

        public PhotoAlbum(ListView listView)
        {
            this.listView = listView;
            LoadPhotos();
        }

        private void LoadPhotos()
        {
            listView.Items.Clear();
            foreach (var photo in photos)
            {
                listView.Items.Add(new ListViewItem(new[] { photo.Path, photo.Description,
                    photo.DateTaken.ToString("dd.MM.yyyy") }));
            }
        }

        public void AddPhoto()
        {
            using (var openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory =
                    Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                openFileDialog.Title = "Выберите фото";
                openFileDialog.Filter = "Изображения (*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    var photoPath = openFileDialog.FileName;
                    using (var descriptionForm = new DescriptionForm())
                    {
                        if (descriptionForm.ShowDialog() != DialogResult.OK)
                        {
                            return;
                        }

                        var description = descriptionForm.Description;
                        var dateInput = descriptionForm.DateInput;

                        if (string.IsNullOrEmpty(description) || string.IsNullOrEmpty(dateInput))
                        {
                            MessageBox.Show("Описание и дата не могут быть пустыми.");
                            return;
                        }

                        try
                        {
                            var dateTaken = DateTime.ParseExact(dateInput, "dd.MM.yyyy",
                                System.Globalization.CultureInfo.InvariantCulture);
                            photos.Add(new Photo(photoPath, description, dateTaken));
                            LoadPhotos();
                            MessageBox.Show("Фото добавлено.");
                        }
                        catch (FormatException)
                        {
                            MessageBox.Show("Неверный формат даты. Используйте формат дд.ММ.гггг.");
                        }
                    }
                }
            }
        }

        public void RemovePhoto()
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Сначала выберите фото для удаления.");
                return;
            }

            int selectedIndex = listView.SelectedIndices[0];
            if (selectedIndex >= 0 && selectedIndex < photos.Count)
            {
                photos.RemoveAt(selectedIndex);
                LoadPhotos();
                MessageBox.Show("Фото удалено.");
            }
        }

        public void SortPhotosByDate()
        {
            var sortedPhotos = photos.OrderBy(p => p.DateTaken).ToList();
            photos = new List<Photo>(sortedPhotos);
            LoadPhotos();
            MessageBox.Show("Фото отсортированы по дате.");
        }

        public void ShowPhoto()
        {
            if (listView.SelectedItems.Count == 0)
            {
                MessageBox.Show("Сначала выберите фото для просмотра.");
                return;
            }
            var photoPath = listView.SelectedItems[0].SubItems[0].Text;
            using (var photoShowForm = new PhotoShowForm(photoPath))
            {
                photoShowForm.ShowDialog();
            }
        }

        private string GetDescription()
        {
            using (var descriptionForm = new DescriptionForm())
            {
                descriptionForm.ShowDialog();
                return descriptionForm.Description;
            }
        }
    }
}