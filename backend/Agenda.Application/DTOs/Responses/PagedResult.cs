using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agenda.Application.DTOs.Responses
{
    public sealed record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalItems,
        int Totalpages
    );
}
