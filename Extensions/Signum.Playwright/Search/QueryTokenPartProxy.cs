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
        var plusButton = Element.Locator(".sf-query-token-plus");
        if (await plusButton.IsVisibleAsync())
        {
            await plusButton.ClickAsync();
        }
        else
        {
            var isAlreadyOpen = await Element.Locator(".rw-dropdown-list[aria-expanded='true']").CountAsync() > 0
                || await Element.Locator(".rw-popup-container").IsVisibleAsync();

            if (!isAlreadyOpen)
            {
                await Element.Locator(".rw-dropdown-list").ClickAsync();
            }
        }

        var dropdownContainer = Element.Locator(".rw-popup-container");
        await dropdownContainer.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        var tokenSelector = !string.IsNullOrEmpty(fullKey) ? $"[data-full-token='{fullKey}']" : ":not([0])";
        var optionElement = dropdownContainer.Locator($"div > span{tokenSelector}");
        await optionElement.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        try
        {
            await optionElement.ClickAsync();
        }
        catch (Exception)
        {
            await optionElement.DispatchEventAsync("click");
        }

        await Element.Locator($".rw-dropdown-list-value span{tokenSelector}")
                     .WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
    }
}
