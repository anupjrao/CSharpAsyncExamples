namespace AsyncExample04
{
    internal class Program
    {
        /// <summary>
        /// Purpose: To display the usage of async event handlers.
        /// </summary>
        public static event EventHandler<string> OperationCompleted;
        public static async Task DoSomethingAsync()
        {
            await Task.Delay(1000);
            Console.WriteLine("Operation completed.");
            //general signature would have "obj" with a string argument as is specified in the generics.
            OperationCompleted?.Invoke(null,"Operation completed.");
        }
        public static async void HandleAsyncEvent(object? sender, string message)
        {
            Console.WriteLine("Async event handler");
        }
        static async Task Main(string[] args)
        {
            OperationCompleted += (sender, message) =>
            {
                Console.WriteLine("Event handler: " + message);
            };
            OperationCompleted += HandleAsyncEvent;
            await DoSomethingAsync();
        }
        
    }
}
///Notes///
/*
 * OperationCompleted is an event.
 * When our async operation is completed in "DoSomethingAsync".
 * Async void is not for regular methods, it is instead reserved for "async event handlers" as shown in line 16
 * This is because "async void" is a fire and forget mechanism. Event handling is the responsibility of the handler and is decoupled from where the event is fired..
 * In this case after DoSomethingAsync() completes execution of its task, the Event handler is invoked.
 */
