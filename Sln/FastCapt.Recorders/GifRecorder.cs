using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FastCapt.Recorders.Interfaces;
using FastCapt.Recorders.Internals;
using FastCapt.Recorders.Interop;
using SharpDX.WIC;
using WicBitmap = SharpDX.WIC.Bitmap;
using HBitmap = System.Drawing.Bitmap;
using PixelFormat = SharpDX.WIC.PixelFormat;

namespace FastCapt.Recorders
{
    [Export(typeof(IRecorder))]
    public class GifRecorder : IRecorder
    {
        #region "Fields"

        private readonly ImagingFactory _imagingFactory;
        private GifBitmapEncoder _gifBitmapEncoder;
        private MemoryStream _stream;
        private int _width;
        private int _height;
        private int _interval;
        private Timer _timer;
        private IList<BitmapFrame> _frames;
        private HBitmap _previousBitmap;

        private SnapshotManager _snapshotManager;
        private DesktopManager _desktopManager;

        #endregion

        #region "Properties"

        /// <summary>
        /// Gets or sets the repeat count of the produced GIF.
        /// </summary>
        public int GifRepeatCount { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of frames per second.
        /// </summary>
        public byte MaxFramePerSecond { get; set; }

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
            MaxFramePerSecond = 8;
            _desktopManager = new DesktopManager();
        }

        #endregion

        #region "Methods"

        public void Stop()
        {
            _timer.Dispose();
            WriteGlobalMetadata();
            EncodeFrames();
            _gifBitmapEncoder.Commit();
        }

        public void Save(Stream stream)
        {
            using (var writer = new BinaryWriter(stream))
            {
                var buffer = _stream.GetBuffer();
                writer.Write(buffer, 0, buffer.Length);
            }
            
            _stream.Dispose();
            _stream = null;
        }

        public async void Start(Rectangle rect)
        {
            // intoduce a delay of one second.
            await Task.Delay(TimeSpan.FromSeconds(1));
            _snapshotManager = new SnapshotManager(rect.Left, rect.Top, rect.Width, rect.Height);
            _stream = new MemoryStream();
            _gifBitmapEncoder = new GifBitmapEncoder(_imagingFactory);
            _gifBitmapEncoder.Initialize(_stream);
            _width = rect.Width;
            _height = rect.Height;
            InitAndLaunchTimer();
        }

        public void Pause()
        {
        }
        
        private void InitAndLaunchTimer()
        {
            // initialize and launch the timer
            Delay = _interval = Convert.ToInt32((1000/MaxFramePerSecond));
            _timer = new Timer(OnTakeSnapshot, null, 0, _interval);
        }

        private void OnTakeSnapshot(object state)
        {
            HBitmap bitmap;

            try
            {
                _desktopManager.AcquireDeviceContext();
                _snapshotManager.Initialize(_desktopManager.DeviceContext);
                bitmap = _snapshotManager.TakeSnapshot();
            }
            finally
            {
                _desktopManager.RelaseDeviceContext();
            }

            AddFrame(bitmap, _interval);
        }

        private void AddFrame(HBitmap frameBitmap, int delay)
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

            //WriteApplicationMetadata(metadataWriter.NativePointer);
            var metadataQwPtr = metadataWriter.NativePointer;
            WriteMetadata(metadataQwPtr, "/appext/Application", Encoding.ASCII.GetBytes("NETSCAPE2.0"));
            WriteMetadata(metadataQwPtr, "/appext/Data", new byte[] {3, 1, 0, 0, 0});
        }

        private void WriteMetadata(IntPtr metadataWriter, string name, byte[] buffer)
        {
            // allocate unmaaged memory for the buffer
            var bufferPtr = Marshal.AllocHGlobal(buffer.Length);

            // copy content
            unsafe
            {
                for (int i = 0; i < buffer.Length; i++)
                {
                    ((byte*)bufferPtr)[i] = buffer[i];
                }
            }

            try
            {
                PROPVARIANT propVariant;
                HResult.Check(NativeMethods.InitPropVariantFromBuffer(bufferPtr, (uint) buffer.Length, out propVariant));

                using (propVariant)
                {
                    HResult.Check(WICMetadataQueryWriter.SetMetadataByName(metadataWriter, name, ref propVariant));
                }
            }
            finally
            {
                Marshal.FreeHGlobal(bufferPtr);
            }
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
                using (frame)
                {
                    FlushCurrentFrame(_gifBitmapEncoder, _imagingFactory, frame);
                }
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
                metadataWriter.SetMetadataByName("/grctlext/Delay", Convert.ToUInt16(bitmap.Delay/100));
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
                throw new ArgumentNullException("newBitmap");
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

            if (originalBitmap.PixelFormat != newBitmap.PixelFormat)
            {
                throw new InvalidOperationException("The images must have the same PixelFormat");
            }

            /* TIP: stride represents the number of bytes for a given row image. */

            const ImageLockMode COMPARISON_LOCK_MODE = ImageLockMode.ReadOnly;

            var width = originalBitmap.Width;
            var height = originalBitmap.Height;
            bool breakOuterLoop = false;
            var originalData = originalBitmap.LockBits(originalBitmap.GetBounds(), COMPARISON_LOCK_MODE, originalBitmap.PixelFormat);
            var newData = newBitmap.LockBits(newBitmap.GetBounds(), COMPARISON_LOCK_MODE, originalBitmap.PixelFormat);

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