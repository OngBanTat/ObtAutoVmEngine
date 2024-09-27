using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using KAutoHelper;
using ObtSDK;
using ObtSDK.AutoAndroidVm;
using UI.Model;
using Brushes = System.Windows.Media.Brushes;
using Color = System.Drawing.Color;
using Point = System.Windows.Point;
using Rectangle = System.Windows.Shapes.Rectangle;

namespace UI.View;

[Obfuscation(Exclude = false, Feature = "-rename")]
public class SizePlusConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is double size) return size + 40;

        return value;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}

public partial class ImageViewer : INotifyPropertyChanged
{
    private readonly Device device;
    private readonly bool isLDPlayer;
    private readonly double scaleX = 1;
    private readonly double scaleY = 1;
    private double _h;
    private Bitmap _img;
    private bool _isInClearProcess;
    private Point _origin;
    private Rectangle? _rect;
    private List<Rectangle?> _rectangles = [];
    private Point _start;
    private Point _startPoint;
    private string _text = "";
    private string _textArgb = "";
    private TextBlock? _textBlock;
    private List<TextBlock?> _textBlocks = [];
    private string _textPerXy = "";
    private string _textXy = "";
    private DispatcherTimer? _updateImageDeviceTimer;
    private double _w;
    private double _x;
    private double _y = 20;
    private int currentImgPosX;
    private int currentImgPosY;

    public ImageViewer(Device device)
    {
        this.device = device;
        isLDPlayer = this.device.DType == BaseDeviceInfo.DeviceType.LdPlayer;
        InitializeComponent();
        Title = this.device.DType + " --- " + this.device.DeviceName;
        DataContext = this;
        // var bitmap = new BitmapImage();
        // bitmap.BeginInit();
        // bitmap.UriSource = new Uri(imagePath);
        // bitmap.CacheOption = BitmapCacheOption.OnLoad;
        // bitmap.EndInit();
        // bitmap.Freeze();
        // if (isLdPlayer)
        // {
        //     scaleX = bitmap.PixelWidth / bitmap.Width;
        //     scaleY = bitmap.PixelHeight / bitmap.Height;
        // }
        //
        // ImgView.Source = bitmap;
        // var ms = new MemoryStream();
        // var encoder = new BmpBitmapEncoder();
        // encoder.Frames.Add(BitmapFrame.Create(bitmap));
        // encoder.Save(ms);
        // ms.Position = 0;
        // _img = new Bitmap(ms);
        // File.Delete(imagePath);
        Canvas.SetLeft(ImgView, 0);
        Canvas.SetTop(ImgView, 20);
        Closed += (sender, args) => { Common.ClearInterval(ref _updateImageDeviceTimer); };
    }

    public event PropertyChangedEventHandler? PropertyChanged;


