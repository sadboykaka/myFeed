﻿using System.Collections.Generic;
using System.Threading.Tasks;
using myFeed.Models;

namespace myFeed.Interfaces
{
    public interface IFeedStoreService
    {
        Task<IEnumerable<Article>> Load(IEnumerable<Channel> channels);
    }
}