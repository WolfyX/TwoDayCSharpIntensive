using SingleDayCSharpIntensive;

namespace TwoDaysCSharpIntensive
{
    public class Program
    {
        static void Main(string[] args)
        {
            string telegramBotToken = "536734836:AAG9AWr2Fgk6z3SE4PRFtCLbeczym7xbjhA";
            new CatchNewMessageFeature(telegramBotToken).Catch();
        }
    }
}
