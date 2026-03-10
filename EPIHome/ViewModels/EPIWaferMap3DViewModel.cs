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
using System.Windows.Media;
using System.Windows.Threading;

namespace EPIHome.ViewModels
{
    public class EPIWaferMap3DViewModel : Screen
    {
        private readonly WaferFileService _fileService = new WaferFileService();
        private readonly ColorSets _colorSets = new ColorSets();
        private List<WaferCellModel> _rawCells = new List<WaferCellModel>();

        // ── UI ───────────────────────────────────────────────────────────────

        private string _title = "PL Map 3D Viewer";
        public string Title
        {
            get => _title;
            set { _title = value; NotifyOfPropertyChange(); }
        }

        private string _statusMessage = "파일을 열어 웨이퍼 3D 맵을 로드하세요.";
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

        // ── 웨이퍼 셀 ────────────────────────────────────────────────────────

        private ObservableCollection<WaferCell> _mapCells = new ObservableCollection<WaferCell>();
        public ObservableCollection<WaferCell> MapCells
        {
            get => _mapCells;
            set { _mapCells = value; NotifyOfPropertyChange(); }
        }

        // ── 레인지 ───────────────────────────────────────────────────────────

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

        // ── ColorSet (EPIWaferMapRange 팔레트 TwoWay) ────────────────────────

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

        // ── 3D 전용: 높이 배율 ───────────────────────────────────────────────

        private double _heightScale = 3.0;
        public double HeightScale
        {
            get => _heightScale;
            set { _heightScale = value; NotifyOfPropertyChange(); }
        }

        // ── 요약 정보 ────────────────────────────────────────────────────────

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

        // ════════════════════════════════════════════════════════════════════
        // 생성자
        // ════════════════════════════════════════════════════════════════════

        public EPIWaferMap3DViewModel() { }

        // ════════════════════════════════════════════════════════════════════
        // 파일 열기 (EPIWaferMapViewModel.OpenFile 과 동일)
        // ════════════════════════════════════════════════════════════════════

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

        // ════════════════════════════════════════════════════════════════════
        // 데이터 적용 (EPIWaferMapViewModel.ApplyWaferData 와 동일한 패턴)
        // ════════════════════════════════════════════════════════════════════

        private void ApplyWaferData(WaferDataModel data)
        {
            _rawCells = data.Cells;
            WaferId = data.WaferId;
            TotalCells = data.Cells.Count;

            if (data.Cells.Count == 0)
            {
                StatusMessage = $"로드 완료: {data.WaferId}  |  셀 없음";
                Title = $"PL Map 3D Viewer — {data.WaferId}";
                return;
            }

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
                    _rangeStart = newStart - 1.0;
                    _rangeEnd = newEnd + 1.0;
                    NotifyOfPropertyChange(nameof(RangeStart));
                    NotifyOfPropertyChange(nameof(RangeEnd));

                    RangeStart = newStart;
                    RangeEnd = newEnd;

                    StatusMessage =
                        $"로드 완료: {data.WaferId}  |  셀 {data.Cells.Count}개  |  " +
                        $"Range={newStart:F1} ~ {newEnd:F1}";
                    Title = $"PL Map 3D Viewer — {data.WaferId}";
                }));
        }

        // ── 셀 전체 재생성 ────────────────────────────────────────────────────

        private void RebuildCells(double rangeStart, double rangeEnd)
        {
            var cells = new ObservableCollection<WaferCell>();
            foreach (var c in _rawCells)
            {
                Color color = _colorSets.ValueToColor(rangeStart, rangeEnd, c.PeakWavelength);
                var brush = new SolidColorBrush(color);
                brush.Freeze();

                // EPIWaferMapViewModel 과 동일: Row = -c.Y, Col = c.X
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
        }

        // ── 레인지 변경 시 색상만 재매핑 ─────────────────────────────────────

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

            // 컬렉션 교체로 바인딩 갱신
            MapCells = new ObservableCollection<WaferCell>(MapCells);
        }
    }
}
