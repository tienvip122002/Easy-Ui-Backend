using System;
using System.Collections.Generic;

namespace EasyUiBackend.Domain.Models.UIComponent
{
    public class AddTagsToComponentRequest
    {
        public required List<Guid> TagIds { get; set; }
    }
}
