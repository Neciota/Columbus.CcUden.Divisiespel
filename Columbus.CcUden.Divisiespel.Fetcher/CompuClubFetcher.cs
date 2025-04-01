using Columbus.CcUden.Divisiespel.Models;

namespace Columbus.CcUden.Divisiespel.Fetcher
{
    public class CompuClubFetcher
    {
        private readonly HttpClient _httpClient;
        private readonly IHtmlParser _htmlParser;

        public CompuClubFetcher(IHtmlParser htmlParser)
        {
            _htmlParser = htmlParser;

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://www.compuclub.nu")
            };

            _httpClient.DefaultRequestHeaders.Add("accept", "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8");
            _httpClient.DefaultRequestHeaders.Add("accept-encoding", "gzip, deflate, br, zstd");
            _httpClient.DefaultRequestHeaders.Add("accept-language", "en-US,en;q=0.9");
            _httpClient.DefaultRequestHeaders.Add("cache-control", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("connection", "keep-alive");
            _httpClient.DefaultRequestHeaders.Add("host", "www.compuclub.nu");
            _httpClient.DefaultRequestHeaders.Add("pragma", "no-cache");
            _httpClient.DefaultRequestHeaders.Add("referer", "https://www.compuclub.nu/uitslag/index.php");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-dest", "document");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-mode", "navigate");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-site", "same-origin");
            _httpClient.DefaultRequestHeaders.Add("sec-fetch-user", "?1");
            _httpClient.DefaultRequestHeaders.Add("sec-gpc", "1");
            _httpClient.DefaultRequestHeaders.Add("upgrade-insecure-requests", "1");
            _httpClient.DefaultRequestHeaders.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/134.0.0.0 Safari/537.36");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua", "\"Chromium\";v=\"134\", \"Not:A-Brand\";v=\"24\", \"Brave\";v=\"134\"");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0");
            _httpClient.DefaultRequestHeaders.Add("sec-ch-ua-platform", "\"Windows\"");
        }

        public async Task<bool> TryUpdateSessionIdAsync()
        {
            HttpRequestMessage request = new(HttpMethod.Get, $"/uitslag/index.php");
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            if (response.Headers.TryGetValues("set-cookie", out IEnumerable<string>? values))
            {
                string? sessionCookie = values.FirstOrDefault(v => v.Contains("PHPSESSID=", StringComparison.InvariantCultureIgnoreCase));
                if (sessionCookie is null)
                    return false;

                _httpClient.DefaultRequestHeaders.Add("cookie", sessionCookie);
                return true;
            }

            return false;
        }

        public async Task<bool> SetYear(int year)
        {
            HttpRequestMessage request = new(HttpMethod.Get, $"/uitslag/actueeljaar.php?jaar={year}");
            HttpResponseMessage response = await _httpClient.SendAsync(request);

            return response.StatusCode == System.Net.HttpStatusCode.Redirect;
        }

        public async Task<IEnumerable<string>> GetCcFlightLinks()
        {
            HttpRequestMessage ccRequest = new(HttpMethod.Get, "/uitslag/index1.php?afd=4300");
            HttpResponseMessage ccResponse = await _httpClient.SendAsync(ccRequest);

            string intialResultsPath = _htmlParser.GetCcResultsPath(await ccResponse.Content.ReadAsStringAsync());

            HttpRequestMessage flightsRequest = new(HttpMethod.Get, $"/uitslag/{intialResultsPath}");
            HttpResponseMessage flightsResponse = await _httpClient.SendAsync(flightsRequest);

            return _htmlParser.GetFlightPaths(await flightsResponse.Content.ReadAsStringAsync());
        }

        public async Task<List<ResultLine>> GetResults(string initialFlightPath)
        {
            HttpRequestMessage initialPageRequest = new(HttpMethod.Get, $"/uitslag/{initialFlightPath}");
            HttpResponseMessage initialPageResponse = await _httpClient.SendAsync(initialPageRequest);

            ResultsPage initialResultsPage = _htmlParser.GetResultsPage(await initialPageResponse.Content.ReadAsStringAsync());

            IEnumerable<ResultLine> totalResults = initialResultsPage.ResultLines;
            foreach (var page in initialResultsPage.OtherPages.Skip(1)) // The first page results are already included
            {
                HttpRequestMessage pageRequest = new(HttpMethod.Get, $"/uitslag/{page}");
                HttpResponseMessage pageResponse = await _httpClient.SendAsync(pageRequest);

                ResultsPage pageResults = _htmlParser.GetResultsPage(await pageResponse.Content.ReadAsStringAsync());
                totalResults = totalResults.Concat(pageResults.ResultLines);
            }

            return totalResults.OrderBy(r => r.Position).ToList();
        }
    }
}
