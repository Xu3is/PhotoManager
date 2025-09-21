using FlaUI.Core;
using FlaUI.Core.AutomationElements;
using FlaUI.Core.Conditions;
using FlaUI.Core.Input;
using FlaUI.Core.WindowsAPI;
using FlaUI.UIA3;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Flauitests
{
    [TestClass]
    public class PhotoManagerTests
    {
        private Application _app;
        private UIA3Automation _automation;
        private Window _mainWindow;

        [TestInitialize]
        public void TestInitialize()
        {
            _app = Application.Launch(@"C:\учеба\долги\PhotoManager\PhotoManager\PhotoManager\bin\Debug\PhotoManager.exe");
          //_app = Application.Launch();
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _automation.Dispose();
            _app?.Close();
        }

        private void AddPhotoWithDetails(Window mainWindow, string fileName, string description, string date)
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var addPhotoButton = mainWindow.FindFirstDescendant(cf.ByName("Добавить фото")).AsButton();
            addPhotoButton.Click();

            Thread.Sleep(500);
            var openFileDialog = mainWindow.ModalWindows.FirstOrDefault();

            var fileItem = openFileDialog.FindFirstDescendant(cf.ByName(fileName));
            fileItem.Click();
            Keyboard.Type(VirtualKeyShort.RETURN);

            Thread.Sleep(500);
            var descriptionForm = mainWindow.ModalWindows.FirstOrDefault();


            var descriptionTextBox = descriptionForm.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            descriptionTextBox.Text = description ?? "";

            var dateTextBox = descriptionForm.FindAllDescendants(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit))[1].AsTextBox();
            dateTextBox.Text = date ?? "";

            var okButton = descriptionForm.FindFirstDescendant(cf.ByName("ОК")).AsButton();
            okButton.Click();
        }

        [TestMethod]
        public void TC0001_AddPhotoSuccessMessage()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном", "12.06.2025");

            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Фото добавлено.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            messageOkButton.Click();
        }

        [TestMethod]
        public void TC0002_AddPhotoWithCorrectValues()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "13.06.2025");

            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Фото добавлено.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            messageOkButton.Click();
        }

        [TestMethod]
        public void TC0003_DeletePhotoMessage()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном", "12.06.2025");

            Thread.Sleep(500);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("гном")));
            photoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();

            Thread.Sleep(500);
            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.");

            var deleteMessageOkButton = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            deleteMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0004_DeletePhoto()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "13.06.2025");

            Thread.Sleep(500);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("орк")));
            photoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();

            Thread.Sleep(500);
            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.");

            var deleteMessageOkButton = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            deleteMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0005_DeletePhotoWithoutSelection()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();

            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Сначала выберите фото для удаления.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            messageOkButton.Click();
        }

        [TestMethod]
        public void TC0006_SortPhotosByDate()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "13.05.2025");

            Thread.Sleep(500);
            var firstAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var firstAddMessageText = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(firstAddMessageText.Name, "Фото добавлено.");

            var firstAddMessageOkButton = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            firstAddMessageOkButton.Click();

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном", "12.05.2025");

            Thread.Sleep(500);
            var secondAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var secondAddMessageText = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(secondAddMessageText.Name, "Фото добавлено.");

            var secondAddMessageOkButton = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            secondAddMessageOkButton.Click();

            var sortByDateButton = _mainWindow.FindFirstDescendant(cf.ByName("Отсортировать по дате")).AsButton();
            sortByDateButton.Click();

            Thread.Sleep(500);
            var sortMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var sortMessageText = sortMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(sortMessageText.Name, "Фото отсортированы по дате.");

            var sortMessageOkButton = sortMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            sortMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0007_AddPhotoWithEmptyDescription()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "", "13.06.2025");

            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();

            Thread.Sleep(500);
            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("гном")));
        }

        [TestMethod]
        public void TC0008_AddPhotoWithEmptyDate()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "");

            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            messageOkButton.Click();

            Thread.Sleep(500);
            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("орк")));
        }

        [TestMethod]
        public void TC0009_AddPhotoWithEmptyFields()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "", "");
            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            messageOkButton.Click();

            Thread.Sleep(500);
            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фотография1.jpg")));
        }

        [TestMethod]
        public void TC0010_SortEmptyListView()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);

            var sortByDateButton = _mainWindow.FindFirstDescendant(cf.ByName("Отсортировать по дате")).AsButton();
            sortByDateButton.Click();

            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Фото отсортированы по дате.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            messageOkButton.Click();

            Assert.AreEqual(0, listView.Items.Length);
        }

        [TestMethod]
        public void TC0011_RemovePhotoWithSamePath()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном1", "01.06.2025");

            Thread.Sleep(500);
            var firstAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var firstAddMessageText = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(firstAddMessageText.Name, "Фото добавлено.");

            var firstAddMessageOkButton = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            firstAddMessageOkButton.Click();

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном2", "02.06.2025");

            Thread.Sleep(500);
            var secondAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var secondAddMessageText = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(secondAddMessageText.Name, "Фото добавлено.");

            var secondAddMessageOkButton = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            secondAddMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();

            var firstPhotoItem = listView.Items[0];
            firstPhotoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();

            Thread.Sleep(500);
            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.");

            var deleteMessageOkButton = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            deleteMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0012_OpenPhotoViewForm()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "фото", "14.06.2025");

            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            Thread.Sleep(500);
            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();
            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();

            Thread.Sleep(500);
            closeButton.Click();
        }

        [TestMethod]
        public void TC0013_ZoomInPhoto()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "фото", "14.06.2025");

            Thread.Sleep(500);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            Thread.Sleep(500);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();

            var zoomInButton = viewForm.FindFirstDescendant(cf.ByName("Приблизить")).AsButton();

            for (int i = 0; i < 3; i++)
            {
                zoomInButton.Click();
                Thread.Sleep(300);
            }

            var viewFormAfterZoom = _mainWindow.ModalWindows.FirstOrDefault();

            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();
            closeButton.Click();
        }

        [TestMethod]
        public void TC0014_ZoomOutPhoto()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "фото", "14.06.2025");

            Thread.Sleep(500);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            Assert.IsNotNull(photoItem, "Фотография не найдена в ListView для просмотра");

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            Thread.Sleep(500);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();

            var zoomOutButton = viewForm.FindFirstDescendant(cf.ByName("Отдалить")).AsButton();

            for (int i = 0; i < 3; i++)
            {
                zoomOutButton.Click();
                Thread.Sleep(300);
            }

            var viewFormAfterZoom = _mainWindow.ModalWindows.FirstOrDefault();

            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();
            closeButton.Click();
        }

        [TestMethod]
        public void TC0015_ClosePhotoViewForm()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "фото", "14.06.2025");

            Thread.Sleep(500);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            Thread.Sleep(500);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();

            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();
            closeButton.Click();

            Thread.Sleep(500);
            var viewFormAfterClose = _mainWindow.ModalWindows.FirstOrDefault();
        }

        [TestMethod]
        public void TC0016_OpenPhotoViewWithoutSelection()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length, "ListView должен быть пустым перед началом теста");

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            Thread.Sleep(500);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Сначала выберите фото для просмотра.");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            messageOkButton.Click();

            Thread.Sleep(500);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();
        }
    }
}