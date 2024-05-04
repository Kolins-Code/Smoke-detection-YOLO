using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Drawing.Processing;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;


namespace YOLOLogic
{
    public class Model
    {
        IModelParams parameters;
        public Model(IModelParams images)
        {
            parameters = images;
        }

        public void infer()
        {
            int num = 0;
            foreach (var imagePath in parameters.imagePaths)
            {
                using var image = Image.Load<Rgba32>(imagePath);

                const int targetWidth = 640;
                const int targetHeight = 640;

                using var resizedImage = image.Clone(x =>
                {
                    x.Resize(new ResizeOptions
                    {
                        Size = new Size(targetWidth, targetHeight),
                        Mode = ResizeMode.Pad
                    });
                });
                image.Dispose();

                var input = new DenseTensor<float>(new[] { 1, 3, targetHeight, targetWidth });
                resizedImage.ProcessPixelRows(accessor =>
                {

                    for (int y = 0; y < accessor.Height; y++)
                    {
                        Span<Rgba32> pixelRow = accessor.GetRowSpan(y);

                        for (int x = 0; x < pixelRow.Length; x++)
                        {
                            input[0, 0, y, x] = pixelRow[x].R / 255f;
                            input[0, 1, y, x] = pixelRow[x].G / 255f;
                            input[0, 2, y, x] = pixelRow[x].B / 255f;
                        }
                    }
                });

                var inputs = new List<NamedOnnxValue>
                {
                   NamedOnnxValue.CreateFromTensor("images", input),
                };
                using var session = new InferenceSession("model.onnx");
                using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = session.Run(inputs);

                var outputs = results.First().AsTensor<float>();

                for (int i = 0; i < outputs.Dimensions[1]; i++)
                {
                    var x = outputs[0, i, 0];
                    var y = outputs[0, i, 1];
                    var width = outputs[0, i, 2];
                    var height = outputs[0, i, 3];
                    var objectConfidence = outputs[0, i, 4];
                    var classConfidence = outputs[0, i, 5];
                    if (classConfidence * objectConfidence > parameters.precision)
                    {
                        resizedImage.Mutate(
                        ctx => ctx.DrawPolygon(
                            Pens.Dash(Color.Red, 2),
                            new PointF[] {
                            new PointF(x - width / 2, y - height / 2),
                            new PointF(x + width / 2, y - height / 2),
                            new PointF(x + width / 2, y + height / 2),
                            new PointF(x - width / 2, y + height / 2)
                            }));
                    }
                }
                resizedImage.Save(parameters.saveDir + "\\result" + num++ + ".jpg");
                resizedImage.Dispose();
            }
        }
    }
}