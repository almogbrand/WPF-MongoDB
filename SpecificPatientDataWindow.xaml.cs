using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace D2P_Exam
{
    /// <summary>
    /// Interaction logic for SpecificPatientDataWindow.xaml
    /// </summary>
    public partial class SpecificPatientDataWindow : Window
    {
        private Patient _patient;

        public SpecificPatientDataWindow()
        {
            InitializeComponent();
        }

        public void SetPatient(Patient p)
        {
            _patient = p;
            UploadPatientData();
        }

        private void UploadPatientData()
        {
            tb_id.Text = _patient.Id;
            tb_name.Text = _patient.Name;
            tb_DOB.Text = _patient.DathOfBirth;
            tb_phone.Text = _patient.Phone;
            tb_email.Text = _patient.Email;

            List<byte[]> list = _patient.images;
            ImageData[] arr = new ImageData[list.Count];
            int i = 0;
            foreach (var item in list)
            {
                arr[i] = new ImageData { Name = "", Image = ImageFromBuffer(item) };
                i++;
            }

            lv_images.ItemsSource = arr;
        }

        public BitmapImage ImageFromBuffer(Byte[] bytes)
        {
            MemoryStream stream = new MemoryStream(bytes);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            image.StreamSource = stream;
            image.EndInit();
            return image;
        }

        private void btn_back_Click(object sender, RoutedEventArgs e)
        {
            MainWindow main = new MainWindow();

            main.Height = this.Height;
            main.Width = this.Width;
            main.Left = this.Left;
            main.Top = this.Top;

            main.Show();
            this.Close();
        }

        public class ImageData
        {
            private string _name;
            public string Name
            {
                get { return this._name; }
                set { this._name = value; }
            }

            private BitmapImage _iamge;
            public BitmapImage Image
            {
                get { return this._iamge; }
                set { this._iamge = value; }
            }
        }

    }
}
