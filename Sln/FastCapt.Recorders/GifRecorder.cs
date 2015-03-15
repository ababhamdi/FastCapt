﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using FastCapt.Recorders.Internals;
using FastCapt.Recorders.Interop;
using SharpDX.WIC;
using WicBitmap = SharpDX.WIC.Bitmap;
using HBitmap = System.Drawing.Bitmap;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace FastCapt.Recorders
{
    public class GifRecorder
    {
        #region "Fields"

        private readonly ImagingFactory _imagingFactory;
        private GifBitmapEncoder _gifBitmapEncoder;
        private MemoryStream _stream;
        private int _width;
        private int _height;
        private IList<BitmapFrame> _frames;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the repeat count of the produced GIF.
        /// </summary>
        public int GifRepeatCount { get; set; }

        /// <summary>
        /// Gets or sets a value that indicates the current frame delay.
        /// </summary>
        public Double Delay { get; set; }

        #endregion

        #region "Constructors"

        /// <summary>
        /// Initializes instance members of the <see cref="GifRecorder"/> class.
        /// </summary>
        public GifRecorder()
        {
            GifRepeatCount = 0;
            _imagingFactory = new ImagingFactory();
            _frames = new List<BitmapFrame>();
        }

        #endregion

        #region "Methods"

        public void Save(string filePath)
        {
            WriteGlobalMetadata();
            EncodeFrames();
            _gifBitmapEncoder.Commit();
            File.WriteAllBytes(filePath, _stream.GetBuffer());
        }

        public void StartRecording(int width, int height)
        {
            _stream = new MemoryStream();
            _gifBitmapEncoder = new GifBitmapEncoder(_imagingFactory);
            _gifBitmapEncoder.Initialize(_stream);

            _width = width;
            _height = height;
        }

        private HBitmap _previousBitmap = null;

        public void AddFrame(HBitmap frameBitmap, int delay)
        {
            if (_frames.Count == 0)
            {
                _frames.Add(new BitmapFrame(frameBitmap, delay, 0, 0));
                _previousBitmap = frameBitmap;
                return;
            }
            Debug.Assert(_previousBitmap != null);

            // compare the current frame to the previous one.
            int xPos;
            int yPos;
            HBitmap clippedFrame;
            if (CompareFrames(_previousBitmap, frameBitmap, out xPos, out yPos, out clippedFrame))
            {
                _frames.Add(new BitmapFrame(clippedFrame, delay, xPos, yPos));
                _previousBitmap = frameBitmap;
            }
            else
            {
                // just increase the delay of the last frame.
                var lastFrame = _frames[_frames.Count - 1];
                lastFrame.Delay += delay;
            }
        }

        private void WriteGlobalMetadata()
        {
            MetadataQueryWriter metadataWriter = _gifBitmapEncoder.MetadataQueryWriter;
            metadataWriter.SetMetadataByName("/logscrdesc/Width", Convert.ToUInt16(_width));
            metadataWriter.SetMetadataByName("/logscrdesc/Height", Convert.ToUInt16(_height));

            WriteApplicationMetadata(metadataWriter.NativePointer);
            WriteLoopingMetadata(metadataWriter.NativePointer);
        }

        private void WriteApplicationMetadata(IntPtr handle)
        {
            // writing application metadata block. (VT_UI1 | VT_VECTOR)
            var netscapeValue = Encoding.ASCII.GetBytes("NETSCAPE2.0");
            var bufferPtr = Marshal.AllocHGlobal(netscapeValue.Length);

            unsafe
            {
                for (int i = 0; i < netscapeValue.Length; i++)
                {
                    ((byte*)bufferPtr)[i] = netscapeValue[i];
                }
            }

            PROPVARIANT propvariant;
            HResult.Check(NativeMethods.InitPropVariantFromBuffer(bufferPtr, (uint)netscapeValue.Length, out propvariant));
            HResult.Check(WICMetadataQueryWriter.SetMetadataByName(handle, "/appext/Application", ref propvariant));
        }

        private void WriteLoopingMetadata(IntPtr handle)
        {
            var buffer = new byte[] { 3, 1, 0, 0, 0 };
            var bufferPtr = Marshal.AllocHGlobal(buffer.Length);
            unsafe
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    ((byte*)bufferPtr)[i] = buffer[i];
                }
            }

            PROPVARIANT dataVariant;
            HResult.Check(NativeMethods.InitPropVariantFromBuffer(bufferPtr, (uint)buffer.Length, out dataVariant));
            HResult.Check(WICMetadataQueryWriter.SetMetadataByName(handle, "/appext/Data", ref dataVariant));
        }

        private void WriteFrameMetadata()
        {

        }

        private void EncodeFrames()
        {
            if (_frames.Count == 0)
            {
                return;
            }

            for (int index = 0; index < _frames.Count; index++)
            {
                var frame = _frames[index];
                FlushCurrentFrame(_gifBitmapEncoder, _imagingFactory, frame);
            }
        }

        private static void FlushCurrentFrame(GifBitmapEncoder encoder,
            ImagingFactory factory,
            BitmapFrame bitmap)
        {
            // nothing to flush.
            if (bitmap == null)
            {
                return;
            }

            using (var frameEncoder = new BitmapFrameEncode(encoder))
            {
                frameEncoder.Initialize();
                frameEncoder.SetSize(bitmap.Data.Width, bitmap.Data.Height);
                frameEncoder.SetResolution(bitmap.Data.HorizontalResolution, bitmap.Data.VerticalResolution);

                // embed frame metadata.
                var metadataWriter = frameEncoder.MetadataQueryWriter;
                metadataWriter.SetMetadataByName("/grctlext/Delay", Convert.ToUInt16(bitmap.Delay / 100));
                metadataWriter.SetMetadataByName("/imgdesc/Left", Convert.ToUInt16(bitmap.XPos));
                metadataWriter.SetMetadataByName("/imgdesc/Top", Convert.ToUInt16(bitmap.YPos));
                metadataWriter.SetMetadataByName("/imgdesc/Width", Convert.ToUInt16(bitmap.Data.Width));
                metadataWriter.SetMetadataByName("/imgdesc/Height", Convert.ToUInt16(bitmap.Data.Height));

                using (var bitmapSource = new WicBitmap(
                    factory,
                    bitmap.Data,
                    BitmapAlphaChannelOption.UsePremultipliedAlpha))
                {
                    var converter = new FormatConverter(factory);
                    converter.Initialize(bitmapSource,
                        PixelFormat.Format8bppIndexed,
                        BitmapDitherType.Solid,
                        null,
                        0.8,
                        BitmapPaletteType.MedianCut);

                    frameEncoder.WriteSource(converter);
                    frameEncoder.Commit();
                }
            }
        }

        // the following is a slow implementation
        private bool CompareFrames(
            HBitmap originalBitmap,
            HBitmap newBitmap,
            out int xPos,
            out int yPos,
            out HBitmap clippedFrame)
        {
            if (newBitmap == null)
            {
                throw new ArgumentNullException("@newBitmap");
            }

            if (originalBitmap == null)
            {
                xPos = yPos = 0;
                clippedFrame = newBitmap;
                return true;
            }

            if (originalBitmap.Width != newBitmap.Width || originalBitmap.Height != newBitmap.Height)
            {
                throw new InvalidOperationException("The images must have the same Width and Height.");
            }

            var pixelFormat = originalBitmap.PixelFormat;
            if (pixelFormat != newBitmap.PixelFormat)
            {
                throw new InvalidOperationException("The images must have the same PixelFormat");
            }

            /* TIP: stride represents the number of bytes for a given row image. */

            const ImageLockMode COMPARISON_LOCK_MODE = ImageLockMode.ReadOnly;

            var width = originalBitmap.Width;
            var height = originalBitmap.Height;
            bool breakOuterLoop = false;
            var originalData = originalBitmap.LockBits(originalBitmap.GetBounds(), COMPARISON_LOCK_MODE, pixelFormat);
            var newData = newBitmap.LockBits(newBitmap.GetBounds(), COMPARISON_LOCK_MODE, pixelFormat);

            // find the number of bytes per pixel
            int pixelSize = 4;
            int top = 0,
                bottom = 0,
                left = 0,
                right = 0;

            try
            {
                unsafe
                {
                    // find the top
                    for (int y = 0; y < height; y++)
                    {
                        var currentOriginalRow = (byte*)originalData.Scan0 + (y * originalData.Stride);
                        var currentNewRow = (byte*)newData.Scan0 + (y * newData.Stride);

                        // compare both rows pixels
                        for (int x = 0; x < width; x += 4)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                var originalByte = currentOriginalRow[(x * pixelSize) + 1];
                                var newByte = currentNewRow[(x * pixelSize) + 1];

                                if (originalByte == newByte)
                                    continue;
                                top = y;
                                breakOuterLoop = true;
                                break;
                            }

                            if (breakOuterLoop)
                                break;
                        }

                        if (breakOuterLoop)
                            break;
                    }

                    // both images have the same pixels.
                    if (!breakOuterLoop)
                    {
                        xPos = yPos = 0;
                        clippedFrame = null;
                        return false;
                    }

                    // find the bottom
                    breakOuterLoop = false;
                    for (int y = height - 1; y > top; y--)
                    {
                        var currentOriginalRow = (byte*)originalData.Scan0 + (y * originalData.Stride);
                        var currentNewRow = (byte*)newData.Scan0 + (y * newData.Stride);

                        // compare both rows pixels
                        for (int x = 0; x < width; x += 4)
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                var originalByte = currentOriginalRow[(x * pixelSize) + i];
                                var newByte = currentNewRow[(x * pixelSize) + i];

                                if (originalByte == newByte)
                                    continue;
                                bottom = y;
                                breakOuterLoop = true;
                                break;
                            }

                            if (breakOuterLoop)
                                break;
                        }

                        if (breakOuterLoop)
                            break;
                    }

                    // find the left edge.
                    for (int x = 0; x < width; x++)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            var originalRow = (byte*)originalData.Scan0 + (y * originalData.Stride);
                            var newRow = (byte*)newData.Scan0 + (y * originalData.Stride);

                            for (int i = 0; i < 4; i++)
                            {
                                var originalByte = originalRow[(x * pixelSize) + i];
                                var newByte = newRow[(x * pixelSize) + i];

                                if (originalByte == newByte)
                                    continue;
                                left = x;
                                breakOuterLoop = true;
                                break;
                            }

                            if (breakOuterLoop)
                                break;
                        }

                        if (breakOuterLoop)
                            break;
                    }

                    breakOuterLoop = false;
                    // find the right edge.
                    for (int x = width - 1; x > left; x--)
                    {
                        for (int y = 0; y < height; y++)
                        {
                            var originalRow = (byte*)originalData.Scan0 + (y * originalData.Stride);
                            var newRow = (byte*)newData.Scan0 + (y * originalData.Stride);

                            for (int i = 0; i < 4; i++)
                            {
                                var originalByte = originalRow[(x * pixelSize) + i];
                                var newByte = newRow[(x * pixelSize) + i];

                                if (originalByte == newByte)
                                    continue;
                                right = x;
                                breakOuterLoop = true;
                                break;
                            }

                            if (breakOuterLoop)
                                break;
                        }

                        if (breakOuterLoop)
                            break;
                    }
                }
            }
            finally
            {
                originalBitmap.UnlockBits(originalData);
                newBitmap.UnlockBits(newData);
            }

            // we widen the diff a little bit, just to be on the safe side.
            left = Math.Max(0, left - 2);
            right = Math.Min(width, right + 2);
            top = Math.Max(0, top - 2);
            bottom = Math.Min(height, bottom + 2);

            xPos = left;
            yPos = top;
            clippedFrame = newBitmap.Clip(xPos, yPos, right - left, bottom - top);
            return true;
        }

        #endregion
    }
}