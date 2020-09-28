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

        public Visual(string id, string imagePath, bool relative = false)
        {
            ID = id;
            ImagePath = imagePath;
            if (File.Exists(imagePath))
                LoadImage(imagePath, relative);
        }

        private void LoadImage(string imagePath, bool relative = false)
        {
            if (relative)
                BitmapFrame = BitmapFrame.Create(new System.Uri(imagePath, UriKind.Relative));
            else
                BitmapFrame = BitmapFrame.Create(new System.Uri(imagePath));
        }

        public void SetImagePath(string imagePath, bool relative = false)
        {
            LoadImage(imagePath, relative);
            ImagePath = imagePath;
        }

    }
}
