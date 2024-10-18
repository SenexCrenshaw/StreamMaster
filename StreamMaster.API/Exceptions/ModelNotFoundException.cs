namespace StreamMaster.API.Exceptions
{
    public class ModelNotFoundException : NzbDroneException
    {
        public ModelNotFoundException(Type modelType, int modelId)
            : base("{0} with ID {1} does not exist", modelType.Name, modelId)
        {
        }

        protected ModelNotFoundException(string message, params object[] args) : base(message, args)
        {
        }

        protected ModelNotFoundException(string message) : base(message)
        {
        }

        protected ModelNotFoundException(string message, Exception innerException, params object[] args) : base(message, innerException, args)
        {
        }

        protected ModelNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public ModelNotFoundException() : base()
        {
        }
    }
}
