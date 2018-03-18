using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using YoutubeExtractor;

namespace YoutubeDownloader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            cmbQuality.SelectedIndex = 0;
        }

        private void btnDownload_Click(object sender, EventArgs e)
        {
            progressBar1.Minimum = 0;
            progressBar1.Maximum = 100;
            try
            {
                IEnumerable<VideoInfo> videos = DownloadUrlResolver.GetDownloadUrls(txtUrl.Text);
                            
            VideoInfo video = videos.First(p => p.VideoType == VideoType.Mp4 && p.Resolution == Convert.ToInt32(cmbQuality.Text));
            cmbQuality.Text = video.Resolution.ToString();
            if (video.RequiresDecryption)
            {
                DownloadUrlResolver.DecryptDownloadUrl(video);
            }
                
                
            VideoDownloader downloader = new VideoDownloader(video, Path.Combine(Application.StartupPath + "\\", video.Title + video.VideoExtension));
            downloader.DownloadProgressChanged += downloader_DownloadProgressChanged;
            Thread thread = new Thread(() => {downloader.Execute();}) {IsBackground= true};
            thread.Start();
            }
            catch (System.Net.WebException )
            {
                MessageBox.Show("There is a problem with your internet connection");
            }
            catch (System.ArgumentException o)
            {
                MessageBox.Show(o.Message);
            }
        }

        void downloader_DownloadProgressChanged(object sender, ProgressEventArgs e)
        {
            Invoke(new MethodInvoker(delegate()
            {
                progressBar1.Value = (int)e.ProgressPercentage;
                lblProg.Text = string.Format("{0:0.0}", e.ProgressPercentage) + "%";
                progressBar1.Update();
                
            }
                )
                );
        }
    }
}
