namespace Signum.Playwright.Search;

/// <summary>
/// Proxy for QueryTokenPart in QueryTokenBuilder.tsx
/// </summary>
public class QueryTokenPartProxy
{
    public ILocator Element { get; }

    public QueryTokenPartProxy(ILocator element)
    {
        Element = element;
    }

    public async Task SelectAsync(string? fullKey)
    {
        var dropdownContainer = Element.Locator(".rw-popup-container");
        var isVisible = await dropdownContainer.IsVisibleAsync();
        if (!isVisible)
        {
            // If the popup is still animating closed (rw-slide-transition-exiting), clicking to open
            // mid-animation causes a conflicted state: the span items become unstable/invisible as
            // the close animation wins, making ClickAsync fail for the full 20s timeout.
            // Wait for the exiting animation to fully complete before clicking to open.
            try
            {
                await Element.Locator(".rw-popup-container.rw-slide-transition-exiting")
                             .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 2000 });
            }
            catch { /* not in exiting state or already done — safe to proceed */ }

            await Element.Locator(".rw-dropdown-list, .sf-query-token-plus").ClickAsync();
            await dropdownContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

            // Also wait for the open animation (rw-slide-transition-entering) to finish.
            // Items inside the popup are unstable/clipped during the slide-in and ClickAsync
            // will fail with "element is not stable" → "element is not visible" if clicked too early.
            try
            {
                await Element.Locator(".rw-popup-container.rw-slide-transition-entering")
                             .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Hidden, Timeout = 2000 });
            }
            catch { /* animation already done — safe to proceed */ }
        }

        var tokenSelector = !string.IsNullOrEmpty(fullKey) ? $"[data-full-token='{fullKey}']" : ":not([0])";
        var optionElement = dropdownContainer.Locator($"div > span{tokenSelector}");
        await optionElement.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        try
        {
            await optionElement.ClickAsync();
        }
        catch (Exception)
        {
            await optionElement.ClickAsync(new() { Force = true });
        }

        await Element.Locator($".rw-dropdown-list-value span{tokenSelector}")
                     .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
}
