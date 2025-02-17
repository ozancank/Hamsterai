﻿using OCK.Core.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.BookService;

public interface IBookService : IBusinessService
{
    Task BookPrepare(int bookId, CancellationToken cancellationToken = default);
}
