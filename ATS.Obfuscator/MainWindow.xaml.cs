using ATS.Obfuscator.Utils;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ATS.Obfuscator.Obfuscation;
using ATS.Obfuscator.Obfuscation.Protections;
using System.Diagnostics;

namespace ATS.Obfuscator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly Dictionary<Type, CheckBox> _obfParams;
        
        public MainWindow()
        {
            MiscUtil.ShowSplash();
            
            InitializeComponent();
            
            _obfParams = new Dictionary<Type, CheckBox>
            {
                {typeof(StringProtection), ValuesRename},
                {typeof(NamespaceProtection), HideNamespace},
                {typeof(ConstantProtection), ConstMelting},
                {typeof(MathProtection), MathChange},
                {typeof(ModuleProtection), ModuleRename},
            };
            
            ConsoleWriter.SetConsole(ConsoleInput);
            ConsoleWriter.WriteLine("Привет, Джакс!", Colors.Blue);
        }
        
        private void SelectFilesClicked(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Executable files (*.exe)|*.exe|Library files (*.dll)|*.dll",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if(openFileDialog.ShowDialog() == true)
            {
                openFileDialog.FileNames.ToList().ForEach(f =>
                {
                    if (Assemblies.Items.Contains(f))
                    {
                        ConsoleWriter.WriteWarn($"Файл {Path.GetFileName(f)} уже существует.");
                        return;
                    }
                    
                    Assemblies.Items.Add(f);
                    ConsoleWriter.WriteComplete($"Загружен файл {Path.GetFileName(f)}.");
                });
            }
            
        }

        private async void ObfuscateClicked(object sender, RoutedEventArgs e)
        {
            if (Assemblies.Items.Count == 0)
            {
                await DialogHost.Show(MiscUtil.CreateDialog("Вам необходимо выбрать файлы!"));
                return;
            }

            if (!_obfParams.Any(c => c.Value.IsChecked != null && c.Value.IsChecked.Value))
            {
                await DialogHost.Show(MiscUtil.CreateDialog("Вам необходимо выбрать параметры обфускации!"));
                return;
            }

            var obfuscateParams = _obfParams
                .Where(c => c.Value.IsChecked.HasValue && c.Value.IsChecked.Value)
                .Select(c => c.Key);

            if (Assemblies.SelectedItems.Count == 0)
            {
                ConsoleWriter.WriteWarn("Нет выбранных файлов, обфускация будет происходит в порядке очереди.");

                await Assemblies.Items.Cast<string>().ForEachAsync(s => GlobalObfuscation.RunObfuscation(s, obfuscateParams));
                await DialogHost.Show(MiscUtil.CreateDialog($"Успешно обфусцировано {Assemblies.Items.Count} файлов."));
                
                return;
            }

            ConsoleWriter.WriteLine($"Выбрано {Assemblies.SelectedItems.Count} файлов.", Colors.Blue);

            await Assemblies.SelectedItems.Cast<string>().ForEachAsync(s => GlobalObfuscation.RunObfuscation(s, obfuscateParams));
            await DialogHost.Show(MiscUtil.CreateDialog($"Успешно обфусцировано {Assemblies.SelectedItems.Count} файлов."));
        }
        
        private void OnDropFiles(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

            ((string[])e.Data.GetData(DataFormats.FileDrop) ?? new []{""}).ToList().ForEach(f =>
            {
                if (Assemblies.Items.Contains(f))
                {
                    ConsoleWriter.WriteWarn($"Файл {Path.GetFileName(f)} уже существует.");
                    return;
                }
                
                Assemblies.Items.Add(f);
                ConsoleWriter.WriteComplete($"Загружен файл {Path.GetFileName(f)}.");
            });
        }

        private void ClearAssemblies(object sender, RoutedEventArgs e)
        {
            Assemblies.Items.Clear();
        }

        private void OpenFolder(object sender, RoutedEventArgs e)
        {
            if (!Directory.Exists(GlobalObfuscation.SaveDir)) Directory.CreateDirectory(GlobalObfuscation.SaveDir);

            Process.Start(GlobalObfuscation.SaveDir);
        }

        private void ClearConsole(object sender, RoutedEventArgs e)
        {
            ConsoleWriter.Clear();
        }

        private void InformationClicked(object sender, RoutedEventArgs e)
        {
            DialogHost.Show(MiscUtil.CreateObfuscationDialog("Информация по методам защиты."));
        }
    }
}
