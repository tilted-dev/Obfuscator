using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ATS.Obfuscator.Utils
{
    public static class ConsoleWriter
    {
        private static TextBlock _localConsole;

        private static Run MakeRun(string type, object text, Color color)
        {
            return new Run
            {
                Foreground = new SolidColorBrush(color),
                Text = string.Concat(type, text, "\r\n")
            };
        }

        public static void SetConsole(TextBlock console)
        {
            _localConsole = console;
        }
        public static void WriteLine(object value, Color color)
        {
            _localConsole.Inlines.Add(MakeRun(string.Empty, value, color));
        }

        public static void WriteError(object value)
        {
            _localConsole.Inlines.Add(MakeRun("[ERROR] ", value, Colors.Red));
        }

        public static void WriteWarn(object value)
        {
            _localConsole.Inlines.Add(MakeRun("[WARN] ", value, Colors.Magenta));
        }

        public static void WriteComplete(object value)
        {
            _localConsole.Inlines.Add(MakeRun("[COMPLETE] ", value, Colors.Green));
        }

        public static void Clear()
        {
            _localConsole.Inlines.Clear();
        }

    }
}
