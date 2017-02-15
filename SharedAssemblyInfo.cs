using MbUnit.Framework;
//[assembly: DefaultTestCaseTimeout(300)]
[assembly: DegreeOfParallelism(15)]
[assembly: Parallelizable(TestScope.All)]