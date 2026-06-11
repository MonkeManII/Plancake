namespace Test.TestLibrary
{
    internal readonly record struct TestResult
    {
        public readonly bool Success;
        public readonly string? Message;
        public readonly string TestName;
        public readonly bool IsEmpty;

        public TestResult(bool Success, string TestName, string? Message = null)
        {
            this.Success = Success;
            this.Message = Message;
            this.TestName = TestName;
            IsEmpty = false;
        }

        public TestResult()
        {
            TestName = string.Empty;
            IsEmpty = true;
        }
    }
}
