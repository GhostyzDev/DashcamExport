using System;
using System.IO;
using System.Media;

namespace FolderWatcherAlert
{
    class Program
    {
        // Path to the folder you want to watch
        private static readonly string FolderToWatch = @"E:\Dashcam\OBS Dashcam";

        // Path to the sound file (WAV). Can be absolute or relative to the .exe
        private static readonly string SoundFilePath = @"E:\DashcamExpot\alert.wav";

        private static SoundPlayer _player;

        static void Main(string[] args)
        {
            
            if (args.Length > 0 && Directory.Exists(args[0]))
            {
                Console.WriteLine($"Using folder from args: {args[0]}");
            }

            if (!Directory.Exists(FolderToWatch))
            {
                Console.WriteLine($"Folder does not exist: {FolderToWatch}");
                Console.WriteLine("Create the folder or change FolderToWatch in the code, then run again.");
                Console.WriteLine("Press Enter to exit...");
                Console.ReadLine();
                return;
            }

            if (!File.Exists(SoundFilePath))
            {
                Console.WriteLine($"Warning: Sound file not found: {SoundFilePath}");
                Console.WriteLine("No sound will play until you place a WAV file at that path.");
            }
            else
            {
                _player = new SoundPlayer(SoundFilePath);
                try
                {
                    _player.Load();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading sound file: {ex.Message}");
                    _player = null;
                }
            }

            Console.WriteLine("========================================");
            Console.WriteLine(" Folder Watcher Alert");
            Console.WriteLine("========================================");
            Console.WriteLine($"Watching folder: {FolderToWatch}");
            Console.WriteLine($"Sound file: {SoundFilePath}");
            Console.WriteLine("Press Enter at any time to quit.");
            Console.WriteLine();

            using (var watcher = new FileSystemWatcher())
            {
                watcher.Path = FolderToWatch;

                // Watch for new files
                watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.CreationTime | NotifyFilters.Size;

                // Only react to files, not directory creations
                watcher.IncludeSubdirectories = false;

                // If you want to filter by extension, set e.g. "*.txt"
                watcher.Filter = "*.*";

                watcher.Created += OnCreated;

                // Start watching
                watcher.EnableRaisingEvents = true;

                // Keep the app alive
                Console.ReadLine();
            }
        }

        private static void OnCreated(object sender, FileSystemEventArgs e)
        {
            // Filter out directory creations just in case
            if (Directory.Exists(e.FullPath))
                return;

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] New file detected: {e.Name}");

            // Play sound if loaded
            if (_player != null)
            {
                try
                {
                    // Play once
                    _player.Play();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error playing sound: {ex.Message}");
                }
            }
        }
    }
}