    public void EnableMirrorDevice()
    {
        _updateImageDeviceTimer = Common.SetInterval(() =>
        {
            var img = CaptureHelper.CaptureWindow(device.GetDeviceIdForAuto());
            var sizePlus = device.GetSizePlus();
            img = CaptureHelper.CropImage(img, new System.Drawing.Rectangle(0, (int)Math.Round(sizePlus.Height),
                img.Width - (int)Math.Round(sizePlus.Width), img.Height));
            using var memory = new MemoryStream();
            img.Save(memory, ImageFormat.Png);
            memory.Position = 0;
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = memory;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            bitmapImage.Freeze();
            try
            {
                ImgView.Source = bitmapImage;
                _img = new Bitmap(memory);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }, 16);
    }


    private void Image_MouseMove(object sender, MouseEventArgs e)
    {
        var pos = e.GetPosition(ImgView);
        if (ImgView.Source is not BitmapSource bitmapSource) return;
        if (pos.X * scaleX < bitmapSource.PixelWidth && pos.Y * scaleY < bitmapSource.PixelHeight)
        {
            var pixel = new byte[4];
            bitmapSource.CopyPixels(new Int32Rect((int)(pos.X * scaleX), (int)(pos.Y * scaleY), 1, 1), pixel,
                bitmapSource.PixelWidth * 4, 0);
            var color = Color.FromArgb(pixel[3], pixel[2], pixel[1], pixel[0]);

            var posX = Math.Round(pos.X * scaleX);
            var posY = Math.Round(pos.Y * scaleY);
            currentImgPosX = (int)posX;
            currentImgPosY = (int)posY;
            var perX = (posX * 100.00 / bitmapSource.PixelWidth).ToString("F2");
            var perY = (posY * 100.00 / bitmapSource.PixelHeight).ToString("F2");
            _textArgb = $"ARGB: {color.A}, {color.R}, {color.G}, {color.B}";
            _textXy = $"XY: {posX},{posY}";
            _textPerXy = $"Per XY: {perX},{perY}";
            _text = $"X: {posX}; Y: {posY}; %X: {perX}; %Y: {perY};  {_textArgb}";
            PixelInformation.Text = _text;
        }
        else
        {
            PixelInformation.Text = $"Out of bounds: {Math.Round(pos.X * scaleX)}, {Math.Round(pos.Y * scaleY)}";
        }
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    private void ImageViewer_OnMouseRightClick(object sender, MouseButtonEventArgs e)
    {
        TxtXy.Text = _textXy;
        TxtArgb.Text = _textArgb;
        TxtPerCentXy.Text = _textPerXy;
        TxtLastRect.Text =
            $"Last Rect: {Math.Round(_x * scaleX)},{Math.Round((_y - 20) * scaleY)},{Math.Round(_w * scaleX)},{Math.Round(_h * scaleY)}";
    }

    private void MainCanvas_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _startPoint = e.GetPosition(MainCanvas);
        _x = _startPoint.X;
        _y = _startPoint.Y;
        _w = 0;
        _h = 0;
        _rect = new Rectangle
        {
            Stroke = Brushes.Red,
            StrokeThickness = 0.5
        };


        _textBlock = new TextBlock
        {
            Text =
                $"X: {Math.Round(_startPoint.X * scaleX)}, Y: {Math.Round((_startPoint.Y - 20) * scaleY)}, Width: 0, Height: 0",
            Foreground = Brushes.Red
        };

        Canvas.SetLeft(_rect, _startPoint.X);
        Canvas.SetTop(_rect, _startPoint.Y);

        Canvas.SetLeft(_textBlock, _startPoint.X);
        Canvas.SetTop(_textBlock, _startPoint.Y - 15);

        MainCanvas.Children.Add(_rect);
        MainCanvas.Children.Add(_textBlock);
    }

    private void MainCanvas_MouseMove(object sender, MouseEventArgs e)
    {
        if (e.LeftButton == MouseButtonState.Released || _rect == null)
            return;

        var pos = e.GetPosition(MainCanvas);

        _x = Math.Min(pos.X, _startPoint.X);
        _y = Math.Min(pos.Y, _startPoint.Y);

        _w = Math.Max(pos.X, _startPoint.X) - _x;
        _h = Math.Max(pos.Y, _startPoint.Y) - _y;

        _rect.Width = _w;
        _rect.Height = _h;

        Canvas.SetLeft(_rect, _x);
        Canvas.SetTop(_rect, _y);

        _textBlock!.Text =
            $"X: {Math.Round(_x * scaleX)}, Y: {Math.Round((_y - 20) * scaleY)}, W: {Math.Round(_w * scaleX)}, H: {Math.Round(_h * scaleY)}";
        Canvas.SetLeft(_textBlock, _x);
        Canvas.SetTop(_textBlock, _y - 15);
    }

    private void MainCanvas_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        _rectangles.Add(_rect);
        _textBlocks.Add(_textBlock);
        _rect = null;
        _textBlock = null;
    }

