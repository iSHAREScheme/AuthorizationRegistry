using System;

namespace NLIP.iShare.EntityFramework
{
    /// <summary>
    /// Defines a contract for the long term persisted entities
    /// </summary>
    public interface IEntity
    {
        /// <summary>
        /// The unique id for this entity, based on which can be located in the persistent store
        /// </summary>
        Guid Id { get; set; }
    }
}
