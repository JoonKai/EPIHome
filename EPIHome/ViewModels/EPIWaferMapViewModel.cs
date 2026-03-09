using Caliburn.Micro;
using EPI.Themes;
using EPI.Wafers;
using EPIControls.Controls.BaseUserControls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace EPIHome.ViewModels
{
    public class EPIWaferMapViewModel : Screen
    {
        private readonly WaferFileService _fileService = new WaferFileService();
        private readonly ColorSets _colorSets = new ColorSets();
        private List<WaferCellModel> _rawCells = new List<WaferCellModel>();

        // ── UI ───────────────────────────────────────
        private string _title = "PL Map Viewer";
        public string Title
        {
            get => _title;
            set { _title = value; NotifyOfPropertyChange(); }
        }

        private string _statusMessage = "파일을 열어 웨이퍼 맵을 로드하세요.";
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; NotifyOfPropertyChange(); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; NotifyOfPropertyChange(); }
        }

        // ── 웨이퍼 맵 ────────────────────────────────
        private ObservableCollection<WaferCell> _mapCells = new ObservableCollection<WaferCell>();
        public ObservableCollection<WaferCell> MapCells
        {
            get => _mapCells;
            set { _mapCells = value; NotifyOfPropertyChange(); }
        }

        private double _waferSize = 24.0;
        public double WaferSize
        {
            get => _waferSize;
            set { _waferSize = value; NotifyOfPropertyChange(); }
        }

        private double _edgeExclusion = 0.0;
        public double EdgeExclusion
        {
            get => _edgeExclusion;
            set
            {
                _edgeExclusion = value;
                NotifyOfPropertyChange();
                if (_rawCells.Count > 0)
                {
                    var tmp = new WaferDataModel { Cells = _rawCells };
                    double s = WaferFileService.EstimateStepSize(tmp);
                    WaferSize = WaferFileService.EstimateWaferSize(tmp, s, value);
                }
            }
        }

        private double _cellWidth = 1.0;
        public double CellWidth
        {
            get => _cellWidth;
            set { _cellWidth = value; NotifyOfPropertyChange(); }
        }

        private double _cellHeight = 1.0;
        public double CellHeight
        {
            get => _cellHeight;
            set { _cellHeight = value; NotifyOfPropertyChange(); }
        }

        // ── 레인지 ───────────────────────────────────
        private bool _useAutoRange = false;
        public bool UseAutoRange
        {
            get => _useAutoRange;
            set
            {
                _useAutoRange = value;
                NotifyOfPropertyChange();
                RebuildColors();
            }
        }

        private double _rangeStart = 430.0;
        public double RangeStart
        {
            get => _rangeStart;
            set
            {
                _rangeStart = value;
                NotifyOfPropertyChange();
                RebuildColors();
            }
        }

        private double _rangeEnd = 460.0;
        public double RangeEnd
        {
            get => _rangeEnd;
            set
            {
                _rangeEnd = value;
                NotifyOfPropertyChange();
                RebuildColors();
            }
        }

        // ✅ ColorSet: WaferMapRange 팔레트 교체 시 TwoWay 바인딩으로 역전달
        private IList<Color> _colorSet;
        public IList<Color> ColorSet
        {
            get => _colorSet;
            set
            {
                _colorSet = value;
                NotifyOfPropertyChange();
                if (value != null && value.Count > 0)
                    _colorSets.SetColors(value);
                RebuildColors();
            }
        }

        private ObservableCollection<long> _distributions = new ObservableCollection<long>();
        public ObservableCollection<long> Distributions
        {
            get => _distributions;
            set { _distributions = value; NotifyOfPropertyChange(); }
        }

        // ── 요약 ─────────────────────────────────────
        private string _waferId = string.Empty;
        public string WaferId
        {
            get => _waferId;
            set { _waferId = value; NotifyOfPropertyChange(); }
        }

        private string _pwStatText = string.Empty;
        public string PwStatText
        {
            get => _pwStatText;
            set { _pwStatText = value; NotifyOfPropertyChange(); }
        }

        private int _totalCells;
        public int TotalCells
        {
            get => _totalCells;
            set { _totalCells = value; NotifyOfPropertyChange(); }
        }

        // ── Commands ─────────────────────────────────
        

        public EPIWaferMapViewModel()
        {
            
        }

        // ── 파일 열기 ─────────────────────────────────
        public void OpenFile()
        {
            var dlg = new OpenFileDialog
            {
                Title = "웨이퍼 .map 파일 선택",
                Filter = "Map Files (*.map)|*.map|All Files (*.*)|*.*"
            };
            if (dlg.ShowDialog() != true) return;
            LoadMapFile(dlg.FileName);
        }

        public void LoadMapFile(string filePath)
        {
            try
            {
                IsLoading = true;
                StatusMessage = $"로딩 중: {Path.GetFileName(filePath)}";

                WaferDataModel data = _fileService.LoadFromMapFile(filePath);
                ApplyWaferData(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"파일 로드 실패:\n{ex.Message}", "오류",
                                MessageBoxButton.OK, MessageBoxImage.Error);
                StatusMessage = "로드 실패";
            }
            finally
            {
                IsLoading = false;
            }
        }

        // ── 웨이퍼 데이터 적용 ────────────────────────
        private void ApplyWaferData(WaferDataModel data)
        {
            _rawCells = data.Cells;
            WaferId = data.WaferId;
            TotalCells = data.Cells.Count;

            double step = WaferFileService.EstimateStepSize(data);
            CellWidth = step;
            CellHeight = step;

            _edgeExclusion = 0.0;
            NotifyOfPropertyChange(nameof(EdgeExclusion));
            WaferSize = WaferFileService.EstimateWaferSize(data, step, 0.0);

            if (data.Cells.Count > 0)
            {
                double mn = data.Cells.Min(c => c.PeakWavelength);
                double mx = data.Cells.Max(c => c.PeakWavelength);
                double avg = data.Cells.Average(c => c.PeakWavelength);
                double std = Math.Sqrt(data.Cells.Average(c =>
                    Math.Pow(c.PeakWavelength - avg, 2)));
                PwStatText = $"PW: {avg:F3} ± {std:F3} nm  [{mn:F3} ~ {mx:F3}]";

                double newStart = Math.Floor(mn * 10.0) / 10.0;
                double newEnd = Math.Ceiling(mx * 10.0) / 10.0;

                // 셀 먼저 생성
                RebuildCells(newStart, newEnd);

                // Dispatcher 지연: Control Loaded 후 DP 전달 보장
                Application.Current.Dispatcher.BeginInvoke(
                    DispatcherPriority.Loaded,
                    new System.Action(() =>
                    {
                        // backing field만 먼저 변경해 Control DP에 이전 값 전달 후
                        // setter 호출로 RebuildColors 1회 실행
                        _rangeStart = newStart - 1.0;
                        _rangeEnd = newEnd + 1.0;
                        NotifyOfPropertyChange(nameof(RangeStart));
                        NotifyOfPropertyChange(nameof(RangeEnd));

                        RangeStart = newStart; // → RebuildColors
                        RangeEnd = newEnd;   // → RebuildColors

                        StatusMessage =
                            $"로드 완료: {data.WaferId}  |  셀 {data.Cells.Count}개  |  " +
                            $"WaferSize={WaferSize:F1}  Step={step:F2}  " +
                            $"Range={newStart:F1}~{newEnd:F1}";
                        Title = $"PL Map Viewer — {data.WaferId}";
                    }));
            }
            else
            {
                StatusMessage = $"로드 완료: {data.WaferId}  |  셀 없음";
                Title = $"PL Map Viewer — {data.WaferId}";
            }
        }

        // ── 셀 전체 재생성 ────────────────────────────
        private void RebuildCells(double rangeStart, double rangeEnd)
        {
            var cells = new ObservableCollection<WaferCell>();
            foreach (var c in _rawCells)
            {
                Color color = _colorSets.ValueToColor(rangeStart, rangeEnd, c.PeakWavelength);
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                cells.Add(new WaferCell(-c.Y, c.X, brush, c.PeakWavelength)
                {
                    DominantWavelength = c.DominantWavelength,
                    FWHM = c.FWHM,
                    PeakIntensity = c.PeakIntensity,
                    IntegratedIntensity = c.IntegratedIntensity,
                    PhotoDetect = c.PhotoDetect,
                    Reflection = c.Reflection,
                    Transmittance = c.Transmittance,
                    Thickness = c.Thickness,
                    ZPos = c.ZPos,
                    BlueReflection = c.BlueReflection,
                });
            }
            MapCells = cells;
            BuildDistributions(rangeStart, rangeEnd);
        }

        // ── 레인지 변경 시 색상만 재매핑 ─────────────
        private void RebuildColors()
        {
            if (_rawCells == null || _rawCells.Count == 0) return;
            if (MapCells == null || MapCells.Count != _rawCells.Count) return;

            for (int i = 0; i < _rawCells.Count; i++)
            {
                Color color = _colorSets.ValueToColor(
                    _rangeStart, _rangeEnd, _rawCells[i].PeakWavelength);
                var brush = new SolidColorBrush(color);
                brush.Freeze();
                MapCells[i].Brush = brush;
            }

            MapCells = new ObservableCollection<WaferCell>(MapCells);
            BuildDistributions(_rangeStart, _rangeEnd);
        }

        // ── 분포 계산 ─────────────────────────────────
        private void BuildDistributions(double rangeStart, double rangeEnd)
        {
            int bucketCount = _colorSets.ColorSet?.Count ?? 32;
            double span = rangeEnd - rangeStart;
            var dist = new long[bucketCount];

            if (span > 0)
            {
                foreach (var c in _rawCells)
                {
                    int idx = (int)((c.PeakWavelength - rangeStart) / span * bucketCount);
                    idx = Math.Max(0, Math.Min(bucketCount - 1, idx));
                    dist[idx]++;
                }
            }

            Distributions = new ObservableCollection<long>(dist);
        }
    }
}