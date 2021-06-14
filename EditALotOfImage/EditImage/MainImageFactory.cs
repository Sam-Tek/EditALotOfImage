using ImageProcessor;
using ImageProcessor.Imaging.Formats;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace EditALotOfImage.EditImage
{
    class MainImageFactory
    {
        public List<string> ListPathImage { get; set; }
        public string DirectoryBuffer { get; set; } = $"{System.AppContext.BaseDirectory}\\DirectoryBuffer\\";
        public string NewDirectory { get; set; } = @"\ImageFactory\";

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public string GetRandName(string path)
        {
            return new Random().Next(1000000, 9999999).ToString() + Path.GetExtension(path);
        }

        public void RemoveBuffer(string path)
        {
            if (Directory.Exists(DirectoryBuffer))
            {
                foreach (var item in Directory.GetFiles(DirectoryBuffer))
                {
                    try
                    {
                        File.Delete(item);
                    }
                    catch (Exception e)
                    {
                        //MessageBox.Show(e.Message);
                        //MessageBox.Show(System.AppContext.BaseDirectory);
                    }
                }
            }
        }

        public async Task EditAll(int ratio, int contrast, int brightness, string pathToSave, IProgress<int> progress)
        {
            string newNameImageInDirectoryBuffer = "";
            int totalCount = ListPathImage.Count-1;
            await Task.Run(() =>
            {
                int tempCount = 0;
                foreach (string PathImage in ListPathImage)
                {
                    byte[] photoBytes = File.ReadAllBytes(PathImage);
                    // Format is automatically detected though can be changed.
                    using (MemoryStream inStream = new MemoryStream(photoBytes))
                    {
                        using (MemoryStream outStream = new MemoryStream())
                        {
                            // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                            using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                            {
                                // Load, resize, set the format and quality and save an image.
                                Image image = imageFactory.Load(inStream).Image;
                                int widthImage = (image.Size.Width * ratio) / 100;
                                int heightImage = (image.Size.Height * ratio) / 100;
                                System.Drawing.Size resizeImage = new System.Drawing.Size(widthImage, heightImage);
                                newNameImageInDirectoryBuffer = GetRandName(GetFileName(PathImage));
                                imageFactory.Load(inStream).Resize(resizeImage).Contrast(contrast).Brightness(brightness).Save($"{pathToSave}{NewDirectory}{GetFileName(PathImage)}");
                                progress.Report(tempCount * 100 / totalCount);
                                tempCount++;
                            }
                        }
                    }
                }
                MessageBox.Show("Images are saved !!!");
            });
        }

        public async Task<string> Edit(int ratio, int contrast, int brightness, string pathImage, IProgress<int> progress)
        {
            string newNameImageInDirectoryBuffer = "";
            byte[] photoBytes = File.ReadAllBytes(pathImage);
            // Format is automatically detected though can be changed.
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (MemoryStream outStream = new MemoryStream())
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(preserveExifData: true))
                    {
                        await Task.Run(() =>
                        {
                            progress.Report(10);
                            Image image = imageFactory.Load(inStream).Image;
                            progress.Report(20);
                            int widthImage = (image.Size.Width * ratio) / 100;
                            int heightImage = (image.Size.Height * ratio) / 100;
                            progress.Report(40);
                            System.Drawing.Size resizeImage = new System.Drawing.Size(widthImage, heightImage);
                            newNameImageInDirectoryBuffer = GetRandName(GetFileName(pathImage));
                            progress.Report(60);
                            imageFactory.Load(inStream).Resize(resizeImage).Contrast(contrast).Brightness(brightness).Save($"{DirectoryBuffer}{newNameImageInDirectoryBuffer}");
                            progress.Report(100);
                        });
                    }
                }
            }
            return $"{DirectoryBuffer}{newNameImageInDirectoryBuffer}";
        }
    }
}
