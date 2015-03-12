using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using FastCapt.Controls.Core;

namespace FastCapt.Controls
{
    /// <summary>
    /// Interaction logic for AreaAdorner.xaml
    /// </summary>
    public partial class AreaAdorner
    {
        #region "Constructors"

        /// <summary>
        /// Initializes static members of the <see cref="AreaAdorner"/> class.
        /// </summary>
        static AreaAdorner()
        {
            EventManager.RegisterClassHandler(typeof(AreaAdorner), Thumb.DragStartedEvent, new DragStartedEventHandler(OnDragStartedEventHandler));
            EventManager.RegisterClassHandler(typeof(AreaAdorner), Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDeltaEventHandler));
            EventManager.RegisterClassHandler(typeof(AreaAdorner), Thumb.DragCompletedEvent, new DragCompletedEventHandler(OnDragCompletedEventHandler));
        }

        public AreaAdorner()
        {
            InitializeComponent();
        } 

        #endregion

        #region "Properties"

        #region RecordingRect

        /// <summary>
        /// Identifies the <see cref="RecordingRect"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RecordingRectProperty = DependencyProperty.Register(
            "RecordingRect",
            typeof(Rect),
            typeof(AreaAdorner),
            new FrameworkPropertyMetadata(Rect.Empty, FrameworkPropertyMetadataOptions.None));

        /// <summary>
        /// Gets or sets the RecordingRect property. This is a dependency property.
        /// </summary>
        /// <value>
        ///
        /// </value>
        [Bindable(true)]
        public Rect RecordingRect
        {
            get { return (Rect)GetValue(RecordingRectProperty); }
            set { SetValue(RecordingRectProperty, value); }
        }

        #endregion

        #region IsResizing

        private static readonly DependencyPropertyKey IsResizingPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsResizing",
            typeof(bool),
            typeof(AreaAdorner),
            new FrameworkPropertyMetadata(BooleanBoxes.False));

        /// <summary>
        /// Identifies the <see cref="IsResizing"/> read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty IsResizingProperty = IsResizingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsResizing property. This dependency property 
        /// indicates ....
        /// </summary>
        public bool IsResizing
        {
            get { return (bool)GetValue(IsResizingProperty); }
            private set { SetValue(IsResizingPropertyKey, BooleanBoxes.Box(value)); }
        }


        #endregion

        #endregion

        #region "Methods"

        private static void OnDragStartedEventHandler(object sender, DragStartedEventArgs e)
        {
            var view = (AreaAdorner) sender;
            var thumb = (Thumb) e.OriginalSource;
            var location = (Dock)thumb.Tag;
            if (location != Dock.None)
            {
                view.IsResizing = true;
            }
        }

        private static void OnDragCompletedEventHandler(object sender, DragCompletedEventArgs e)
        {
            var view = (AreaAdorner)sender;
            var thumb = (Thumb)e.OriginalSource;
            var location = (Dock)thumb.Tag;
            if (location != Dock.None)
            {
                view.IsResizing = false;
            }
        }

        private static void OnDragDeltaEventHandler(object sender, DragDeltaEventArgs e)
        {
            var areaAdorner = (AreaAdorner) sender;
            var thumb = (Thumb)e.OriginalSource;
            var hwndSource = PresentationSource.FromDependencyObject(thumb) as HwndSource;
            if (hwndSource == null)
            {
                return;
            }
            
            var position = (Dock)thumb.Tag;
            var horizontalChange = e.HorizontalChange;
            var verticalChange = e.VerticalChange;

            areaAdorner.MoveRecordingArea(horizontalChange, verticalChange, position);
        }

        private void MoveRecordingArea(double horizontalChange, double verticalChange, Dock direction)
        {
            var rect = RecordingRect;
            bool handled = false;
            
            if ((Dock.Left & direction) == Dock.Left)
            {
                handled = true;
                rect.Width += horizontalChange*-1;
                rect.X += horizontalChange;

                Width += horizontalChange*-1;

                var left = Canvas.GetLeft(this);
                Canvas.SetLeft(this, left + horizontalChange);
            }

            if ((Dock.Right & direction) == Dock.Right)
            {
                handled = true;
                rect.Width += horizontalChange;
                Width += horizontalChange;
            }

            if ((Dock.Bottom & direction) == Dock.Bottom)
            {
                handled = true;
                rect.Height += verticalChange;
                Height += verticalChange;
            }

            if ((Dock.Top & direction) == Dock.Top)
            {
                handled = true;
                rect.Height += verticalChange * -1;
                rect.Y += verticalChange;

                Height += verticalChange * -1;
                var top = Canvas.GetTop(this);
                Canvas.SetTop(this, top + verticalChange);
            }

            if (!handled)
            {
                // ensure the area adorner is correctly placed in the monitor boundary.
                var mousePos = Mouse.GetPosition(null);
                var currentScreen = Screen.FromPoint(new System.Drawing.Point((int)mousePos.X, (int)mousePos.Y));
                var screenBounds = currentScreen.Bounds;

                var top = Canvas.GetTop(this);
                var newTop = top+verticalChange;

                if (newTop < screenBounds.Top)
                    newTop = screenBounds.Top;
                else if ((newTop + rect.Height) > screenBounds.Bottom)
                    newTop = screenBounds.Bottom - rect.Height;
                Canvas.SetTop(this, newTop);

                // check the left side of the rect
                var left = Canvas.GetLeft(this);
                var newLeft = left + horizontalChange;
                if (newLeft < screenBounds.Left)
                    newLeft = screenBounds.Left;
                else if ((newLeft + rect.Width) > screenBounds.Right)
                    newLeft = screenBounds.Right - rect.Width;
                Canvas.SetLeft(this, newLeft);

                // check the right edge of the screen
                var newX = rect.X + horizontalChange;
                if (newX < screenBounds.Left)
                    newX = screenBounds.Left;
                else if ((newX + rect.Width) > screenBounds.Right)
                    newX = screenBounds.Right - rect.Width;
                rect.X = newX;

                // check the bottom of the screen.
                var newY = rect.Y + verticalChange;
                if (newY < screenBounds.Top)
                    newY = screenBounds.Top;
                else if ((newY + rect.Height) > screenBounds.Height)
                    newY = screenBounds.Height - rect.Height;
                rect.Y = newY;
            }

            RecordingRect = rect;
        }

        #endregion
    }
}
