using ModengTerm.Document;
using ModengTerm.Terminal;
using ModengTerm.UnitTest.TestCases;
using System.Reflection;
using System.Text;

namespace ModengTerm.UnitTest
{
    internal class Program
    {
        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger("Program");

        static void RunTestCase(Type type)
        {
            object testObject = Activator.CreateInstance(type);

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

            Console.ReadLine();
        }
    }
}
