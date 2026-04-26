using Gameplay.TomaGirl;

namespace Gameplay.Throwables
{
    public interface IThrowableObject
    {
        public ThrowableObjectInfo GetInfo();

        bool AlreadyImpacted { get; }

        public void Impact(ThrowableImpact impact);
    }
}