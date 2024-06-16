namespace ModelCache.Api.EventArgs
{
    public class ModelChangedEventArgs : System.EventArgs
    {
        public Type ModelType { get; }

        public ModelChangedEventArgs(Type modelType)
        {
            ModelType = modelType;
        }
    }
}
