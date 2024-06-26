using ConfigurationBuilder = Reinforced.Typings.Fluent.ConfigurationBuilder;

namespace StreamMaster.API
{
    public static class ReinforcedTypingsConfiguration
    {
        public static void Configure(ConfigurationBuilder builder)
        {
            //builder
            //    .ExportAsInterface<SMStreamDto>()
            //    .WithMethods(prop => prop.Name == "Mapping", conf => conf.Ignore())
            //    .WithPublicProperties();
        }
    }
}
