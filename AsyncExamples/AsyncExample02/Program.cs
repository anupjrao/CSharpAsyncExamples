namespace AsyncExample02
{
    internal class Program
    {
        /// <summary>
        /// Simulating some asynchronous process that takes some time to complete processing.
        /// </summary>
        public static async Task SimulateSomeLongProcess()
        {
            await Task.Delay(5000);
            Console.WriteLine("Completed some long task");
        }
        public static async Task Main(string[] args)
        {
            await SimulateSomeLongProcess();
        }
    }
}