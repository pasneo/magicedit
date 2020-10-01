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

        [JsonProperty]
        private bool IsPathRelative { get; set; }
        public string ImagePath { get; private set; }

        /* *** */

        public Visual(string id, string imagePath, bool isPathRelative = false)
        {
            ID = id;
            ImagePath = imagePath;
            IsPathRelative = isPathRelative;

            if (File.Exists(imagePath))
                LoadImage(imagePath);
        }

        private void LoadImage(string imagePath)
        {
            if (IsPathRelative)
                BitmapFrame = BitmapFrame.Create(new System.Uri(imagePath, UriKind.Relative));
            else
                BitmapFrame = BitmapFrame.Create(new System.Uri(imagePath));
        }

        public void SetImagePath(string imagePath, bool isPathRelative = false)
        {
            IsPathRelative = isPathRelative;
            LoadImage(imagePath);
            ImagePath = imagePath;
        }

    }
}
