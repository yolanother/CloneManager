using System;
using System.Diagnostics;

namespace Doubtech.CloneManager
{
    public class FileIO
    {
        /// <summary>
 
        /// Method to Perform Xcopy to copy files/folders from Source machine to Target Machine
 
        /// </summary>
 
        /// <param name="SolutionDirectory"></param>
 
        /// <param name="TargetDirectory"></param>
 
        public static void ProcessXcopy(string source, string target)
 
        {
            // Use ProcessStartInfo class
 
            ProcessStartInfo startInfo = new ProcessStartInfo();
 
            startInfo.CreateNoWindow = false;
 
            startInfo.UseShellExecute = false;
 
            //Give the name as Xcopy
 
            startInfo.FileName = "xcopy";
 
            //make the window Hidden
 
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
 
            //Send the Source and destination as Arguments to the process
 
            startInfo.Arguments = "\"" + source + "\"" + " " + "\"" + target + "\"" + @" /e /y /I";
 
            try
 
            {
                // Start the process with the info we specified.
 
                // Call WaitForExit and then the using statement will close.
 
                using (Process exeProcess = Process.Start(startInfo))
 
                {
                    exeProcess.WaitForExit();
                }
            }
            catch (Exception exp)
 
            {
                throw exp;
            }
        }
    }
}