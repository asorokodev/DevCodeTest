This is a RESTful Api to get data from https://hacker-news.firebaseio.com/ written with ASP.Net Core.

The application doesn't require any setup. To start it just open console in "DevCodeTest.WebApi" folder and run "dotnet run" command.
After that open a browser and navigate to "http://localhost:5293/swagger/index.html" url.

The application uses Polly nuget package (https://github.com/App-vNext/Polly) for implementation Retry, Timeout and Rate-Limit strategies,
InMemory cache for caching items already received. All requests are divided into parallel groups.

All configurations are stored in appsettings.json file:
"DataSource": {
  "BaseUrl": "https://hacker-news.firebaseio.com/v0/",
  "BestStoriesIdsRoute": "beststories.json",
  "ItemDetailsRoute": "item/{0}.json",
  "RetryNumber": "3",
  "RateLimit": "500"
},
"RequestProcessing": {
  "MaxParallelRequest": "50"
}

