open System.IO
open FSharp.Formatting.Markdown

let readFile (path: string) =
    path
    |> File.Exists
    |> function
        | true -> File.ReadAllText(path)
        | false -> ""

let writeFile (path: string) (content: string) = File.WriteAllText(path, content)

let toUrlFriendly (input: string) =
    input.ToLowerInvariant()
    |> fun text -> System.Text.RegularExpressions.Regex.Replace(text, @"[^\w\s]", "") // Remove all non-alphanumeric characters
    |> fun text -> System.Text.RegularExpressions.Regex.Replace(text, @"\s+", "-") // Replace spaces with hyphens


let generateHtml (header: string) (footer: string) (content: string) (title: string) =
    $"""
    <!DOCTYPE html>
    <html lang="en" color-mode="user">
    <head>
        <meta charset="UTF-8">
        <meta name="viewport" content="width=device-width, initial-scale=1.0">
        <link rel="stylesheet" href="styles.css">
        <title>{title}</title>
    </head>
    <body>
        <header>
            {header}
        </header>
        <main>
            {content}
        </main>
        <hr />
        <footer>
            {footer}
        </footer>
    </body>
    </html>
    """

let generateHtmlFromMarkdown (header: string) (footer: string) (filePath: string) (outputDir: string) =
    let title =
        File.ReadAllLines(filePath)
        |> Array.tryFind (fun line -> line.StartsWith("# "))
        |> Option.defaultValue "# No Title"
        |> fun title -> title.TrimStart('#').Trim()

    let fileName = toUrlFriendly (title)
    let outputFilePath = Path.Combine(outputDir, fileName + ".html")
    let markdownContent = File.ReadAllText(filePath)
    let htmlContent = Markdown.ToHtml(markdownContent)
    let fullHtmlContent = generateHtml header footer htmlContent fileName

    printfn "Generating %s" outputFilePath
    writeFile outputFilePath fullHtmlContent

let generateIndexPage
    (header: string)
    (footer: string)
    (articles: (string * string * string) list)
    (outputDir: string)
    =
    let indexMarkdownPath = Path.Combine(__SOURCE_DIRECTORY__, "markdown", "index.md")

    let indexContent =
        if File.Exists(indexMarkdownPath) then
            Markdown.ToHtml(File.ReadAllText(indexMarkdownPath))
        else
            ""

    let articlesContent =
        articles
        |> List.map (fun (date, title, link) -> $"""<li>{date}: <a href="{link}">{title}</a></li>""")
        |> String.concat "\n"

    let content =
        $"""
    {indexContent}
    <section>
        <h1>Articles</h1>
        <ul>
            {articlesContent}
        </ul>
    </section>
    """

    let fullHtmlContent = generateHtml header footer content "Main Page"
    let outputFilePath = Path.Combine(outputDir, "index.html")
    printfn "Generating %s" outputFilePath
    writeFile outputFilePath fullHtmlContent

[<EntryPoint>]
let main argv =
    let sourceDir = __SOURCE_DIRECTORY__
    let markdownDir = Path.Combine(sourceDir, "markdown")
    let outputDir = Path.Combine(sourceDir, "output")
    let partialsDir = Path.Combine(sourceDir, "partials")

    if not (Directory.Exists(markdownDir)) then
        printfn "Markdown directory does not exist: %s" markdownDir
        failwith "Markdown directory not found"

    if not (Directory.Exists(outputDir)) then
        Directory.CreateDirectory(outputDir) |> ignore

    let header = readFile (Path.Combine(partialsDir, "header.inc"))
    let footer = readFile (Path.Combine(partialsDir, "footer.inc"))

    let cssSourcePath = Path.Combine(sourceDir, "..", "styles.css")
    let cssDestinationPath = Path.Combine(outputDir, "styles.css")

    if File.Exists(cssSourcePath) then
        File.Copy(cssSourcePath, cssDestinationPath, true)

    let markdownFiles = Directory.GetFiles(markdownDir, "*.md")

    let isArticle (file: string) =
        System.Char.IsDigit(Path.GetFileName(file).[0])

    let articleFiles =
        markdownFiles
        |> Array.filter (fun file -> isArticle (file))



    let articles =
        articleFiles
        |> Array.map (fun file ->
            let date = Path.GetFileNameWithoutExtension(file)

            let title =
                File.ReadAllLines(file)
                |> Array.tryFind (fun line -> line.StartsWith("# "))
                |> Option.defaultValue "# No Title"
                |> fun title -> title.TrimStart('#').Trim()

            let urlFriendlyTitle = toUrlFriendly (title)
            (date, title, $"{urlFriendlyTitle}.html"))
        |> Array.sortByDescending (fun (date, _, _) -> date)
        |> Array.toList


    generateIndexPage header footer articles outputDir

    articleFiles
    |> Array.iter (fun file -> generateHtmlFromMarkdown header footer file outputDir)

    markdownFiles
    |> Array.filter (fun file -> not (isArticle (file)))
    |> Array.iter (fun file -> generateHtmlFromMarkdown header footer file outputDir)


    0
