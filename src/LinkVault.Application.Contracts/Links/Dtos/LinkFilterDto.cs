using System;
using System.Collections.Generic;
using Volo.Abp.Application.Dtos;

namespace LinkVault.Links.Dtos;

public class LinkFilterDto : PagedAndSortedResultRequestDto
{
    public string? Filter { get; set; }

    public Guid? CollectionId { get; set; }

    public List<Guid>? TagIds { get; set; }

    public string? Domain { get; set; }

    public bool? IsFavorite { get; set; }

    public bool IncludeDeleted { get; set; }
}
