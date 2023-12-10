using Azure;
using Azure.AI.OpenAI;

if (Environment.GetCommandLineArgs().Contains<string>("--help")) 
{
    Console.WriteLine("Usage: HelloGpt [--openai-key <key>] [--prompt <prompt>]");
    Console.WriteLine("  --openai-key <key>    OpenAI API key");
    Console.WriteLine("  --prompt <prompt>     Prompt to send to OpenAI");
    Console.WriteLine("");
    Console.WriteLine("  If --openai-key is not specified, the environment variable OPENAI_KEY is used.");
    Console.WriteLine("  If --prompt is not specified, the environment variable PROMPT is used.");
    Console.WriteLine("");
    Console.WriteLine("  If neither --openai-key nor OPENAI_KEY is specified, an exception is thrown.");
    Console.WriteLine("  If neither --prompt nor PROMPT is specified, the default prompt is used.");
    Console.WriteLine("");

    Console.WriteLine("  If the environment variable AZURE_OPENAI_ENDPOINT is specified, it is used as the endpoint.");
    Console.WriteLine("  Otherwise, the default endpoint is used.");
    Console.WriteLine("");
    return;
}

// extract key from command line argument --openai-key into variable openAiKey
string openAiKey = Environment.GetCommandLineArgs()
    .SkipWhile(arg => arg != "--openai-key")
    .Skip<string>(1)
    .FirstOrDefault<string>() ?? string.Empty;

// extract prompt from command line argument --prompt into variable prompt
string prompt = Environment.GetCommandLineArgs()
    .SkipWhile(arg => arg != "--prompt")
    .Skip<string>(1)
    .FirstOrDefault<string>() ?? string.Empty;

// extract string from environment variable PROMPT
if (string.IsNullOrEmpty(prompt)) 
{
    prompt = Environment.GetEnvironmentVariable("PROMPT")
    ?? "Hello, world!";
}

// extract string from environment variable KEY
if (string.IsNullOrEmpty(openAiKey)) 
{
    openAiKey = Environment.GetEnvironmentVariable("OPENAI_KEY")
    ?? throw new Exception("OPENAI_KEY environment variable not set.");
}

OpenAIClient client;
if (Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") is string endpoint)
{
    client = new OpenAIClient(new Uri(endpoint), new AzureKeyCredential(openAiKey));
}
else
{
    client = new OpenAIClient(openAiKey);
}

Console.WriteLine("Sending prompt: " + prompt);
Console.WriteLine("");
Console.WriteLine("Using api key: " + openAiKey);
Console.WriteLine("");
Console.WriteLine("Using endpoint: " + client);

Response<Completions> response = await client.GetCompletionsAsync(new CompletionsOptions()
{
    DeploymentName = "text-davinci-003",
    Prompts = { prompt },
});

Console.WriteLine("Response status code: " + response.GetRawResponse().Status);

foreach (Choice choice in response.Value.Choices)
{
    Console.WriteLine(choice.Text);
}
