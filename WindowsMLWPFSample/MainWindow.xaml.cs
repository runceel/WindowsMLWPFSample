using Microsoft.Toolkit.Wpf.UI.XamlHost;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Windows.AI.MachineLearning;

namespace WindowsMLWPFSample
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private Windows.UI.Xaml.Controls.Grid _grid;

        private void WindowsXamlHost_ChildChanged(object sender, EventArgs e)
        {
            var xamlHost = (WindowsXamlHost)sender;
            _grid = xamlHost.Child as Windows.UI.Xaml.Controls.Grid;
            if (_grid == null)
            {
                return;
            }

            _grid.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black);
            _grid.Width = 300;
            _grid.Height = 300;
            var inkCanvas = new Windows.UI.Xaml.Controls.InkCanvas();
            inkCanvas.InkPresenter.UpdateDefaultDrawingAttributes(new Windows.UI.Input.Inking.InkDrawingAttributes
            {
                Color = Windows.UI.Colors.White,
                Size = new Windows.Foundation.Size(22, 22),
            });
            inkCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
            _grid.Children.Add(inkCanvas);
        }

        private MnistModel _model;

        private async void InkPresenter_StrokesCollected(Windows.UI.Input.Inking.InkPresenter sender, Windows.UI.Input.Inking.InkStrokesCollectedEventArgs args)
        {
            if (_model == null)
            {
                var mnistModelFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Mnist.onnx");
                var modelFile = await Windows.Storage.StorageFile.GetFileFromPathAsync(mnistModelFilePath);
                _model = await MnistModel.CreateFromStreamAsync(modelFile);
            }

            var visual = await new Helper().GetHandWrittenImage(_grid);
            var result = await _model.EvaluateAsync(new MnistInput
            {
                Input3 = ImageFeatureValue.CreateFromVideoFrame(visual),
            });
            var scores = result.Plus214_Output_0.GetAsVectorView().ToArray();
            var answer = Array.IndexOf(scores, scores.Max());
            textBlock.Text = $"Result: {answer}";
        }
    }
}
