﻿using Knightware.Primitives;
using Spyder.Client.Common;
using System;
using System.Collections.Generic;

namespace Spyder.Client.Models.StackupProviders
{
    public interface IStackupProvider
    {
        /// <summary>
        /// Event raised with the Stackup map for the provider has changed
        /// </summary>
        event EventHandler StackupInvalidated;

        /// <summary>
        /// Translates a provided rectangle from a native coordinate space into the correlating stackup location
        /// </summary>
        /// <returns>Translated rectangle into the current stackup, or an empty rectangle if the PixelSpace is not visible</returns>
        Rectangle GenerateOffsetRect(Rectangle sourceRect, int pixelSpaceID);


        void UpdateStackupMap(IEnumerable<PixelSpace> pixelSpaces, Size displaySize);
    }
}
