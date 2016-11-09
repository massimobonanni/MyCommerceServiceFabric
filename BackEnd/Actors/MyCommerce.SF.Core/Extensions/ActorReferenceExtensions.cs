using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors.Runtime;

namespace MyCommerce.SF.Core.Extensions
{
    public static class ActorReferenceExtensions
    {
        public static MyCommerce.SF.Dto.ActorReferenceString ToActorReferenceString(this Actor actor)
        {
            return new MyCommerce.SF.Dto.ActorReferenceString()
            {
                ActorId = actor.Id.GetStringId(),
                ApplicationName = actor.ApplicationName,
                ServiceName = actor.ActorService.ActorTypeInformation.ServiceName
            };
        }
    }
}
