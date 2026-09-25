namespace Catalog.ConsoleApp.Logging;
public static class Logger
{
    private static Action<string, string> logStreamer = (string logType, string message) => {
        using (StreamWriter writer = new StreamWriter("log.txt", append: true))
        {
            string dateString = DateTime.Now.ToString("ddd, dd MMM yyy HH’:’mm’:’ss ‘GMT’");
            String formatMsg = String.Format("{0}: | {1} | {2}", logType, dateString, message);
            writer.WriteLine(formatMsg);
        }
    };

    public static void Debug(string message)
    {
        logStreamer("Debug", message);
    }

    public static void Debug(string message, params object[] values)
    {
        String formatMsg = String.Format(message, values);
        logStreamer("Debug", formatMsg);
    }
}