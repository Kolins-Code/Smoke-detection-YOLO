using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UILogic
{
    public interface IUIRealization
    {
        public string getDirByDialog();
        public void showImage(string filename);
        public void showLoardingMenu();
        public void hideLoardingMenu();
    }
}
