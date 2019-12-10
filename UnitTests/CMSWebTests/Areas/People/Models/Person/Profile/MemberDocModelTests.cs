﻿using CMSWebTests.Properties;
using SharedTestFixtures;
using Shouldly;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace CmsWeb.Areas.People.Models.Tests
{
    [Collection(Collections.Database)]
    public class MemberDocModelTests : DatabaseTestBase
    {
        [Fact]
        public void MemberDocModelTest()
        {
            var model = new MemberDocModel
            {
                DocDate = DateTime.Now,
                Docid = RandomNumber(),
                Finance = true,
                FormName = RandomString(),
                Id = RandomNumber(),
                IsDocument = true,
                LargeId = null,
                Name = RandomString(),
                ThumbId = null,
                Uploader = RandomString()
            };
            model.DocUrl.ShouldBe($"/FinanceDocs/{model.Docid}");

            model.Finance = false;
            model.DocUrl.ShouldBe($"/MemberDocs/{model.Docid}");

            model.IsDocument = false;
            model.LargeId = RandomNumber();
            model.ThumbId = RandomNumber();
            model.DocUrl.ShouldBe($"/MemberDocs/{model.LargeId}");
            model.ImgUrl.ShouldBe($"/Image/{model.ThumbId}");
        }

        [Fact]
        public void DocFormsTest()
        {
            var rm = CMSWebTestsResources.ResourceManager;
            var person = CreatePerson();
            var doc1 = RandomString();
            var doc2 = RandomString();
            person.UploadDocument(db, idb, rm.GetFileStream("DocFormsTest1"), doc1, "application/pdf", true);
            person.UploadDocument(db, idb, rm.GetFileStream("DocFormsTest2"), doc2, "application/pdf", false);
            var financedocs = MemberDocModel.DocForms(db, person.PeopleId, true);
            financedocs.Count().ShouldBe(1);
            financedocs.First().Name.ShouldBe(person.Name);
            financedocs.First().FormName.ShouldBe(doc1);

            var memberdocs = MemberDocModel.DocForms(db, person.PeopleId, false);
            memberdocs.Count().ShouldBe(1);
            memberdocs.First().Name.ShouldBe(person.Name);
            memberdocs.First().FormName.ShouldBe(doc2);
        }

        [Fact]
        public void DeleteDocumentTest()
        {
        }
    }
}
