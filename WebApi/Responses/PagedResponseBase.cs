using System;
using WebApi.Dto;

namespace WebApi.Responses
{
    public abstract class PagedResponseBase<TResponseEntity> :
        ResponseBase<PageResponseDto<TResponseEntity>>
    {

        public Type GetEntityType()
        {
            return typeof(TResponseEntity);
        }
    }

    
}
