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
            Console.WriteLine($"[{DateTime.Now}] Запуск приложения для теста...");
            try
            {
                _app = Application.Launch(@"C:\учеба\PhotoManager\PhotoManager\PhotoManager\bin\Debug\PhotoManager.exe");
                _automation = new UIA3Automation();
                _mainWindow = _app.GetMainWindow(_automation, TimeSpan.FromSeconds(5));
                Assert.IsNotNull(_mainWindow, "Главное окно приложения не найдено");
                Console.WriteLine($"[{DateTime.Now}] Приложение успешно запущено, PID: {_app.ProcessId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Ошибка при инициализации теста: {ex.Message}");
                throw;
            }
        }

        [TestCleanup]
        public void TestCleanup()
        {
            Console.WriteLine($"[{DateTime.Now}] Очистка после теста...");
            try
            {
                if (_mainWindow != null)
                {
                    _mainWindow.Close();
                    Console.WriteLine($"[{DateTime.Now}] Главное окно закрыто");
                }
                if (_app != null)
                {
                    _app.Close();
                    var process = Process.GetProcessById(_app.ProcessId);
                    if (!process.HasExited)
                    {
                        process.WaitForExit(4000);
                        Console.WriteLine($"[{DateTime.Now}] Приложение закрыто, PID: {_app.ProcessId}");
                    }
                    if (!process.HasExited)
                    {
                        process.Kill();
                        process.WaitForExit(1000);
                        Console.WriteLine($"[{DateTime.Now}] Приложение принудительно завершено, PID: {_app.ProcessId}");
                    }
                }
                _automation?.Dispose();
                Console.WriteLine($"[{DateTime.Now}] Ресурсы автоматизации освобождены");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now}] Ошибка при очистке: {ex.Message}");
            }
            finally
            {
                _app = null;
                _automation = null;
                _mainWindow = null;
            }
        }

        private void AddPhotoWithDetails(Window mainWindow, string fileName, string description, string date)
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var addPhotoButton = mainWindow.FindFirstDescendant(cf.ByName("Добавить фото")).AsButton();
            Assert.IsNotNull(addPhotoButton, "Кнопка 'Добавить фото' не найдена");
            addPhotoButton.Click();

            Thread.Sleep(1000);
            var openFileDialog = mainWindow.ModalWindows.FirstOrDefault()
                ?? mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Выберите фото")));
            Assert.IsNotNull(openFileDialog, "Диалог выбора файла не найден");

            var fileItem = openFileDialog.FindFirstDescendant(cf.ByName(fileName));
            Assert.IsNotNull(fileItem, $"Файл '{fileName}' не найден в диалоге выбора файла");
            fileItem.Click();
            Keyboard.Type(VirtualKeyShort.RETURN);

            Thread.Sleep(1000);
            var descriptionForm = mainWindow.ModalWindows.FirstOrDefault()
                ?? mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Введите описание и дату")));
            Assert.IsNotNull(descriptionForm, "Форма ввода описания не найдена");

            var descriptionTextBox = descriptionForm.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit)).AsTextBox();
            Assert.IsNotNull(descriptionTextBox, "Поле описания не найдено");
            descriptionTextBox.Text = description ?? "";

            var dateTextBox = descriptionForm.FindAllDescendants(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Edit))[1].AsTextBox();
            Assert.IsNotNull(dateTextBox, "Поле даты не найдено");
            dateTextBox.Text = date ?? "";

            var okButton = descriptionForm.FindFirstDescendant(cf.ByName("ОК")).AsButton();
            Assert.IsNotNull(okButton, "Кнопка 'ОК' не найдена");
            okButton.Click();
        }

        [TestMethod]
        public void TC0001_AddPhotoSuccessMessage()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном", "12.06.2025");

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();
        }

        [TestMethod]
        public void TC0002_AddPhotoWithCorrectValues()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "13.06.2025");

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();
        }

        [TestMethod]
        public void TC0003_DeletePhotoMessage()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном", "12.06.2025");

            Thread.Sleep(1000);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(addMessageBox, "MessageBox с подтверждением добавления не найден");

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(addMessageText, "Текст сообщения о добавлении не найден");
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(addMessageOkButton, "Кнопка 'OK' в MessageBox добавления не найдена");
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("гном")));
            Assert.IsNotNull(photoItem, "Фотография не найдена в ListView для удаления");
            photoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            Assert.IsNotNull(removePhotoButton, "Кнопка 'Удалить фото' не найдена");
            removePhotoButton.Click();

            Thread.Sleep(1000);
            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(deleteMessageBox, "MessageBox с подтверждением удаления не найден");

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(deleteMessageText, "Текст сообщения об удалении не найден");
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.", "Сообщение 'Фото удалено.' не отобразилось");

            var deleteMessageOkButton = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(deleteMessageOkButton, "Кнопка 'OK' в MessageBox удаления не найдена");
            deleteMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0004_DeletePhoto()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "13.06.2025");

            Thread.Sleep(1000);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(addMessageBox, "MessageBox с подтверждением добавления не найден");

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(addMessageText, "Текст сообщения о добавлении не найден");
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(addMessageOkButton, "Кнопка 'OK' в MessageBox добавления не найдена");
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("орк")));
            Assert.IsNotNull(photoItem, "Фотография не найдена в ListView для удаления");
            photoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            Assert.IsNotNull(removePhotoButton, "Кнопка 'Удалить фото' не найдена");
            removePhotoButton.Click();

            Thread.Sleep(1000);
            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(deleteMessageBox, "MessageBox с подтверждением удаления не найден");

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(deleteMessageText, "Текст сообщения об удалении не найден");
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.", "Сообщение 'Фото удалено.' не отобразилось");

            var deleteMessageOkButton = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(deleteMessageOkButton, "Кнопка 'OK' в MessageBox удаления не найдена");
            deleteMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0005_DeletePhotoWithoutSelection()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length, "ListView должен быть пустым перед началом теста");

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            Assert.IsNotNull(removePhotoButton, "Кнопка 'Удалить фото' не найдена");
            removePhotoButton.Click();

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Сначала выберите фото для удаления.", "Сообщение 'Сначала выберите фото для удаления.' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();
        }

        [TestMethod]
        public void TC0006_SortPhotosByDate()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "13.05.2025");

            Thread.Sleep(1000);
            var firstAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(firstAddMessageBox, "MessageBox с подтверждением добавления первой фотографии не найден");

            var firstAddMessageText = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(firstAddMessageText, "Текст сообщения о добавлении первой фотографии не найден");
            StringAssert.Contains(firstAddMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось для первой фотографии");

            var firstAddMessageOkButton = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(firstAddMessageOkButton, "Кнопка 'OK' в MessageBox добавления первой фотографии не найдена");
            firstAddMessageOkButton.Click();

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном", "12.05.2025");

            Thread.Sleep(1000);
            var secondAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(secondAddMessageBox, "MessageBox с подтверждением добавления второй фотографии не найден");

            var secondAddMessageText = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(secondAddMessageText, "Текст сообщения о добавлении второй фотографии не найден");
            StringAssert.Contains(secondAddMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось для второй фотографии");

            var secondAddMessageOkButton = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(secondAddMessageOkButton, "Кнопка 'OK' в MessageBox добавления второй фотографии не найдена");
            secondAddMessageOkButton.Click();

            var sortByDateButton = _mainWindow.FindFirstDescendant(cf.ByName("Отсортировать по дате")).AsButton();
            Assert.IsNotNull(sortByDateButton, "Кнопка 'Отсортировать по дате' не найдена");
            sortByDateButton.Click();

            Thread.Sleep(1000);
            var sortMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(sortMessageBox, "MessageBox с подтверждением сортировки не найден");

            var sortMessageText = sortMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(sortMessageText, "Текст сообщения о сортировке не найден");
            StringAssert.Contains(sortMessageText.Name, "Фото отсортированы по дате.", "Сообщение 'Фото отсортированы по дате.' не отобразилось");

            var sortMessageOkButton = sortMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(sortMessageOkButton, "Кнопка 'OK' в MessageBox сортировки не найдена");
            sortMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0007_AddPhotoWithEmptyDescription()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "", "13.06.2025");

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.", "Сообщение 'Описание и дата не могут быть пустыми.' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();

            Thread.Sleep(1000);
            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("гном")));
            Assert.IsNull(photoItem, "Фотография с пустым описанием была добавлена в ListView, хотя не должна была");
        }

        [TestMethod]
        public void TC0008_AddPhotoWithEmptyDate()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "орк", "");

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.", "Сообщение 'Описание и дата не могут быть пустыми.' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();

            Thread.Sleep(1000);
            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("орк")));
            Assert.IsNull(photoItem, "Фотография с пустой датой была добавлена в ListView, хотя не должна была");
        }

        [TestMethod]
        public void TC0009_AddPhotoWithEmptyFields()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "", "");

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Описание и дата не могут быть пустыми.", "Сообщение 'Описание и дата не могут быть пустыми.' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();

            Thread.Sleep(1000);
            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фотография1.jpg")));
            Assert.IsNull(photoItem, "Фотография с пустыми полями была добавлена в ListView, хотя не должна была");
        }

        [TestMethod]
        public void TC0010_SortEmptyListView()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length, "ListView должен быть пустым перед началом теста");

            var sortByDateButton = _mainWindow.FindFirstDescendant(cf.ByName("Отсортировать по дате")).AsButton();
            Assert.IsNotNull(sortByDateButton, "Кнопка 'Отсортировать по дате' не найдена");
            sortByDateButton.Click();

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Фото отсортированы по дате.", "Сообщение 'Фото отсортированы по дате' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();

            Assert.AreEqual(0, listView.Items.Length, "ListView должен остаться пустым после сортировки");
        }

        [TestMethod]
        public void TC0011_RemovePhotoWithSamePath()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном1", "01.06.2025");

            Thread.Sleep(1000);
            var firstAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(firstAddMessageBox, "MessageBox с подтверждением добавления первой фотографии не найден");

            var firstAddMessageText = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(firstAddMessageText, "Текст сообщения о добавлении первой фотографии не найден");
            StringAssert.Contains(firstAddMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось для первой фотографии");

            var firstAddMessageOkButton = firstAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(firstAddMessageOkButton, "Кнопка 'OK' в MessageBox добавления первой фотографии не найдена");
            firstAddMessageOkButton.Click();

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "гном2", "02.06.2025");

            Thread.Sleep(1000);
            var secondAddMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(secondAddMessageBox, "MessageBox с подтверждением добавления второй фотографии не найден");

            var secondAddMessageText = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(secondAddMessageText, "Текст сообщения о добавлении второй фотографии не найден");
            StringAssert.Contains(secondAddMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось для второй фотографии");

            var secondAddMessageOkButton = secondAddMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(secondAddMessageOkButton, "Кнопка 'OK' в MessageBox добавления второй фотографии не найдена");
            secondAddMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(2, listView.Items.Length, "В ListView должно быть ровно 2 фотографии");

            var firstPhotoItem = listView.Items[0];
            Assert.IsNotNull(firstPhotoItem, "Первая фотография в ListView не найдена");
            firstPhotoItem.Click();

            var removePhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Удалить фото")).AsButton();
            Assert.IsNotNull(removePhotoButton, "Кнопка 'Удалить фото' не найдена");
            removePhotoButton.Click();

            Thread.Sleep(1000);
            var deleteMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(deleteMessageBox, "MessageBox с подтверждением удаления не найден");

            var deleteMessageText = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(deleteMessageText, "Текст сообщения об удалении не найден");
            StringAssert.Contains(deleteMessageText.Name, "Фото удалено.", "Сообщение 'Фото удалено.' не отобразилось");

            var deleteMessageOkButton = deleteMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(deleteMessageOkButton, "Кнопка 'OK' в MessageBox удаления не найдена");
            deleteMessageOkButton.Click();
        }

        [TestMethod]
        public void TC0012_OpenPhotoViewForm()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "фото", "14.06.2025");

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            Assert.IsNotNull(photoItem, "Фотография не добавлена в ListView");

            var subItems = photoItem.FindAllChildren();
            Assert.IsTrue(subItems.Length >= 3, "Недостаточно элементов в ListViewItem");
            Assert.AreEqual("фото", subItems[1].Name, "Описание в ListView не совпадает");
            Assert.AreEqual("14.06.2025", subItems[2].Name, "Дата в ListView не совпадает");

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            Assert.IsNotNull(viewPhotoButton, "Кнопка 'Посмотреть фото' не найдена");
            viewPhotoButton.Click();

            Thread.Sleep(1000);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNotNull(viewForm, "Форма просмотра фотографии не открылась");

            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();
            Assert.IsNotNull(closeButton, "Кнопка 'Закрыть' не найдена");
            closeButton.Click();
        }

        [TestMethod]
        public void TC0013_ZoomInPhoto()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "гном.jpg", "фото", "14.06.2025");

            Thread.Sleep(1000);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(addMessageBox, "MessageBox с подтверждением добавления не найден");

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(addMessageText, "Текст сообщения о добавлении не найден");
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(addMessageOkButton, "Кнопка 'OK' в MessageBox добавления не найдена");
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            Assert.IsNotNull(photoItem, "Фотография не найдена в ListView для просмотра");

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            Assert.IsNotNull(viewPhotoButton, "Кнопка 'Посмотреть фото' не найдена");
            viewPhotoButton.Click();

            Thread.Sleep(1000);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNotNull(viewForm, "Форма просмотра фотографии не открылась");

            var zoomInButton = viewForm.FindFirstDescendant(cf.ByName("Приблизить")).AsButton();
            Assert.IsNotNull(zoomInButton, "Кнопка 'Приблизить' не найдена");

            for (int i = 0; i < 3; i++)
            {
                zoomInButton.Click();
                Thread.Sleep(500);
            }

            var viewFormAfterZoom = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNotNull(viewFormAfterZoom, "Форма просмотра фотографии закрылась после нажатия 'Приблизить'");

            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();
            Assert.IsNotNull(closeButton, "Кнопка 'Закрыть' не найдена");
            closeButton.Click();
        }

        [TestMethod]
        public void TC0014_ZoomOutPhoto()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "фото", "14.06.2025");

            Thread.Sleep(1000);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(addMessageBox, "MessageBox с подтверждением добавления не найден");

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(addMessageText, "Текст сообщения о добавлении не найден");
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(addMessageOkButton, "Кнопка 'OK' в MessageBox добавления не найдена");
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            Assert.IsNotNull(photoItem, "Фотография не найдена в ListView для просмотра");

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            Assert.IsNotNull(viewPhotoButton, "Кнопка 'Посмотреть фото' не найдена");
            viewPhotoButton.Click();

            Thread.Sleep(1000);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNotNull(viewForm, "Форма просмотра фотографии не открылась");

            var zoomOutButton = viewForm.FindFirstDescendant(cf.ByName("Отдалить")).AsButton();
            Assert.IsNotNull(zoomOutButton, "Кнопка 'Отдалить' не найдена");

            for (int i = 0; i < 3; i++)
            {
                zoomOutButton.Click();
                Thread.Sleep(500);
            }

            var viewFormAfterZoom = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNotNull(viewFormAfterZoom, "Форма просмотра фотографии закрылась после нажатия 'Отдалить'");

            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();
            Assert.IsNotNull(closeButton, "Кнопка 'Закрыть' не найдена");
            closeButton.Click();
        }

        [TestMethod]
        public void TC0015_ClosePhotoViewForm()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            AddPhotoWithDetails(_mainWindow, "орк.jpg", "фото", "14.06.2025");

            Thread.Sleep(1000);
            var addMessageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(addMessageBox, "MessageBox с подтверждением добавления не найден");

            var addMessageText = addMessageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(addMessageText, "Текст сообщения о добавлении не найден");
            StringAssert.Contains(addMessageText.Name, "Фото добавлено.", "Сообщение 'Фото добавлено.' не отобразилось");

            var addMessageOkButton = addMessageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(addMessageOkButton, "Кнопка 'OK' в MessageBox добавления не найдена");
            addMessageOkButton.Click();

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            var photoItem = listView.Items.FirstOrDefault(i => i.FindAllChildren().Any(c => c.Name.Contains("фото")));
            Assert.IsNotNull(photoItem, "Фотография не найдена в ListView для просмотра");

            photoItem.Click();

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            Assert.IsNotNull(viewPhotoButton, "Кнопка 'Посмотреть фото' не найдена");
            viewPhotoButton.Click();

            Thread.Sleep(1000);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNotNull(viewForm, "Форма просмотра фотографии не открылась");

            var closeButton = viewForm.FindFirstDescendant(cf.ByName("Закрыть")).AsButton();
            Assert.IsNotNull(closeButton, "Кнопка 'Закрыть' не найдена");
            closeButton.Click();

            Thread.Sleep(1000);
            var viewFormAfterClose = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNull(viewFormAfterClose, "Форма просмотра фотографии не закрылась после нажатия 'Закрыть'");
        }

        [TestMethod]
        public void TC0016_OpenPhotoViewWithoutSelection()
        {
            var cf = new ConditionFactory(new UIA3PropertyLibrary());

            var listView = _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.List)).AsListBox();
            Assert.AreEqual(0, listView.Items.Length, "ListView должен быть пустым перед началом теста");

            var viewPhotoButton = _mainWindow.FindFirstDescendant(cf.ByName("Посмотреть фото")).AsButton();
            Assert.IsNotNull(viewPhotoButton, "Кнопка 'Посмотреть фото' не найдена");
            viewPhotoButton.Click();

            Thread.Sleep(1000);
            var messageBox = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window));
            Assert.IsNotNull(messageBox, "MessageBox не найден");

            var messageText = messageBox.FindFirstDescendant(cf.ByAutomationId("65535"));
            Assert.IsNotNull(messageText, "Текст сообщения не найден");
            StringAssert.Contains(messageText.Name, "Сначала выберите фото для просмотра.", "Сообщение 'Сначала выберите фото для просмотра.' не отобразилось");

            var messageOkButton = messageBox.FindFirstDescendant(cf.ByAutomationId("2")).AsButton();
            Assert.IsNotNull(messageOkButton, "Кнопка 'OK' в MessageBox не найдена");
            messageOkButton.Click();

            Thread.Sleep(1000);
            var viewForm = _mainWindow.ModalWindows.FirstOrDefault()
                ?? _mainWindow.FindFirstDescendant(cf.ByControlType(FlaUI.Core.Definitions.ControlType.Window).And(cf.ByName("Просмотр фотографии")));
            Assert.IsNull(viewForm, "Форма просмотра фотографии открылась, хотя не должна была");
        }
    }
}