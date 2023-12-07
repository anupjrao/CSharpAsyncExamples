namespace AsyncExample03
{
    /// <summary>
    /// Purpose: This is to demonstrate deadlocks that occur with "ConfigureAwait" being set to true in library code.
    /// </summary>
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = GetDataAsync().Result;
        }
        private static async Task<string> GetDataAsync()
        {
            using (var httpClient = new HttpClient())
            using (var httpResonse = await httpClient.GetAsync("https://www.google.com").ConfigureAwait(false))
            {
                return await httpResonse.Content.ReadAsStringAsync().ConfigureAwait(false);
            }
        }

        private static async Task<string> CauseDeadlockAsync()
        {
            using (var httpClient = new HttpClient())
            using (var httpResonse = await httpClient.GetAsync("https://www.google.com").ConfigureAwait(true))
            {
                return await httpResonse.Content.ReadAsStringAsync().ConfigureAwait(true);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.textBox2.Text = "String added at " + DateTime.Now.ToString("hh:mm:ss");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            this.textBox3.Text = CauseDeadlockAsync().Result;
        }

        private async void button4_Click(object sender, EventArgs e)
        {
            var res = await CauseDeadlockAsync();
            this.textBox4.Text = res;
        }

    }
}
/*----Notes----*/
/*
 * NOTE: This happens only when we try to execute things synchronously i.e. not using "await" but instead using ".Result"
 * 1. We have two asynchronous methods that do not block calls "GetDataAsync()" and "CauseDeadlockAsync()" at lines 19 and 29 respectively.
 * 2. They are identical functions, basically no difference other than the way "ConfigureAwait is written". Both of them just make a request to "https://www.google.com".
 * 3. There are three text-boxes here. The first one makes a call with ConfigureAwait set to false, the third one makes a call with ConfigureAwait set to true.
 * 4. The second text-box is just to check responsiveness of the form.
 * 
 * Instructions to test:
 * 1. Click the first button, text is filled in the first text-box after a delay. You are still able to click the second button while this happens, i.e. UI thread is not blocked.
 * 2. Click the third button, the UI just freezes this is due to a deadlock.
 * 
 * Reasoning behind this:
 * UI Applications work in the following way:
 * There is a UI thread which handles all the UI related interaction, i.e. the UI is updated by this thread. If this thread is blocked, the application becomes unresponsive.
 * 
 * The UI thread performs actions based on a message queue.
 * 
 * The UI thread has a synchronization context by default. If we are in a synchronization context, i.e. we're in the UI thread, code after await will execute on the same thread context.
 * 
 * Modifying the UI from any thread other than the UI thread causes the "System.InvalidOperationException". This happens here in the function for button 4, at line 52. When clicked, box 4 no longer takes input.

/////This is what gets executed in the "GetDataAsync()" function.

var currentContext = SynchronizationContext.Current;
var httpResponseTask = httpClient.GetAsync("https://www.google.com");
httpResponseTask.ContinueWith(delegate
{
    if (currentContext == null)
    {
        return await httpResonse.Content.ReadAsStringAsync();
    }    
    else
    {
        currentContext.Post(delegate { 
            await httpResonse.Content.ReadAsStringAsync(); 
        }, null);
     }
//This post call sends a message to the UI Thread message pump, 
to finish it, it is mandatory for ***UI thread*** to execute await the ReadAsStringAsync(). 
Therefore when we do GetDataAsync().Result, the UI thread can not process this since it is already blocked above and has not finished.
ConfigureAwait(false) configures the task so that the continuation task after await doesn't have to be run in the caller/ UI context.
Usually the continuation task after await runs on the same thread context as the caller.
}, TaskScheduler.Current);

//////////////////////////////////////////////////////////////

NOTE: 

    await asynchronously unwraps the result of your task, whereas just using Result would block until the task had completed.

///////////////////////////////////////////////////////////////

Any method declared as async has to have a return type of:

    void (avoid if possible)
    Task (no result beyond notification of completion/failure)
    Task<T> (for a logical result of type T in an async manner)

The compiler does all the appropriate wrapping. The point is that you're asynchronously returning urlContents.Length - you can't make the method just return int, as the actual method will return when it hits the first await expression which hasn't already completed. So instead, it returns a Task<int> which will complete when the async method itself completes.

Note that await does the opposite - it unwraps a Task<T> to a T value, which is how this line works:

string urlContents = await getStringTask;

... but of course it unwraps it asynchronously, whereas just using Result would block until the task had completed. (await can unwrap other types which implement the awaitable pattern, but Task<T> is the one you're likely to use most often.)

This dual wrapping/unwrapping is what allows async to be so composable. For example, I could write another async method which calls yours and doubles the result:

public async Task<int> AccessTheWebAndDoubleAsync()
{
    var task = AccessTheWebAsync();
    int result = await task;
    return result * 2;
}

(Or simply return await AccessTheWebAsync() * 2; of course.)

Ref: Jon Skeet
///////////////////////////////////////////////////////////////
 * 
 * ref: https://medium.com/bynder-tech/c-why-you-should-use-configureawait-false-in-your-library-code-d7837dce3d7f
*/