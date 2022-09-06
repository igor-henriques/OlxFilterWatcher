namespace OlxFilterWatcher.Services.Services;

public class OlxVehiclePostService : IOlxVehiclePostService
{
	private readonly IMongoService<OlxVehiclePost> collection;
	private readonly IMapper<OlxVehiclePostDTO, OlxVehiclePost> mapper;

	public OlxVehiclePostService(IMongoService<OlxVehiclePost> collection, IMapper<OlxVehiclePostDTO, OlxVehiclePost> mapper)
	{
		this.collection = collection;
		this.mapper = mapper;
	}

	public async ValueTask AddAsync(OlxVehiclePostDTO olxPostDTO, CancellationToken cancellationToken = default)
	{
		if (olxPostDTO is null)
			return;

		if (await AnyAsync(olxPostDTO.URL))
			return;

		await collection.AddOneAsync(mapper.Map(olxPostDTO), cancellationToken);
	}

    public async ValueTask<OlxVehiclePostDTO> GetAsync(string URL, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(URL))
            return null;

		var searchFilter = Builders<OlxVehiclePost>.Filter.Eq(x => x.URL, URL);

		var olxPost = await collection.FindOneAsync(searchFilter, null, cancellationToken);

        return mapper.Map(olxPost);
    }

    public async ValueTask<bool> AnyAsync(string URL, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(URL))
            return false;

        var searchFilter = Builders<OlxVehiclePost>.Filter.Eq(x => x.URL, URL);

		return await collection.AnyAsync(searchFilter, cancellationToken);
    }
}
