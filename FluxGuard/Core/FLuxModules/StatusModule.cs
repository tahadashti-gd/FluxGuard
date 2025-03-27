using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Text;
using System.Management;
using FluxGuard.Core.Services;
using GUI;
using OpenCvSharp; 
using NAudio.Wave; 
using NAudio.CoreAudioApi;

namespace FluxGuard.Core.FLuxModules
{
    internal class StatusModule
    {
        public static byte[] StatusCommand()
        {
            LogService.LogServiceActivity("Screenshot: Screen");
            ScreenshotService.Screenshot("Screen");
            byte[] imageBytes = File.ReadAllBytes("Screenshot.png");
            return imageBytes;
        }

        public static string GetUpTime()
        {
            using var upTime = new PerformanceCounter("System", "System Up Time");
            upTime.NextValue();
            string DT = TimeSpan.FromSeconds(upTime.NextValue()).ToString();
            int DTIndex = DT.LastIndexOf(':');
            LogService.LogServiceActivity("GetUpTime");

            return DT.Substring(0, DTIndex);
        }

        public static string CheckDriversAndDevices()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine(LanguageService.Translate("answers", "status", "driver_report"));

            report.AppendLine(LanguageService.Translate("answers", "status", "usb_devices"));
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE 'USB%' AND Service='USBSTOR'"))
                {
                    var usbDevices = searcher.Get();
                    if (usbDevices.Count > 0)
                    {
                        using (var diskSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DriveType=2")) // DriveType=2 یعنی removable
                        {
                            foreach (ManagementObject usbDevice in usbDevices)
                            {
                                string deviceName = usbDevice["Caption"]?.ToString() ?? "Unknown USB Device";
                                string status = usbDevice["Status"]?.ToString() == "OK"
                                    ? LanguageService.Translate("answers", "status", "connected")
                                    : LanguageService.Translate("answers", "status", "has_issue");
                                string deviceId = usbDevice["DeviceID"]?.ToString() ?? string.Empty;

                                string driveLetter = string.Empty;
                                foreach (ManagementObject disk in diskSearcher.Get())
                                {
                                    string diskDeviceId = disk["DeviceID"]?.ToString() ?? string.Empty;

                                    using (var diskToPartitionSearcher = new ManagementObjectSearcher(
                                        $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{diskDeviceId}'}} WHERE AssocClass=Win32_LogicalDiskToPartition"))
                                    {
                                        foreach (ManagementObject partition in diskToPartitionSearcher.Get())
                                        {
                                            using (var partitionToDiskSearcher = new ManagementObjectSearcher(
                                                $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{partition["DeviceID"]}'}} WHERE AssocClass=Win32_DiskDriveToDiskPartition"))
                                            {
                                                foreach (ManagementObject diskDrive in partitionToDiskSearcher.Get())
                                                {
                                                    string diskDriveId = diskDrive["PNPDeviceID"]?.ToString() ?? string.Empty;
                                                    if (diskDriveId == deviceId)
                                                    {
                                                        driveLetter = diskDeviceId; 
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }

                                report.AppendLine($"- {deviceName}: {status} ({LanguageService.Translate("answers", "status", "drive_label")} {driveLetter})");
                            }
                        }
                    }
                    else
                    {
                        report.AppendLine($"- {LanguageService.Translate("answers", "status", "no_usb_storage_found")}");
                    }
                }
                LogService.LogServiceActivity("Checked USB storage devices successfully using Win32_PnPEntity");
            }
            catch (ManagementException mex)
            {
                report.AppendLine(LanguageService.Translate("answers", "status", "error_usb"));
                LogService.LogError(mex, "ManagementException while checking USB storage devices");
            }
            catch (Exception ex)
            {
                report.AppendLine(LanguageService.Translate("answers", "status", "error_usb"));
                LogService.LogError(ex, "Unexpected error while checking USB storage devices");
            }

            report.AppendLine(LanguageService.Translate("answers", "status", "input_devices"));
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Keyboard"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string status = obj["Status"]?.ToString() == "OK"
                            ? LanguageService.Translate("answers", "status", "connected")
                            : LanguageService.Translate("answers", "status", "has_issue");
                        report.AppendLine($"- {LanguageService.Translate("answers", "status", "keyboard_label")} {status}");
                    }
                }
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PointingDevice"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string status = obj["Status"]?.ToString() == "OK"
                            ? LanguageService.Translate("answers", "status", "connected")
                            : LanguageService.Translate("answers", "status", "has_issue");
                        report.AppendLine($"- {LanguageService.Translate("answers", "status", "mouse_label")} {status}");
                    }
                }
                LogService.LogServiceActivity("Checked input devices successfully");
            }
            catch (Exception ex)
            {
                report.AppendLine(LanguageService.Translate("answers", "status", "error_input"));
                LogService.LogError(ex, "Failed to check input devices");
            }

            report.AppendLine(LanguageService.Translate("answers", "status", "network_adapters"));
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapter WHERE PhysicalAdapter = True"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        string adapterName = obj["Name"]?.ToString() ?? "Unknown Adapter";
                        string status = obj["NetEnabled"]?.ToString() == "True"
                            ? LanguageService.Translate("answers", "status", "connected")
                            : LanguageService.Translate("answers", "status", "has_issue");
                        report.AppendLine($"- {adapterName}: {status}");
                    }
                }
                LogService.LogServiceActivity("Checked network adapters successfully");
            }
            catch (Exception ex)
            {
                report.AppendLine(LanguageService.Translate("answers", "status", "error_network"));
                LogService.LogError(ex, "Failed to check network adapters");
            }

            LogService.LogServiceActivity("Generated driver and device status report");
            return report.ToString();
        }

