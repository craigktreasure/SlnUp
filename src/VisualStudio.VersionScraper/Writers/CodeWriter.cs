namespace VisualStudio.VersionScraper.Writers;

using SlnUp.Core;
using System;
using Treasure.Utils;

internal class CodeWriter
{
    private readonly int indentSpaces = 4;

    private readonly StreamWriter writer;

    private int currentIndentationLevel;

    public CodeWriter(StreamWriter writer) => this.writer = Argument.NotNull(writer);

    public void Indent() => ++this.currentIndentationLevel;

    public void Unindent() => --this.currentIndentationLevel;

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

    public IDisposable WithIndent()
    {
        this.Indent();
        return new ScopedAction(() => this.Unindent());
    }

    public CodeWriter WriteLine(string? value = null)
    {
        if (value is not null)
        {
            this.WriteIndentation();
        }

        this.writer.WriteLine(value);

        return this;
    }

    private string GetIndentationWhiteSpace() => new(' ', this.indentSpaces * this.currentIndentationLevel);

    private void WriteIndentation() => this.writer.Write(this.GetIndentationWhiteSpace());
}
