using System;
using FluentAssertions;
using NUnit.Framework;
using Coworkee.Application.Common.Models;
using Coworkee.Application.Configurations;
using Coworkee.Domain.Contracts;
using Nextended.Core.Extensions;
using Nextended.Core.Helper;

namespace Coworkee.Application.UnitTests.Common.Models
{
    public class HashedIntTests
    {
        [OneTimeSetUp]
        public void RunBeforeAnyTests()
        {
            ClassMappingConfiguration.RegisterConverters(new IdHashing() { Enabled = true, AllowAccessWithNotHashedId = false, MinLength = 5, Salt = Guid.NewGuid().ToString() });
        }

        [Test]
        public void CanConvertImplicit()
        {
            int id = 27;
            HashedInt hashed = id;
            string hash = hashed.Hash;
            hashed.Id.Should().Be(id);

            HashedInt hashed2 = hash;
            hashed2.Id.Should().Be(id);
            hashed2.Hash.Should().Be(hash);
        }

        [Test]
        public void CanMapEntityToDtoWithHashedInt()
        {
            var testId = 27;
            var entity = new MyEntity()
            {
                Id = testId,
                Name = "A test",
                Description = "This is a Tens entity instance"
            };
            var dto = entity.MapTo<MyEntityDto>();
            var id = dto.Id.Id;
            var hash = dto.Id.Hash;
            id.Should().Be(testId);
            hash.Should().Be(new HashedInt(id).Hash);
        }

        [Test]
        public void CanMapDtoToEntityWithHashedInt()
        {
            var testId = 27;
            var hashedId = new HashedInt(testId).Hash;
            var dto = new MyEntityDto()
            {
                Id = hashedId,
                Name = "A test",
                Description = "This is a Tens entity instance"
            };
            var entity = dto.MapTo<MyEntity>();
            entity.Id.Should().Be(testId);
        }

        [Test]
        public void CanMapDtoToEntityWithEmptyHashedInt()
        {
            var dto = new MyEntityDto()
            {
                Name = "A test",
                Description = "This is a Tens entity instance"
            };
            dto.IsNew.Should().Be(true);
            var entity = dto.MapTo<MyEntity>();
            entity.Id.Should().Be(default);
        }

        [Test]
        public void CanMapEntityToHashedDto()
        {
            var testId = 27;
            var hash = new HashedInt(testId).Hash;
            var entity = new MyEntity()
            {
                Id = testId,
                Name = "A test",
                Description = "This is a Tens entity instance"
            };
            var dto = entity.MapTo<MyEntityDtoHashable>();

            int realId = ((IDtoBase<int>)dto).Id;
            realId.Should().Be(testId);
            dto.Id.Should().Be(hash);
        }

        [Test]
        public void CanMapHashedDtoToEntity()
        {
            var testId = 27;
            var hash = new HashedInt(testId).Hash;
            var dto = new MyEntityDtoHashable()
            {
                Id = hash,
                Name = "A test",
                Description = "This is a Tens entity instance"
            };

            string strid = dto.Id;
            int intid = ((IDtoBase<int>)dto).Id;
            var entity = dto.MapTo<MyEntity>();
            entity.Id.Should().Be(testId);
            intid.Should().Be(testId);
            strid.Should().Be(hash);
        }

        [Test]
        public void CanMapHashedDtoToEntityWithEmptyId()
        {
            var dto = new MyEntityDtoHashable()
            {
                Name = "A test",
                Description = "This is a Tens entity instance"
            };
            dto.IsNew.Should().Be(true);
            var entity = dto.MapTo<MyEntity>();
            entity.Id.Should().Be(default);
        }

        #region Test Models

        private class MyEntityDtoHashable : HashableDtoBase
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        private class MyEntityDto : DtoBase<HashedInt>
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        public class MyEntity : AuditableEntity<int>
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        #endregion
    }
}
