using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YOLOLogic
{
    public interface IModelParams
    {
        List<string> imagePaths { get; set; }
        string saveDir { get; set; }
        public double precision { get; set; }
    }
}
