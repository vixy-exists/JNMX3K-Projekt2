using UnityEngine;
using Unity.Entities;

namespace testtest
{
    public partial class TestSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            // Example of how to use the TestComponent in a system
            Entities.ForEach((ref TestComponent testComponent) =>
            {
                // Increment the value of TestComponent by 1 every frame
                testComponent.Value += 1;
            }).Schedule();
        }
    }

    public struct TestComponent : IComponentData
    {
        public int Value;
    }
}
