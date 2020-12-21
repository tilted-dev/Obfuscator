using MaterialDesignThemes.Wpf;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ATS.Obfuscator.Utils
{
    public static class MiscUtil 
    {
        public static void ShowSplash()
        {
            var splash = new SplashScreen("Resources/Splash.png");
            splash.Show(false);
            Thread.Sleep(2000);
            splash.Close(TimeSpan.FromMilliseconds(0));
        }
        
        public static object CreateDialog(string message)
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(30)
            };

            stack.Children.Add(new TextBlock
            {
                Text = message,
                FontSize = 15
            });

            var btn = new Button
            {
                Content = "OK",
                Margin = new Thickness(0, 7, 0, 0),
            };

            btn.Click += LocalClick;

            stack.Children.Add(btn);
         
            return stack;
        }
        
        
        public static object CreateObfuscationDialog(string message)
        {
            var stack = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Margin = new Thickness(30),
                MaxHeight = 480,
                MaxWidth = 700
            };

            stack.Children.Add(new TextBlock
            {
                Text = message,
                FontSize = 15,
            });

            stack.Children.Add(new TextBlock
            {
                Text = "Values Rename - защищает строки с помощью преобразования в Base64String, не рекомендуется применять данную защиту без дополнительных параметров обфускации.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 7, 0, 10)
            });
            
            stack.Children.Add(new TextBlock
            {
                Text = "Const Melting - выносит объявление строк/чисел в отдельные статические методы.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 7, 0, 10)
            });
            
            stack.Children.Add(new TextBlock
            {
                Text = "Module Rename - заменяет названия классов, методов, параметров на cлучайно сгенерированные названия.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 7, 0, 10)
            });
            
            stack.Children.Add(new TextBlock
            {
                Text = "Hide Namespace - удаляет все пространства имен из сборки, т.е. объеденяет все классы в одну единую безымянную структуру.",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 7, 0, 10)
            });
            
            stack.Children.Add(new TextBlock
            {
                Text = "Math Change - преобразует числовые значения в математические вычисления по типу (a + b - c + d + ... -).",
                TextWrapping = TextWrapping.Wrap,
                Margin = new Thickness(0, 7, 0, 10)
            });
            
            var btn = new Button
            {
                Content = "OK",
                Margin = new Thickness(0, 7, 0, 0),
            };

            btn.Click += LocalClick;

            stack.Children.Add(btn);

            return stack;
        }

        private static void LocalClick(object sender, RoutedEventArgs e) => DialogHost.CloseDialogCommand.Execute(null, null);
    }
}
