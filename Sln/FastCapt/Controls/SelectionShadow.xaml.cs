using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using FastCapt.Controls.Core;
using FastCapt.Interop;

namespace FastCapt.Controls
{
    /// <summary>
    /// Interaction logic for SelectionShadow.xaml
    /// </summary>
    public partial class SelectionShadow
    {
        #region "Fields"

        private Point _anchorPoint; 

        #endregion

        #region "Constructors"

        public SelectionShadow()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                this.ShadowGeometry.Rect = new Rect(this.RenderSize);
            };
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
            typeof(SelectionShadow),
            new FrameworkPropertyMetadata(Rect.Empty,
                FrameworkPropertyMetadataOptions.None,
                (o, args) =>
                {
                    var control = (SelectionShadow)o;
                    control.OnRecordingRectChanged(o, args);
                }));

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

        #region IsSelecting

        private static readonly DependencyPropertyKey IsSelectingPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsSelecting",
            typeof(bool),
            typeof(SelectionShadow),
            new FrameworkPropertyMetadata(BooleanBoxes.False,
                FrameworkPropertyMetadataOptions.None,
                (o, args) =>
                {
                    var control = (SelectionShadow)o;
                    var isSelecting = (bool)args.NewValue;
                    if (!isSelecting)
                    {
                        var isSelected = control.RecordingRect != Rect.Empty;
                        control.IsSelected = isSelected;

                        if (isSelected)
                        {
                            var hwndSource = PresentationSource.FromDependencyObject(control) as HwndSource;
                            if (hwndSource != null)
                            {
                                control.DisableShadowWindow(hwndSource.Handle);
                            }
                        }
                    }
                }));

        private void DisableShadowWindow(IntPtr hwnd)
        {
            int currentExStyle = Win32.GetWindowLong(hwnd, GWL.EXSTYLE);
            currentExStyle |= (int)(WS_EX.LAYERED | WS_EX.TRANSPARENT);
            Win32.SetWindowLong(hwnd, GWL.EXSTYLE, currentExStyle);
        }

        /// <summary>
        /// Identifies the <see cref="IsSelecting"/> read-only dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectingProperty = IsSelectingPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets a value that indicates if the user is defining the recording area,
        /// either by dragging and dropping the mouse, or by using the various resize thumbs.
        /// </summary>
        public bool IsSelecting
        {
            get { return (bool)GetValue(IsSelectingProperty); }
            private set { SetValue(IsSelectingPropertyKey, BooleanBoxes.Box(value)); }
        }

        #endregion

        #region IsSelected

        private static readonly DependencyPropertyKey IsSelectedPropertyKey = DependencyProperty.RegisterReadOnly(
            "IsSelected",
            typeof(bool),
            typeof(SelectionShadow),
            new FrameworkPropertyMetadata(
                BooleanBoxes.False,
                FrameworkPropertyMetadataOptions.None,
                (o, args) =>
                {
                    var control = (SelectionShadow)o;
                    control.RaiseAreaSelectedEvent();
                }));

        public static readonly DependencyProperty IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;

        /// <summary>
        /// Gets the IsSelected property. This dependency property 
        /// indicates ...
        /// </summary>
        public bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            private set { SetValue(IsSelectedPropertyKey, BooleanBoxes.Box(value)); }
        }

        #endregion

        #endregion

        #region "Events"

        #region AreaSelected

        /// <summary>
        /// AreaSelected Routed Event
        /// </summary>
        public static readonly RoutedEvent AreaSelectedEvent = EventManager.RegisterRoutedEvent("AreaSelected",
            RoutingStrategy.Direct, typeof(RoutedEventHandler), typeof(SelectionShadow));

        /// <summary>
        /// Occurs when ...
        /// </summary>
        public event RoutedEventHandler AreaSelected
        {
            add { AddHandler(AreaSelectedEvent, value); }
            remove { RemoveHandler(AreaSelectedEvent, value); }
        }

        /// <summary>
        /// A helper method to raise the AreaSelected event.
        /// </summary>
        protected RoutedEventArgs RaiseAreaSelectedEvent()
        {
            return RaiseAreaSelectedEvent(this);
        }

        /// <summary>
        /// A static helper method to raise the AreaSelected event on a target element.
        /// </summary>
        /// <param name="target">UIElement or ContentElement on which to raise the event</param>
        internal static RoutedEventArgs RaiseAreaSelectedEvent(DependencyObject target)
        {
            if (target == null) return null;

            var args = new RoutedEventArgs
            {
                RoutedEvent = AreaSelectedEvent
            };
            RoutedEventHelper.RaiseEvent(target, args);
            return args;
        }

        #endregion

        #endregion

        #region "Methods"

        private void OnRecordingRectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (this.AreaGeometry == null)
                return;

            var newRect = (Rect)e.NewValue;
            this.AreaGeometry.Rect = newRect;
            InvalidateAreaAdornerPlacement();
        }

        private void InvalidateAreaAdornerPlacement()
        {
            Canvas.SetTop(this.AreaAdorner, RecordingRect.Top);
            Canvas.SetLeft(this.AreaAdorner, RecordingRect.Left);
            this.AreaAdorner.Width = RecordingRect.Width;
            this.AreaAdorner.Height = RecordingRect.Height;
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            _anchorPoint = e.GetPosition(this);
            IsSelecting = true;
            base.OnMouseLeftButtonDown(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var newAnchorPoint = e.GetPosition(this);
                RecordingRect = new Rect(_anchorPoint, newAnchorPoint);
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);
            IsSelecting = false;
        }

        #endregion
    }
}
