using System;
using System.Windows.Forms;
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
    public partial class DescriptionForm : Form
    {
        private TextBox descriptionTextBox;
        private TextBox dateTextBox;
        private Button okButton;
        private Button cancelButton;

        public string Description { get; private set; }
        public string DateInput { get; private set; }

        public DescriptionForm()
        {
            this.Text = "Введите описание и дату";
            this.Width = 300;
            this.Height = 200;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;

            descriptionTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 30),
                Size = new System.Drawing.Size(260, 20)
            };

            dateTextBox = new TextBox
            {
                Location = new System.Drawing.Point(10, 80),
                Size = new System.Drawing.Size(260, 20)
            };

            okButton = new Button
            {
                Location = new System.Drawing.Point(110, 120),
                Text = "ОК",
                Size = new System.Drawing.Size(80, 25)
            };
            okButton.Click += (sender, e) =>
            {
                Description = descriptionTextBox.Text;
                DateInput = dateTextBox.Text;
                this.DialogResult = DialogResult.OK;
                this.Close();
            };

            cancelButton = new Button
            {
                Location = new System.Drawing.Point(200, 120),
                Text = "Отмена",
                Size = new System.Drawing.Size(80, 25)
            };
            cancelButton.Click += (sender, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            this.Controls.Add(new Label { Location = new System.Drawing.Point(10, 10), Text = "Описание:", Size = new System.Drawing.Size(80, 20) });
            this.Controls.Add(descriptionTextBox);
            this.Controls.Add(new Label { Location = new System.Drawing.Point(10, 60), Text = "Дата (дд.ММ.гггг):", Size = new System.Drawing.Size(100, 20) });
            this.Controls.Add(dateTextBox);
            this.Controls.Add(okButton);
            this.Controls.Add(cancelButton);
        }
    }
}