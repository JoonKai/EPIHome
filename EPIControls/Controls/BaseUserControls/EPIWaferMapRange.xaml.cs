using EPI.EPIColors;
using EPI.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace EPIControls.Controls.BaseUserControls
{
    public partial class EPIWaferMapRange : UserControl
    {
        #region /////디펜던시프로퍼티
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty RangeStartProperty = DependencyProperty.Register(nameof(RangeStart), typeof(double), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty RangeEndProperty = DependencyProperty.Register(nameof(RangeEnd), typeof(double), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty DisplayFormatProperty = DependencyProperty.Register(nameof(DisplayFormat), typeof(string), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata("Auto", FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ColorBarWidthProperty = DependencyProperty.Register(nameof(ColorBarWidth), typeof(int), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(100, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty TickCountProperty = DependencyProperty.Register(nameof(TickCount), typeof(int), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(16, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty BarWidthProperty = DependencyProperty.Register(nameof(BarWidth), typeof(double), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(150.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty UseOverlappingProperty = DependencyProperty.Register(nameof(UseOverlapping), typeof(bool), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty UseLogScaleBarProperty = DependencyProperty.Register(nameof(UseLogScaleBar), typeof(bool), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty UseAutoRangeProperty = DependencyProperty.Register(nameof(UseAutoRange), typeof(bool), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty UseDistributionProperty = DependencyProperty.Register(nameof(UseDistribution), typeof(bool), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty IgnoreOutOfRangeProperty = DependencyProperty.Register(nameof(IgnoreOutOfRange), typeof(bool), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ColorSetProperty = DependencyProperty.Register(nameof(ColorSet), typeof(IList<Color>), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty DistributionsProperty = DependencyProperty.Register(nameof(Distributions), typeof(IList<long>), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty UseBlackBarBorderProperty = DependencyProperty.Register(nameof(UseBlackBarBorder), typeof(bool), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty LabelFontSizeProperty = DependencyProperty.Register(nameof(LabelFontSize), typeof(double), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(10.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty LabelReservedWidthProperty = DependencyProperty.Register(nameof(LabelReservedWidth), typeof(double), typeof(EPIWaferMapRange), new FrameworkPropertyMetadata(60.0, FrameworkPropertyMetadataOptions.AffectsRender));


        public double LabelReservedWidth
        {
            get => (double)GetValue(LabelReservedWidthProperty);
            set => SetValue(LabelReservedWidthProperty, value);
        }

        public double LabelFontSize
        {
            get => (double)GetValue(LabelFontSizeProperty);
            set => SetValue(LabelFontSizeProperty, value);
        }

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public double RangeStart
        {
            get => (double)GetValue(RangeStartProperty);
            set => SetValue(RangeStartProperty, value);
        }

        public double RangeEnd
        {
            get => (double)GetValue(RangeEndProperty);
            set => SetValue(RangeEndProperty, value);
        }

        public string DisplayFormat
        {
            get => (string)GetValue(DisplayFormatProperty);
            set => SetValue(DisplayFormatProperty, value);
        }

        public int ColorBarWidth
        {
            get => (int)GetValue(ColorBarWidthProperty);
            set => SetValue(ColorBarWidthProperty, value);
        }

        public int TickCount
        {
            get => (int)GetValue(TickCountProperty);
            set => SetValue(TickCountProperty, value);
        }

        public double BarWidth
        {
            get => (double)GetValue(BarWidthProperty);
            set => SetValue(BarWidthProperty, value);
        }

        public bool UseOverlapping
        {
            get => (bool)GetValue(UseOverlappingProperty);
            set => SetValue(UseOverlappingProperty, value);
        }

        public bool UseLogScaleBar
        {
            get => (bool)GetValue(UseLogScaleBarProperty);
            set => SetValue(UseLogScaleBarProperty, value);
        }

        public bool UseAutoRange
        {
            get => (bool)GetValue(UseAutoRangeProperty);
            set => SetValue(UseAutoRangeProperty, value);
        }

        public bool UseDistribution
        {
            get => (bool)GetValue(UseDistributionProperty);
            set => SetValue(UseDistributionProperty, value);
        }

        public bool IgnoreOutOfRange
        {
            get => (bool)GetValue(IgnoreOutOfRangeProperty);
            set => SetValue(IgnoreOutOfRangeProperty, value);
        }

        public IList<Color> ColorSet
        {
            get => (IList<Color>)(GetValue(ColorSetProperty) ?? Array.Empty<Color>());
            set => SetValue(ColorSetProperty, value);
        }

        public IList<long> Distributions
        {
            get => (IList<long>)(GetValue(DistributionsProperty) ?? Array.Empty<long>());
            set => SetValue(DistributionsProperty, value);
        }

        public bool UseBlackBarBorder
        {
            get => (bool)GetValue(UseBlackBarBorderProperty);
            set => SetValue(UseBlackBarBorderProperty, value);
        }
        #endregion

        #region /////멤버변수
        // ✅ 패딩 상수 — 필요에 따라 조절
        private const double RenderPaddingTop = 4.0;
        private const double RenderPaddingBottom = 4.0;
        private const double RenderPaddingLeft = 4.0;
        private const double RenderPaddingRight = 4.0;
        private const double DragThreshold = 3.0;
        private double _previousMovingDelta = double.MinValue;
        private bool _isDragging;
        private Point _dragStartPoint;
        private double _dragOriginalStart;
        private double _dragOriginalEnd;
        private object _lastTooltipedCell;
        private List<Tuple<Rect, long>> _lastRangeBarCellPositions = new List<Tuple<Rect, long>>();
        public event EventHandler RangeChanged;
        #endregion

        #region /////생성자

        public EPIWaferMapRange()
        {
            InitializeComponent();
            RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);

            var converter = new ColorSets();
            ColorSet = converter.GetColors().ToList();
            PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            PreviewMouseLeftButtonUp += OnMouseLeftButtonUp;
            PreviewMouseDoubleClick += OnMouseDoubleClicked;
        }
        #endregion

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);


            if (double.IsNaN(ActualWidth) || double.IsNaN(ActualHeight) ||
                ActualWidth <= 0.0 || ActualHeight <= 0.0)
                return;

            var colors = ColorSet?.ToList() ?? new List<Color>();
            if (colors.Count == 0)
            {
                var converter = new ColorSets();
                colors = converter.GetColors().ToList();
            }
            if (colors.Count == 0) return;

            DrawColorBar(dc, colors);
            DrawNumberBar(dc, colors);
        }
        private (double barLeft, double barWidth, double barTop, double barHeight) CalcLayout(List<Color> colors)
        {
            var gutter = 4.0;
            var tickLen = 6.0;
            var tickGap = 4.0;

            var totalTick = tickLen + tickGap;

            // ✅ 좌우 패딩 반영
            var usableWidth = Math.Max(1.0, ActualWidth - totalTick - gutter - RenderPaddingLeft - RenderPaddingRight);
            var labelWidth = usableWidth * 0.20;
            var barWidth = usableWidth * 0.80;
            var barLeft = RenderPaddingLeft + labelWidth + totalTick + gutter;

            // ✅ 상하 패딩 반영
            var barTop = RenderPaddingTop;
            var barHeight = Math.Max(1.0, ActualHeight - RenderPaddingTop - RenderPaddingBottom);

            return (barLeft, barWidth, barTop, barHeight);
        }
        private void DrawTitle(DrawingContext dc)
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            var formatted = new FormattedText(
                Title,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(FontFamily, FontStyle, FontWeights.SemiBold, FontStretch),
                12.0,
                Brushes.White,
                dpi.PixelsPerDip);

            dc.DrawText(formatted, new Point((ActualWidth - formatted.Width) / 2.0, 2.0));
        }

        // ✅ barLeft 계산을 하나의 메서드로 통일
        private double GetFixedBarLeft()
        {
            var gutter = 4.0;
            return LabelReservedWidth + gutter;
        }

        private void DrawColorBar(DrawingContext dc, List<Color> colors)
        {
            var (barLeft, barWidth, barTop, barHeight) = CalcLayout(colors);

            var height = barHeight / colors.Count;
            if (height <= 1.0) return;

            var background = Background;
            if (background == null)
            {
                var parent = FindParent<Control>(this);
                if (parent != null)
                    background = parent.Background;
            }

            var dist = Distributions?.ToList() ?? new List<long>();
            var useDist = UseDistribution && dist.Count == colors.Count;
            var max = useDist ? dist.Max() : 0;
            var logMax = max > 0 ? Math.Log10(Math.Max(1, max)) : 0.0;

            var cellPositions = new List<Tuple<Rect, long>>();

            for (var i = 0; i < colors.Count; i++)
            {
                // ✅ y 시작점에 barTop 반영
                var y = barTop + height * i;
                var rect = new Rect(0.0, y, ActualWidth, height);

                if (background != null)
                    dc.DrawRectangle(background, null, rect);

                if (useDist)
                {
                    var width = max > 0 ? barWidth * dist[i] / max : 0.0;
                    if (UseLogScaleBar && logMax > 0.0)
                        width = barWidth * Math.Log10(Math.Max(1, dist[i])) / logMax;

                    cellPositions.Add(Tuple.Create(rect, dist[i]));

                    dc.DrawRectangle(Brushes.White, null,
                        new Rect(barLeft, y, barWidth, height - 1.0));
                    dc.DrawRectangle(new SolidColorBrush(colors[i]), null,
                        new Rect(barLeft, y, width, height - 1.0));
                }
                else
                {
                    dc.DrawRectangle(new SolidColorBrush(colors[i]), null,
                        new Rect(barLeft, y, barWidth, height - 1.0));
                }
            }

            var borderBrush = UseBlackBarBorder ? Brushes.Black : Brushes.DarkGray;
            // ✅ 보더도 barTop ~ barHeight 기준
            dc.DrawRectangle(null, new Pen(borderBrush, 1.0),
                new Rect(barLeft, barTop, barWidth, barHeight));

            _lastRangeBarCellPositions = cellPositions;
        }

        private void DrawNumberBar(DrawingContext dc, List<Color> colors)
        {
            var delta = RangeEnd - RangeStart;
            if (delta <= 0.0) return;

            var (barLeft, barWidth, barTop, barHeight) = CalcLayout(colors);

            var height = barHeight / colors.Count;
            if (height <= 1.0) return;

            var tickLength = 6.0;
            var tickGap = 4.0;
            var tickX2 = barLeft;
            var tickX1 = Math.Max(0.0, tickX2 - tickLength);
            var textRight = Math.Max(0.0, tickX1 - tickGap);

            var step = delta / colors.Count;
            var dpi = VisualTreeHelper.GetDpi(this);

            double lastBottom = 0.0;
            for (var i = 0; i < colors.Count; i++)
            {
                var value = RangeStart + step * (i + 0.5);
                var formatted = new FormattedText(
                    FormatValue(value),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(FontFamily, FontStyle, FontWeights.Normal, FontStretch),
                    LabelFontSize,  // ✅ 10.0 → LabelFontSize
                    Brushes.White,
                    dpi.PixelsPerDip);

                // ✅ y 시작점에 barTop 반영
                var y = barTop + height * i + (height - formatted.Height) / 2.0;
                var x = Math.Max(RenderPaddingLeft, textRight - formatted.Width);

                if (y >= lastBottom && y + formatted.Height <= ActualHeight - RenderPaddingBottom)
                {
                    lastBottom = y + formatted.Height;
                    dc.DrawLine(new Pen(Brushes.Gray, 1.0),
                        new Point(tickX1, y + formatted.Height / 2.0),
                        new Point(tickX2, y + formatted.Height / 2.0));
                    dc.DrawText(formatted, new Point(x, y));
                }
            }
        }

        private double MeasureLabelColumn(DrawingContext dc, List<Color> colors)
        {
            var delta = RangeEnd - RangeStart;
            if (delta <= 0.0 || colors.Count == 0)
            {
                return 0.0;
            }

            var dpi = VisualTreeHelper.GetDpi(this);
            var step = delta / colors.Count;
            double maxWidth = 0.0;

            for (var i = 0; i < colors.Count; i++)
            {
                var value = RangeStart + step * (i + 0.5);
                var formatted = new FormattedText(
                    FormatValue(value),
                    CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(FontFamily, FontStyle, FontWeights.Normal, FontStretch),
                    LabelFontSize,  // ✅ 10.0 → LabelFontSize
                    Brushes.White,
                    dpi.PixelsPerDip);

                if (formatted.Width > maxWidth)
                {
                    maxWidth = formatted.Width;
                }
            }

            return Math.Min(ActualWidth * 0.6, maxWidth);
        }

        private void UserControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            var delta = (RangeEnd - RangeStart) * Math.Sign(e.Delta) / 20.0;
            UseAutoRange = false;
            RangeStart += delta;
            RangeEnd -= delta;
            RaiseRangeChanged();
            e.Handled = true;
            InvalidateVisual();
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!TryGetBarGeometry(out var barTop, out var barBottom))
            {
                return;
            }

            var pt = e.GetPosition(this);
            if (pt.Y < barTop || pt.Y > barBottom)
            {
                return;
            }

            _isDragging = true;
            _dragStartPoint = pt;
            _dragOriginalStart = RangeStart;
            _dragOriginalEnd = RangeEnd;
            _previousMovingDelta = double.MinValue;
            CaptureMouse();
            e.Handled = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging)
            {
                return;
            }

            _isDragging = false;
            ReleaseMouseCapture();
            e.Handled = true;
        }

        private void OnMouseDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            mnuSetRange_Click(sender, new RoutedEventArgs());
        }

        private void UserControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (_isDragging)
            {
                OnDragMoving(e.GetPosition(this));
                return;
            }

            if (_lastRangeBarCellPositions == null || _lastRangeBarCellPositions.Count == 0)
            {
                return;
            }

            var pt = e.GetPosition(this);
            var index = _lastRangeBarCellPositions.FindIndex(x => x.Item1.Contains(pt));
            if (index == -1)
            {
                return;
            }

            var cell = _lastRangeBarCellPositions[index];
            if (ReferenceEquals(_lastTooltipedCell, cell))
            {
                return;
            }

            var range = GetHistogramRange(index, _lastRangeBarCellPositions.Count);
            if (range == null)
            {
                return;
            }

            _lastTooltipedCell = cell;
            var tooltip = new ToolTip
            {
                Content = string.Format(
                    CultureInfo.CurrentCulture,
                    "Range: {0} ~ {1}\r\nCount: {2:#,##0}",
                    FormatValue(range.Value.Begin),
                    FormatValue(range.Value.End),
                    cell.Item2)
            };

            ToolTip = null;
            ToolTip = tooltip;
        }

        private void OnDragMoving(Point delta)
        {
            if (ActualHeight <= 0.0)
            {
                return;
            }

            var dy = delta.Y - _dragStartPoint.Y;
            if (Math.Abs(dy - _previousMovingDelta) < DragThreshold)
            {
                return;
            }

            _previousMovingDelta = dy;
            var ratio = -dy / ActualHeight;
            var span = _dragOriginalEnd - _dragOriginalStart;
            UseAutoRange = false;
            RangeStart = _dragOriginalStart + span * ratio;
            RangeEnd = _dragOriginalEnd + span * ratio;
            RaiseRangeChanged();
            InvalidateVisual();
        }

        private bool TryGetBarGeometry(out double barTop, out double barBottom)
        {
            barTop = 0.0;
            barBottom = 0.0;
            if (ActualHeight <= 0.0)
            {
                return false;
            }

            var titleHeight = string.IsNullOrWhiteSpace(Title) ? 0.0 : 16.0;
            barTop = titleHeight;
            barBottom = ActualHeight;
            return barBottom > barTop;
        }

        private (double Begin, double End)? GetHistogramRange(int index, int count)
        {
            if (count <= 0)
            {
                return null;
            }

            var delta = RangeEnd - RangeStart;
            if (delta <= 0.0)
            {
                return null;
            }

            var step = delta / count;
            var begin = RangeStart + step * index;
            var end = begin + step;
            return (begin, end);
        }

        private string FormatValue(double value)
        {
            if (string.IsNullOrWhiteSpace(DisplayFormat) ||
                DisplayFormat.Equals("Auto", StringComparison.OrdinalIgnoreCase))
            {
                return value.ToString("0.###", CultureInfo.CurrentCulture);
            }

            try
            {
                return value.ToString(DisplayFormat, CultureInfo.CurrentCulture);
            }
            catch
            {
                return value.ToString("0.###", CultureInfo.CurrentCulture);
            }
        }

        private void RaiseRangeChanged()
        {
            RangeChanged?.Invoke(this, EventArgs.Empty);
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            if (chkAutoRange != null)
            {
                chkAutoRange.IsChecked = UseAutoRange;
            }

            if (chkUseOverlap != null)
            {
                chkUseOverlap.IsChecked = UseOverlapping;
            }

            if (chkUseLogScaleBar != null)
            {
                chkUseLogScaleBar.IsChecked = UseLogScaleBar;
            }

            if (chkUseIgnoreOutOfRange != null)
            {
                chkUseIgnoreOutOfRange.IsChecked = IgnoreOutOfRange;
            }
        }

        private void chkAutoRange_Click(object sender, RoutedEventArgs e)
        {
            if (chkAutoRange != null)
            {
                UseAutoRange = chkAutoRange.IsChecked;
                RaiseRangeChanged();
            }
        }

        private void chkEnableDistribution_Click(object sender, RoutedEventArgs e)
        {
            UseDistribution = true;
            RaiseRangeChanged();
            InvalidateVisual();
        }

        private void chkDisableDistribution_Click(object sender, RoutedEventArgs e)
        {
            UseDistribution = false;
            InvalidateVisual();
        }

        private void chkUseOverlap_Click(object sender, RoutedEventArgs e)
        {
            if (chkUseOverlap != null)
            {
                UseOverlapping = chkUseOverlap.IsChecked;
                InvalidateVisual();
            }
        }

        private void chkUseLogScaleBar_Clicked(object sender, RoutedEventArgs e)
        {
            if (chkUseLogScaleBar != null)
            {
                UseLogScaleBar = chkUseLogScaleBar.IsChecked;
                InvalidateVisual();
            }
        }

        private void chkUseIgnoreOutOfRange_Click(object sender, RoutedEventArgs e)
        {
            if (chkUseIgnoreOutOfRange != null)
            {
                IgnoreOutOfRange = chkUseIgnoreOutOfRange.IsChecked;
                RaiseRangeChanged();
            }
        }

        private void mnuSetRange_Click(object sender, RoutedEventArgs e)
        {
            var result = ShowRangeDialog(RangeStart, RangeEnd);
            if (!result.HasValue)
            {
                return;
            }

            UseAutoRange = false;
            RangeStart = result.Value.Start;
            RangeEnd = result.Value.End;
            RaiseRangeChanged();
            InvalidateVisual();
        }

        private void SelectGrayColor_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string>
            {
                "FFFFFF","EEEEEE","DDDDDD","CCCCCC","BBBBBB","AAAAAA","999999","888888",
                "777777","666666","555555","444444","333333","222222","111111","000000"
            });
        }

        private void SelectBlueColor_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string>
            {
                "DEFFFF","CDEFFF","BCDEFF","ABCDEF","9ABCDE","89ABCD","789ABC",
                "6789AB","56789A","456789","345678","234567","123456"
            });
        }

        private void SelectComplementaryColor1_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string> { Colors.Blue.ToRRGGBBText(), Colors.Orange.ToRRGGBBText() });
        }

        private void SelectComplementaryColor2_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string> { "660099", Colors.Yellow.ToRRGGBBText() });
        }

        private void SelectComplementaryColor3_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string> { Colors.Green.ToRRGGBBText(), Colors.Red.ToRRGGBBText() });
        }

        private void SelectPalette1Color_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string>
            {
                "FF00FF","EF00FF","CE00FF","AD00FF","8C04FF","6B00FF","5200FF","2900E7",
                "0000FF","0028E7","0041BD","005194","006573","007D52","008E29","00A210",
                "00B600","00D300","00EB00","00FF08","39FF00","6BFF00","A5FF00","CEFF00",
                "FFFF00","FFDB00","FFC300","FF9600","FF8200","FF5500","FF4100","FF0000"
            });
        }

        private void SelectPalette2Color_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string>
            {
                "000080","010F84","031F88","05308C","074190","095294","0B6498","0E779C",
                "108AA1","139DA5","15A9A1","1BB189","21B971","27C158","2BC54B","32CD32",
                "41D030","50D32E","61D52B","73D829","85DB27","99DD24","AEE022","C4E21F",
                "DBE51D","E8DD1A","EAC918","EDB415","F09E12","F2870F","F56E0C","F85409",
                "FA3906","FD1D03","FF0000","F60606","E11313","CD1D1D","B92424","A52A2A"
            });
        }

        private void SelectPalette3Color_Click(object sender, RoutedEventArgs e)
        {
            UpdateColors(new List<string>
            {
                "C01ACE","9B19E8","6019E6","221AD2","1A45D3","1A7AD3","1AB0D4","1AD5C2",
                "1AD58B","1AD754","1AD81C","4BDA1A","84DC1A","BEDD19","E1C61A","E58E1A",
                "ED561A","F71A1A"
            });
        }

        private void SelectCustomPalette_Click(object sender, RoutedEventArgs e)
        {
            var text = ShowTextInput("Custom Color Set", string.Join(Environment.NewLine, ColorSet.Select(x => x.ToRRGGBBText())));
            if (string.IsNullOrWhiteSpace(text))
            {
                return;
            }

            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();
            UpdateColors(lines);
        }

        private void OnDataFormat_Default_Clicked(object sender, RoutedEventArgs e)
        {
            DisplayFormat = "Auto";
            InvalidateVisual();
        }

        private void OnDataFormat_Clicked(object sender, RoutedEventArgs e)
        {
            var text = ShowTextInput("Display Format", DisplayFormat);
            if (!string.IsNullOrWhiteSpace(text))
            {
                DisplayFormat = text;
                InvalidateVisual();
            }
        }

        private void UpdateColors(List<string> textColors)
        {
            try
            {
                ColorSet = textColors.Select(x => x.ToColor()).ToList();
                RaiseRangeChanged();
                InvalidateVisual();
            }
            catch
            {
                // ignore invalid colors
            }
        }

        private static (double Start, double End)? ShowRangeDialog(double initialStart, double initialEnd, Window owner = null)
        {
            var dialog = new Window
            {
                Title = "Range Input Dialog",
                Width = 260,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = owner
            };

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(80) });
            grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

            var startLabel = new TextBlock { Text = "Range Start", VerticalAlignment = VerticalAlignment.Center };
            var endLabel = new TextBlock { Text = "Range End", VerticalAlignment = VerticalAlignment.Center };

            var startBox = new TextBox { Text = initialStart.ToString("0.###############", CultureInfo.CurrentCulture) };
            var endBox = new TextBox { Text = initialEnd.ToString("0.###############", CultureInfo.CurrentCulture) };

            Grid.SetRow(startLabel, 0); Grid.SetColumn(startLabel, 0);
            Grid.SetRow(startBox, 0); Grid.SetColumn(startBox, 1);

            Grid.SetRow(endLabel, 1); Grid.SetColumn(endLabel, 0);
            Grid.SetRow(endBox, 1); Grid.SetColumn(endBox, 1);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new Button
            {
                Content = "Apply",
                Width = 70,
                Margin = new Thickness(0, 8, 6, 0),
                IsDefault = true // Enter
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 70,
                Margin = new Thickness(0, 8, 0, 0),
                IsCancel = true // Esc
            };

            okButton.Click += (sender, e) => dialog.DialogResult = true;
            cancelButton.Click += (sender, e) => dialog.DialogResult = false;

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(buttonPanel, 2);
            Grid.SetColumnSpan(buttonPanel, 2);

            grid.Children.Add(startLabel);
            grid.Children.Add(startBox);
            grid.Children.Add(endLabel);
            grid.Children.Add(endBox);
            grid.Children.Add(buttonPanel);

            dialog.Content = grid;

            dialog.Loaded += (sender, e) =>
            {
                startBox.Focus();
                startBox.SelectAll();
            };

            if (dialog.ShowDialog() != true)
                return null;

            double s, en;
            if (double.TryParse(startBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out s) &&
                double.TryParse(endBox.Text, NumberStyles.Float, CultureInfo.CurrentCulture, out en))
            {
                return (s, en);
            }

            return null;
        }
        private static string ShowTextInput(string title, string initial, Window owner = null)
        {
            var dialog = new Window
            {
                Title = title,
                Width = 320,
                Height = 240,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize,
                Owner = owner
            };

            var grid = new Grid { Margin = new Thickness(10) };
            grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var textBox = new TextBox
            {
                Text = initial ?? string.Empty,
                AcceptsReturn = true,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            Grid.SetRow(textBox, 0);

            var buttonPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            var okButton = new Button
            {
                Content = "Apply",
                Width = 70,
                Margin = new Thickness(0, 8, 6, 0),
                IsDefault = true   // Enter
            };

            var cancelButton = new Button
            {
                Content = "Cancel",
                Width = 70,
                Margin = new Thickness(0, 8, 0, 0),
                IsCancel = true    // Esc
            };

            okButton.Click += (sender, e) => dialog.DialogResult = true;
            cancelButton.Click += (sender, e) => dialog.DialogResult = false;

            buttonPanel.Children.Add(okButton);
            buttonPanel.Children.Add(cancelButton);

            Grid.SetRow(buttonPanel, 1);

            grid.Children.Add(textBox);
            grid.Children.Add(buttonPanel);

            dialog.Content = grid;

            dialog.Loaded += (sender, e) =>
            {
                textBox.Focus();
                textBox.CaretIndex = textBox.Text.Length;
            };

            var result = dialog.ShowDialog();
            return result == true ? textBox.Text : null;
        }


        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            if (child == null)
                return null;

            DependencyObject parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                var typed = parent as T;
                if (typed != null)
                    return typed;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }
    }
}