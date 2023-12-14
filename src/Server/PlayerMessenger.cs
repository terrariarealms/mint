using Microsoft.Xna.Framework;

namespace Mint.Server;

public class PlayerMessenger
{
    public PlayerMessenger()
    {
    }

    public PlayerMessenger(Player player)
    {
        Player = player;
        colorMap = new Color[]
        {
            // BASIC
            new Color(34, 139, 34), // OK
            new Color(255, 255, 0), // INFO
            new Color(178, 34, 34), // ERROR
            new Color(255, 69, 0),  // WARNING

            // PAGE
            new Color(0, 128, 0),   // PAGE HEADER
            new Color(0, 100, 0),   // PAGE ITEM
            new Color(46, 139, 87)  // PAGE FOOTER
        };
    }

    protected Player? Player;
    protected Color[]? colorMap;

    public virtual void Begin() {}
    public virtual void End() {}

    /// <summary>
    /// Send message to player.
    /// </summary>
    /// <param name="mark">Message mark</param>
    /// <param name="source">Message source</param>
    /// <param name="message">Message text</param>
    /// <param name="objects">Format objects</param>
    public virtual void Send(MessageMark mark, string? source, string message, params object?[] objects)
    {
        if (colorMap == null) return;

        message = MintServer.Localization.Translate(message, Player?.Account?.LanguageID);
        if (source != null)
        {
            source = MintServer.Localization.Translate(source, Player?.Account?.LanguageID);
            message = $"[c/691a7d:[][c/861aa1:{source}][c/691a7d:]] [c/595959:»] {message}";
        }

        string formatted = string.Format(message, objects);
        Player?.SendMessage(formatted, colorMap[(byte)mark]);
    }

    /// <summary>
    /// Send message to player without translating.
    /// </summary>
    /// <param name="mark">Message mark</param>
    /// <param name="source">Message source</param>
    /// <param name="message">Message text</param>
    /// <param name="objects">Format objects</param>
    public virtual void CleanSend(MessageMark mark, string? source, string message, params object?[] objects)
    {
        if (colorMap == null) return;

        if (source != null)
        {
            message = $"[c/691a7d:[][c/861aa1:{source}][c/691a7d:]] [c/595959:»] {message}";
        }

        string formatted = string.Format(message, objects);
        Player?.SendMessage(formatted, colorMap[(byte)mark]);
    }

    /// <summary>
    /// Send page to player.
    /// Format arguments: {0} - current page; {1} - max page, {2} - total items, {3} - next page (currentPage + 1).
    /// </summary>
    /// <param name="headerFormat">Header format</param>
    /// <param name="lines">Items</param>
    /// <param name="page">Current page</param>
    /// <param name="footerFormat">Footer format</param>
    /// <param name="nextPageFormat">Next page format. This element is appearing when next page is available</param>
    public virtual void SendPage(string headerFormat, IList<string> lines, int page, string? footerFormat = null, string? nextPageFormat = null)
    {
        // pages calculation
        int currentPage = Math.Min(1, page);
        int nextPage = currentPage + 1;
        int items; int maxPage;
        CalculatePages(lines.GetEnumerator(), 5, out items, out maxPage);

        // header
        Send(MessageMark.PageHeader, null, headerFormat, currentPage, maxPage, items, nextPage);

        // items
        for (int i = currentPage * 5; i < items; i++)
        {
            Send(MessageMark.PageItem, null, lines[i]);
        }

        // footers 
        List<string> footer = new List<string>(2);
        if (footerFormat != null)
            footer.Add(footerFormat);

        if (nextPageFormat != null)
            footer.Add(nextPageFormat);

        Send(MessageMark.PageFooter, null, string.Join(" • ", footer), currentPage, maxPage, items, nextPage);
    }

    internal void CalculatePages(IEnumerator<string> enumerator, int maxItems, out int items, out int maxPage)
    {        
        maxPage = 1;
        items = 0;

        while (enumerator.MoveNext())
        {
            if (items % 5 == 0)
                maxPage++;

            items++;
        }
    }
}