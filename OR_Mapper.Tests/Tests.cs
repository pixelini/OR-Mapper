using System;
using System.Collections.Generic;
using NUnit.Framework;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework;
using OR_Mapper.Framework.Caching;

namespace OR_Mapper.Tests
{
    public class Tests
    {
        private ICache Cache { get; set; } = new Cache();
        
        private ICache TrackingCache { get; set; } = new TrackingCache();

        
        [SetUp]
        public void Setup()
        {
        }
        
        [Test]
        public void Cache_Add_IsSuccessful()
        {
            var teacher = new Teacher
            {
                Id = 1,
                FirstName = "Susi",
                Name = "Sorglos",
                Gender = Gender.Female,
                BirthDate = new DateTime(2000, 1, 1),
                HireDate = new DateTime(2020, 12, 31),
                Salary = 50000
            };
            
            Cache.Add(teacher);

            var result = (Teacher) Cache.Get(1, typeof(Teacher));
            Assert.That(result.Id == 1);
        }
        
        [Test]
        public void Cache_Delete_IsSuccessful()
        {
            var teacher = new Teacher
            {
                Id = 2,
                FirstName = "Tom",
                Name = "Turbo",
                Gender = Gender.Male,
                BirthDate = new DateTime(2000, 1, 1),
                HireDate = new DateTime(2020, 12, 31),
                Salary = 50000
            };
            Cache.Add(teacher);
            
            Cache.Remove(teacher);
            
            Assert.That(() => (Teacher)Cache.Get(2, typeof(Teacher)), Throws.Exception.TypeOf<KeyNotFoundException>());
        }
        
        [Test]
        public void Model_Constructor_HasCorrectAmountOfExternalFields()
        {
            var model = new Model(typeof(Teacher));

            Assert.That(model.ExternalFields.Count == 2);
        }
        
        [Test]
        public void Model_Constructor_HasCorrectAmountOfInternalFields()
        {
            var model = new Model(typeof(Teacher));
            Console.WriteLine();
            Assert.That(model.Fields.Count == 8);
        }
        
        [Test]
        public void InheritedClass_HasCorrectTableName()
        {
            var model = new Model(typeof(Teacher));
            Assert.That(model.TableName == "Person");
        }
        
        [Test]
        public void BaseClass_HasCorrectTableName()
        {
            var model = new Model(typeof(Course));
            Assert.That(model.TableName == "Course");
        }


    }
}