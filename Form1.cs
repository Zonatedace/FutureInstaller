using System;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace FutureInstaller
{
    public partial class Form1 : Form
    {
        private const string BASE_URL = "https://s3.amazonaws.com/ces-web-files/-2/{0}.zip";
        private const string DOWNLOAD_ERROR = "Download error: ";
        private const string CONNECTION_ERROR_MESSAGE = "Connection Error";
        private const string TOOLTIP_TEXT = "Location: C:\\Users\\username\\Documents\\Future POS Support\\Toolbox";
        private const int RETRY_DELAY_SECONDS = 5;

        public Form1()
        {

            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem == null)
            {
                MessageBox.Show("Please select a version");
                return;
            }

            string selectedString = comboBox1.SelectedItem.ToString();
            string tempFilePath = selectedString + ".zip";

            // Display progress bar
            label2.Visible = true;
            label3.Visible = false;
            progressBar1.Visible = true;

            // Handle downloading
            await DownloadFile(selectedString, tempFilePath);

            // Handle zip and unzip process
            ExtractFile(selectedString, tempFilePath);

            label3.Text = "Installing...";

            // Run the extracted .exe
            RunExtractedExe(selectedString, tempFilePath);
        }

       

        private async Task DownloadFile(string selectedString, string tempFilePath)
        {
            if (File.Exists(tempFilePath))
            {
                File.Delete(tempFilePath);
            }
            else
            {
                bool downloadSuccess = false;

                while (!downloadSuccess)
                {
                    label2.Text = "Downloading...";
                    using (WebClient wc = new WebClient())
                    {
                        TaskCompletionSource<bool> downloadCompletionSource = new TaskCompletionSource<bool>();

                        wc.DownloadProgressChanged += wc_DownloadProgressChanged;
                        wc.DownloadFileCompleted += (s, e) =>
                        {
                            if (e.Error != null)
                            {
                                if (e.Error is WebException webException && (webException.Status == WebExceptionStatus.ReceiveFailure || webException.Status == WebExceptionStatus.ConnectFailure))
                                {
                                    // Wait for a few seconds then retry the download
                                    label2.Text = "Connection Loss, Retrying...";
                                    Task.Delay(TimeSpan.FromSeconds(RETRY_DELAY_SECONDS)).ContinueWith(async t => await DownloadFile(selectedString, tempFilePath));
                                }
                                else
                                {
                                    HandleDownloadError(e, tempFilePath);
                                    downloadCompletionSource.SetResult(true);
                                }
                            }
                            else if (e.Cancelled)
                            {
                                HandleDownloadCancelled(tempFilePath);
                                downloadCompletionSource.SetResult(true);
                            }
                            else
                            {
                                downloadSuccess = true;
                                downloadCompletionSource.SetResult(true);
                            }
                        };

                        await wc.DownloadFileTaskAsync(new Uri(string.Format(BASE_URL, selectedString)), tempFilePath);
                        await downloadCompletionSource.Task;
                    }
                }
            }
        }

        private void ExtractFile(string selectedString, string tempFilePath)
        {
            if (!File.Exists(selectedString + ".exe"))
            {
                try
                {
                    ZipFile.ExtractToDirectory(tempFilePath, Application.StartupPath);
                }
                catch (Exception) { }
            }
        }

        private void RunExtractedExe(string selectedString, string tempFilePath)
        {
            try
            {
                Process.Start(selectedString + ".exe").WaitForExit();

                // Handle post-installation
                if (keepFile.Checked)
                {
                    Process.Start("explorer.exe", selectedString);
                }
                else
                {
                    SafeDelete(tempFilePath);
                    SafeDelete(selectedString + ".exe");
                }
                label3.Text = "Installation Complete";
                progressBar1.Visible = false;
            }
            catch (Exception ex)
            {
                HandleInstallationException(ex, tempFilePath, selectedString);
            }
        }

        private void HandleInstallationException(Exception ex, string tempFilePath, string selectedString)
        {
            DialogResult result = MessageBox.Show("The file already exists, the existing file will be deleted. Please Retry", "Error", MessageBoxButtons.OK);
            if (result == DialogResult.Yes)
            {
                SafeDelete(tempFilePath);
                SafeDelete(selectedString + ".exe");
            }
            
        }

        private void wc_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

        private void wc_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            string tempFilePath = comboBox1.SelectedItem.ToString() + ".zip";

            if (e.Error != null)
            {
                HandleDownloadError(e, tempFilePath);
            }
            else if (e.Cancelled)
            {
                HandleDownloadCancelled(tempFilePath);
            }
            else
            {
                label3.Visible = true;
                label2.Visible = false;
            }
        }

        private void HandleDownloadError(AsyncCompletedEventArgs e, string tempFilePath)
        {
            label3.Text = DOWNLOAD_ERROR + e.Error.Message;
            SafeDelete(tempFilePath);

            if (e.Error is WebException webException)
            {
                if (webException.Status == WebExceptionStatus.ReceiveFailure)
                {
                    MessageBox.Show("Connection was forcibly closed by the remote host. Retrying in a few seconds...", CONNECTION_ERROR_MESSAGE, MessageBoxButtons.OK);
                }
                else if (webException.Status == WebExceptionStatus.ConnectFailure)
                {
                    MessageBox.Show("Internet connection is unstable. Retrying in a few seconds...", CONNECTION_ERROR_MESSAGE, MessageBoxButtons.OK);
                }
                else
                {
                    // Handle other cases if needed
                    MessageBox.Show("An error occurred during download. Please try again.", "Download Error", MessageBoxButtons.OK);
                }
            }
            else
            {
                // Handle non-WebException cases if any
                MessageBox.Show("An error occurred during download. Please try again.", "Download Error", MessageBoxButtons.OK);
            }
        }

        private void HandleDownloadCancelled(string tempFilePath)
        {
            label3.Text = "Download cancelled.";
            SafeDelete(tempFilePath);
            MessageBox.Show("Download was cancelled, please try again.", "Download Cancelled", MessageBoxButtons.OK);
        }

        private void SafeDelete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (Exception) { }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label3.Visible = true;
            label3.Text = FutureProgram.GetVersion();
            toolTip1.SetToolTip(keepFile, TOOLTIP_TEXT);
        }
    }
}