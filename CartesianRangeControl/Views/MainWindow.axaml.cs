using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using CartesianRangeControl.ViewModels;
using Eremex.AvaloniaUI.Charts;

namespace CartesianRangeControl.Views;

public partial class MainWindow : Window
{
    enum MoveState
    {
        None,
        Min,
        Max,
        Move
    }
    
    readonly Cursor moveCursor = new(StandardCursorType.Hand);
    readonly Cursor sizeCursor = new(StandardCursorType.SizeWestEast);
    MoveState state = MoveState.None;
    double start;
    
    MainWindowViewModel? ViewModel => DataContext as MainWindowViewModel;
    
    public MainWindow()
    {
        InitializeComponent();
    }
    bool IsHit(Point? p1, Point p2) => p1.HasValue && Math.Abs(p1.Value.X - p2.X) <= 3;
    bool IsInside(double value) => (double)MainChart.AxesX.Single().Range!.WholeMin! <= value && (double)MainChart.AxesX.Single().Range!.WholeMax! >= value;
    Point? GetScreenPosition(double value) => RangeSelectorChart.DiagramPointToScreenPoint(new AxisXCoordinate(RangeSelectorChart.AxesX.Single(), value), new AxisYCoordinate(RangeSelectorChart.AxesY.Single(), 0)); 
    void RangeSelectorChart_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (ViewModel is null)
            return;
        var position = e.GetPosition(RangeSelectorChart);
        var diagramCoords = RangeSelectorChart.ScreenPointToDiagramPoint(position);
        if (diagramCoords.InsideViewport)
        {
            var minPosition = GetScreenPosition(ViewModel.VisualMin);
            var maxPosition = GetScreenPosition(ViewModel.VisualMax);
            if (IsHit(minPosition, position))
            {
                state = MoveState.Min;
                RangeSelectorChart.Cursor = sizeCursor;
            }
            else if (IsHit(maxPosition, position))
            {
                state = MoveState.Max;
                RangeSelectorChart.Cursor = sizeCursor;
            }
            else if (minPosition.HasValue && minPosition.Value.X < position.X && maxPosition.HasValue && maxPosition.Value.X > position.X)
            {
                state = MoveState.Move;
                RangeSelectorChart.Cursor = moveCursor;
            }

            start = (double)diagramCoords.AxesX.Single().Value;
        }
    }
    void RangeSelectorChart_OnPointerReleased(object? sender, PointerReleasedEventArgs e) => state = MoveState.None;
    void RangeSelectorChart_OnPointerMoved(object? sender, PointerEventArgs e)
    {
        if (ViewModel is null)
            return;
        
        var position = e.GetPosition(RangeSelectorChart);
        var diagramCoords = RangeSelectorChart.ScreenPointToDiagramPoint(position);
        double end = (double)diagramCoords.AxesX.Single().Value;
        double delta = end - start;
        switch (state)
        {
            case MoveState.Min:
                if (ViewModel.VisualMin + delta < ViewModel.VisualMax)
                    ViewModel.VisualMin += delta;
                break;
            case MoveState.Max:
                if (ViewModel.VisualMin < delta + ViewModel.VisualMax)
                    ViewModel.VisualMax += delta;
                break;
            case MoveState.Move:
                if (IsInside(ViewModel.VisualMin + delta) && IsInside(ViewModel.VisualMax + delta))
                {
                    ViewModel.VisualMin += delta;
                    ViewModel.VisualMax += delta;
                }
                break;
            case MoveState.None:
                UpdateCursor(position);
                break;
        }
        start = end;
    }
    void UpdateCursor(Point position)
    {
        var minPosition = GetScreenPosition(ViewModel!.VisualMin);
        var maxPosition = GetScreenPosition(ViewModel.VisualMax);
        RangeSelectorChart.Cursor = IsHit(minPosition, position) || IsHit(maxPosition, position) ? sizeCursor :
            minPosition.HasValue && maxPosition.HasValue && position.X >= minPosition.Value.X &&
            position.X <= maxPosition.Value.X ? moveCursor : Cursor.Default;
    }
}
