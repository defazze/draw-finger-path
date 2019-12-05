using Unity.Entities;

public class DestroySystem : ComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<MustBeDestroyed>().ForEach((Entity e) =>
        {
            PostUpdateCommands.DestroyEntity(e);
        });
    }
}