    private void MainCanvas_OnMouseRightButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_isInClearProcess) return;
        _isInClearProcess = true;
        var rectCount = _rectangles.Count;
        Rectangle? lastRect = null;
        TextBlock? lastText = null;
        for (var i = 0; i < _rectangles.Count; i++)
        {
            lastRect = _rectangles[i];
            lastText = _textBlocks[i];
            if (i == rectCount - 1) break;
            MainCanvas.Children.Remove(lastRect);
            MainCanvas.Children.Remove(lastText);
        }

        _rectangles = [];
        _textBlocks = [];
        if (lastText != null)
        {
            _rectangles.Add(lastRect);
            _textBlocks.Add(lastText);
        }

        _isInClearProcess = false;
    }

    private void SaveLastRect_OnClick(object sender, RoutedEventArgs e)
    {
        if (_w == 0 || _h == 0)
        {
            MessageBox.Show("The Last Box Doesn't Appear to save!");
            return;
        }

        var sx = scaleX;
        var sy = scaleY;
        if (!isLDPlayer)
        {
            var a = ImgView.Source as BitmapImage;
            sx = a.PixelWidth / a.Width;
            sy = a.PixelHeight / a.Height;
        }

        var img2 = CaptureHelper.CropImage(_img, new System.Drawing.Rectangle(
            (int)Math.Round(_x * sx), (int)Math.Round((_y - 20) * sy),
            (int)Math.Round(_w * sx), (int)Math.Round(_h * sy)));
        // Common.CopyBitmapToClipboard(img2);
        Common.SaveBitmapAsPng(img2);
    }

    private void ScrollViewer_MouseWheel(object sender, MouseWheelEventArgs e)
    {
        var viewer = sender as ScrollViewer;
        if (viewer?.LayoutTransform is not ScaleTransform st)
        {
            st = new ScaleTransform();
            viewer!.LayoutTransform = st;
        }

        if (e.Delta > 0)
        {
            st.ScaleX *= 1.1;
            st.ScaleY *= 1.1;
        }
        else if (st.ScaleX > 0.4)
        {
            st.ScaleX /= 1.1;
            st.ScaleY /= 1.1;
        }

        e.Handled = true;
    }

    private void ScrollViewer_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (Mouse.Captured != null) return;
        _origin = Mouse.GetPosition((IInputElement)sender);
        _start = _origin;
        Mouse.Capture((IInputElement)sender, CaptureMode.Element);
        e.Handled = true;
    }

    private void ScrollViewer_MouseMove(object sender, MouseEventArgs e)
    {
        if (Mouse.Captured == null) return;
        var v = _start - Mouse.GetPosition((IInputElement)sender);
        _origin += v;
        _start = Mouse.GetPosition((IInputElement)sender);

        var viewer = (ScrollViewer)sender;
        viewer.ScrollToHorizontalOffset(_origin.X);
        viewer.ScrollToVerticalOffset(_origin.Y);
        e.Handled = true;
    }

    private void ScrollViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        ((IInputElement)sender).ReleaseMouseCapture();
    }

    private void ClearAllRect_OnClick(object sender, RoutedEventArgs e)
    {
        if (_isInClearProcess) return;
        _isInClearProcess = true;
        for (var i = 0; i < _rectangles.Count; i++)
        {
            MainCanvas.Children.Remove(_rectangles[i]);
            MainCanvas.Children.Remove(_textBlocks[i]);
        }

        _rectangles = [];
        _textBlocks = [];
        _x = 0;
        _y = 20;
        _w = 0;
        _h = 0;

        _isInClearProcess = false;
    }

    private void PauseMirror_OnClick(object sender, RoutedEventArgs e)
    {
        if (_updateImageDeviceTimer is { IsEnabled: true })
            _updateImageDeviceTimer?.Stop();
    }

    private void ContinueMirror_OnClick(object sender, RoutedEventArgs e)
    {
        if (_updateImageDeviceTimer is { IsEnabled: false })
            _updateImageDeviceTimer?.Start();
    }

    private void CopyXY_OnClick(object sender, RoutedEventArgs e)
    {
        var text = "";
        var d = TxtXy.Text.Split(':');
        if (d.Length == 2) text = d[1].Trim();
        Clipboard.SetText(text);
    }

    private void CopyPercentXY_OnClick(object sender, RoutedEventArgs e)
    {
        var text = "";
        var d = TxtPerCentXy.Text.Split(':');
        if (d.Length == 2) text = d[1].Trim();
        Clipboard.SetText(text);
    }

    private void CopyLastRect_OnClick(object sender, RoutedEventArgs e)
    {
        var text = "";
        var d = TxtLastRect.Text.Split(':');
        if (d.Length == 2) text = d[1].Trim();
        Clipboard.SetText(text);
    }

    private void CopyARGB_OnClick(object sender, RoutedEventArgs e)
    {
        var text = "";
        var d = TxtArgb.Text.Split(':');
        if (d.Length == 2) text = d[1].Trim();
        Clipboard.SetText(text);
    }


    private void MainCanvas_OnMouseDown(object sender, MouseButtonEventArgs e)
    {
        if (e.MiddleButton == MouseButtonState.Pressed) device.ClickOnPosition(currentImgPosX, currentImgPosY);
    }

    private void MainCanvas_OnKeyDown(object sender, KeyEventArgs e)
    {
        device.SendKeyBoardPress(e.Key);
    }
}