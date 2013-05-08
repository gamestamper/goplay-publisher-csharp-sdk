GoPlay Publisher C# SDK
========================

Our Publisher C# SDK lets you access our Graph API in as little as one line.

You can download the [GoPlay Publisher C# SDK](https://github.com/gamestamper/goplay-publisher-csharp-sdk) from GitHub [here](https://github.com/gamestamper/goplay-publisher-csharp-sdk).

* * *

## Installing and Initializing

To install the Publisher C# SDK, extract and download the files and copy the files from the `PublisherSDK/` directory to a directory on the server where you will host your app, for example `csharp-sdk`. Add `using` statements for both the `GoPlay` and `GoPlay.Serialization` namespaces. Finally, create a new `PublisherSDK` object, passing your Publisher ID and Publisher Secret.

<div class="preWide"><pre>
using GoPlay;
using GoPlay.Serialization;

...

dynamic sdk = new PublisherSDK("YOUR_PUBLISHER_ID", "YOUR_PUBLISHER_SECRET");

</pre></div>

* * *

## Getting Data

To get data from the Graph, you have a few options, depending on your preference. Let's say that you want to retrieve your Player's Club data.  This data lives at the following location on our graph: https://graph.goplay.com/[publisher_id]/playersClub. All of the following calls return the same result:

<div class="preWide"><pre>
using GoPlay;
using GoPlay.Serialization;
dynamic sdk = new PublisherSDK("YOUR_PUBLISHER_ID", "YOUR_PUBLISHER_SECRET");

// the standard way
GraphResponse response = sdk.Get("[publisher_id]/playersClub");

// and it's even better to not have to type as much
GraphResponse response = sdk.Pub.PlayersClub.Get();
</pre></div>

### Understanding the Response

The response that is returned has a few properties to simplify things a bit, most notably `Data` and `Error`.

<div class="preWide"><pre>
// get some data
try
{
  GraphResponse response = sdk.Pub.PlayersClub.Get();
	Console.Write(response.Data);
}
catch (Exception e)
{
	Console.Write(response.Error);
}
</pre></div>

### Passing parameters

There are times when there are parameters you might want to pass, such as limit and fields, which work as follows:

<div class="preWide"><pre>
JsonDictionary params = new JsonDictionary("{\"limit\":10, \"fields\":\"email\"}");

// the standard way
GraphResponse response = sdk.Get(
	"[publisher_id]/playersClub", 
	params
);

// short-hand
GraphResponse response = sdk.Pub.PlayersClub.Get(params);
</pre></div>

### Paging

To iterate through pages of data, use the `Next()` and `Previous()` functions:

<div class="preWide"><pre>
// get the data
JsonDictionary params = new JsonDictionary("{\"limit\":10}");
GraphResponse response = sdk.Pub.PlayersClub.Get(params);

// get the next page
response = response.Next();

// get the previous page
response = response.Previous();
</pre></div>

* * *

## Posting Data

Posting to the Graph API works almost identically to how getting data works. Instead of calling `get`, we instead call `post`. The following shows how to post some data (in this case users) to the graph:

<div class="preWide"><pre>
using GoPlay;
using GoPlay.Serialization;
dynamic sdk = new PublisherSDK("YOUR_PUBLISHER_ID", "YOUR_PUBLISHER_SECRET");

JsonArray players = new JsonArray();
players.Add(new JsonDictionary("{\"accountId\":\"act123\", \"email\":\"abc@def.com\", \"zip\":\"12345\", \"birthday\":\"01/01/1970\"}"));
players.Add(new JsonDictionary("{\"accountId\":\"act456\", \"email\":\"def@ghi.com\", \"zip\":\"67890\", \"birthday\":\"01/01/1971\"}"));

JsonDictionary params = new JsonDictionary();
params.add('players', players);

// the standard way
GraphResponse = sdk.Post("[publisher_id]/playersClub", params);

// shorter with no publisher_id
GraphResponse = sdk.Pub.PlayersClub.Post(params);
</pre></div>

* * *

## Deleting Data

Though rare, there are times when you might need to delete data from our Graph API. Once again, the SDK offers the same flexibility. We'll delete some data in a few ways:

<div class="preWide"><pre>
// assumes we have three accounts with accountIds 'act123' and 'act456'

// remove one the standard way
JsonDictionary params = new JsonDictionary("{\"accountId\":\"act123\"}");
GraphResponse response = sdk.Delete("[publisher_id]/playersClub", params);

// remove another a different way
JsonDictionary params = new JsonDictionary("{\"accountId\":\"act456\"}");
GraphResponse response = sdk.Pub.Delete(params);
</pre></div>
