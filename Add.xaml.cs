using Microsoft.Win32;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Media.Imaging;
using Image = System.Drawing.Image;

namespace D2P_Exam
{
    /// <summary>
    /// Interaction logic for Add.xaml
    /// </summary>
    public partial class Add : Window
    {
        private Dictionary<string, string> _tempImagesPath = new Dictionary<string, string>();
        private static IMongoCollection<Patient> _collection;
        List<byte[]> images = new List<byte[]>();

        List<ImageData> pictures = new List<ImageData>();

        public Add()
        {
            InitializeComponent();
            lv_images.ItemsSource = pictures;
            datePicker1.DisplayDateEnd = DateTime.Today;
        }

        public void SetCollection(IMongoCollection<Patient> c)
        {
            _collection = c;
        }

        private void btn_add_Click(object sender, RoutedEventArgs e)
        {
            if (!ValidateData()) return;

            Patient patient = new Patient(tb_id.Text, tb_name.Text, datePicker1.Text, tb_phone.Text, tb_email.Text, images);
            _collection.InsertOne(patient);

            BackToMain();
        }

        private byte[] ConvertImageToByteArray(Image image, string extension)
        {
            using (var memoryStream = new MemoryStream())
            {
                switch (extension)
                {
                    case ".jpeg":
                    case ".jpg":
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                        break;
                    case ".png":
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        break;
                    case ".bmp":
                        image.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Bmp);
                        break;
                }
                return memoryStream.ToArray();
            }
        }

        private bool ValidateData()
        {
            if (tb_id.Text == "" || tb_name.Text == "" || datePicker1.Text == "" || tb_phone.Text == "" || tb_email.Text == "")
            {
                MessageBox.Show("All fields are requierd");
                return false;
            }

            if (!int.TryParse(tb_id.Text, out int n) || tb_id.Text.Length != 9)
            {
                MessageBox.Show("ID must be a 9 digit number");
                return false;
            }

            if (!Regex.IsMatch(tb_name.Text, @"^[a-zA-Z]+$"))
            {
                MessageBox.Show("Name must contains only a-z letters");
                return false;
            }

            if (!int.TryParse(tb_phone.Text, out int m) || tb_phone.Text.Length > 10 || tb_phone.Text.Length < 9)
            {
                MessageBox.Show("Phone must be a 9 or 10 digits number");
                return false;
            }

            if(!Regex.IsMatch(tb_email.Text, @"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$"))
            {
                MessageBox.Show("Email address is not valid");
                return false;
            }

            if (_collection.AsQueryable().Any(patient => patient.Id == tb_id.Text))
            {
                MessageBox.Show("Patient ID " + tb_id.Text + " already exists");
                return false;
            }

            return true;
        }

        private void BackToMain()
        {
            MainWindow main = new MainWindow();

            main.Height = this.Height;
            main.Width = this.Width;
            main.Left = this.Left;
            main.Top = this.Top;

            main.Show();
            this.Close();
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e)
        {
            BackToMain();
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

        private BitmapImage LoadImage(string filename)
        {
            return new BitmapImage(new Uri(filename));
        }

        private void btn_upload_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Multiselect = true;
            fileDialog.DefaultExt = ".jpg"; 
            fileDialog.Filter = "Image files (*.jpg, *.jpeg, *.jpe, *.jfif, *.png) | *.jpg; *.jpeg; *.jpe; *.jfif; *.png";
            fileDialog.ShowDialog();

            string[] result = fileDialog.FileNames;

            if(result.Length > 0)
            {
                foreach (var path in result)
                {
                    string name = Path.GetFileName(path);

                    //add path to dicionary container - prevents duplicates 
                    if (_tempImagesPath.ContainsKey(path))
                    {
                        MessageBox.Show("Already added image named: " + name);
                        return;
                    }
                    _tempImagesPath.Add(path, "");

                    //convert to byte[] for mongoDB storage
                    string[] splitedPath = path.Split('.');
                    byte[] b = ConvertImageToByteArray(Image.FromFile(path), "." + splitedPath[splitedPath.Length - 1]);
                    images.Add(b);

                    //display in add's page selected images 
                    pictures.Add(new ImageData { Name = name, Image = LoadImage(path) });
                    ImageData[] arr = new ImageData[pictures.Count];
                    int i = 0;
                    foreach (var item in pictures)
                    {
                        arr[i] = item;
                        i++;
                    }

                    this.lv_images.ItemsSource = arr;
                }
            }
        }
    }
}
