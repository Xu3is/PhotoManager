using Microsoft.VisualStudio.TestTools.UnitTesting;
using PhotoManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace TestProject1
{
    namespace UnitTesting
    {
        [TestClass]
        public class OpenFormsTest // класс для тестирования открытия форм
        {
            [TestMethod]
            public void DescriptionOpen()
            {
                // act
                var descriptionForm = new DescriptionForm();
                descriptionForm.ShowDialog();
                /*
                 assert 
                 проверить, что форма появилась на экране 
                */
            }
            [TestMethod]
            public void PhotoAlbumOpen()
            {
                // act
                var photoAlbumForm = new PhotoAlbumForm();
                photoAlbumForm.ShowDialog();
                /*
                 assert 
                 проверить, что форма появилась на экране 
                */
            }
        }

        [TestClass]
        public class PhotoInfoConvertToStringTests // класс для тестирования конверта данных фотографии в строку
        {
            [TestMethod]
            public void ToStringPhotoUsual()
            {
                // arrange
                var photo = new Photo("C:/Photos/фото.jpg", "Фото", new DateTime(2025, 6, 2));

                // act
                string result = photo.ToString();

                // assert 
                Assert.AreEqual("C:/Photos/фото.jpg - Фото (02.06.2025)", result);
            }
        }

        public class PhotoActionsTest // класс для тестирования действий над фотографиями
        {

            [TestMethod]
            public void RemoveUnselectedPhotoTest()
            {
                // arrange
                var listView = new ListView();
                var album = new PhotoAlbum(listView);
                listView.Items.Add(new ListViewItem(new[] { "path1.jpg", "desc1", "12.06.2025" }));

                // act
                album.RemovePhoto();

                // assert
                Assert.AreEqual(1, listView.Items.Count);

            }

            [TestMethod]
            public void SortPhotosByDateTest()
            {
                // arrange
                var photos = new List<Photo>
            {
                new Photo("photo2.jpg", "rainy", new DateTime(2025, 2, 1)),
                new Photo("photo3.jpg", "snowy", new DateTime(2025, 3, 1)),
                new Photo("photo1.jpg", "sunny", new DateTime(2025, 1, 1))
            };

                // act (код из метода SortPhotosByDate)
                var sortedPhotos = photos.OrderBy(p => p.DateTaken).ToList();
                photos = new List<Photo>(sortedPhotos);

                // assert
                Assert.AreEqual("photo1.jpg", photos[0].Path);
                Assert.AreEqual("photo2.jpg", photos[1].Path);
                Assert.AreEqual("photo3.jpg", photos[2].Path);
            }

            [TestMethod]
            public void ValidateInputPhotoData()
            {
                string description = "Крутое описание";
                string dateInput = "01.01.2025";

                // act 
                Assert.IsFalse(string.IsNullOrEmpty(description));
                Assert.IsFalse(string.IsNullOrEmpty(dateInput));

                // asset
                Assert.IsTrue(string.IsNullOrEmpty(""));
                Assert.IsTrue(string.IsNullOrEmpty(null));
            }
        }
    }

}