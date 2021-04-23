namespace CoreLib.UnityExt
{
  using Unity.Builder;
  using Unity.Extension;

  /// <summary>
  /// UnityPropertyContainerExt
  /// </summary>
  public sealed class UnityPropertyContainerExt : UnityContainerExtension
  {
    protected override void Initialize()
    {
      Context.Strategies.Add(new PropertyContainerStrategy(Container), UnityBuildStage.TypeMapping);
      Context.Strategies.Add(new PropertyErrorsContainerStrategy(), UnityBuildStage.TypeMapping);
      Context.Strategies.Add(new SubComponentStrategy(), UnityBuildStage.TypeMapping);
    }
  }
}