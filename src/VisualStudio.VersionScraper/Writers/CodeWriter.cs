namespace VisualStudio.VersionScraper.Writers;

using System;
using System.Diagnostics.CodeAnalysis;

using SlnUp.Core;

using Treasure.Utils;

internal sealed class CodeWriter
{
    private readonly int indentSpaces = 4;

    private readonly StreamWriter writer;

    private int currentIndentationLevel;

    /// <summary>
    /// Initializes a new instance of the <see cref="CodeWriter"/> class.
    /// </summary>
    /// <param name="writer">The writer.</param>
    public CodeWriter(StreamWriter writer)
        => this.writer = Argument.NotNull(writer);

    /// <summary>
    /// Indents the code.
    /// </summary>
    public void Indent() => ++this.currentIndentationLevel;

    /// <summary>
    /// Unindents the code.
    /// </summary>
    public void Unindent() => --this.currentIndentationLevel;

    /// <summary>
    /// Creates a scope with brackets.
    /// </summary>
    /// <param name="closingSemicolon">if set to <c>true</c>, the closing bracket will include a semicolon.</param>
    /// <returns><see cref="IDisposable"/>.</returns>
    public IDisposable WithBrackets(bool closingSemicolon = false)
    {
        this.WriteLine("{");
        this.Indent();
        return new ScopedAction(() =>
        {
            this.Unindent();

            if (closingSemicolon)
            {
                this.WriteLine("};");
            }
            else
            {
                this.WriteLine("}");
            }
        });
    }

    /// <summary>
    /// Creates a scope with indentation.
    /// </summary>
    /// <returns><see cref="IDisposable"/>.</returns>
    public IDisposable WithIndent()
    {
        this.Indent();
        return new ScopedAction(this.Unindent);
    }

    /// <summary>
    /// Writes the line.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><see cref="CodeWriter"/>.</returns>
    [SuppressMessage("Globalization", "CA1307:Specify StringComparison for clarity")]
    public CodeWriter WriteLine(string? value = null)
    {
        if (value is not null)
        {
            if (value.Contains(Environment.NewLine))
            {
                throw new InvalidOperationException("Use WriteLines to write content that contains multiple lines.");
            }

            this.WriteIndentation();
        }

        this.writer.WriteLine(value);

        return this;
    }

    /// <summary>
    /// Writes the lines.
    /// </summary>
    /// <param name="value">The value.</param>
    /// <returns><see cref="CodeWriter"/>.</returns>
    public CodeWriter WriteLines(string value)
    {
        foreach (string line in value.Split(Environment.NewLine, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            this.WriteLine(line);
        }

        return this;
    }

    private string GetIndentationWhiteSpace() => new(' ', this.indentSpaces * this.currentIndentationLevel);

    private void WriteIndentation() => this.writer.Write(this.GetIndentationWhiteSpace());
}
