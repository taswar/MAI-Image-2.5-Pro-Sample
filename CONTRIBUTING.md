# Contributing

Thanks for helping improve this sample. It's a small .NET 10 console app that generates a product image with MAI-Image-2.5-Pro on Microsoft Foundry and then edits it, calling the `/mai/v1/images` REST routes with `HttpClient` and Entra ID. Contributions that make it clearer, more correct, or more useful to other .NET developers are welcome.

## Ways to contribute

- **Report a bug.** A call fails, or the README doesn't match what the code does.
- **Fix the docs.** Typos, unclear steps, or a missing troubleshooting case (for example, an error code you hit that isn't listed).
- **Improve the sample.** For example, a different image size, a second edit, or clearer error output.
- **Share results.** If you tried other prompts or edits, an issue describing what worked helps others.

For anything larger than a small fix, open an issue first so we can agree on the approach before you write code.

## Set up

1. Install the [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) and the Azure CLI.
2. Clone the repo and build it:

   ```bash
   git clone https://github.com/taswar/MAI-Image-2.5-Pro-Sample.git
   cd MAI-Image-2.5-Pro-Sample
   dotnet build
   ```

3. To run it, deploy MAI-Image-2.5-Pro on a Microsoft Foundry resource, set `AZURE_AI_FOUNDRY_ENDPOINT` (and `AZURE_AI_IMAGE_MODEL` if your deployment isn't named `MAI-Image-2.5-Pro`), and sign in with `az login`, as described in the [README](README.md#configuration). Your account needs the **Cognitive Services User** role on the resource. The sample uses Entra ID only, with no API key.

## Guidelines

- **Never commit secrets.** No keys, endpoints, tenant IDs or tokens in code, config files or screenshots.
- **Keep the MAI routes.** MAI image models use `/mai/v1/images/generations` and `/mai/v1/images/edits`, not the OpenAI image routes. Don't switch to the SDK's `ImageClient`, which returns 404 for these models.
- **Keep Entra ID.** The sample uses `DefaultAzureCredential` with no API key. Keep it that way.
- **Keep the error output.** Failed calls print the status code and the response body. Keep that for any new call, so problems are easy to diagnose.
- **Keep generated images small.** If you replace the example images, keep them PNG and under 1 MB each, and keep the same file names so the README still shows them.
- **Keep it a sample.** The code should stay short and readable in one `Program.cs`. Prefer clarity over abstraction, and add a dependency only when it clearly helps.
- **Match the existing style.** Target `net10.0`, keep nullable reference types on, and follow the layout already in `Program.cs`.
- **Keep the README in sync.** If you change settings, routes or output, update the README in the same pull request.
- **One change per pull request.** Small, focused pull requests are easier to review and merge.

## Pull request checklist

- [ ] `dotnet build` succeeds with no new warnings.
- [ ] The app runs against your own MAI-Image-2.5-Pro deployment, or you explain why you couldn't run it.
- [ ] No secrets, endpoints or personal data are in the diff.
- [ ] The README reflects any change in settings, routes or output.
- [ ] The pull request description says what changed and why.

The build workflow runs on every pull request and has to pass before a merge.

## Reporting a bug

Open an issue and include:

- Whether the generate or the edit step failed, and what you expected.
- The status code and response body the app printed, with any endpoints removed.
- Your .NET SDK version (`dotnet --version`) and operating system.
- Your deployment name.

## Security issues

Please don't report security problems in a public issue. Use **Security → Report a vulnerability** on the repo to send a private report.

## License

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE) that covers this project.
