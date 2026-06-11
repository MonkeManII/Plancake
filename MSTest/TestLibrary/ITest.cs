using System.Reflection;

namespace Test.TestLibrary
{
    public interface ITest
    {
        public bool RunTest(out string? error);

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
                    ITest test = (ITest)info.Invoke(null);
                    results[i] = new(test.RunTest(out string? error), t.Name, error);
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
