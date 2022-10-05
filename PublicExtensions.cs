using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Automation_of_Applications_Through_CMD
{
    public static class PublicExtensions
    {
        public static string processModelEnums(this MainWindow.whisperModels model)
        {
            switch (model)
            {
                case MainWindow.whisperModels.tiny:
                    return "tiny";
                    break;
                case MainWindow.whisperModels.base_:
                    return "base";
                    break;
                case MainWindow.whisperModels.small:
                    return "small";
                    break;
                case MainWindow.whisperModels.medium:
                    return "medium";
                    break;
                case MainWindow.whisperModels.large:
                    return "large";
                    break;
                case MainWindow.whisperModels.medium_en:
                    return "medium.en";
                    break;
                default:
                    return "tiny";
                    break;
            }
        }
    }
}
