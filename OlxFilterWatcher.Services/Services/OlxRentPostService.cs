namespace OlxFilterWatcher.Services.Services;

public sealed class OlxRentPostService : IOlxRentPostService
{
	private readonly IMongoService<OlxRentPost> collection;
	private readonly IMapper<OlxRentPostDTO, OlxRentPost> mapper;

	public OlxRentPostService(IMongoService<OlxRentPost> collection, IMapper<OlxRentPostDTO, OlxRentPost> mapper)
	{
		this.collection = collection;
		this.mapper = mapper;
	}

	public async Task AddAsync(OlxRentPostDTO olxPostDTO, CancellationToken cancellationToken = default)
	{
		if (olxPostDTO is null)
			return;

		if (await AnyAsync(olxPostDTO.URL, cancellationToken))
			return;

		await collection.AddOneAsync(mapper.Map(olxPostDTO), cancellationToken);
	}

    public async ValueTask<OlxRentPostDTO> GetAsync(string URL, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(URL))
            return null;

		var searchFilter = Builders<OlxRentPost>.Filter.Eq(x => x.URL, URL);

		var olxPost = await collection.FindOneAsync(searchFilter, null, cancellationToken);

        return mapper.Map(olxPost);
    }

    public async ValueTask<bool> AnyAsync(string URL, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(URL))
            return false;

        var searchFilter = Builders<OlxRentPost>.Filter.Eq(x => x.URL, URL);

		return await collection.AnyAsync(searchFilter, cancellationToken);
    }
}
