namespace AsyncExample05
{
    /// <summary>
    /// Purpose: To explore usage of Task.WhenAll, Task.WhenAny to run tasks concurrently.
    /// </summary>
    internal class Program
    {
        public static async Task<string> DoFirstTaskAsync()
        {
            await Task.Delay(3000);
            return "Task one complete.";
        }
        public static async Task<string> DoSecondTaskAsync()
        {
            await Task.Delay(2000);
            return "Task two complete.";
        }
        public static async Task<string> DoThirdTaskAsync()
        {
            await Task.Delay(5000);
            return "Task three complete.";
        }
        static async Task Main(string[] args)
        {
            //Task WhenAll vs WaitAll


            //Task.WaitAll blocks the current thread until everything has completed.

            //Task.WhenAll returns a task which represents the action of waiting until everything has completed.
            //Task.WaitAll throws an AggregateException when any of the tasks throws and you can examine all thrown exceptions.
            //The await in await Task.WhenAll unwraps the AggregateException and 'returns' only the first exception.
            var tasks = new List<Task<string>>
                        {
                            DoFirstTaskAsync(),
                            DoSecondTaskAsync(),
                            DoThirdTaskAsync()
                        };
            //Here we await the completion of all tasks. They're happening parallely.
            string[] results = await Task.WhenAll(tasks);
            
            foreach (string result in results)
            {
                Console.WriteLine(result);
            }
        }
    }
}