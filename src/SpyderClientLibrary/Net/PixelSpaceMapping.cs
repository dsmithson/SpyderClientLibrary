﻿namespace Spyder.Client.Net
{
    public class PixelSpaceMapping
    {
        public int ProgramID { get; set; }
        public int PreviewID { get; set; }
        public double Scale { get; set; }

        public PixelSpaceMapping()
        {

        }

        public PixelSpaceMapping(int programID, int previewID, double scale)
        {
            this.ProgramID = programID;
            this.PreviewID = previewID;
            this.Scale = scale;
        }
    }
}
