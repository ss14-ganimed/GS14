namespace Content.Server.Drone.Components
{
    [RegisterComponent]
    public sealed partial class DroneComponent : Component
    {
        public float InteractionBlockRange = 2.15f;
		public bool blockInteraction = false;
    }
}
