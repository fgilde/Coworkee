using System;
using System.ComponentModel.DataAnnotations;

namespace Coworkee.Domain.Contracts
{
    //[ProvideAsEdm(ProvideInherits = true)]
    public abstract class AuditableEntity<TId> : IAuditableEntity<TId>
    {
        public TId Id { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime? LastModifiedOn { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}