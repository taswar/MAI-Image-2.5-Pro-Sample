using Azure.Core;
using Azure.Identity;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

var endpoint = Environment.GetEnvironmentVariable("AZURE_AI_FOUNDRY_ENDPOINT")
    ?? throw new InvalidOperationException("Set AZURE_AI_FOUNDRY_ENDPOINT.");

var model = Environment.GetEnvironmentVariable("AZURE_AI_IMAGE_MODEL") ?? "MAI-Image-2.5-Pro";

var prompt = """
    A minimalist product photo of a stainless steel water bottle on a plain
    white background, studio lighting, soft shadow beneath the bottle,
    e-commerce catalog style, no text on the label.
    """;

string url = $"{endpoint.TrimEnd('/')}/mai/v1/images/generations?api-version=preview";

var credential = new DefaultAzureCredential();
AccessToken token = await credential.GetTokenAsync(
    new TokenRequestContext(new[] { "https://cognitiveservices.azure.com/.default" }));

using var http = new HttpClient();
http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);

var mai_payload = new
{
    prompt = prompt,
    model = model,
    width = 1024,
    height = 1024,
    output_compression = 100,
    output_format = "png",
    n = 1
};

using var content = new StringContent(
    JsonSerializer.Serialize(mai_payload),
    Encoding.UTF8,
    "application/json");

using HttpResponseMessage response = await http.PostAsync(url, content);
string responseBody = await response.Content.ReadAsStringAsync();

if (!response.IsSuccessStatusCode)
{
    Console.WriteLine($"Image generation failed: {(int)response.StatusCode} {response.ReasonPhrase}");
    Console.WriteLine(responseBody);
    throw new InvalidOperationException($"{model} image generation request failed.");
}

using JsonDocument json = JsonDocument.Parse(responseBody);
string? b64 = json.RootElement
    .GetProperty("data")[0]
    .GetProperty("b64_json")
    .GetString();

if (string.IsNullOrWhiteSpace(b64))
{
    throw new InvalidOperationException("Response did not contain data[0].b64_json.");
}

byte[] imageBytes = Convert.FromBase64String(b64);
string outputPath = Path.Combine(Environment.CurrentDirectory, "product-bottle.png");
await File.WriteAllBytesAsync(outputPath, imageBytes);
Console.WriteLine($"Image saved to: {outputPath}");

// Edit the file and change the bottle color from stainless steel to matte black using the MAI-Image-2.5-Pro model.

var editPrompt = """
    Change the bottle color from stainless steel to matte black.
    Keep the background, lighting, shadow, and camera angle exactly the same.
    """;

string editUrl = $"{endpoint.TrimEnd('/')}/mai/v1/images/edits?api-version=preview";

using var editContent = new MultipartFormDataContent
{
    { new StringContent(editPrompt), "prompt" },
    { new StringContent(model), "model" }
};

byte[] sourceImageBytes = await File.ReadAllBytesAsync(outputPath);
var imagePart = new ByteArrayContent(sourceImageBytes);
imagePart.Headers.ContentType = new MediaTypeHeaderValue("image/png");
editContent.Add(imagePart, "image", "product-bottle.png");

Console.WriteLine("Editing image...");

using HttpResponseMessage editResponse = await http.PostAsync(editUrl, editContent);
string editResponseBody = await editResponse.Content.ReadAsStringAsync();

if (!editResponse.IsSuccessStatusCode)
{
    Console.WriteLine($"Image edit failed: {(int)editResponse.StatusCode} {editResponse.ReasonPhrase}");
    Console.WriteLine(editResponseBody);
    throw new InvalidOperationException($"{model} image edit request failed.");
}

using JsonDocument editJson = JsonDocument.Parse(editResponseBody);
string? editB64 = editJson.RootElement
    .GetProperty("data")[0]
    .GetProperty("b64_json")
    .GetString();

if (string.IsNullOrWhiteSpace(editB64))
{
    throw new InvalidOperationException("Edit response did not contain data[0].b64_json.");
}

byte[] editedImageBytes = Convert.FromBase64String(editB64);
string editedOutputPath = Path.Combine(Environment.CurrentDirectory, "product-bottle-black.png");
await File.WriteAllBytesAsync(editedOutputPath, editedImageBytes);
Console.WriteLine($"Image saved to: {editedOutputPath}");

