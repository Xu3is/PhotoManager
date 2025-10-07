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
        private ConditionFactory _cf;
        
        private string orkFileName = "орк.jpg";
        private string orkDescription = "орк";
        private string orkDate = "13.06.2025";

        private string gnomeFileName = "гном.jpg";
        private string gnomeDescription = "гном";
        private string gnomeDate = "12.06.2025";


        [TestInitialize]
        public void TestInitialize()
        {
            _app = Application.Launch(@"C:\учеба\долги\PhotoManager\PhotoManager\PhotoManager\bin\Debug\PhotoManager.exe");
          //_app = Application.Launch();
            _automation = new UIA3Automation();
            _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
            _cf = new ConditionFactory(new UIA3PropertyLibrary());
        }

        [TestCleanup]
        public void TestCleanup()
        {
            _automation.Dispose();
            _app?.Close();
        }

          public void AddPhotoWithDetails(Window mainWindow, string fileName, string description, string date)
        {
            var addPhotoButton = mainWindow.FindFirstDescendant(_cf.ByName("Добавить фото")).AsButton();
            addPhotoButton.Click();

            Thread.Sleep(500);
            var openFileDialog = mainWindow.ModalWindows.FirstOrDefault();

            var fileItem = openFileDialog.FindFirstDescendant(_cf.ByName(fileName));
            fileItem.Click();
            Keyboard.Type(VirtualKeyShort.RETURN);

            Thread.Sleep(500);
            var descriptionForm = mainWindow.ModalWindows.FirstOrDefault();


            var descriptionTextBox = descriptionForm.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            descriptionTextBox.Text = description;

            var dateTextBox = descriptionForm.FindAllDescendants(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit))[1].AsTextBox();
            dateTextBox.Text = date;

            var okButton = descriptionForm.FindFirstDescendant(_cf.ByName("ОК")).AsButton();
            okButton.Click();
        }

  
        [TestMethod]
        public void TC0001_AddPhotoSuccessMessage()
        {
            AddPhotoWithDetails(_mainWindow, gnomeFileName, gnomeDescription, gnomeDate);
            
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            
            StringAssert.Contains(messageText.Name, "Фото добавлено.");
        }

        [TestMethod]
        public void TC0002_AddPhotoWithCorrectValues()
        {
            AddPhotoWithDetails(_mainWindow, orkFileName, orkDescription, orkDate);

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            
            StringAssert.Contains(messageText.Name, "Фото добавлено.");

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i =>
                    i.FindAllChildren().Any(c => c.Name.Contains(orkDescription)) &&
                    i.FindAllChildren().Any(c => c.Name.Contains(orkDate)));
            Assert.IsNotNull(photoItem, $"Фотография с {orkDescription}, дата: {orkDate}) не найдена в ListView");
        }

        [TestMethod]
        public void TC0003_DeletePhotoMessage()
        {
            AddPhotoWithDetails(_mainWindow, orkFileName, orkDescription, orkDate);

            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("орк")));
            photoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();

            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.");
        }

        [TestMethod]
        public void TC0004_DeletePhoto()
        {

            AddPhotoWithDetails(_mainWindow, orkFileName, orkDescription, orkDate);

            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("орк")));
            photoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();

            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.");

            listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);
        }

        [TestMethod]
        public void TC0005_DeletePhotoWithoutSelection()
        {
            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);

            var removePhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Сначала выберите фото для удаления.");

        }

        [TestMethod]
        public void TC0006_SortPhotosByDate()
        {
            AddPhotoWithDetails(_mainWindow, orkFileName, orkDescription, orkDate);

  
            var firstAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var firstAddMessageText = firstAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(firstAddMessageText.Name, "Фото добавлено.");

            var firstAddMessageOkButton = firstAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            firstAddMessageOkButton.Click();

            AddPhotoWithDetails(_mainWindow, gnomeFileName, gnomeDescription, gnomeDate);

            var secondAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var secondAddMessageText = secondAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(secondAddMessageText.Name, "Фото добавлено.");

            var secondAddMessageOkButton = secondAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            secondAddMessageOkButton.Click();

            var sortByDateButton = _mainWindow.FindFirstDescendant(_cf.ByName("Отсортировать по дате")).AsButton();
            sortByDateButton.Click();


            var sortMessageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var sortMessageText = sortMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(sortMessageText.Name, "Фото отсортированы по дате.");
            
            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var firstItem = listView.Items[0];
            Assert.IsTrue(firstItem.FindAllChildren().Any(c => c.Name.Contains(gnomeDate)));
        }

        [TestMethod]
        public void TC0007_AddPhotoWithEmptyDescription()
        {

            AddPhotoWithDetails(_mainWindow, gnomeFileName, "", gnomeDate);

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.");

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length); 
        }

        [TestMethod]
        public void TC0008_AddPhotoWithEmptyDate()
        {
            AddPhotoWithDetails(_mainWindow, orkFileName, orkDescription, "");

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.");

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);
        }

        [TestMethod]
        public void TC0009_AddPhotoWithEmptyFields()
        {
            AddPhotoWithDetails(_mainWindow, gnomeFileName, "", "");

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.");

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);
        }

        [TestMethod]
        public void TC0010_SortEmptyListView()
        {
            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length);

            var sortByDateButton = _mainWindow.FindFirstDescendant(_cf.ByName("Отсортировать по дате")).AsButton();
            sortByDateButton.Click();

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Фото отсортированы по дате.");
            Assert.AreEqual(0, listView.Items.Length);
        }

        [TestMethod]
        public void TC0011_RemovePhotoWithSamePath()
        {

            AddPhotoWithDetails(_mainWindow, gnomeFileName, "гном1", "01.06.2025");

            var firstAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var firstAddMessageText = firstAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(firstAddMessageText.Name, "Фото добавлено.");

            var firstAddMessageOkButton = firstAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            firstAddMessageOkButton.Click();

            AddPhotoWithDetails(_mainWindow, gnomeFileName, "гном2", "02.06.2025");

            var secondAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var secondAddMessageText = secondAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(secondAddMessageText.Name, "Фото добавлено.");

            var secondAddMessageOkButton = secondAddMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            secondAddMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();

            var firstPhotoItem = listView.Items[0];
            firstPhotoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Удалить фото")).AsButton();
            removePhotoButton.Click();

            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.");
            
            
            listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            
            var remainingItem = listView.Items[0];
            Assert.IsTrue(remainingItem.FindAllChildren().Any(c => c.Name.Contains("гном2")));
            Assert.IsTrue(remainingItem.FindAllChildren().Any(c => c.Name.Contains("02.06.2025")));
        }

        [TestMethod]
        public void TC0012_OpenPhotoViewForm()
        {
            AddPhotoWithDetails(_mainWindow, gnomeFileName, "фото", "14.06.2025");

            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();
            var addMessageText = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();
        }

        [TestMethod]
        public void TC0013_ZoomInPhoto()
        {
            AddPhotoWithDetails(_mainWindow, gnomeFileName, "фото", "14.06.2025");

            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();

            var zoomInButton = viewForm.FindFirstDescendant(_cf.ByName("Приблизить")).AsButton();

            for (int i = 0; i < 5; i++)
            {
                zoomInButton.Click();
                Thread.Sleep(250);
            }
        }

        [TestMethod]
        public void TC0014_ZoomOutPhoto()
        {

            AddPhotoWithDetails(_mainWindow, orkFileName, "фото", "14.06.2025");

            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();

            var zoomOutButton = viewForm.FindFirstDescendant(_cf.ByName("Отдалить")).AsButton();

            for (int i = 0; i < 5; i++)
            {
                zoomOutButton.Click();
                Thread.Sleep(250);
            }

        }

        [TestMethod]
        public void TC0015_ClosePhotoViewForm()
        {

            AddPhotoWithDetails(_mainWindow, orkFileName, "фото", "14.06.2025");
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var addMessageText = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(_cf.ByAutomationId("2")).AsButton();
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(_cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            var viewForm = _mainWindow.ModalWindows.FirstOrDefault();

            var closeButton = viewForm.FindFirstDescendant(_cf.ByName("Закрыть")).AsButton();
            closeButton.Click();
            Thread.Sleep(500);
        }

        [TestMethod]
        public void TC0016_OpenPhotoViewWithoutSelection()
        {
            var viewPhotoButton = _mainWindow.FindFirstDescendant(_cf.ByName("Посмотреть фото")).AsButton();
            viewPhotoButton.Click();

            var messageBox = _mainWindow.ModalWindows.FirstOrDefault();

            var messageText = messageBox.FindFirstDescendant(_cf.ByAutomationId("65535"));
            StringAssert.Contains(messageText.Name, "Сначала выберите фото для просмотра.");
        }
    }
}