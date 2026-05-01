using StudentMS.Common.Models;

namespace StudentMS.Business.Base
{
    /// <summary>
    /// Base class for all business services.
    /// Provides CreateRequest helper to map RequestModelBase -> DomainRequestModelBase.
    /// </summary>
    public abstract class BusinessServiceBase
    {
        protected TDomain CreateRequest<TDomain>(RequestModelBase source)
            where TDomain : DomainRequestModelBase, new()
        {
            return new TDomain
            {
                Language  = source.Language,
                UserId    = source.UserId,
                IPAddress = source.IPAddress
            };
        }
    }
}
