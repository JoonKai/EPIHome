using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace EPIControls.Controls.BaseUserControls
{
    public partial class EPIWaferMap : UserControl
    {
        #region /////디펜던시 프로퍼티
        public static readonly DependencyProperty WaferSizeProperty = DependencyProperty.Register(nameof(WaferSize), typeof(double), typeof(EPIWaferMap), new FrameworkPropertyMetadata(100.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty EdgeExclusionProperty = DependencyProperty.Register(nameof(EdgeExclusion), typeof(double), typeof(EPIWaferMap), new FrameworkPropertyMetadata(4.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty CellWidthProperty = DependencyProperty.Register(nameof(CellWidth), typeof(double), typeof(EPIWaferMap), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty CellHeightProperty = DependencyProperty.Register(nameof(CellHeight), typeof(double), typeof(EPIWaferMap), new FrameworkPropertyMetadata(2.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty HorizontalLineThicknessProperty = DependencyProperty.Register(nameof(HorizontalLineThickness), typeof(double), typeof(EPIWaferMap), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty VerticalLineThicknessProperty = DependencyProperty.Register(nameof(VerticalLineThickness), typeof(double), typeof(EPIWaferMap), new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty WaferFillProperty = DependencyProperty.Register(nameof(WaferFill), typeof(Brush), typeof(EPIWaferMap), new FrameworkPropertyMetadata(Brushes.Transparent, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty WaferStrokeProperty = DependencyProperty.Register(nameof(WaferStroke), typeof(Brush), typeof(EPIWaferMap), new FrameworkPropertyMetadata(new SolidColorBrush((Color)ColorConverter.ConvertFromString("#FFF58513")), FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty GridStrokeProperty = DependencyProperty.Register(nameof(GridStroke), typeof(Brush), typeof(EPIWaferMap), new FrameworkPropertyMetadata(Brushes.Black, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty ShowGridProperty = DependencyProperty.Register(nameof(ShowGrid), typeof(bool), typeof(EPIWaferMap), new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.AffectsRender));
        public static readonly DependencyProperty MapCellsProperty = DependencyProperty.Register(nameof(MapCells), typeof(IEnumerable), typeof(EPIWaferMap), new FrameworkPropertyMetadata(null, OnMapCellsChanged));

        public double WaferSize
        {
            get { return (double)GetValue(WaferSizeProperty); }
            set { SetValue(WaferSizeProperty, value); }
        }

        public double EdgeExclusion
        {
            get { return (double)GetValue(EdgeExclusionProperty); }
            set { SetValue(EdgeExclusionProperty, value); }
        }

        public double CellWidth
        {
            get { return (double)GetValue(CellWidthProperty); }
            set { SetValue(CellWidthProperty, value); }
        }

        public double CellHeight
        {
            get { return (double)GetValue(CellHeightProperty); }
            set { SetValue(CellHeightProperty, value); }
        }

        public double HorizontalLineThickness
        {
            get { return (double)GetValue(HorizontalLineThicknessProperty); }
            set { SetValue(HorizontalLineThicknessProperty, value); }
        }

        public double VerticalLineThickness
        {
            get { return (double)GetValue(VerticalLineThicknessProperty); }
            set { SetValue(VerticalLineThicknessProperty, value); }
        }

        public Brush WaferFill
        {
            get { return (Brush)GetValue(WaferFillProperty); }
            set { SetValue(WaferFillProperty, value); }
        }

        public Brush WaferStroke
        {
            get { return (Brush)GetValue(WaferStrokeProperty); }
            set { SetValue(WaferStrokeProperty, value); }
        }

        public Brush GridStroke
        {
            get { return (Brush)GetValue(GridStrokeProperty); }
            set { SetValue(GridStrokeProperty, value); }
        }

        public bool ShowGrid
        {
            get { return (bool)GetValue(ShowGridProperty); }
            set { SetValue(ShowGridProperty, value); }
        }

        public IEnumerable MapCells
        {
            get { return (IEnumerable)GetValue(MapCellsProperty); }
            set { SetValue(MapCellsProperty, value); }
        }
        #endregion

        public event EventHandler<WaferCellClickedEventArgs> CellClicked;
        private readonly Dictionary<(int Row, int Col), WaferCell> _cellLookup = new Dictionary<(int Row, int Col), WaferCell>();
        private INotifyCollectionChanged _cellsObservable;
        private (int Row, int Col)? _hoverCell;

        public EPIWaferMap()
        {
            InitializeComponent();
            //RenderOptions.SetEdgeMode(this, EdgeMode.Aliased);
            //RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            SizeChanged += OnControlSizeChanged;
        }
        private void OnControlSizeChanged(object sender, SizeChangedEventArgs e)
        {
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            var width = ActualWidth;
            var height = ActualHeight;
            if (AnyInvalid(width, height)) return;

            if (Background != null)
                dc.DrawRectangle(Background, null, new Rect(0, 0, width, height));

            var pad = Padding;
            var renderWidth = Math.Max(0, width - pad.Left - pad.Right);
            var renderHeight = Math.Max(0, height - pad.Top - pad.Bottom);
            if (renderWidth <= 0 || renderHeight <= 0) return;

            var minSize = Math.Min(renderWidth, renderHeight);
            if (minSize <= 0.0 || WaferSize <= 0.0) return;

            var pixelsPerMm = minSize / WaferSize;
            var edgePixels = EdgeExclusion * pixelsPerMm;
            var innerDiameter = minSize - (edgePixels * 2.0);
            if (innerDiameter <= 0.0) return;

            var center = new Point(
                pad.Left + renderWidth / 2.0,
                pad.Top + renderHeight / 2.0);

            var waferRadius = minSize / 2.0;
            var innerRadius = innerDiameter / 2.0;

            var waferPen = new Pen(WaferStroke, 2.0);
            var gridPen = new Pen(GridStroke, Math.Max(0.1, (HorizontalLineThickness + VerticalLineThickness) * 0.5));

            // ✅ 순서 중요
            // 1. 그림자 먼저 (웨이퍼 뒤에)
            DrawShadow(dc, center, waferRadius);

            // 2. 웨이퍼 베이스
            dc.DrawEllipse(WaferFill, null, center, waferRadius, waferRadius);

            // ✅ 2단계 — 셀 렌더링
            var innerRect = new Rect(
                center.X - innerRadius,
                center.Y - innerRadius,
                innerDiameter, innerDiameter);

            var clip = new EllipseGeometry(center, waferRadius, waferRadius);
            dc.PushClip(clip);
            DrawCells(dc, innerRect, WaferSize, EdgeExclusion, center, innerRadius, ShowGrid, gridPen);
            DrawHoverCell(dc, innerRect, WaferSize, EdgeExclusion, center, innerRadius);
            dc.Pop();

            // ✅ 3단계 — 입체감 레이어 (셀 위에 반투명 그라디언트 오버레이)
            DrawSphereEffect(dc, center, waferRadius);

            // ✅ 4단계 — 외곽선
            dc.DrawEllipse(null, waferPen, center, waferRadius, waferRadius);
        }
        private void DrawSphereEffect(DrawingContext dc, Point center, double radius)
        {
            var rimBrush = new RadialGradientBrush();
            rimBrush.GradientOrigin = new Point(0.5, 0.5);
            rimBrush.Center = new Point(0.5, 0.5);
            rimBrush.RadiusX = 0.5;
            rimBrush.RadiusY = 0.5;
            rimBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            rimBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.0));   // 내부 투명
            rimBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.91));  // ✅ 0.82 → 0.91
            rimBrush.GradientStops.Add(new GradientStop(Color.FromArgb(20, 0, 0, 0), 0.97));  // ✅ 0.94 → 0.97
            rimBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));
            rimBrush.Freeze();
            dc.DrawEllipse(rimBrush, null, center, radius, radius);
        }
        private void DrawShadow(DrawingContext dc, Point center, double radius)
        {
            // ✅ 외부로 삐져나오지 않도록 radius 그대로 사용
            // 우하단으로 치우친 그라디언트로 그림자 방향 표현
            var shadowBrush = new RadialGradientBrush();
            shadowBrush.GradientOrigin = new Point(0.35, 0.35); // ✅ 좌상단이 밝음 (광원)
            shadowBrush.Center = new Point(0.5, 0.5);
            shadowBrush.RadiusX = 0.5;
            shadowBrush.RadiusY = 0.5;
            shadowBrush.MappingMode = BrushMappingMode.RelativeToBoundingBox;
            shadowBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.0));
            shadowBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 0.91));
            shadowBrush.GradientStops.Add(new GradientStop(Color.FromArgb(180, 0, 0, 0), 0.97)); // 우하단 어두움
            shadowBrush.GradientStops.Add(new GradientStop(Color.FromArgb(0, 0, 0, 0), 1.0));
            shadowBrush.Freeze();

            // ✅ radius 그대로 — 컨트롤 밖으로 안 나감
            dc.DrawEllipse(shadowBrush, null, center, radius, radius);
        }
        private void DrawCells(
            DrawingContext dc,
            Rect innerRect,
            double waferSizeMm,
            double edgeExclusionMm,
            Point center,
            double innerRadius,
            bool showGrid,
            Pen gridPen)
        {
            if (MapCells == null) return;

            var drawableMm = waferSizeMm - (edgeExclusionMm * 2.0);
            if (drawableMm <= 0.0) return;

            var pixelsPerMm = innerRect.Width / drawableMm;
            var cellWidthPx = Math.Max(1.0, CellWidth * pixelsPerMm);
            var cellHeightPx = Math.Max(1.0, CellHeight * pixelsPerMm);
            if (cellWidthPx <= 0.0 || cellHeightPx <= 0.0) return;

            foreach (var item in MapCells)
            {
                var cell = item as WaferCell;
                if (cell == null) continue;

                // ✅ 픽셀 경계에 스냅 — 셀이 선명하게 렌더링됨
                var rawX = center.X + (cell.Col * cellWidthPx) - (cellWidthPx / 2.0);
                var rawY = center.Y + (cell.Row * cellHeightPx) - (cellHeightPx / 2.0);
                var snappedX = Math.Floor(rawX);
                var snappedY = Math.Floor(rawY);
                var snappedW = Math.Floor(rawX + cellWidthPx) - snappedX;
                var snappedH = Math.Floor(rawY + cellHeightPx) - snappedY;

                var rect = new Rect(snappedX, snappedY, snappedW, snappedH);

                if (!IsRectInsideCircle(rect, center, innerRadius)) continue;

                if (rect.Right < innerRect.Left || rect.Left > innerRect.Right ||
                    rect.Bottom < innerRect.Top || rect.Top > innerRect.Bottom)
                    continue;

                var fill = cell.Brush ?? Brushes.Transparent;
                var pen = showGrid ? gridPen : null;
                dc.DrawRectangle(fill, pen, rect);
            }
        }

        private static bool AnyInvalid(params double[] values)
        {
            foreach (var value in values)
            {
                if (double.IsNaN(value) || double.IsInfinity(value))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool IsRectInsideCircle(Rect rect, Point center, double radius)
        {
            var r2 = radius * radius;
            var corners = new[]
            {
                new Point(rect.Left, rect.Top),
                new Point(rect.Right, rect.Top),
                new Point(rect.Left, rect.Bottom),
                new Point(rect.Right, rect.Bottom)
            };

            foreach (var corner in corners)
            {
                var dx = corner.X - center.X;
                var dy = corner.Y - center.Y;
                if ((dx * dx + dy * dy) > r2)
                {
                    return false;
                }
            }

            return true;
        }

        private void DrawHoverCell(DrawingContext dc, Rect innerRect, double waferSizeMm, double edgeExclusionMm, Point center, double innerRadius)
        {
            if (_hoverCell == null)
            {
                return;
            }

            var drawableMm = waferSizeMm - (edgeExclusionMm * 2.0);
            if (drawableMm <= 0.0)
            {
                return;
            }

            var cellWidthPx = Math.Max(1.0, CellWidth * (innerRect.Width / drawableMm));
            var cellHeightPx = Math.Max(1.0, CellHeight * (innerRect.Height / drawableMm));
            if (cellWidthPx <= 0.0 || cellHeightPx <= 0.0)
            {
                return;
            }

            var rect = new Rect(
                center.X + (_hoverCell.Value.Col * cellWidthPx) - (cellWidthPx / 2.0),
                center.Y + (_hoverCell.Value.Row * cellHeightPx) - (cellHeightPx / 2.0),
                cellWidthPx,
                cellHeightPx);

            if (!IsRectInsideCircle(rect, center, innerRadius))
            {
                return;
            }

            var pen = new Pen(Brushes.Red, 4);
            dc.DrawRectangle(null, pen, rect);
        }

        protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            var hit = HitTestCell(e.GetPosition(this));
            if (hit != null)
            {
                CellClicked?.Invoke(this, new WaferCellClickedEventArgs(hit.Value.Cell, hit.Value.Row, hit.Value.Col));
                e.Handled = true;
            }
        }

        protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            var hit = HitTestCell(e.GetPosition(this));
            var next = hit == null ? ((int Row, int Col)?)null : (hit.Value.Row, hit.Value.Col);
            if (_hoverCell != next)
            {
                _hoverCell = next;
                InvalidateVisual();
            }
        }

        protected override void OnMouseLeave(System.Windows.Input.MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (_hoverCell != null)
            {
                _hoverCell = null;
                InvalidateVisual();
            }
        }

        private (WaferCell Cell, int Row, int Col)? HitTestCell(Point point)
        {
            var width = ActualWidth;
            var height = ActualHeight;
            if (AnyInvalid(width, height)) return null;

            // ✅ Padding 반영
            var pad = Padding;
            var renderWidth = Math.Max(0, width - pad.Left - pad.Right);
            var renderHeight = Math.Max(0, height - pad.Top - pad.Bottom);

            var minSize = Math.Min(renderWidth, renderHeight);
            if (minSize <= 0.0 || WaferSize <= 0.0) return null;

            var pixelsPerMm = minSize / WaferSize;
            var edgePixels = EdgeExclusion * pixelsPerMm;
            var innerDiameter = minSize - (edgePixels * 2.0);
            if (innerDiameter <= 0.0) return null;

            // ✅ center도 Padding 반영
            var center = new Point(
                pad.Left + renderWidth / 2.0,
                pad.Top + renderHeight / 2.0);

            var outerRadius = minSize / 2.0;
            var dxOuter = point.X - center.X;
            var dyOuter = point.Y - center.Y;
            if ((dxOuter * dxOuter + dyOuter * dyOuter) > outerRadius * outerRadius) return null;

            var innerRadius = innerDiameter / 2.0;
            var dxInner = point.X - center.X;
            var dyInner = point.Y - center.Y;
            if ((dxInner * dxInner + dyInner * dyInner) > innerRadius * innerRadius) return null;

            var innerRect = new Rect(
                center.X - innerRadius,
                center.Y - innerRadius,
                innerDiameter, innerDiameter);

            var drawableMm = WaferSize - (EdgeExclusion * 2.0);
            if (drawableMm <= 0.0) return null;

            var innerPixelsPerMm = innerDiameter / drawableMm;
            var cellWidthPx = Math.Max(1.0, CellWidth * innerPixelsPerMm);
            var cellHeightPx = Math.Max(1.0, CellHeight * innerPixelsPerMm);
            if (cellWidthPx <= 0.0 || cellHeightPx <= 0.0) return null;

            var col = (int)Math.Floor(((point.X - center.X) / cellWidthPx) + 0.5);
            var row = (int)Math.Floor(((point.Y - center.Y) / cellHeightPx) + 0.5);

            if (_cellLookup.TryGetValue((row, col), out var cell))
                return (cell, row, col);

            return null;
        }

        private static void OnMapCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var viewer = d as EPIWaferMap;
            if (viewer == null)
                return;

            // 기존 구독 해제
            viewer.UnsubscribeMapCells();

            // 새 소스 구독
            var newSource = e.NewValue as IEnumerable;
            if (newSource != null)
                viewer.SubscribeMapCells(newSource);

            viewer.RebuildCellLookup();
            viewer.InvalidateVisual();
        }

        private void SubscribeMapCells(IEnumerable source)
        {
            if (source == null)
                return;

            var observable = source as INotifyCollectionChanged;
            if (observable == null)
                return;

            UnsubscribeMapCells();

            _cellsObservable = observable;
            _cellsObservable.CollectionChanged += OnMapCellsCollectionChanged; // OK
        }

        private void UnsubscribeMapCells()
        {
            if (_cellsObservable == null)
                return;

            _cellsObservable.CollectionChanged -= OnMapCellsCollectionChanged;
            _cellsObservable = null;
        }
        private void OnMapCellsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RebuildCellLookup();
            InvalidateVisual();
        }

        private void RebuildCellLookup()
        {
            _cellLookup.Clear();
            if (MapCells == null)
            {
                return;
            }

            foreach (var item in MapCells)
            {
                if (item is WaferCell cell)
                {
                    _cellLookup[(cell.Row, cell.Col)] = cell;
                }
            }
        }
    }

    public sealed class WaferCell
    {
        public WaferCell(int row, int col, Brush brush = null, double? value = null)
        {
            Row = row;
            Col = col;
            Brush = brush;
            Value = value;
        }

        public int Row { get; set; }
        public int Col { get; set; }
        public Brush Brush { get; set; }
        public double? Value { get; set; }
    }

    public sealed class WaferCellClickedEventArgs : EventArgs
    {
        public WaferCellClickedEventArgs(WaferCell cell, int row, int col)
        {
            Cell = cell;
            Row = row;
            Col = col;
        }

        public WaferCell Cell { get; }
        public int Row { get; }
        public int Col { get; }

    }
}