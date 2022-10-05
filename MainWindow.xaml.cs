using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Threading;
using System.ComponentModel;


namespace Automation_of_Applications_Through_CMD
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            foreach (var vrLanguage in Enum.GetValues(typeof(enLanguages)).Cast<enLanguages>())
            {
                cmbBoxTaskOptions.Items.Add(vrLanguage);
            }
            foreach (var vrModel in Enum.GetValues(typeof(whisperModels)).Cast<whisperModels>())
            {
                cmbBoxModels.Items.Add(vrModel.processModelEnums());
            }

            cmbBoxTaskOptions.SelectedItem = enLanguages.English;
            cmbBoxModels.SelectedItem = whisperModels.tiny.ToString();
        }

        private static string srSelectedLanguage = "English";
        private static string srSelectedModel = whisperModels.tiny.ToString();

        public enum enLanguages
        {
            Afrikaans, Albanian, Amharic, Arabic, Armenian, Assamese, Azerbaijani, Bashkir, Basque, Belarusian, Bengali, Bosnian, Breton, Bulgarian, Burmese, Castilian, Catalan, Chinese, Croatian, Czech, Danish, Dutch, English, Estonian, Faroese, Finnish, Flemish, French, Galician, Georgian, German, Greek, Gujarati, Haitian, Haitian_Creole, Hausa, Hawaiian, Hebrew, Hindi, Hungarian, Icelandic, Indonesian, Italian, Japanese, Javanese, Kannada, Kazakh, Khmer, Korean, Lao, Latin, Latvian, Letzeburgesch, Lingala, Lithuanian, Luxembourgish, Macedonian, Malagasy, Malay, Malayalam, Maltese, Maori, Marathi, Moldavian, Moldovan, Mongolian, Myanmar, Nepali, Norwegian, Nynorsk, Occitan, Panjabi, Pashto, Persian, Polish, Portuguese, Punjabi, Pushto, Romanian, Russian, Sanskrit, Serbian, Shona, Sindhi, Sinhala, Sinhalese, Slovak, Slovenian, Somali, Spanish, Sundanese, Swahili, Swedish, Tagalog, Tajik, Tamil, Tatar, Telugu, Thai, Tibetan, Turkish, Turkmen, Ukrainian, Urdu, Uzbek, Valencian, Vietnamese, Welsh, Yiddish, Yoruba
        }

        public enum whisperModels
        {
            tiny,
            base_,
            small,
            medium,
            large,
            medium_en
        }

        private static long irCmdFileNameCounter = 0;

        private void btnStartBatchProcessing_Click(object sender, RoutedEventArgs e)
        {
            srSelectedLanguage = cmbBoxTaskOptions.SelectedItem.ToString();
            srSelectedModel = cmbBoxModels.SelectedItem.ToString();

            Directory.CreateDirectory("commands");

            var vrPath = txtFilePath.Text;

            Task.Factory.StartNew(() => { startProcessing(vrPath); });
        }

        private void startProcessing(string srTxtFilePath)
        {
            bool blDirectory = false;

            if (Directory.Exists(srTxtFilePath))
            {
                blDirectory = true;
            }

            string srFilePath = null;

            if (blDirectory == false)
            {
                if (File.Exists(srTxtFilePath))
                    srFilePath = srTxtFilePath;
            }

            if (blDirectory)
            {
                var vrFiles = Directory.GetFiles(srTxtFilePath);

                ParallelOptions po = new ParallelOptions();
                po.MaxDegreeOfParallelism = 2;

                Parallel.ForEach(vrFiles, po, vrFilePath =>
                {
                    startCmd(vrFilePath);
                });
            }
            else
            {
                if (string.IsNullOrEmpty(srFilePath) == false)
                {
                    startCmd(srFilePath);
                }
            }
        }

        private void startCmd(string vrFilePath)
        {
            string srCommand = @$"cmd /K whisper ""{vrFilePath}"" --language {srSelectedLanguage} --model {srSelectedModel} --device cuda --task translate";
            string srCommandName = $"commands/cmd_{Interlocked.Read(ref irCmdFileNameCounter)}.cmd";

            Interlocked.Add(ref irCmdFileNameCounter, 1);

            File.WriteAllText(srCommandName, srCommand);

            ProcessStartInfo psInfo = new ProcessStartInfo(srCommandName);

            psInfo.WindowStyle = ProcessWindowStyle.Normal;

            var vrProcess = Process.Start(psInfo);

            vrProcess.WaitForExit();
        }

        private void btnPickProcessingFolder_Click(object sender, RoutedEventArgs e)
        {

            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            if (folderBrowserDialog.ShowDialog().ToString() == "OK")
            {
                txtFilePath.Text = folderBrowserDialog.SelectedPath;
            }
        }

        private void btnPickProecssingFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog().ToString() == "OK")
                txtFilePath.Text = openFileDialog.FileName;
        }
    }
}
