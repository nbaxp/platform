using System.Diagnostics;
using Dapper;
using DynamicODataToSQL;
using Flurl;
using Flurl.Util;
using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Newtonsoft.Json;

namespace Wta.Infrastructure.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class RestController : ControllerBase
{
    private readonly IODataToSqlConverter _oDataToSqlConverter;
    private readonly string? _connectionString;

    public RestController(IODataToSqlConverter oDataToSqlConverter,
        IConfiguration configuration)
    {
        ArgumentNullException.ThrowIfNull(configuration);

        _oDataToSqlConverter = oDataToSqlConverter ?? throw new ArgumentNullException(nameof(oDataToSqlConverter));
        _connectionString = configuration.GetConnectionString("Default");
    }

    [HttpGet("{tableName}", Name = "QueryRecords")]
    public async Task<IActionResult> QueryAsync(string tableName,
        [FromQuery(Name = "$select")] string select,
        [FromQuery(Name = "$filter")] string filter,
        [FromQuery(Name = "$orderby")] string orderby,
        [FromQuery(Name = "$top")] int top = 10,
        [FromQuery(Name = "$skip")] int skip = 0)
    {
        var query = _oDataToSqlConverter.ConvertToSQL(tableName,
                new Dictionary<string, string>
                {
                        { "select", select },
                        { "filter", filter },
                        { "orderby", orderby },
                        { "top", (top + 1).ToInvariantString() },
                        { "skip", skip.ToInvariantString() }
                }
            );
        Debug.WriteLine(query.Item1);
        IEnumerable<dynamic>? rows;
        using var conn = new MySqlConnection(_connectionString);
        rows = (await conn.QueryAsync(query.Item1, query.Item2).ConfigureAwait(false))?.ToList();

        ODataQueryResult? result = null;
        if (rows == null)
        {
            return new JsonResult(result);
        }

        var isLastPage = rows.Count() <= top;
        result = new ODataQueryResult
        {
            Count = isLastPage ? rows.Count() : rows.Count() - 1,
            Value = rows.Take(top),
            NextLink = isLastPage ? null : BuildNextLink(tableName, @select, filter, @orderby, top, skip)
        };

        return new JsonResult(result);
    }

    private string BuildNextLink(string tableName,
        string select,
        string filter,
        string orderby,
        int top,
        int skip
        )
    {
        var nextLink = Url.Link("QueryRecords", new { tableName });
        nextLink = nextLink
            .SetQueryParam("select", select)
            .SetQueryParam("filter", filter)
            .SetQueryParam("orderBy", orderby)
            .SetQueryParam("top", top)
            .SetQueryParam("skip", skip + top);

        return nextLink;
    }
}

public class ODataQueryResult
{
    [Description("Count of records in result set")]
    public int Count { get; set; }

    [Description("URL to fetch next set of records. Example : For skip = 0 and top = 10, this will contain link to retrieve 10 records after skipping first 10 records." +
        "\n NextLink will be null if this is the last set of records")]
    [JsonProperty(NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
    public string? NextLink { get; set; }

    [Description("Records in current set")]
    public IEnumerable<dynamic>? Value { get; set; }
}
