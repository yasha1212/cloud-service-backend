﻿namespace CloudService.Entities
{
    public interface IEntity<TKey>
    {
        TKey Id { get; set; }
    }
}
