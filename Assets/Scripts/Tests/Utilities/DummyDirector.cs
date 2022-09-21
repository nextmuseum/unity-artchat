using ARtChat.Templates;

namespace TestUtilities
{
    public class DummyDirector : Director
    {
        public int CountNotified = 0;
        public void OnNotified(Colleague colleague)
        {
            CountNotified++;
        }
    }
}