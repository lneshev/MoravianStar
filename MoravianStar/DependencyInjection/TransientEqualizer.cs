using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.ModelBuilder;

namespace MoravianStar.DependencyInjection
{
    internal class TransientEqualizer : IContributeComponentModelConstruction
    {
        public void ProcessModel(IKernel kernel, global::Castle.Core.ComponentModel model)
        {
            if (model.LifestyleType == LifestyleType.Undefined)
            {
                model.LifestyleType = LifestyleType.Transient;
            }
        }
    }
}