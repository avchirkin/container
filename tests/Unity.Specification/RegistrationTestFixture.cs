﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Unity.Specification.Tests
{
    [TestClass]
    public class RegistrationTestFixture : Registration.SpecificationTests
    {
        public override IUnityContainer GetContainer()
        {
            return new UnityContainer();
        }
    }
}
