namespace OlxFilterWatcher.Services.Services;

public sealed class OlxGeneralPostService : IOlxGeneralPostService
{
	private readonly IMongoService<OlxGeneralPost> collection;
	private readonly IMapper<OlxGeneralPostDTO, OlxGeneralPost> mapper;

	public OlxGeneralPostService(IMongoService<OlxGeneralPost> collection, IMapper<OlxGeneralPostDTO, OlxGeneralPost> mapper)
	{
		this.collection = collection;
		this.mapper = mapper;
	}

	public async ValueTask AddAsync(OlxGeneralPostDTO olxPostDTO, CancellationToken cancellationToken = default)
	{
		if (olxPostDTO is null)
			return;

		if (await AnyAsync(olxPostDTO.URL, cancellationToken))
			return;

		await collection.AddOneAsync(mapper.Map(olxPostDTO), cancellationToken);
	}

    public async ValueTask<OlxGeneralPostDTO> GetAsync(string URL, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(URL))
            return null;

		var searchFilter = Builders<OlxGeneralPost>.Filter.Eq(x => x.URL, URL);

        var olxPost = await collection.FindOneAsync(searchFilter, null, cancellationToken);

        return mapper.Map(olxPost);
    }

	public async ValueTask<string> GetLastPostUrlByFilterAsync(string filter, CancellationToken cancellationToken = default)
	{
        if (string.IsNullOrEmpty(filter))
			return null;

        var searchFilter = Builders<OlxGeneralPost>.Filter.Eq(x => x.FoundByFilter, filter);

		var fields = Builders<OlxGeneralPost>.Projection.Include(x => x.URL);
		var sort = Builders<OlxGeneralPost>.Sort.Descending(x => x.TimePosted);

        var olxPosts = await collection.FindOneAsync(searchFilter, new FindOptions<OlxGeneralPost, OlxGeneralPost>() 
		{ 
			Projection = fields,
			Sort = sort
		}, cancellationToken);

		return olxPosts?.URL ?? null;
    }

    public async ValueTask<bool> AnyAsync(string URL, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(URL))
            return false;

        var searchFilter = Builders<OlxGeneralPost>.Filter.Eq(x => x.URL, URL);

		return await collection.AnyAsync(searchFilter, cancellationToken);
    }
}
