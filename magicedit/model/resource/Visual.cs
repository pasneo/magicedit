using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace magicedit
{
    public class Visual
    {
        [JsonIgnore]
        public BitmapFrame BitmapFrame { get; private set; }

        public string ID;
        public string ImagePath { get; private set; }

        /* *** */

        public Visual(string id, string imagePath)
        {
            ID = id;
            ImagePath = imagePath;
            if (File.Exists(imagePath))
                LoadImage(imagePath);
        }

        private void LoadImage(string imagePath)
        {
            BitmapFrame = BitmapFrame.Create(new System.Uri(imagePath));
        }

        public void SetImagePath(string imagePath)
        {
            LoadImage(imagePath);
            ImagePath = imagePath;
        }

    }
}
