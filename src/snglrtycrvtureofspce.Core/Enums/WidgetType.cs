using System.ComponentModel;

namespace snglrtycrvtureofspce.Core.Enums;

/// <description>
/// Defines supported dashboard widget types.
/// </description>
public enum WidgetType
{
    [Description("Currency Exchange Rates")]
    CurrencyRates = 1,

    [Description("Currency Exchange Rate Trend")]
    CurrencyTrend = 2,

    [Description("Currency Rates Converter")]
    CurrencyConverter = 3,

    [Description("Stock Market Overview")]
    StockMarket = 4,

    [Description("Cryptocurrency Rates")]
    CryptoRates = 5,

    [Description("News Feed")]
    NewsFeed = 6,

    [Description("Weather Forecast")]
    Weather = 7,

    [Description("Calendar")]
    Calendar = 8,

    [Description("Quick Notes")]
    Notes = 9,

    [Description("Task List")]
    TaskList = 10,

    [Description("Recent Activity")]
    RecentActivity = 11,

    [Description("Analytics Summary")]
    AnalyticsSummary = 12
}
