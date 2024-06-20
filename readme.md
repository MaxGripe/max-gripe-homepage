
[![Azure Static Web Apps CI/CD](https://github.com/MaxGripe/max-gripe-homepage/actions/workflows/main.yml/badge.svg)](https://github.com/MaxGripe/max-gripe-homepage/actions/workflows/main.yml)

# Max Gripe's Homepage
Welcome to my [homepage](https://max.gripe/)! It serves as a platform for me to share my links and thoughts.

## Some technical details:

- This website is hosted on [Azure](https://azure.microsoft.com/) using free plan. 

- Articles and other content are written in Markdown, allowing for easy content creation and management. These Markdown files are automatically converted to HTML during the build process using [F#](https://fsharp.org/) and the [FSharp.Formatting](https://fsprojects.github.io/FSharp.Formatting/) library.

- The deployment process is fully automated using [GitHub Actions](https://github.com/features/actions). Any changes to this repository are immediately reflected on the live site.

## Repository Structure

- `src/`: Contains the source code for the site, including the F# scripts and Markdown content.
  - `HtmlGenerator/`: The F# project that handles the generation of HTML from Markdown.
  - `markdown/`: Directory containing the Markdown files for articles and other content.
  - `partials/`: Contains reusable HTML snippets like `header.inc` and `footer.inc`.
  - `output/`: Directory where the generated HTML files are stored.
- `README.md`: This file.
- `LICENSE`: License file for the project.

## How It Works

When I write an article in Markdown and place it in the `src/markdown/` folder, the rest happens automatically. GitHub Actions detects changes pushed to the repository, triggers the build process, and deploys the updated site to Azure Static Web Apps.

## Contributing

Feel free to open issues or submit pull requests if you have suggestions or improvements for the site. Contributions are always welcome!

## License

This project is licensed under the terms of the [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/).


