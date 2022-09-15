﻿using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using Updater;
using WPFCustomMessageBox;

namespace LuniiKit
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            WinSparkle.win_sparkle_set_appcast_url("https://raw.githubusercontent.com/Seph29/LuniiKit_App/master/docs/update.ver");
            WinSparkle.win_sparkle_init();
            StartApp();
        }
        private void StartApp()
        {
            Properties.Settings.Default.folderstudio = false;
            Properties.Settings.Default.folderspg = false;
            Closing += MainWindow_Closing;
            Vertext.IsDocumentEnabled = true;
            Vertext.Document.Blocks.FirstBlock.Margin = new Thickness(0);
            string nompartiel = "SNAPSHOT";
            DirectoryInfo rechercherepertoire = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            FileSystemInfo[] fichieretrepertoire = rechercherepertoire.GetFileSystemInfos("*" + nompartiel);
            foreach (FileSystemInfo fichiertrouve in fichieretrepertoire)
            {
                Studio3.IsEnabled = true;
            }
            string nompartiel2 = "spg";
            DirectoryInfo rechercherepertoire2 = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            FileSystemInfo[] fichieretrepertoire2 = rechercherepertoire2.GetFileSystemInfos(nompartiel2);
            foreach (FileSystemInfo fichiertrouve2 in fichieretrepertoire2)
            {
                Spg.IsEnabled = true;
                Properties.Settings.Default.folderspg = true;
            }
            string luniiadminPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "lunii-admin_windows_amd64.exe");
            if (File.Exists(luniiadminPath))
            {
                Luniiadmin.IsEnabled = true;
            }
            Vertext.Document.Blocks.Clear();
            Vertext.Document.Blocks.Add(new Paragraph(new Run("LuniiKit Version : " + FileVersionInfo.GetVersionInfo(System.Reflection.Assembly.GetExecutingAssembly().Location).FileVersion)));

            var fixoldfolder = "LuniiKit.exe_StrongName_";
            var fixdirs = Directory.GetDirectories(AppDomain.CurrentDomain.BaseDirectory, $"{fixoldfolder}*");
            foreach (string dirs in fixdirs)
            {
                if (Directory.Exists(dirs))

                {
                    Directory.Delete(dirs, true);
                }
            }
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            if (CustomMessageBox.ShowYesNo(Application.Current.MainWindow, "Voulez vous quitter LuniiKit?", "Quitter", "Oui", "Non", MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                if (File.Exists("STUdio.bat"))
                {
                    File.Delete("STUdio.bat");
                }
                WinSparkle.win_sparkle_cleanup();
                Environment.Exit(0);
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = FindResource("JavaInstallButton") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }
        private void Jdk11_Click(object sender, RoutedEventArgs e)
        {
            string strCmdText;
            strCmdText = "/C winget install EclipseAdoptium.Temurin.11";
            Process.Start("CMD.exe", strCmdText);
        }
        private void Jdk17_Click(object sender, RoutedEventArgs e)
        {
            string strCmdText;
            strCmdText = "/C winget install EclipseAdoptium.Temurin.17";
            Process.Start("CMD.exe", strCmdText);
        }
        private void Idriver_Click(object sender, RoutedEventArgs e)
        {
            if (Environment.Is64BitOperatingSystem == true)
            {
                ProcessStartInfo process = new ProcessStartInfo("dpinst64.exe")
                {
                    WorkingDirectory = @"driver",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(process);
            }
            else
            {
                ProcessStartInfo process = new ProcessStartInfo("dpinst32.exe")
                {
                    WorkingDirectory = @"driver",
                    UseShellExecute = true,
                    Verb = "runas"
                };
                Process.Start(process);
            }
        }
        private void Studio1_Click(object sender, RoutedEventArgs e)
        {
            {
                StreamWriter SW = new StreamWriter("STudio.bat");
                SW.WriteLine(@"@echo off
mode con cols=120 lines=30
""%__APPDIR__%chcp.com"" 850>nul
color 9");
                SW.WriteLine(@"title STUdio " + Properties.Settings.Default.studio1ver);
                SW.WriteLine(@"set version_LUNII=" + Properties.Settings.Default.studio1ver);
                if (Properties.Settings.Default.studioportable == true)
                {
                    SW.WriteLine(@"set STUDIO_PATH=%~dps0
set DOT_STUDIO=%~dps0.studio
set PATH=%STUDIO_PATH%jre11\bin;%PATH%");
                }
                else
                {
                    SW.WriteLine(@"set STUDIO_PATH=%~dps0
set DOT_STUDIO=""%UserProfile%\.studio""
PATH %PATH%;%JAVA_HOME%\bin\
java -version 1>nul 2>nul || (
   echo Java n'est pas detecte sur votre OS
   pause
   exit /b 2
)
for /f tokens^=2-5^ delims^=.-_^"" %%j in ('java -fullversion 2^>^&1') do set ""jver=%%j%%k%%l%%m""

if %jver% LSS 110000 (
  echo Vous n'avez pas la bonne version de Java
  echo merci d'installer Java 11
  pause
  exit /b 1
)");
                }
                SW.WriteLine(@"IF EXIST %DOT_STUDIO%\db\official.json del %DOT_STUDIO%\db\official.json");
                if (Properties.Settings.Default.eraselog == true)
                {
                    SW.WriteLine(@"echo Suppression des logs");
                    SW.Write(@"for /f ""skip=" + Properties.Settings.Default.nlogs);
                    SW.Write(@" delims="" %%A in ('dir /a:-d /b /o:-d /t:c *.log ^2^>nul') do if exist ""%%~fA"" del ""%%~fA""");
                    SW.WriteLine("");
                }
                else
                {
                    SW.WriteLine("");
                }
                SW.WriteLine(@"if not exist %DOT_STUDIO%\db\* mkdir %DOT_STUDIO%\db
if not exist %DOT_STUDIO%\library\* mkdir %DOT_STUDIO%\library
if not exist %DOT_STUDIO%\agent\* mkdir %DOT_STUDIO%\agent

copy %STUDIO_PATH%\agent\studio-agent-%version_LUNII%-jar-with-dependencies.jar %DOT_STUDIO%\agent\studio-agent.jar
copy %STUDIO_PATH%\agent\studio-metadata-%version_LUNII%-jar-with-dependencies.jar %DOT_STUDIO%\agent\studio-metadata.jar");
                if (Properties.Settings.Default.studioportable == true)
                {
                    SW.WriteLine(@"java -Duser.home=%STUDIO_PATH% -Dvertx.disableDnsResolver=true -Djava.util.logging.manager=org.apache.logging.log4j.jul.LogManager -Dvertx.logger-delegate-factory-class-name=io.vertx.core.logging.Log4j2LogDelegateFactory -Dfile.encoding=UTF-8 -cp %STUDIO_PATH%/studio-web-ui-%version_LUNII%.jar;%STUDIO_PATH%/lib-%version_LUNII%/*;. io.vertx.core.Launcher run studio.webui.MainVerticle");
                }
                else
                {
                    SW.WriteLine(@"java -Dvertx.disableDnsResolver=true -Djava.util.logging.manager=org.apache.logging.log4j.jul.LogManager -Dvertx.logger-delegate-factory-class-name=io.vertx.core.logging.Log4j2LogDelegateFactory -Dfile.encoding=UTF-8 -cp %STUDIO_PATH%/studio-web-ui-%version_LUNII%.jar;%STUDIO_PATH%/lib-%version_LUNII%/*;. io.vertx.core.Launcher run studio.webui.MainVerticle");
                }
                SW.Flush();
                SW.Close();
                Process.Start("STudio.bat");
            }
        }
        private void Studio2_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter SW = new StreamWriter("STudio.bat");
            SW.WriteLine(@"@echo off");
            SW.WriteLine(@"set STUDIO_HOST=" + Properties.Settings.Default.confhost);
            SW.WriteLine(@"set STUDIO_PORT=" + Properties.Settings.Default.confport);
            SW.WriteLine(@"set STUDIO_DB_OFFICIAL=" + Properties.Settings.Default.offdb);
            SW.WriteLine(@"set STUDIO_DB_UNOFFICIAL=" + Properties.Settings.Default.unoffdb);
            SW.WriteLine(@"set STUDIO_LIBRARY=" + Properties.Settings.Default.library);
            SW.WriteLine(@"set STUDIO_TMPDIR=" + Properties.Settings.Default.tmp);
            SW.WriteLine(@"mode con cols=120 lines=30
""%__APPDIR__%chcp.com"" 850>nul
color B");
            SW.WriteLine(@"title STUdio " + Properties.Settings.Default.studio2ver);
            SW.WriteLine(@"set version_LUNII=" + Properties.Settings.Default.studio2ver);
            if (Properties.Settings.Default.studioportable == true)
            {
                SW.WriteLine(@"set STUDIO_PATH=%~dps0
set DOT_STUDIO=%~dps0.studio
set PATH=%STUDIO_PATH%jre11\bin;%PATH%");
            }
            else
            {
                SW.WriteLine(@"set STUDIO_PATH=%~dps0
set DOT_STUDIO=""%UserProfile%\.studio""
PATH %PATH%;%JAVA_HOME%\bin\
java -version 1>nul 2>nul || (
   echo Java n'est pas detecte sur votre OS
   pause
   exit /b 2
)
for /f tokens^=2-5^ delims^=.-_^"" %%j in ('java -fullversion 2^>^&1') do set ""jver=%%j%%k%%l%%m""

if %jver% LSS 110000 (
  echo Vous n'avez pas la bonne version de Java
  echo merci d'installer Java 11
  pause
  exit /b 1
)");
            }
            SW.WriteLine(@"IF EXIST %DOT_STUDIO%\db\official.json del %DOT_STUDIO%\db\official.json");
            SW.WriteLine(@"IF EXIST %STUDIO_DB_OFFICIAL% del %STUDIO_DB_OFFICIAL%");
            if (Properties.Settings.Default.eraselog == true)
            {
                SW.WriteLine(@"echo Suppression des logs");
                SW.Write(@"for /f ""skip=" + Properties.Settings.Default.nlogs);
                SW.Write(@" delims="" %%A in ('dir /a:-d /b /o:-d /t:c *.log ^2^>nul') do if exist ""%%~fA"" del ""%%~fA""");
                SW.WriteLine("");
            }
            else
            {
                SW.WriteLine("");
            }
            SW.WriteLine(@"if not exist %DOT_STUDIO%\db\* mkdir %DOT_STUDIO%\db
if not exist %DOT_STUDIO%\library\* mkdir %DOT_STUDIO%\library");
            if (Properties.Settings.Default.studioportable == true)
            {
                SW.WriteLine(@"java -Duser.home=%STUDIO_PATH% -Dvertx.disableDnsResolver=true -Dfile.encoding=UTF-8 ^
 -cp ""%STUDIO_PATH%/studio-web-ui-%version_LUNII%.jar"";""%STUDIO_PATH%/lib-%version_LUNII%/*"";. ^
 io.vertx.core.Launcher run studio.webui.MainVerticle");
            }
            else
            {
                SW.WriteLine(@"java -Dvertx.disableDnsResolver=true -Dfile.encoding=UTF-8 ^
 -cp ""%STUDIO_PATH%/studio-web-ui-%version_LUNII%.jar"";""%STUDIO_PATH%/lib-%version_LUNII%/*"";. ^
 io.vertx.core.Launcher run studio.webui.MainVerticle");
            }
            SW.Flush();
            SW.Close();
            Process.Start("STudio.bat");
        }
        private void Studio3_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter SW = new StreamWriter("STudio.bat");
            SW.WriteLine(@"@echo off");
            SW.WriteLine(@"set STUDIO_HOST=" + Properties.Settings.Default.confhost);
            SW.WriteLine(@"set STUDIO_PORT=" + Properties.Settings.Default.confport);
            SW.WriteLine(@"set STUDIO_DB_OFFICIAL=" + Properties.Settings.Default.offdb);
            SW.WriteLine(@"set STUDIO_DB_UNOFFICIAL=" + Properties.Settings.Default.unoffdb);
            SW.WriteLine(@"set STUDIO_LIBRARY=" + Properties.Settings.Default.library);
            SW.WriteLine(@"set STUDIO_TMPDIR=" + Properties.Settings.Default.tmp);
            SW.WriteLine(@"mode con cols=120 lines=30
""%__APPDIR__%chcp.com"" 850>nul
color D");
            SW.WriteLine(@"title STUdio " + Properties.Settings.Default.studio3ver);
            SW.WriteLine(@"set version_LUNII=" + Properties.Settings.Default.studio3ver);
            if (Properties.Settings.Default.studioportable == true)
            {
                SW.WriteLine(@"set STUDIO_PATH=%~dps0
set DOT_STUDIO=%~dps0.studio
set PATH=%STUDIO_PATH%jre11\bin;%PATH%");
            }
            else
            {
                SW.WriteLine(@"set STUDIO_PATH=%~dps0
set DOT_STUDIO=""%UserProfile%\.studio""
PATH %PATH%;%JAVA_HOME%\bin\
java -version 1>nul 2>nul || (
   echo Java n'est pas detecte sur votre OS
   pause
   exit /b 2
)
for /f tokens^=2-5^ delims^=.-_^"" %%j in ('java -fullversion 2^>^&1') do set ""jver=%%j%%k%%l%%m""

if %jver% LSS 110000 (
  echo Vous n'avez pas la bonne version de Java
  echo merci d'installer Java 11
  pause
  exit /b 1
)");
            }
            SW.WriteLine(@"IF EXIST %DOT_STUDIO%\db\official.json del %DOT_STUDIO%\db\official.json");
            SW.WriteLine(@"IF EXIST %STUDIO_DB_OFFICIAL% del %STUDIO_DB_OFFICIAL%");
            if (Properties.Settings.Default.eraselog == true)
            {
                SW.WriteLine(@"echo Suppression des logs");
                SW.Write(@"for /f ""skip=" + Properties.Settings.Default.nlogs);
                SW.Write(@" delims="" %%A in ('dir /a:-d /b /o:-d /t:c *.log ^2^>nul') do if exist ""%%~fA"" del ""%%~fA""");
                SW.WriteLine("");
            }
            else
            {
                SW.WriteLine("");
            }
            SW.WriteLine(@"if not exist %DOT_STUDIO%\db\* mkdir %DOT_STUDIO%\db
if not exist %DOT_STUDIO%\library\* mkdir %DOT_STUDIO%\library");
            if (Properties.Settings.Default.studioportable == true)
            {
                SW.WriteLine(@"java -Duser.home=%STUDIO_PATH% -Dvertx.disableDnsResolver=true -Dfile.encoding=UTF-8 ^
 -cp ""%STUDIO_PATH%/studio-web-ui-%version_LUNII%.jar"";""%STUDIO_PATH%/lib-%version_LUNII%/*"";. ^
 io.vertx.core.Launcher run studio.webui.MainVerticle");
            }
            else
            {
                SW.WriteLine(@"java -Dvertx.disableDnsResolver=true -Dfile.encoding=UTF-8 ^
 -cp ""%STUDIO_PATH%/studio-web-ui-%version_LUNII%.jar"";""%STUDIO_PATH%/lib-%version_LUNII%/*"";. ^
 io.vertx.core.Launcher run studio.webui.MainVerticle");
            }
            SW.Flush();
            SW.Close();
            Process.Start("STudio.bat");
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.com/invite/jg9MjHBWQC");
        }
        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/Seph29/LuniiKit_App");
        }
        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/marian-m12l/studio");
        }
        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/kairoh/studio");
        }
        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/jersou/studio-pack-generator");
        }
        private void Button_Click_8(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/olup/lunii-admin");
        }
        private void OpenWindow(object sender, RoutedEventArgs e)
        {
            LuniiKitOptions objOptions = new LuniiKitOptions();
            objOptions.ShowDialog();
        }
        private void Githubchoix(object sender, RoutedEventArgs e)
        {
            ContextMenu cm = FindResource("Github") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }
        private void FolderChoice(object sender, RoutedEventArgs e)
        {
            if (Properties.Settings.Default.studioportable == true)
            {
                string nompartiel = ".studio";
                DirectoryInfo rechercherepertoire = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                FileSystemInfo[] fichieretrepertoire = rechercherepertoire.GetFileSystemInfos(nompartiel);
                foreach (FileSystemInfo fichiertrouve in fichieretrepertoire)
                {
                    Properties.Settings.Default.folderstudio = true;
                }
            }
            else
            {
                string nompartiel = ".studio";
                DirectoryInfo rechercherepertoire = new DirectoryInfo(Environment.GetEnvironmentVariable("USERPROFILE"));
                FileSystemInfo[] fichieretrepertoire = rechercherepertoire.GetFileSystemInfos(nompartiel);
                foreach (FileSystemInfo fichiertrouve in fichieretrepertoire)
                {
                    Properties.Settings.Default.folderstudio = true;
                }

            }
            ContextMenu cm = FindResource("Choixfolder") as ContextMenu;
            cm.PlacementTarget = sender as Button;
            cm.IsOpen = true;
        }
        private void Luniiadmin_Click(object sender, RoutedEventArgs e)
        {
            Process process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "lunii-admin_windows_amd64.exe";
            process.Start();
        }
        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            Process.Start("explorer.exe", path);
        }
        private void OpenFolderSPG(object sender, RoutedEventArgs e)
        {
            string path = Directory.GetCurrentDirectory();
            string spgfolder = path + "\\spg";
            Process.Start("explorer.exe", spgfolder);
        }
        private void OpenSTUdioFolder(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Properties.Settings.Default.library))
            {
                Process.Start("explorer.exe", Properties.Settings.Default.library);
            }
            else
            {
                if (Properties.Settings.Default.studioportable == true)
                {
                    string path = Directory.GetCurrentDirectory();
                    string studiofolder = path + "\\.studio\\library";
                    Process.Start("explorer.exe", studiofolder);
                }
                else
                {
                    string path = Environment.GetEnvironmentVariable("USERPROFILE");
                    string studiofolder = path + "\\.studio\\library";
                    Process.Start("explorer.exe", studiofolder);
                }
            }
        }
        public static void CopyAll(DirectoryInfo source, DirectoryInfo target)
        {
            Directory.CreateDirectory(target.FullName);
            foreach (FileInfo fi in source.GetFiles())
            {
                fi.CopyTo(Path.Combine(target.FullName, fi.Name), true);
            }
            foreach (DirectoryInfo diSourceSubDir in source.GetDirectories())
            {
                DirectoryInfo nextTargetSubDir =
                target.CreateSubdirectory(diSourceSubDir.Name);
                CopyAll(diSourceSubDir, nextTargetSubDir);
            }
        }
        private void USB_Copy(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            folderDialog.Description = "Choisir le dossier où copier LuniiKit";
            if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
            {
                //Cancel
            }
            else
            {
                string spath = folderDialog.SelectedPath;
                string appfolder = Directory.GetCurrentDirectory();
                DirectoryInfo AppFolder = new DirectoryInfo(appfolder);
                if (spath != null || spath.Length != 0)
                {
                    DirectoryInfo sPath = new DirectoryInfo(spath);
                    {
                        try
                        {
                            CopyAll(AppFolder, sPath);
                            CustomMessageBox.Show(Application.Current.MainWindow, "Copy OK !");
                            Process.Start("explorer.exe", spath);
                        }
                        catch (FileNotFoundException ex)
                        {
                            CustomMessageBox.Show(Application.Current.MainWindow, ex.Message);
                        }
                    }
                }
            }
        }
        private void OpenSPG(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            string path = Directory.GetCurrentDirectory();
            string spgfolder = path + "\\spg";
            folderDialog.SelectedPath = spgfolder;
            MessageBoxResult result = CustomMessageBox.ShowYesNoCancel(Application.Current.MainWindow, "Vous voulez utiliser :", "Studio Pack Generator", "Un dossier", "Une URL", "Annuler", MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                folderDialog.Description = "Choisir le dossier du pack à créer";
                if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    //Cancel
                }
                else
                {
                    string spath = folderDialog.SelectedPath;
                    var dialog = new InputBox();
                    dialog.LabelResponseTextBox2.Visibility = Visibility.Hidden;
                    dialog.ResponseTextBox2.Visibility = Visibility.Hidden;
                    dialog.ResponseTextBox2.Margin = new Thickness(5, 2, 5, 0);
                    dialog.ResponseTextBox.Margin = new Thickness(5, 0, 5, 0);
                    dialog.LabelResponseTextBox.Margin = new Thickness(0, 0, 0, 0);
                    dialog.InputOk.Margin = new Thickness(0, -10, 0, 0);
                    if (dialog.ShowDialog() == true)
                    {
                        Process process = new Process();
                        process.StartInfo.FileName = spgfolder + "\\studio-pack-generator-x86_64-windows.exe";
                        process.StartInfo.Arguments = dialog.ResponseText + " " + "\"" + spath + "";
                        process.Start();
                        process.WaitForExit();
                        CustomMessageBox.Show(Application.Current.MainWindow, "Done !");
                    }
                }
            }
            else if (result == MessageBoxResult.No)
            {
                var dialog = new InputBox();
                if (dialog.ShowDialog() == true)
                {
                    Process process = new Process();
                    process.StartInfo.FileName = spgfolder + "\\studio-pack-generator-x86_64-windows.exe";
                    process.StartInfo.Arguments = dialog.ResponseText + " " + dialog.ResponseText2;
                    process.Start();
                    process.WaitForExit();
                    CustomMessageBox.Show(Application.Current.MainWindow, "Done !");

                }
            }
        }
    }
}