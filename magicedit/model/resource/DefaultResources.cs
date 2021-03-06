﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace magicedit
{
    public class DefaultResources
    {

        private static BitmapFrame visualPlaceholder;
        public static BitmapFrame VisualPlaceholder
        {
            get
            {
                if (visualPlaceholder == null)
                {
                    if (File.Exists("images/visual_placeholder.png"))
                        visualPlaceholder = BitmapFrame.Create(new System.Uri("images/visual_placeholder.png", UriKind.Relative));
                }
                return visualPlaceholder;
            }
        }

        private static BitmapFrame plusSymbolImage;
        public static BitmapFrame PlusSymbolImage
        {
            get
            {
                if (plusSymbolImage == null)
                {
                    if (File.Exists("images/plus_symbol.png"))
                        plusSymbolImage = BitmapFrame.Create(new System.Uri("images/plus_symbol.png", UriKind.Relative));
                }
                return plusSymbolImage;
            }
        }

        public static BitmapFrame GetVisualImageOrDefault(Visual visual)
        {
            if (visual?.BitmapFrame != null) return visual.BitmapFrame;
            return VisualPlaceholder;
        }

    }
    
}
