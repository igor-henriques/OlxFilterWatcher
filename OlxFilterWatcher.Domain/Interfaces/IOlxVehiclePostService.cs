﻿namespace OlxFilterWatcher.Domain.Interfaces;

public interface IOlxVehiclePostService
{
    ValueTask<bool> AnyAsync(string URL, CancellationToken cancellationToken = default);
    Task AddAsync(OlxVehiclePostDTO olxPostDTO, CancellationToken cancellationToken = default);
    ValueTask<OlxVehiclePostDTO> GetAsync(string URL, CancellationToken cancellationToken = default);
}