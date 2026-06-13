using System.Reflection;

namespace Test.TestLibrary
{
    public interface ITest
    {
        public void RunTest();

        public static void RunAll()
        {
            List<Type> tests = [];
            Assembly[] appAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly a in appAssemblies)
            {
                Module[] mod = a.GetModules();
                foreach (Module m in mod)
                {
                    Type[] types = m.GetTypes();
                    foreach (Type t in types)
                    {
                        if (t.IsAssignableTo(typeof(ITest)) && t != typeof(ITest))
                        {
                            tests.Add(t);
                        }
                    }
                }
            }

            TestResult[] results = new TestResult[tests.Count];

            int i = 0;
            foreach (Type t in tests)
            {
                ConstructorInfo? info = t.GetConstructor(Type.EmptyTypes);

                if (info is null)
                {
                    Console.WriteLine($"Type {t.Name} has no parameterless constructor, and cannot be tested.");
                }
                else
                {
                    try
                    {
                        ITest test = (ITest)info.Invoke(null);
                        test.RunTest();
                        results[i] = new(true, t.Name, null);
                    } catch (Exception e)
                    {
                        results[i] = new(false, t.Name, e.Message + "\n" + e.StackTrace);
                    }
                }
                ++i;
            }

            foreach (TestResult result in results)
            {
                if (result.IsEmpty) continue;

                if (result.Success)
                {
                    Console.WriteLine("Test {0} Passed!", result.TestName);
                }
                else
                {
                    Console.WriteLine("Test {0} FAILED: {1}", result.TestName, result.Message);
                }
            }
        }
    }
}
