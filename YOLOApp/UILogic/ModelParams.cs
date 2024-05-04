using YOLOLogic;

namespace UILogic
{
    class ModelParams : IModelParams
    {
        public List<string> imagePaths { get; set; }
        public string saveDir { get; set; }
        public double precision { get; set; } = 0.5;

        public ModelParams(List<string> imagePaths, string saveDir)
        {
            this.imagePaths = imagePaths;
            this.saveDir = saveDir;
        }

        public ModelParams(string saveDir)
        {
            imagePaths = new List<string>();
            this.saveDir = saveDir;
        }

        public void addPath(string path)
        {
            imagePaths.Add(path);
        }

        public void removePath(string path)
        {
            imagePaths.Remove(path);
        }
    }
}
