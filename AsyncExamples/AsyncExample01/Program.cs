namespace AsyncExample01
{
    internal class Program
    {
        /// <summary>
        /// This method simulates a request being made to a site asynchronously.
        /// </summary>
        /// <returns>The response from the request, stringified</returns>
        public static async Task<string> GetDataAsync()
        {
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://google.com/");
                return await response.Content.ReadAsStringAsync();
            }
        }
        public static async Task Main(string[] args)
        {
            var result = await GetDataAsync();
            Console.WriteLine(result);
        }
    }
}