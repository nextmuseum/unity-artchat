namespace ARtChat.Templates
{
    public class Colleague
    {
        protected Director director;

        public Colleague(Director _director)
        {
            director = _director;
        }

        protected void NotifyDirector()
        {
            director.OnNotified(this);
        }
    }
}