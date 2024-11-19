using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.UnitTest.TestCases;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace ModengTerm.UnitTest
{
    internal class Program
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("Program");

        static List<MethodInfo> GetUnitTestMethods(Type type) 
        {
            MethodInfo[] methodInfos = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

            // 过滤标记了UnitTestAttribute的方法
            List<MethodInfo> testMethods = new List<MethodInfo>();
            foreach (MethodInfo method in methodInfos)
            {
                UnitTestAttribute unitTestAttribute = method.GetCustomAttribute<UnitTestAttribute>();
                if (unitTestAttribute != null)
                {
                    testMethods.Add(method);
                }
            }

            return testMethods;
        }

        static void RunTestCase(Type type)
        {
            object testObject = Activator.CreateInstance(type);

            List<MethodInfo> testMethods = GetUnitTestMethods(type);

            foreach (MethodInfo methodInfo in testMethods)
            {
                string testName = methodInfo.Name;
                bool result = (bool)methodInfo.Invoke(testObject, null);
                if (result)
                {
                    logger.InfoFormat("{0}:{1}", testName, result);
                }
                else
                {
                    logger.ErrorFormat("{0}:{1}", testName, result);
                }
            }
        }

        static void RunPerformanceTest() 
        {
            TestVideoTerminalPerformance testPerformance = new TestVideoTerminalPerformance();

            Type type = typeof(TestVideoTerminalPerformance);

            List<MethodInfo> testMethods = GetUnitTestMethods(type);

            foreach (MethodInfo methodInfo in testMethods)
            {
                long totalElapsed = 0;
                string testName = methodInfo.Name;

                for (int j = 0; j < 10; j++)
                {
                    int testTimes = 10000;
                    VideoTerminal terminal = UnitTestHelper.CreateVideoTerminal();
                    TermData termData = new TermData()
                    {
                        ViewportRow = terminal.ViewportRow,
                        ViewportColumn = terminal.ViewportColumn
                    };
                    byte[] bytes = (byte[])methodInfo.Invoke(testPerformance, new object[] { termData });

                    Stopwatch stopwatch = Stopwatch.StartNew();
                    for (int i = 0; i < testTimes; i++)
                    {
                        terminal.ProcessData(bytes, bytes.Length);
                    }
                    stopwatch.Stop();

                    logger.InfoFormat("{0}, {1}次, 耗时{2}ms", testName, testTimes, stopwatch.ElapsedMilliseconds);
                    totalElapsed += stopwatch.ElapsedMilliseconds;
                }

                logger.InfoFormat("{0}, 最终测试时间 = {1}", testName, totalElapsed / 10);
            }
        }

        static void Main(string[] args)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            DotNEToolkit.Log4net.InitializeLog4net();

            logger.InfoFormat("--- TestVideoTerminalEngine ---");

            RunTestCase(typeof(TestVideoTerminalEngine));

            logger.InfoFormat("--- TestVideoTerminalAction ---");

            RunTestCase(typeof(TestVideoTerminalAction));

            logger.InfoFormat("--- TestVideoTerminalOptions ---");

            RunTestCase(typeof(TestVideoTerminalOptions));

            logger.InfoFormat("--- TestPerformance ---");

            RunPerformanceTest();

            Console.ReadLine();
        }
    }
}