        public static byte[] CaptureWebcamImage(out string report)
        {
            StringBuilder reportBuilder = new StringBuilder();
            byte[] imageBytes = null;

            try
            {
                using (var capture = new VideoCapture(0))
                {
                    if (capture.IsOpened())
                    {
                        reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "webcam_label")} {LanguageService.Translate("answers", "status", "connected")}");

                        // گرفتن عکس
                        using (var frame = new Mat())
                        {
                            capture.Read(frame);
                            if (!frame.Empty())
                            {
                                imageBytes = frame.ToBytes(".png");
                                LogService.LogServiceActivity("Captured webcam image successfully");
                            }
                            else
                            {
                                reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "error_webcam_capture")}");
                                LogService.LogServiceActivity("Failed to capture webcam image: Frame is empty");
                            }
                        }
                    }
                    else
                    {
                        reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "webcam_label")} {LanguageService.Translate("answers", "status", "not_connected")}");
                        reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "webcam_not_found")}");
                        LogService.LogServiceActivity("No webcam detected");
                    }
                }
            }
            catch (Exception ex)
            {
                reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "webcam_label")} {LanguageService.Translate("answers", "status", "error_webcam")}");
                LogService.LogError(ex, "Failed to check webcam");
            }

            report = reportBuilder.ToString();
            return imageBytes;
        }

        public static Stream RecordMicrophone(out string report)
        {
            StringBuilder reportBuilder = new StringBuilder();
            Stream audioStream = null;

            try
            {
                if (WaveInEvent.DeviceCount > 0)
                {
                    reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "microphone_label")} {LanguageService.Translate("answers", "status", "connected")}");

                    // پیدا کردن میکروفون پیش‌فرض
                    int defaultDeviceIndex = -1;
                    var enumerator = new MMDeviceEnumerator();
                    var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                    string defaultDeviceId = defaultDevice?.ID;

                    if (defaultDeviceId == null)
                    {
                        reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "error_microphone")}: No default microphone found");
                        LogService.LogServiceActivity("No default microphone found");
                        report = reportBuilder.ToString();
                        return null;
                    }

                    for (int i = 0; i < WaveInEvent.DeviceCount; i++)
                    {
                        var deviceInfo = WaveInEvent.GetCapabilities(i);
                        var device = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
                            .FirstOrDefault(d => d.ID == defaultDeviceId);
                        if (device != null)
                        {
                            defaultDeviceIndex = i;
                            LogService.LogServiceActivity($"Default microphone found: {device.FriendlyName}, Index: {defaultDeviceIndex}");
                            break;
                        }
                    }

                    if (defaultDeviceIndex == -1)
                    {
                        reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "error_microphone")}: Could not identify default microphone");
                        LogService.LogServiceActivity("Could not identify default microphone in NAudio");
                        report = reportBuilder.ToString();
                        return null;
                    }

                    using (var memoryStream = new MemoryStream())
                    using (var waveIn = new WaveInEvent())
                    {
                        waveIn.DeviceNumber = defaultDeviceIndex; 
                        waveIn.WaveFormat = new WaveFormat(44100, 1);
                        LogService.LogServiceActivity($"WaveIn initialized with format: 44100 Hz, mono, Device: {defaultDeviceIndex}");

                        using (var writer = new WaveFileWriter(memoryStream, waveIn.WaveFormat))
                        {
                            if (writer == null)
                            {
                                throw new InvalidOperationException("Failed to initialize WaveFileWriter");
                            }

                            bool hasData = false;
                            waveIn.DataAvailable += (s, e) =>
                            {
                                if (e.BytesRecorded > 0)
                                {
                                    writer.Write(e.Buffer, 0, e.BytesRecorded);
                                    hasData = true;
                                    LogService.LogServiceActivity($"Recorded {e.BytesRecorded} bytes from microphone");
                                }
                            };

                            waveIn.StartRecording();
                            LogService.LogServiceActivity("Started recording from default microphone");

                            Thread.Sleep(5000);
                            waveIn.StopRecording();
                            LogService.LogServiceActivity("Stopped recording from default microphone");

                            if (!hasData)
                            {
                                reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "error_microphone")}: No audio data recorded");
                                LogService.LogServiceActivity("No audio data was recorded during 5 seconds");
                                report = reportBuilder.ToString();
                                return null;
                            }

                            writer.Flush();
                            memoryStream.Position = 0;
                            audioStream = new MemoryStream(memoryStream.ToArray());
                            LogService.LogServiceActivity("Recorded 5 seconds of audio from default microphone successfully");
                        }
                    }
                }
                else
                {
                    reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "microphone_label")} {LanguageService.Translate("answers", "status", "not_connected")}");
                    reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "microphone_not_found")}");
                    LogService.LogServiceActivity("No microphone detected");
                }
            }
            catch (Exception ex)
            {
                reportBuilder.AppendLine($"- {LanguageService.Translate("answers", "status", "microphone_label")} {LanguageService.Translate("answers", "status", "error_microphone")}");
                LogService.LogError(ex, "Failed to check or record from default microphone");
            }

            report = reportBuilder.ToString();
            return audioStream;
        }

        public static string GetWebcamAndMicrophoneStatus()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine(LanguageService.Translate("answers", "status", "webcam_and_microphone"));

            try
            {
                using (var capture = new VideoCapture(0))
                {
                    if (capture.IsOpened())
                    {
                        string webcamName = "Unknown Webcam";
                        using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPEntity WHERE PNPDeviceID LIKE 'USB%' AND Service='usbvideo'"))
                        {
                            var devices = searcher.Get();
                            if (devices.Count > 0)
                            {
                                webcamName = devices.Cast<ManagementObject>().FirstOrDefault()?["Caption"]?.ToString() ?? "Unknown Webcam";
                            }
                        }
                        report.AppendLine($"- {LanguageService.Translate("answers", "status", "webcam_label")} {LanguageService.Translate("answers", "status", "connected")} ({webcamName})");
                        LogService.LogServiceActivity("Webcam detected");
                    }
                    else
                    {
                        report.AppendLine($"- {LanguageService.Translate("answers", "status", "webcam_label")} {LanguageService.Translate("answers", "status", "not_connected")}");
                        LogService.LogServiceActivity("No webcam detected");
                    }
                }
            }
            catch (Exception ex)
            {
                report.AppendLine($"- {LanguageService.Translate("answers", "status", "webcam_label")} {LanguageService.Translate("answers", "status", "error_webcam")}");
                LogService.LogError(ex, "Failed to check webcam");
            }

            try
            {
                if (WaveInEvent.DeviceCount > 0)
                {
                    string microphoneName = "Unknown Microphone";
                    var enumerator = new MMDeviceEnumerator();
                    var defaultDevice = enumerator.GetDefaultAudioEndpoint(DataFlow.Capture, Role.Communications);
                    if (defaultDevice != null)
                    {
                        microphoneName = defaultDevice.FriendlyName;
                    }
                    report.AppendLine($"- {LanguageService.Translate("answers", "status", "microphone_label")} {LanguageService.Translate("answers", "status", "connected")} ({microphoneName})");
                    LogService.LogServiceActivity("Microphone detected");
                }
                else
                {
                    report.AppendLine($"- {LanguageService.Translate("answers", "status", "microphone_label")} {LanguageService.Translate("answers", "status", "not_connected")}");
                    LogService.LogServiceActivity("No microphone detected");
                }
            }
            catch (Exception ex)
            {
                report.AppendLine($"- {LanguageService.Translate("answers", "status", "microphone_label")} {LanguageService.Translate("answers", "status", "error_microphone")}");
                LogService.LogError(ex, "Failed to check microphone");
            }

            LogService.LogServiceActivity("Generated webcam and microphone status report");
            return report.ToString();
        }

        #region Resource report
        public static string GetSystemUsageReport()
        {
            StringBuilder report = new StringBuilder();
            report.AppendLine(LanguageService.Translate("answers", "resource", "report"));
            report.AppendLine(LanguageService.Translate("answers", "resource", "cpu") + GetCpuUsage() + "%");
            report.AppendLine(LanguageService.Translate("answers", "resource", "ram") + GetRamUsage() + "%");
            report.AppendLine(LanguageService.Translate("answers", "resource", "gpu") + GetGpuUsage() + "%");
            report.AppendLine(LanguageService.Translate("answers", "resource", "net") + GetInternetSpeed() + "Mb/s");
            LogService.LogServiceActivity($"GetSystemUsageReport");

            return report.ToString();
        }
        private static float GetCpuUsage()
        {
            var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            cpuCounter.NextValue();
            Thread.Sleep(500); // صبر برای نمونه‌گیری
            return (float)Math.Round(cpuCounter.NextValue(), 2);
        }
        private static float GetRamUsage()
        {
            var ramCounter = new PerformanceCounter("Memory", "% Committed Bytes In Use");
            return (float)Math.Round(ramCounter.NextValue(), 2);
        }
        private static float GetGpuUsage()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_VideoController"))
                {
                    foreach (ManagementObject obj in searcher.Get())
                    {
                        if (obj["AdapterRAM"] != null)
                        {
                            return (float)Math.Round(Convert.ToDouble(obj["AdapterRAM"]) / 1073741824, 2); // تبدیل به گیگابایت
                        }
                    }
                }
            }
            catch (Exception) { }
            return 0; // اگر اطلاعات GPU پیدا نشد
        }
        private static double GetInternetSpeed()
        {
            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();
            long totalBytesReceived1 = 0;

            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    totalBytesReceived1 += ni.GetIPv4Statistics().BytesReceived;
                }
            }

            Thread.Sleep(1000); // یک ثانیه صبر کن تا مقدار جدید دریافت شود

            long totalBytesReceived2 = 0;
            foreach (NetworkInterface ni in interfaces)
            {
                if (ni.OperationalStatus == OperationalStatus.Up)
                {
                    totalBytesReceived2 += ni.GetIPv4Statistics().BytesReceived;
                }
            }

            // محاسبه میزان دانلود در یک ثانیه (بایت)
            long bytesReceivedPerSecond = totalBytesReceived2 - totalBytesReceived1;

            // تبدیل به MB/s (مگابایت بر ثانیه)
            double speedMBps = bytesReceivedPerSecond / (1024.0 * 1024.0);

            return Math.Round(speedMBps, 2);
        }
        #endregion
    }
}
