namespace Application.UITests.Base;

[CollectionDefinition("UI Test Collection")]
//[CollectionDefinition("UI Test Collection", DisableParallelization = true)]
public class UiTestCollection
    : ICollectionFixture<UITestFixture>
{
    // No code is needed here.
    // This declaration is enough for xUnit to share the same fixture instance.    
}

