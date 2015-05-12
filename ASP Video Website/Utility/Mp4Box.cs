using System;
using System.Diagnostics;
using System.IO;
using System.Web.Hosting;

namespace ASP_Video_Website.Utility
{
    public class Mp4Box
    {
        #region Properties

     
        private string _mp4BoxExe;
        public string Mp4BoxExe
        {
            get
            {
                return _mp4BoxExe;
            }
            set
            {
                _mp4BoxExe = value;
            }
        }
        #endregion

        #region Constructors
        public Mp4Box()
        {
            Initialize();
        }
        public Mp4Box(string mp4boxExePath)
        {
           

            _mp4BoxExe = mp4boxExePath;
            Initialize();
        }
        #endregion

        #region Initialization
        private void Initialize()
        {
            //first make sure we have a value for the ffexe file setting
            if (string.IsNullOrEmpty(_mp4BoxExe))
            {
                
                var o = HostingEnvironment.MapPath("~/Libs/mp4box.exe");
                if (o == null)
                {
                    throw new Exception("Could not find the location of the mp4box.exe file. ");
                }
                else
                {
                    if (string.IsNullOrEmpty(o.ToString()))
                        throw new Exception("No value was found in the app setting");

                  

                    _mp4BoxExe = o;
                }
            }

            if (!File.Exists(_mp4BoxExe))
                throw new Exception("Could not find a copy of mp4box.exe");
        }
        #endregion

        #region Run the process
        public string RunCommand(string Parameters)
        {
            //create a process info
            ProcessStartInfo oInfo = new ProcessStartInfo(this._mp4BoxExe, Parameters);

            

            oInfo.UseShellExecute = false;
            oInfo.CreateNoWindow = true;
            oInfo.RedirectStandardOutput = true;
            oInfo.RedirectStandardError = true;

            //Create the output and streamreader to get the output
            string output = ""; StreamReader srOutput = null;

            //try the process
            try
            {
                //run the process
                Process proc = Process.Start(oInfo);

                proc.ErrorDataReceived += (sender, args) =>
                {
                        if (args.Data != null)
                        {
                            Debug.WriteLine("Error: " + args.Data);
                            output += args.Data +Environment.NewLine;
                        }
                };

                proc.OutputDataReceived += (sender, args) =>
                    {
                            if (args.Data != null)
                            {
                                Debug.WriteLine("Output: " + args.Data);
                                output += args.Data + Environment.NewLine;
                            }
                    };

                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                proc.WaitForExit();
                
                //get the output
                //srOutput = proc.StandardError;

                //now put it in a string
                //output = srOutput.ReadToEnd();

                proc.Close();
            }
            catch (Exception e)
            {
                output = string.Empty;
            }
            finally
            {
                //now, if we succeded, close out the streamreader
                if (srOutput != null)
                {
                    srOutput.Close();
                    srOutput.Dispose();
                }
            }
            return output;
        }
        #endregion

    }
}