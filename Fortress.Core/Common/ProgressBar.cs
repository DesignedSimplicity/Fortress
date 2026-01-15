namespace Fortress.Core.Common
{
    public class ProgressBar
    {
        public static string Build(double percentComplete)
        {
            var blocks = Convert.ToInt32(Math.Round(percentComplete / 2));
            var progressBar = $"[{new string('▒', blocks)}{new string(' ', 50 - blocks)}]";
            return progressBar;
        }
    }
}
